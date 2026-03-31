using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
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

        string connectionString =    "Server=BRES0056\\PMPB;Database=Transitions_Shop_Floor;Integrated Security=True;TrustServerCertificate=True;";
        string cbFileTemplatesString = string.Empty;
        public Form1()
		{
			InitializeComponent();
			loadCbFileTemplates();

		}
        private void loadCbFileTemplates()
        {
            string folderPath = Path.Combine(Application.StartupPath, "Example");

			if (!Directory.Exists(folderPath)) { 
				Console.WriteLine("Error loading path");
				return;
            }

            var files = Directory
                .EnumerateFiles(folderPath, "*.S84", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .ToList();


			files.Insert(0, "No value selected!");
			cbFileTemplates.DataSource = files;
        }
        private void btnRender_Click(object sender, EventArgs e)
		{
			string sbplCode = txtSBPL.Text;

			// Create a blank image
			int width = 510;
            int height = 370;
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
            bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);

            bmp.Save("C:\\APPS\\label.jpg");
			// Display it
			picPreview.Image = bmp;
		}

		private void btnRenderESC_Click(object sender, EventArgs e)
		{
			string sbplCode = txtSBPL.Text;

			// Create a blank image
			int width = 500;
			int height = 400;
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
        private DataTable GetCamLabelDetails()
        {
            string wipOrder = txtWipOrder.Text.Trim();

            DataTable result = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("CAM_Label_Details", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@WorkOrderID", wipOrder);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(result);
                    }
                }
            }
            catch (Exception ex) { 
                lblDebug.Text = "Debug: GetCamLabelDetails(): "+ ex.Message;
            }
            return result;
        }
        private string PAD(string value, int length, string side)
        {
            if (value == null)
                value = "";

            value = value.Trim();

            if (value.Length >= length)
                return value;

            int spaces = length - value.Length;

            switch (side.ToUpper())
            {
                case "LEFT":
                    return new string(' ', spaces) + value;

                case "RIGHT":
                    return value + new string(' ', spaces);

                case "CENTER":
                    int left = spaces / 2;
                    int right = spaces - left;
                    return new string(' ', left) + value + new string(' ', right);

                default:
                    return value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
			if (txtWipOrder.Text.Length == 0) {
                lblDebug.Text = "Debug: no wip order informed!";
			}
			else
			{
                DataTable labelInfo = GetCamLabelDetails();

                if (labelInfo.Rows.Count > 0)
                {
                    DataRow row = labelInfo.Rows[0];

                    lblDebug.Text = "Debug: "+ row["LabelCode"].ToString().Trim();

                    string folderPath = Path.Combine(Application.StartupPath, "Example");
                    string filePath = Path.Combine(folderPath, row["LabelCode"].ToString().Trim()+".S84");

                    cbFileTemplatesString = ReadFileContent(filePath);

                    cbFileTemplatesString = cbFileTemplatesString.ToUpper();

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@PROD_ID@", row["Prod_ID"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@PRDTYPE@", row["PrdType"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@TITLE@", PAD(row["Title"].ToString().Trim(), 26, "CENTER"));

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@TITLE2@", row["Title2"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@LTITLE@", row["Title"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@DIA@", row["dia"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@DIA2@", row["dia2"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@RX@", PAD(row["RX"].ToString().Trim(), 17, "CENTER"));

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@LRX@", row["RX"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@BARCODE@", row["Barcode"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@UPC@", row["UPC"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@THICK@", row["thick"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@BASE@", row["PrdBase"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@ADD@", row["PrdAdd"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@EYE@", row["PrdEye"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@M.OP_ID@", "BRE");

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@TC@", row["tc"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@NOMBASE@", row["NomBase"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@CHARTLINE@", row["chartline"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@INDEX@", row["Index"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@WORKCODE@", row["WorkCode"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@STOCKCODE@", row["StockCode"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@SEGL@", row["segl"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@SAG@", row["Sag"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@MCOUNTRY@", PAD(row["Country"].ToString().Trim(), 56, "RIGHT"));

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@MLOT@", row["WONUM"].ToString().Trim());

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@DAYCODE@", row["DayCode"].ToString().Trim());



                    string dataMatrix =
                        "240".Trim() +
                        row["barcode"].ToString().Trim() +
                        ("422" + "076" + "424" + "076" + "911").Trim() +
                        row["daycode"].ToString().Trim() +
                        "   " +
                        row["daycodelot"].ToString().Trim() +
                        "   " +
                        row["wonum"].ToString().Trim() +
                        "9200000000000000000";

                    cbFileTemplatesString = cbFileTemplatesString.Replace("@DATAMATRIX@", dataMatrix);
                    cbFileTemplatesString = cbFileTemplatesString.Replace("99999991", row["barcode"].ToString().Trim());



                    txtSBPL.Text = cbFileTemplatesString;

                    btnRender.PerformClick();
                }
                else
                {
                    lblDebug.Text = "Debug: No string loaded";
                }
            }
        }
        private string ReadFileContent(string filePath)
        {
            if (!File.Exists(filePath))
            {
                lblDebug.Text = "Debug: ReadFileContent: Does not exists -> " + filePath;
                return string.Empty;
            }
            return File.ReadAllText(filePath);
        }

        private void cbFileTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
			if (cbFileTemplates.SelectedIndex == 0) {
				txtSBPL.Text = "";
				return;
			}

            string folderPath = Path.Combine(Application.StartupPath, "Example");
            string filePath = Path.Combine(folderPath, cbFileTemplates.SelectedItem.ToString());


            cbFileTemplatesString = ReadFileContent(filePath);

            txtSBPL.Text = cbFileTemplatesString;

        }
    }
}
