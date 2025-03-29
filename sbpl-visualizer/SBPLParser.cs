using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			{ "A3", new SetOffsetCommand() },
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

				string commandKey = null;

				// Match longest possible command key from the registered ones
				foreach (var key in commandMap.Keys.OrderByDescending(k => k.Length))
				{
					if (token.StartsWith(key))
					{
						commandKey = key;
						break;
					}
				}

				if (commandKey != null)
				{
					string argument = token.Substring(commandKey.Length);
					commandMap[commandKey].Execute(g, context, argument);
				}else
				{
					Debug.WriteLine(token);
					// Not an SBPL command? Skip it
					//g.DrawString(token, context.CurrentFont, Brushes.Black, context.X, context.Y);
					//context.Y += 20;
				}
			}
		}

	}
}
