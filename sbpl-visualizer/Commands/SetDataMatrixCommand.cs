using sbpl_visualizer;
using System;
using System.Drawing;
using System.Text;

namespace sbpl_visualizer
{
	internal class SetDataMatrixCommand : ISBPLCommand
	{
		private static readonly int[] SquareSizes =
		{
			10, 12, 14, 16, 18, 20, 22, 24, 26, 32, 36, 40,
			44, 48, 52, 64, 72, 80, 88, 96, 104, 120, 132, 144
		};

		private static readonly int[] BinaryCapacities =
		{
			1, 3, 6, 10, 16, 20, 28, 34, 42, 60, 84, 112,
			142, 172, 202, 278, 366, 454, 574, 694, 814, 1048, 1302, 1556
		};

		public void Execute(Graphics g, SBPLContext context, string argument)
		{
			if (string.IsNullOrWhiteSpace(argument))
			{
				AppLogger.Warn("Data Matrix settings ignored: empty argument", nameof(SetDataMatrixCommand));
				return;
			}

			string[] parts = argument.TrimStart(',').Split(',');
			if (parts.Length != 4)
			{
				AppLogger.Warn("Data Matrix settings ignored: invalid argument format", nameof(SetDataMatrixCommand), new System.Collections.Generic.Dictionary<string, object>
				{
					{ "part_count", parts.Length },
				});
				return;
			}

			if (!int.TryParse(parts[0], out int cellWidth) || cellWidth <= 0)
			{
				AppLogger.Warn("Data Matrix settings ignored: invalid cell width", nameof(SetDataMatrixCommand));
				return;
			}

			if (!int.TryParse(parts[1], out int cellHeight) || cellHeight <= 0)
			{
				AppLogger.Warn("Data Matrix settings ignored: invalid cell height", nameof(SetDataMatrixCommand));
				return;
			}

			if (!int.TryParse(parts[2], out int columns) || columns < 0)
			{
				AppLogger.Warn("Data Matrix settings ignored: invalid columns", nameof(SetDataMatrixCommand));
				return;
			}

			if (!int.TryParse(parts[3], out int rows) || rows < 0)
			{
				AppLogger.Warn("Data Matrix settings ignored: invalid rows", nameof(SetDataMatrixCommand));
				return;
			}

			context.PendingDataMatrixSettings = new DataMatrixSettings
			{
				CellWidth = cellWidth,
				CellHeight = cellHeight,
				Columns = columns,
				Rows = rows,
			};

			AppLogger.Info("Data Matrix settings registered", nameof(SetDataMatrixCommand), new System.Collections.Generic.Dictionary<string, object>
			{
				{ "cell_width", cellWidth },
				{ "cell_height", cellHeight },
				{ "columns", columns },
				{ "rows", rows },
			});
		}

		private static int PickSquareSize(int payloadLength)
		{
			int normalizedLength = Math.Max(1, payloadLength);

			for (int i = 0; i < BinaryCapacities.Length; i++)
			{
				if (normalizedLength <= BinaryCapacities[i])
				{
					return SquareSizes[i];
				}
			}

			return SquareSizes[SquareSizes.Length - 1];
		}

		public static DataMatrixSettings CreateSettingsForPayload(int cellWidth, int cellHeight, int columns, int rows, string payload)
		{
			int resolvedColumns = columns;
			int resolvedRows = rows;

			if (resolvedColumns == 0 && resolvedRows == 0)
			{
				int length = Encoding.ASCII.GetByteCount(payload ?? string.Empty);
				int symbolSize = PickSquareSize(length);
				resolvedColumns = symbolSize;
				resolvedRows = symbolSize;
			}

			return new DataMatrixSettings
			{
				CellWidth = cellWidth,
				CellHeight = cellHeight,
				Columns = resolvedColumns,
				Rows = resolvedRows,
			};
		}
	}
}
