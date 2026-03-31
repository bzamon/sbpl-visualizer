using sbpl_visualizer;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

public class DrawTextCommand : ISBPLCommand
{
	public void Execute_bkp(Graphics g, SBPLContext context, string argument)
	{

        if (argument != null)
        {
            // Compute final integer pixel coordinates
            int drawX = (int)Math.Round(new Decimal(context.X + context.OffsetX));
            int drawY = (int)Math.Round(new Decimal(context.Y + context.OffsetY));

            // Font with no transform scaling
            var font = new Font(context.CurrentFont.FontFamily, context.CurrentFont.Size, context.CurrentFont.Style);

            // Save original state
            var state = g.Save();


            if (context.Rotation != 0)
            {
                // Rotate around origin (0, 0) — can be enhanced to rotate around label center
                g.TranslateTransform(0, 0);
                g.RotateTransform(context.Rotation);
                context.Rotation = 0;
            }

            // Force pixel-aligned rendering
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            // Draw text directly at pixel origin, crisp and unscaled
            g.DrawString(argument, font, Brushes.Black, drawX, drawY);

            g.Restore(state); // Restore original transform
        }

    }

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

            // Force pixel-aligned rendering
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            g.DrawString(argument, scaledFont, Brushes.Black, 0, 0); // Draw at new origin

			g.Restore(state); // Restore original transform
		}
	}
}
