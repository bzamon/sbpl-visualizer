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
			var noOpCommand = new NoOpCommand();

			commandMap = new Dictionary<string, ISBPLCommand>
			{
			{ "2D50", new SetDataMatrixCommand() },
			{ "DN", new DrawDataMatrixPreviewCommand() },
			{ "A3", new SetOffsetCommand() }, // Set Offset for MoveX and MoveY. Template: A3H100V050
			{ "AR", noOpCommand },
			{ "CS", noOpCommand },
			{ "#E", noOpCommand },
			{ "H", new MoveXCommand() }, 
			{ "V", new MoveYCommand() },
			{ "P", new SetCharacterPitchCommand() },
			{ "T", new DrawTextCommand() }, // Optional: command for plain text
			{ "S", new CompositeCommand(new SetSmallFontCommand(), new DrawTextCommand()) }, // Set Small Font and Write Text
			{ "M", new CompositeCommand(new SetMediumFontCommand(), new DrawTextCommand()) }, // Set Medium Font and Write Text
			{ "U", new CompositeCommand(new SetUFontCommand(), new DrawTextCommand()) },
			{ "B", new DrawBarcodeCommand() }, // Its fake barcode generated to simulate a ITF barcode
			{ "L", new SetScaleCommand() }, // To enlarge text or barcode. Template: L0203 → 2× horizontal, 3× vertical
			{ "%", new SetRotationCommand() },
			{ "A", noOpCommand },
			{ "N", noOpCommand },
			{ "Q", noOpCommand },
			{ "Z", noOpCommand },
			};
		}
		

		public void ParseAndRender(Graphics g, string sbpl, string charToSplit)
		{
			using (var scope = LogScope.BeginNested("RUN", "winforms://SBPLParser/ParseAndRender", nameof(SBPLParser)))
			{
				var context = new SBPLContext();

				try
				{
					context.CurrentFont = new Font("Press Start 2P", 8); // Use installed font
				}
				catch (Exception ex)
				{
					AppLogger.Warn("Font fallback activated", nameof(SBPLParser), new Dictionary<string, object>
					{
						{ "font_name", "Press Start 2P" },
						{ "fallback_font", SystemFonts.DefaultFont.Name },
					});
					AppLogger.Exception("Font initialization failed", nameof(SBPLParser), ex);
					context.CurrentFont = SystemFonts.DefaultFont;
				}

				var tokens = Regex.Split(sbpl, charToSplit);
				int nonEmptyTokenCount = 0;
				int executedCommandCount = 0;
				int ignoredTokenCount = 0;

				AppLogger.Info("Parser started", nameof(SBPLParser), new Dictionary<string, object>
				{
					{ "token_count", tokens.Length },
				});

				foreach (var token in tokens)
				{
					string normalizedToken = token.Trim((char)2, (char)3, '\r', '\n', ' ');

					if (string.IsNullOrWhiteSpace(normalizedToken))
						continue;

					nonEmptyTokenCount++;

					string commandKey = null;

					// Match longest possible command key from the registered ones
					foreach (var key in commandMap.Keys.OrderByDescending(k => k.Length))
					{
						if (normalizedToken.StartsWith(key))
						{
							commandKey = key;
							break;
						}
					}

					if (commandKey != null)
					{
						string argument = normalizedToken.Substring(commandKey.Length);

						try
						{
							commandMap[commandKey].Execute(g, context, argument);
							executedCommandCount++;
						}
						catch (Exception ex)
						{
							scope.SetStatus("error");
							AppLogger.Exception("Command execution failed", nameof(SBPLParser), ex, new Dictionary<string, object>
							{
								{ "command_key", commandKey },
							});
							throw;
						}
					}
					else
					{
						ignoredTokenCount++;
					}
				}

				AppLogger.Info("Parser completed", nameof(SBPLParser), new Dictionary<string, object>
				{
					{ "non_empty_token_count", nonEmptyTokenCount },
					{ "executed_command_count", executedCommandCount },
					{ "ignored_token_count", ignoredTokenCount },
				});
			}
		}

	}
}
