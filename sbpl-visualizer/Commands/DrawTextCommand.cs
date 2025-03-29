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

			g.DrawString(argument, context.CurrentFont, Brushes.Black, drawX, drawY);
			context.Y += 20; // Optional: shift cursor for next text
		}
	}
}
