using sbpl_visualizer;
using System.Drawing;

public class DrawTextCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (!(argument == null))
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
			context.ScaleX = 1; // reset
			context.ScaleY = 1; // reset

			// Apply rotation if needed
			if (context.Rotation != 0)
			{
				// Rotate around origin (0, 0) — can be enhanced to rotate around label center
				g.TranslateTransform(0, 0);
				g.RotateTransform(context.Rotation);
				context.Rotation = 0;
			}

			g.DrawString(argument, scaledFont, Brushes.Black, 0, 0); // Draw at new origin

			g.Restore(state); // Restore original transform
		}
	}
}
