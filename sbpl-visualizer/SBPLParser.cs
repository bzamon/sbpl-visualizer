using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

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
			{ "S", new CompositeCommand(new SetSmallFontCommand(), new DrawTextCommand()) }, // Set Small Font and Write Text
			{ "M", new CompositeCommand(new SetMediumFontCommand(), new DrawTextCommand()) }, // Set Medium Font and Write Text
			{ "B", new DrawBarcodeCommand() }, // For now, its not properly implemented. It generates a fake image
			{ "A3", new SetOffsetCommand() }, // Set Offset for MoveX and MoveY. Template: A3H100V050
			{ "L", new SetScaleCommand() }, // To enlarge text or barcode. Template: L0203 → 2× horizontal, 3× vertical

			};
		}
		

		public void ParseAndRender(Graphics g, string sbpl, string charToSplit)
		{
			var context = new SBPLContext();

			try
			{
				context.CurrentFont = new Font("Press Start 2P", 8); // Use installed font
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Could not load font 'Press Start 2P': " + ex.Message);
				context.CurrentFont = SystemFonts.DefaultFont;
			}

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
					//Debug.WriteLine(token);
					// Not an SBPL command? Skip it
					//g.DrawString(token, context.CurrentFont, Brushes.Black, context.X, context.Y);
					//context.Y += 20;
				}
			}
		}

	}
}
