using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sbpl_visualizer
{
	public partial class Form1 : Form
	{
		private const int LabelWidth = 800;
		private const int LabelHeight = 600;

		public Form1()
		{
			InitializeComponent();
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			var sbpl = txtSBPL.Text;
			var image = RenderSBPL(sbpl);
			picPreview.Image = image;
		}

		private Bitmap RenderSBPL(string sbpl)
		{
			Bitmap bmp = new Bitmap(LabelWidth, LabelHeight);
			Graphics g = Graphics.FromImage(bmp);
			g.Clear(Color.White);

			int x = 0, y = 0;

			// Tokenize and parse simple commands
			var lines = sbpl.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				if (line.Contains("<ESC>"))
				{
					var tokens = Regex.Split(line, @"<ESC>");
					foreach (var token in tokens)
					{
						if (token.StartsWith("H"))
							x = int.Parse(token.Substring(1));
						else if (token.StartsWith("V"))
							y = int.Parse(token.Substring(1));
						else if (token.StartsWith("L"))
						{
							// Use default font for now
						}
						else if (!string.IsNullOrWhiteSpace(token))
						{
							g.DrawString(token, new Font("Arial", 12), Brushes.Black, x, y);
							y += 20;
						}
					}
				}
			}

			g.Dispose();
			return bmp;
		}
	}
}
