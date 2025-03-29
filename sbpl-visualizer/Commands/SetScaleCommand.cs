using System;
using System.Drawing;

namespace sbpl_visualizer
{
	internal class SetScaleCommand : ISBPLCommand
	{
		public void Execute(Graphics g, SBPLContext context, string argument)
		{
			if (argument.Length != 4)
				return;

			string hor = argument.Substring(0, 2);
			string ver = argument.Substring(2, 2);

			if (int.TryParse(hor, out int scaleX) && int.TryParse(ver, out int scaleY))
			{
				if (scaleX >= 1 && scaleX <= 36 && scaleY >= 1 && scaleY <= 36)
				{
					context.ScaleX = scaleX;
					context.ScaleY = scaleY;
				}
			}
		}
	}
}
