using sbpl_visualizer;
using System.Drawing;

public class DrawBarcodeCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (string.IsNullOrWhiteSpace(argument) || argument.Length < 7)
			return;

		string type = argument.Substring(0, 1); // Barcode type (1 = Code 39)
		string heightStr = argument.Substring(6); // Actual data after settings
		string data = heightStr;

		int height = 80;
		int widthPerChar = 10;


		Rectangle rect = new Rectangle(context.X, context.Y, data.Length * widthPerChar, height);

		// Simulate a barcode (for now): black & white bars
		using (SolidBrush brush = new SolidBrush(Color.Black))
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (i % 2 == 0)
				{
					g.FillRectangle(brush, context.X + i * widthPerChar, context.Y, widthPerChar / 2, height);
				}
			}
		}

		// Optional: draw the raw data below
		g.DrawString(data, new Font("Arial", 10), Brushes.Black, context.X + context.OffsetX, context.Y + context.OffsetY  + height + 5);

		context.Y += height + 25;
	}
}
