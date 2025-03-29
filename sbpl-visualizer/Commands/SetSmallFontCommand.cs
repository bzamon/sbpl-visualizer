using sbpl_visualizer;
using System.Drawing;

public class SetSmallFontCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		// Set font to Small (W8 x H15 dots ≈ 7pt font)
		context.FontCode = "S";
		context.CurrentFont = new Font("Arial", 7, FontStyle.Regular);
	}
}
