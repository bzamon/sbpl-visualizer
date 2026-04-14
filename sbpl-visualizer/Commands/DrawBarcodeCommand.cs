using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace sbpl_visualizer
{
	internal class DrawBarcodeCommand : ISBPLCommand
	{
		private const float DotUnit = 1f;

		private readonly string[] ITF_ENCODING = {
			"00110", "10001", "01001", "11000", "00101",
			"10100", "01100", "00011", "10010", "01010"
		};

		public void Execute(Graphics g, SBPLContext context, string argument)
		{
			if (argument.Length < 7)
			{
				AppLogger.Warn("Barcode command ignored: argument too short", nameof(DrawBarcodeCommand));
				return;
			}

			char symbology = argument[0];
			if (symbology != '2')
			{
				AppLogger.Warn("Barcode command ignored: unsupported symbology", nameof(DrawBarcodeCommand), new System.Collections.Generic.Dictionary<string, object>
				{
					{ "symbology", symbology.ToString() },
				});
				return; // Only ITF supported for now
			}

			if (!int.TryParse(argument.Substring(1, 2), out int narrowBase) || narrowBase < 1 || narrowBase > 36)
			{
				AppLogger.Warn("Barcode command ignored: invalid narrow base", nameof(DrawBarcodeCommand));
				return;
			}

			if (!int.TryParse(argument.Substring(3, 3), out int heightBase) || heightBase < 1 || heightBase > 999)
			{
				AppLogger.Warn("Barcode command ignored: invalid height base", nameof(DrawBarcodeCommand));
				return;
			}

			// Apply scaling from L command
			int narrow = Math.Max(1, (int)Math.Round(narrowBase * DotUnit * context.ScaleX));
			int height = Math.Max(1, (int)Math.Round(heightBase * DotUnit * context.ScaleY));
			int wide = Math.Max(narrow + 1, (int)Math.Round(narrow * 3.0));



			string data = argument.Substring(6).Trim();
			if (data.Length % 2 != 0 || !IsNumeric(data))
			{
				AppLogger.Warn("Barcode command ignored: invalid numeric payload", nameof(DrawBarcodeCommand), new System.Collections.Generic.Dictionary<string, object>
				{
					{ "digit_count", data.Length },
				});
				return;
			}

			var state = g.Save();

			int drawX = context.X + context.OffsetX;
			int drawY = context.Y + context.OffsetY;

			AppLogger.Info("Barcode render started", nameof(DrawBarcodeCommand), new System.Collections.Generic.Dictionary<string, object>
			{
				{ "digit_count", data.Length },
				{ "x", drawX },
				{ "y", drawY },
			});

			if (context.Rotation != 0)
			{
				g.TranslateTransform(drawX, drawY);                 // Move to the barcode origin
				g.RotateTransform(context.Rotation);                // Apply rotation around that point
				drawX = 0;
				drawY = 0;
				//context.Rotation = 0;
			}


			// Start pattern
			DrawPattern(g, drawX, drawY, "1010", narrow, wide, height);
			drawX += GetPatternWidth("1010", narrow, wide);

			// Data encoding
			for (int i = 0; i < data.Length; i += 2)
			{
				int d1 = data[i] - '0';
				int d2 = data[i + 1] - '0';

				string bars = ITF_ENCODING[d1];
				string spaces = ITF_ENCODING[d2];

				for (int j = 0; j < 5; j++)
				{
					int barWidth = bars[j] == '1' ? wide : narrow;
					g.FillRectangle(Brushes.Black, drawX, drawY, barWidth, height);
					drawX += barWidth;

					int spaceWidth = spaces[j] == '1' ? wide : narrow;
					drawX += spaceWidth;
				}
			}

			// Stop pattern
			DrawPattern(g, drawX, drawY, "1101", narrow, wide, height);

			g.Restore(state); // Restore original transform

			AppLogger.Info("Barcode render completed", nameof(DrawBarcodeCommand), new System.Collections.Generic.Dictionary<string, object>
			{
				{ "digit_count", data.Length },
				{ "height", height },
			});
		}

		private void DrawPattern(Graphics g, int x, int y, string pattern, int narrow, int wide, int height)
		{
			for (int i = 0; i < pattern.Length; i++)
			{
				int width = pattern[i] == '1' ? wide : narrow;
				if (i % 2 == 0) // bar
					g.FillRectangle(Brushes.Black, x, y, width, height);
				x += width;
			}
		}

		private int GetPatternWidth(string pattern, int narrow, int wide)
		{
			int total = 0;
			foreach (char c in pattern)
				total += c == '1' ? wide : narrow;
			return total;
		}

		private bool IsNumeric(string input)
		{
			return !string.IsNullOrEmpty(input) && input.All(char.IsDigit);
		}
	}
}
