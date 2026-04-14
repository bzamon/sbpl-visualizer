using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace sbpl_visualizer
{
	internal static class AppLogger
	{
		private static readonly object SyncRoot = new object();
		private static readonly AsyncLocal<string> CurrentRequestIdSlot = new AsyncLocal<string>();

		private static string logFilePath;
		private static bool initialized;

		public static void Initialize(string basePath)
		{
			if (initialized)
			{
				return;
			}

			if (string.IsNullOrWhiteSpace(basePath))
			{
				basePath = AppDomain.CurrentDomain.BaseDirectory;
			}

			string logDirectory = Path.Combine(basePath, "logs");
			Directory.CreateDirectory(logDirectory);
			logFilePath = Path.Combine(logDirectory, "sbpl-visualizer.log");
			initialized = true;
		}

		public static void Info(string message, string module)
		{
			Info(message, module, null);
		}

		public static void Info(string message, string module, IDictionary<string, object> fields)
		{
			Write("INFO", message, module, fields);
		}

		public static void Warn(string message, string module)
		{
			Warn(message, module, null);
		}

		public static void Warn(string message, string module, IDictionary<string, object> fields)
		{
			Write("WARN", message, module, fields);
		}

		public static void Error(string message, string module)
		{
			Error(message, module, null);
		}

		public static void Error(string message, string module, IDictionary<string, object> fields)
		{
			Write("ERROR", message, module, fields);
		}

		public static void Exception(string message, string module, Exception exception)
		{
			Exception(message, module, exception, null);
		}

		public static void Exception(string message, string module, Exception exception, IDictionary<string, object> fields)
		{
			var mergedFields = CloneFields(fields);

			if (exception != null)
			{
				mergedFields["exception_type"] = exception.GetType().FullName;
				mergedFields["stack_trace"] = BuildExceptionTrace(exception);
			}

			Write("ERROR", message, module, mergedFields);
		}

		public static string GenerateRequestId()
		{
			return Guid.NewGuid().ToString("N");
		}

		public static string GetCurrentRequestId()
		{
			return string.IsNullOrWhiteSpace(CurrentRequestIdSlot.Value) ? "system" : CurrentRequestIdSlot.Value;
		}

		internal static string GetAmbientRequestId()
		{
			return CurrentRequestIdSlot.Value;
		}

		internal static string SetCurrentRequestId(string requestId)
		{
			string previous = CurrentRequestIdSlot.Value;
			CurrentRequestIdSlot.Value = requestId;
			return previous;
		}

		private static void Write(string level, string message, string module, IDictionary<string, object> fields)
		{
			var payload = new List<KeyValuePair<string, object>>
			{
				new KeyValuePair<string, object>("timestamp", DateTimeOffset.UtcNow.ToString("o", CultureInfo.InvariantCulture)),
				new KeyValuePair<string, object>("level", level),
				new KeyValuePair<string, object>("message", message),
				new KeyValuePair<string, object>("module", module),
				new KeyValuePair<string, object>("request_id", GetCurrentRequestId()),
			};

			if (fields != null)
			{
				foreach (var field in fields)
				{
					if (field.Key == "timestamp" ||
						field.Key == "level" ||
						field.Key == "message" ||
						field.Key == "module" ||
						field.Key == "request_id")
					{
						continue;
					}

					payload.Add(field);
				}
			}

			string line = SerializeJson(payload);

			try
			{
				if (initialized)
				{
					lock (SyncRoot)
					{
						File.AppendAllText(logFilePath, line + Environment.NewLine, new UTF8Encoding(false));
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Failed to write application log: " + ex.GetType().FullName);
			}

			Trace.WriteLine(line);
		}

		private static Dictionary<string, object> CloneFields(IDictionary<string, object> fields)
		{
			return fields == null
				? new Dictionary<string, object>()
				: new Dictionary<string, object>(fields);
		}

		private static string BuildExceptionTrace(Exception exception)
		{
			if (exception == null)
			{
				return null;
			}

			var builder = new StringBuilder();
			Exception current = exception;
			int depth = 0;

			while (current != null)
			{
				if (depth > 0)
				{
					builder.AppendLine("--- Inner Exception ---");
				}

				builder.AppendLine(current.GetType().FullName);
				builder.AppendLine(current.StackTrace ?? string.Empty);

				current = current.InnerException;
				depth++;
			}

			return builder.ToString().TrimEnd();
		}

		private static string SerializeJson(IList<KeyValuePair<string, object>> fields)
		{
			var builder = new StringBuilder();
			builder.Append('{');

			for (int i = 0; i < fields.Count; i++)
			{
				if (i > 0)
				{
					builder.Append(',');
				}

				builder.Append('"');
				builder.Append(Escape(fields[i].Key));
				builder.Append("\":");
				builder.Append(SerializeValue(fields[i].Value));
			}

			builder.Append('}');
			return builder.ToString();
		}

		private static string SerializeValue(object value)
		{
			if (value == null)
			{
				return "null";
			}

			if (value is bool)
			{
				return ((bool)value) ? "true" : "false";
			}

			if (value is byte || value is sbyte || value is short || value is ushort ||
				value is int || value is uint || value is long || value is ulong)
			{
				return Convert.ToString(value, CultureInfo.InvariantCulture);
			}

			if (value is float || value is double || value is decimal)
			{
				return Convert.ToString(value, CultureInfo.InvariantCulture);
			}

			return "\"" + Escape(Convert.ToString(value, CultureInfo.InvariantCulture)) + "\"";
		}

		private static string Escape(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return string.Empty;
			}

			var builder = new StringBuilder(value.Length + 16);

			foreach (char ch in value)
			{
				switch (ch)
				{
					case '\\':
						builder.Append("\\\\");
						break;
					case '"':
						builder.Append("\\\"");
						break;
					case '\r':
						builder.Append("\\r");
						break;
					case '\n':
						builder.Append("\\n");
						break;
					case '\t':
						builder.Append("\\t");
						break;
					case '\b':
						builder.Append("\\b");
						break;
					case '\f':
						builder.Append("\\f");
						break;
					default:
						if (char.IsControl(ch))
						{
							builder.Append("\\u");
							builder.Append(((int)ch).ToString("x4", CultureInfo.InvariantCulture));
						}
						else
						{
							builder.Append(ch);
						}
						break;
				}
			}

			return builder.ToString();
		}
	}

	internal sealed class LogScope : IDisposable
	{
		private readonly string method;
		private readonly string path;
		private readonly string module;
		private readonly string requestId;
		private readonly string previousRequestId;
		private readonly Stopwatch stopwatch;
		private bool disposed;
		private string status;

		private LogScope(string method, string path, string module, string requestId)
		{
			this.method = method;
			this.path = path;
			this.module = module;
			this.requestId = requestId;
			previousRequestId = AppLogger.SetCurrentRequestId(requestId);
			status = "success";
			stopwatch = Stopwatch.StartNew();

			AppLogger.Info("request start", module, new Dictionary<string, object>
			{
				{ "method", method },
				{ "path", path },
				{ "status", "started" },
			});
		}

		public static LogScope Begin(string method, string path, string module)
		{
			return new LogScope(method, path, module, AppLogger.GenerateRequestId());
		}

		public static LogScope BeginNested(string method, string path, string module)
		{
			string ambientRequestId = AppLogger.GetAmbientRequestId();
			if (string.IsNullOrWhiteSpace(ambientRequestId))
			{
				ambientRequestId = AppLogger.GenerateRequestId();
			}

			return new LogScope(method, path, module, ambientRequestId);
		}

		public void SetStatus(string status)
		{
			if (!string.IsNullOrWhiteSpace(status))
			{
				this.status = status;
			}
		}

		public void Fail(Exception exception, string message)
		{
			SetStatus("error");
			AppLogger.Exception(message, module, exception, new Dictionary<string, object>
			{
				{ "method", method },
				{ "path", path },
				{ "status", status },
			});
		}

		public void Dispose()
		{
			if (disposed)
			{
				return;
			}

			disposed = true;
			stopwatch.Stop();

			AppLogger.Info("request end", module, new Dictionary<string, object>
			{
				{ "method", method },
				{ "path", path },
				{ "status", status },
				{ "duration_ms", stopwatch.ElapsedMilliseconds },
			});

			AppLogger.SetCurrentRequestId(previousRequestId);
		}
	}
}
