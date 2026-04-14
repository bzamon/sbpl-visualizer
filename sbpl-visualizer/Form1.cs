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
		private const string OutputImagePath = "C:\\APPS\\label.jpg";

		string connectionString = "Server=BRES0056\\PMPB;Database=Transitions_Shop_Floor;Integrated Security=True;TrustServerCertificate=True;";
		string cbFileTemplatesString = string.Empty;

		public Form1()
		{
			InitializeComponent();
			loadCbFileTemplates();
			AppLogger.Info("Form initialized", nameof(Form1));
		}

		private void loadCbFileTemplates()
		{
			using (var scope = LogScope.BeginNested("RUN", "winforms://Form1/loadCbFileTemplates", nameof(Form1)))
			{
				string folderPath = Path.Combine(Application.StartupPath, "Example");

				if (!Directory.Exists(folderPath))
				{
					scope.SetStatus("missing_directory");
					AppLogger.Warn("Template directory missing", nameof(Form1), new Dictionary<string, object>
					{
						{ "directory_name", "Example" },
					});
					return;
				}

				var files = Directory
					.EnumerateFiles(folderPath, "*.S84", SearchOption.TopDirectoryOnly)
					.Select(Path.GetFileName)
					.ToList();

				files.Insert(0, "No value selected!");
				cbFileTemplates.DataSource = files;

				AppLogger.Info("Template enumeration completed", nameof(Form1), new Dictionary<string, object>
				{
					{ "template_count", Math.Max(0, files.Count - 1) },
				});
			}
		}

		private void btnRender_Click(object sender, EventArgs e)
		{
			using (var scope = LogScope.BeginNested("CLICK", "winforms://Form1/btnRender", nameof(Form1)))
			{
				try
				{
					RenderPreview(txtSBPL.Text, "\u001b", 510, 370, true, "raw-esc");
				}
				catch (Exception ex)
				{
					scope.Fail(ex, "Raw ESC render failed");
					throw;
				}
			}
		}

		private void btnRenderESC_Click(object sender, EventArgs e)
		{
			using (var scope = LogScope.BeginNested("CLICK", "winforms://Form1/btnRenderESC", nameof(Form1)))
			{
				try
				{
					RenderPreview(txtSBPL.Text, "<ESC>", 500, 400, false, "literal-esc");
				}
				catch (Exception ex)
				{
					scope.Fail(ex, "Literal ESC render failed");
					throw;
				}
			}
		}

		private void RenderPreview(string sbplCode, string splitToken, int width, int height, bool saveOutput, string renderMode)
		{
			using (var scope = LogScope.BeginNested("RUN", "winforms://Form1/RenderPreview/" + renderMode, nameof(Form1)))
			{
				AppLogger.Info("Preview render started", nameof(Form1), new Dictionary<string, object>
				{
					{ "image_width", width },
					{ "image_height", height },
					{ "save_output", saveOutput },
					{ "render_mode", renderMode },
				});

				Bitmap bmp = new Bitmap(width, height);

				using (Graphics g = Graphics.FromImage(bmp))
				{
					g.Clear(Color.White);
					g.InterpolationMode = InterpolationMode.NearestNeighbor;
					g.PixelOffsetMode = PixelOffsetMode.None;

					SBPLParser parser = new SBPLParser();
					parser.ParseAndRender(g, sbplCode, splitToken);

					if (saveOutput)
					{
						using (Pen blackPen = new Pen(Color.Black, 1))
						{
							g.DrawLine(blackPen, 0, 180, 530, 180);
						}
					}
				}

				if (saveOutput)
				{
					bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
					SavePreviewImage(bmp);
				}

				picPreview.Image = bmp;

				AppLogger.Info("Preview render completed", nameof(Form1), new Dictionary<string, object>
				{
					{ "image_width", width },
					{ "image_height", height },
					{ "render_mode", renderMode },
				});
			}
		}

		private void SavePreviewImage(Bitmap bmp)
		{
			using (var scope = LogScope.BeginNested("RUN", "winforms://Form1/SavePreviewImage", nameof(Form1)))
			{
				AppLogger.Info("Output image save started", nameof(Form1), new Dictionary<string, object>
				{
					{ "output_name", Path.GetFileName(OutputImagePath) },
					{ "image_width", bmp.Width },
					{ "image_height", bmp.Height },
				});

				bmp.Save(OutputImagePath);

				AppLogger.Info("Output image save completed", nameof(Form1), new Dictionary<string, object>
				{
					{ "output_name", Path.GetFileName(OutputImagePath) },
					{ "image_width", bmp.Width },
					{ "image_height", bmp.Height },
				});
			}
		}

		private DataTable GetCamLabelDetails()
		{
			using (var scope = LogScope.BeginNested("RUN", "winforms://Form1/GetCamLabelDetails", nameof(Form1)))
			{
				DataTable result = new DataTable();

				try
				{
					using (SqlConnection conn = new SqlConnection(connectionString))
					using (SqlCommand cmd = new SqlCommand("CAM_Label_Details", conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.AddWithValue("@WorkOrderID", txtWipOrder.Text.Trim());

						AppLogger.Info("Database call started", nameof(Form1), new Dictionary<string, object>
						{
							{ "stored_procedure", "CAM_Label_Details" },
						});

						using (SqlDataAdapter da = new SqlDataAdapter(cmd))
						{
							da.Fill(result);
						}
					}

					AppLogger.Info("Database call completed", nameof(Form1), new Dictionary<string, object>
					{
						{ "stored_procedure", "CAM_Label_Details" },
						{ "row_count", result.Rows.Count },
					});
				}
				catch (Exception ex)
				{
					scope.Fail(ex, "Database call failed");
					lblDebug.Text = "Debug: GetCamLabelDetails(): " + ex.Message;
				}

				return result;
			}
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
			using (var scope = LogScope.BeginNested("CLICK", "winforms://Form1/button1", nameof(Form1)))
			{
				try
				{
					if (txtWipOrder.Text.Length == 0)
					{
						scope.SetStatus("validation_failed");
						lblDebug.Text = "Debug: no wip order informed!";
						AppLogger.Warn("Work order load skipped: missing input", nameof(Form1));
						return;
					}

					DataTable labelInfo = GetCamLabelDetails();

					if (labelInfo.Rows.Count > 0)
					{
						DataRow row = labelInfo.Rows[0];
						string labelCode = row["LabelCode"].ToString().Trim();
						lblDebug.Text = "Debug: " + labelCode;

						string folderPath = Path.Combine(Application.StartupPath, "Example");
						string filePath = Path.Combine(folderPath, labelCode + ".S84");

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

						AppLogger.Info("Hydrated template loaded into editor", nameof(Form1), new Dictionary<string, object>
						{
							{ "template_name", Path.GetFileName(filePath) },
							{ "row_count", labelInfo.Rows.Count },
						});

						btnRender.PerformClick();
					}
					else
					{
						scope.SetStatus("not_found");
						lblDebug.Text = "Debug: No string loaded";
						AppLogger.Warn("Work order load returned no rows", nameof(Form1), new Dictionary<string, object>
						{
							{ "row_count", 0 },
						});
					}
				}
				catch (Exception ex)
				{
					scope.Fail(ex, "Work order load failed");
					throw;
				}
			}
		}

		private string ReadFileContent(string filePath)
		{
			using (var scope = LogScope.BeginNested("RUN", "winforms://Form1/ReadFileContent", nameof(Form1)))
			{
				string fileName = Path.GetFileName(filePath);

				AppLogger.Info("File read started", nameof(Form1), new Dictionary<string, object>
				{
					{ "file_name", fileName },
				});

				if (!File.Exists(filePath))
				{
					scope.SetStatus("missing_file");
					lblDebug.Text = "Debug: ReadFileContent: Does not exists -> " + filePath;
					AppLogger.Warn("Template file missing", nameof(Form1), new Dictionary<string, object>
					{
						{ "file_name", fileName },
					});
					return string.Empty;
				}

				try
				{
					string content = File.ReadAllText(filePath);

					AppLogger.Info("File read completed", nameof(Form1), new Dictionary<string, object>
					{
						{ "file_name", fileName },
						{ "char_count", content.Length },
					});

					return content;
				}
				catch (Exception ex)
				{
					scope.Fail(ex, "File read failed");
					throw;
				}
			}
		}

		private void cbFileTemplate_SelectedIndexChanged(object sender, EventArgs e)
		{
			using (var scope = LogScope.BeginNested("CHANGE", "winforms://Form1/cbFileTemplates", nameof(Form1)))
			{
				try
				{
					if (cbFileTemplates.SelectedIndex == 0)
					{
						scope.SetStatus("cleared");
						txtSBPL.Text = "";
						AppLogger.Info("Template selection cleared", nameof(Form1));
						return;
					}

					string folderPath = Path.Combine(Application.StartupPath, "Example");
					string filePath = Path.Combine(folderPath, cbFileTemplates.SelectedItem.ToString());

					cbFileTemplatesString = ReadFileContent(filePath);
					txtSBPL.Text = cbFileTemplatesString;

					AppLogger.Info("Template selection loaded", nameof(Form1), new Dictionary<string, object>
					{
						{ "template_name", Path.GetFileName(filePath) },
					});
				}
				catch (Exception ex)
				{
					scope.Fail(ex, "Template selection failed");
					throw;
				}
			}
		}
	}
}
