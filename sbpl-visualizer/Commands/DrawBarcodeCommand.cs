using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace sbpl_visualizer
{
	internal class DrawBarcodeCommand : ISBPLCommand
	{
		private const float DotUnit = 0.5f;

		private readonly string[] ITF_ENCODING = {
			"00110", "10001", "01001", "11000", "00101",
			"10100", "01100", "00011", "10010", "01010"
		};

		public void Execute(Graphics g, SBPLContext context, string argument)
		{
			if (argument.Length < 7)
				return;

			char symbology = argument[0];
			if (symbology != '2') return; // Only ITF supported for now

			if (!int.TryParse(argument.Substring(1, 2), out int narrowBase) || narrowBase < 1 || narrowBase > 36)
				return;

			if (!int.TryParse(argument.Substring(3, 3), out int heightBase) || heightBase < 1 || heightBase > 999)
				return;

			// Apply scaling from L command
			int narrow = Math.Max(1, (int)Math.Round(narrowBase * DotUnit * context.ScaleX));
			int height = Math.Max(1, (int)Math.Round(heightBase * DotUnit * context.ScaleY));
			int wide = Math.Max(narrow + 1, (int)Math.Round(narrow * 3.0));


			string data = argument.Substring(6).Trim();
			if (data.Length % 2 != 0 || !IsNumeric(data)) return;

			int x = context.X + context.OffsetX;
			int y = context.Y + context.OffsetY;
			Debug.WriteLine("0253279830");
			Debug.WriteLine(data);

			// Start pattern
			DrawPattern(g, x, y, "1010", narrow, wide, height);
			x += GetPatternWidth("1010", narrow, wide);

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
					g.FillRectangle(Brushes.Black, x, y, barWidth, height);
					x += barWidth;

					int spaceWidth = spaces[j] == '1' ? wide : narrow;
					x += spaceWidth;
				}
			}

			// Stop pattern
			DrawPattern(g, x, y, "1101", narrow, wide, height);
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
