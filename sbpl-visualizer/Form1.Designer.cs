﻿namespace sbpl_visualizer
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
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// txtSBPL
			// 
			this.txtSBPL.Location = new System.Drawing.Point(12, 67);
			this.txtSBPL.Multiline = true;
			this.txtSBPL.Name = "txtSBPL";
			this.txtSBPL.Size = new System.Drawing.Size(263, 292);
			this.txtSBPL.TabIndex = 0;
			// 
			// btnRender
			// 
			this.btnRender.Location = new System.Drawing.Point(12, 374);
			this.btnRender.Name = "btnRender";
			this.btnRender.Size = new System.Drawing.Size(263, 23);
			this.btnRender.TabIndex = 1;
			this.btnRender.Text = "button1";
			this.btnRender.UseVisualStyleBackColor = true;
			this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
			// 
			// picPreview
			// 
			this.picPreview.Location = new System.Drawing.Point(281, 67);
			this.picPreview.Name = "picPreview";
			this.picPreview.Size = new System.Drawing.Size(800, 600);
			this.picPreview.TabIndex = 2;
			this.picPreview.TabStop = false;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1118, 675);
			this.Controls.Add(this.picPreview);
			this.Controls.Add(this.btnRender);
			this.Controls.Add(this.txtSBPL);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtSBPL;
		private System.Windows.Forms.Button btnRender;
		private System.Windows.Forms.PictureBox picPreview;
	}
}

