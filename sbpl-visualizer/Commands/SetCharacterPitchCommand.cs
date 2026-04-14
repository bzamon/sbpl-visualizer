using sbpl_visualizer;
using System.Drawing;

public class SetCharacterPitchCommand : ISBPLCommand
{
	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		if (int.TryParse(argument, out int value) && value >= 0)
		{
			context.CharacterPitch = value;
		}
	}
}
