namespace sbpl_visualizer
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.txtSBPL = new System.Windows.Forms.TextBox();
            this.btnRender = new System.Windows.Forms.Button();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.btnRenderESC = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSBPL
            // 
            this.txtSBPL.Location = new System.Drawing.Point(16, 15);
            this.txtSBPL.Margin = new System.Windows.Forms.Padding(4);
            this.txtSBPL.Multiline = true;
            this.txtSBPL.Name = "txtSBPL";
            this.txtSBPL.Size = new System.Drawing.Size(349, 701);
            this.txtSBPL.TabIndex = 0;
            // 
            // btnRender
            // 
            this.btnRender.Location = new System.Drawing.Point(16, 724);
            this.btnRender.Margin = new System.Windows.Forms.Padding(4);
            this.btnRender.Name = "btnRender";
            this.btnRender.Size = new System.Drawing.Size(172, 28);
            this.btnRender.TabIndex = 1;
            this.btnRender.Text = "Parse like EDAM";
            this.btnRender.UseVisualStyleBackColor = true;
            this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
            // 
            // picPreview
            // 
            this.picPreview.Location = new System.Drawing.Point(373, 15);
            this.picPreview.Margin = new System.Windows.Forms.Padding(4);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(1050, 701);
            this.picPreview.TabIndex = 2;
            this.picPreview.TabStop = false;
            // 
            // btnRenderESC
            // 
            this.btnRenderESC.Location = new System.Drawing.Point(196, 724);
            this.btnRenderESC.Margin = new System.Windows.Forms.Padding(4);
            this.btnRenderESC.Name = "btnRenderESC";
            this.btnRenderESC.Size = new System.Drawing.Size(171, 28);
            this.btnRenderESC.TabIndex = 3;
            this.btnRenderESC.Text = "Parse using <ESC>";
            this.btnRenderESC.UseVisualStyleBackColor = true;
            this.btnRenderESC.Click += new System.EventHandler(this.btnRenderESC_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1491, 831);
            this.Controls.Add(this.btnRenderESC);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.btnRender);
            this.Controls.Add(this.txtSBPL);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "SBPL Visualizer";
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtSBPL;
		private System.Windows.Forms.Button btnRender;
		private System.Windows.Forms.PictureBox picPreview;
		private System.Windows.Forms.Button btnRenderESC;
	}
}

