using sbpl_visualizer;
using System.Drawing;

public class DrawTextCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (!string.IsNullOrWhiteSpace(argument))
		{
			g.DrawString(argument, context.CurrentFont, Brushes.Black, context.X, context.Y);
			context.Y += 20; // Optional: shift cursor for next text
		}
	}
}
