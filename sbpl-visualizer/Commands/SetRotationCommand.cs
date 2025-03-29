using System;
using System.Drawing;

namespace sbpl_visualizer
{
	internal class SetRotationCommand : ISBPLCommand
	{
		public void Execute(Graphics g, SBPLContext context, string argument)
		{
			if (!string.IsNullOrWhiteSpace(argument) && int.TryParse(argument, out int rotationMode))
			{
				if (rotationMode >= 0 && rotationMode <= 3)
				{
					context.Rotation = rotationMode * 90;
				}
			}
		}
	}
}
