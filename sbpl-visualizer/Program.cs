using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sbpl_visualizer
{
	internal static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			AppLogger.Initialize(Application.StartupPath);
			Application.ThreadException += OnThreadException;
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			AppLogger.Info("application startup complete", nameof(Program));
			Application.Run(new Form1());
		}

		private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			AppLogger.Exception("Unhandled UI exception", nameof(Program), e.Exception);
		}

		private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			AppLogger.Exception(
				"Unhandled application exception",
				nameof(Program),
				e.ExceptionObject as Exception,
				new Dictionary<string, object>
				{
					{ "is_terminating", e.IsTerminating },
				});
		}
	}
}
