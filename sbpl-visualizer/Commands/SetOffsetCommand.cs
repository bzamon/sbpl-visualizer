using sbpl_visualizer;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

public class SetOffsetCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (string.IsNullOrWhiteSpace(argument)) return;

		Debug.WriteLine(argument);

		if (argument.Contains('H') && argument.Contains('V'))
		{
			argument = argument.Substring(1);
			var parts = argument.Split('V');
			Debug.WriteLine(parts[0]);

			if (parts.Length == 2 && int.TryParse(parts[0], out int offsetX) &&	int.TryParse(parts[1], out int offsetY))
			{
				context.OffsetX = offsetX;
				context.OffsetY = offsetY;
			}
		}

		
	}

	//<ESC>A3 H100 V050

}
