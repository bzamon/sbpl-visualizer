using sbpl_visualizer;
using System.Collections.Generic;
using System.Drawing;

public class CompositeCommand : ISBPLCommand
{
	private readonly List<ISBPLCommand> commands;

	public CompositeCommand(params ISBPLCommand[] commands)
	{
		this.commands = new List<ISBPLCommand>(commands);
	}

	public void Execute(Graphics g, SBPLContext context, string argument)
	{
		foreach (var cmd in commands)
		{
			cmd.Execute(g, context, argument);
		}
	}
}
