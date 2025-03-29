using sbpl_visualizer;
using System.Drawing;

public class SetMediumFontCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		// Set font to Medium (W13 x H20 dots ≈ 10pt font)
		context.FontCode = "M";
		context.CurrentFont = new Font(context.CurrentFont.FontFamily.Name, 10);
	}
}
