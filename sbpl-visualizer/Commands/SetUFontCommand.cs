using sbpl_visualizer;
using System.Drawing;

public class SetUFontCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		context.FontCode = "U";
		context.CurrentFont = new Font(context.CurrentFont.FontFamily.Name, 5);
	}
}
