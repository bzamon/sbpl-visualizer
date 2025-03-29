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
		private SBPLParser SBPLParser;

		public Form1()
		{
			InitializeComponent();
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			string sbplCode = txtSBPL.Text;

			// Create a blank image
			int width = 800;
			int height = 600;
			Bitmap bmp = new Bitmap(width, height);

			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.White); // background

				SBPLParser parser = new SBPLParser();
				parser.ParseAndRender(g, sbplCode, "\u001b");
			}

			// Display it
			picPreview.Image = bmp;
		}

		private void btnRenderESC_Click(object sender, EventArgs e)
		{
			string sbplCode = txtSBPL.Text;

			// Create a blank image
			int width = 800;
			int height = 600;
			Bitmap bmp = new Bitmap(width, height);

			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.White); // background

				SBPLParser parser = new SBPLParser();
				parser.ParseAndRender(g, sbplCode, "<ESC>");
			}

			// Display it
			picPreview.Image = bmp;
		}
	}
}
