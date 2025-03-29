using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace sbpl_visualizer
{
	internal class SBPLParser
	{
		private readonly Dictionary<string, ISBPLCommand> commandMap;

		public SBPLParser()
		{
			commandMap = new Dictionary<string, ISBPLCommand>
		{
			{ "H", new MoveXCommand() },
			{ "V", new MoveYCommand() },
			{ "T", new DrawTextCommand() }, // Optional: command for plain text
			{ "S", new CompositeCommand(new SetSmallFontCommand(), new DrawTextCommand()) },
			{ "M", new CompositeCommand(new SetMediumFontCommand(), new DrawTextCommand()) },
			{ "B", new DrawBarcodeCommand() }, // For now, its not properly implemented. It generates a fake image
		};
		}

		public void ParseAndRender(Graphics g, string sbpl, string charToSplit)
		{
			var context = new SBPLContext();

			
			var tokens = Regex.Split(sbpl, charToSplit);


			foreach (var token in tokens)
			{
				if (string.IsNullOrWhiteSpace(token))
					continue;

				string commandKey = token.Substring(0, 1);
				string argument = token.Length > 1 ? token.Substring(1) : "";

				if (commandMap.TryGetValue(commandKey, out var command))
				{
					command.Execute(g, context, argument);
				}
				else
				{
					// Not an SBPL command? Skip it
					//g.DrawString(token, context.CurrentFont, Brushes.Black, context.X, context.Y);
					//context.Y += 20;
				}
			}
		}

	}
}
