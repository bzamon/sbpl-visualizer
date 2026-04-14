using sbpl_visualizer;
using System.Drawing;

public class NoOpCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
	}
}
