using sbpl_visualizer;
using System.Drawing;

public class DrawTextCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (!string.IsNullOrWhiteSpace(argument))
		{
			int drawX = context.X + context.OffsetX;
			int drawY = context.Y + context.OffsetY;

			// Use only vertical scale to set font height
			var scaledFont = new Font(context.CurrentFont.FontFamily, context.CurrentFont.Size);

			// Save original state
			var state = g.Save();

			// Apply independent scaling
			g.TranslateTransform(drawX, drawY);               // Move origin to text position
			g.ScaleTransform(context.ScaleX, context.ScaleY); // Scale X and Y

			g.DrawString(argument, scaledFont, Brushes.Black, 0, 0); // Draw at new origin

			g.Restore(state); // Restore original transform
		}
	}
}
