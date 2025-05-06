using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace sbpl_visualizer
{
	public partial class Form1 : Form
	{
		private const int LabelWidth = 1050;
		private const int LabelHeight = 701;

		public Form1()
		{
			InitializeComponent();
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			string sbplCode = txtSBPL.Text;

			// Create a blank image
			int width = 1050;
			int height = 701;
			Bitmap bmp = new Bitmap((int)width, (int)height);
            
            using (Graphics g = Graphics.FromImage(bmp))
			{
				g.Clear(Color.White); // background
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.None;

                SBPLParser parser = new SBPLParser();
				parser.ParseAndRender(g, sbplCode, "\u001b");
                
				Pen blackPen = new Pen(Color.Black, 1);
				g.DrawLine(blackPen, 0, 180, 530, 180);


            }

            bmp.Save("C:\\APPS\\label.jpg");
			// Display it
			picPreview.Image = bmp;
		}

		private void btnRenderESC_Click(object sender, EventArgs e)
		{
			string sbplCode = txtSBPL.Text;

			// Create a blank image
			int width = 600;
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
