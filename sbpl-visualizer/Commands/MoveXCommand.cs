﻿using sbpl_visualizer;
using System.Drawing;

public class MoveXCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (int.TryParse(argument, out int value))
		{
			context.X = value;
		}
	}
}