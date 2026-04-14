using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;

namespace sbpl_visualizer
{
	internal class DrawDataMatrixPreviewCommand : ISBPLCommand
	{
		public void Execute(Graphics g, SBPLContext context, string argument)
		{
			if (context.PendingDataMatrixSettings == null || string.IsNullOrWhiteSpace(argument))
			{
				AppLogger.Warn("Data Matrix command ignored: missing settings or payload", nameof(DrawDataMatrixPreviewCommand));
				return;
			}

			int commaIndex = argument.IndexOf(',');
			if (commaIndex <= 0)
			{
				AppLogger.Warn("Data Matrix command ignored: missing length prefix", nameof(DrawDataMatrixPreviewCommand));
				context.PendingDataMatrixSettings = null;
				return;
			}

			string lengthText = argument.Substring(0, commaIndex);
			string payload = argument.Substring(commaIndex + 1);

			if (!int.TryParse(lengthText, out int declaredLength))
			{
				AppLogger.Warn("Data Matrix command ignored: invalid length prefix", nameof(DrawDataMatrixPreviewCommand));
				context.PendingDataMatrixSettings = null;
				return;
			}

			int actualLength = Encoding.ASCII.GetByteCount(payload);
			if (actualLength != declaredLength)
			{
				AppLogger.Warn("Data Matrix length prefix mismatch", nameof(DrawDataMatrixPreviewCommand), new System.Collections.Generic.Dictionary<string, object>
				{
					{ "declared_length", declaredLength },
					{ "actual_length", actualLength },
				});
			}

			DataMatrixSettings settings = SetDataMatrixCommand.CreateSettingsForPayload(
				context.PendingDataMatrixSettings.CellWidth,
				context.PendingDataMatrixSettings.CellHeight,
				context.PendingDataMatrixSettings.Columns,
				context.PendingDataMatrixSettings.Rows,
				payload);

			if (settings.Columns <= 0 || settings.Rows <= 0)
			{
				AppLogger.Warn("Data Matrix command ignored: invalid resolved settings", nameof(DrawDataMatrixPreviewCommand));
				context.PendingDataMatrixSettings = null;
				return;
			}

			int drawX = context.X + context.OffsetX;
			int drawY = context.Y + context.OffsetY;
			var state = g.Save();

			if (context.Rotation != 0)
			{
				g.TranslateTransform(drawX, drawY);
				g.RotateTransform(context.Rotation);
				drawX = 0;
				drawY = 0;
			}

			AppLogger.Info("Data Matrix render started", nameof(DrawDataMatrixPreviewCommand), new System.Collections.Generic.Dictionary<string, object>
			{
				{ "declared_length", declaredLength },
				{ "actual_length", actualLength },
				{ "columns", settings.Columns },
				{ "rows", settings.Rows },
				{ "cell_width", settings.CellWidth },
				{ "cell_height", settings.CellHeight },
				{ "x", drawX },
				{ "y", drawY },
			});

			byte[] digest = BuildDigest(payload);
			int bitIndex = 0;

			for (int row = 0; row < settings.Rows; row++)
			{
				for (int column = 0; column < settings.Columns; column++)
				{
					if (!ShouldFillModule(row, column, settings.Rows, settings.Columns, digest, ref bitIndex))
					{
						continue;
					}

					g.FillRectangle(
						Brushes.Black,
						drawX + (column * settings.CellWidth),
						drawY + (row * settings.CellHeight),
						settings.CellWidth,
						settings.CellHeight);
				}
			}

			g.Restore(state);
			context.PendingDataMatrixSettings = null;

			AppLogger.Info("Data Matrix render completed", nameof(DrawDataMatrixPreviewCommand), new System.Collections.Generic.Dictionary<string, object>
			{
				{ "columns", settings.Columns },
				{ "rows", settings.Rows },
				{ "cell_width", settings.CellWidth },
				{ "cell_height", settings.CellHeight },
			});
		}

		private static byte[] BuildDigest(string payload)
		{
			byte[] source = Encoding.ASCII.GetBytes(payload ?? string.Empty);

			using (SHA256 sha256 = SHA256.Create())
			{
				if (source.Length == 0)
				{
					return sha256.ComputeHash(new byte[] { 0 });
				}

				byte[] output = new byte[128];
				byte[] current = source;

				for (int offset = 0; offset < output.Length; offset += 32)
				{
					current = sha256.ComputeHash(current);
					Buffer.BlockCopy(current, 0, output, offset, current.Length);
				}

				return output;
			}
		}

		private static bool ShouldFillModule(int row, int column, int totalRows, int totalColumns, byte[] digest, ref int bitIndex)
		{
			bool isTop = row == 0;
			bool isBottom = row == totalRows - 1;
			bool isLeft = column == 0;
			bool isRight = column == totalColumns - 1;

			if (isLeft || isBottom)
			{
				return true;
			}

			if (isTop)
			{
				return column % 2 == 0;
			}

			if (isRight)
			{
				return row % 2 == 0;
			}

			int digestIndex = (bitIndex / 8) % digest.Length;
			int bitOffset = 7 - (bitIndex % 8);
			bool fill = ((digest[digestIndex] >> bitOffset) & 1) == 1;
			bitIndex++;
			return fill;
		}
	}
}
