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
            this.txtWipOrder = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.lblDebug = new System.Windows.Forms.Label();
            this.cbFileTemplates = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSBPL
            // 
            this.txtSBPL.Location = new System.Drawing.Point(12, 52);
            this.txtSBPL.Multiline = true;
            this.txtSBPL.Name = "txtSBPL";
            this.txtSBPL.Size = new System.Drawing.Size(263, 570);
            this.txtSBPL.TabIndex = 0;
            // 
            // btnRender
            // 
            this.btnRender.Location = new System.Drawing.Point(12, 628);
            this.btnRender.Name = "btnRender";
            this.btnRender.Size = new System.Drawing.Size(129, 23);
            this.btnRender.TabIndex = 1;
            this.btnRender.Text = "Parse like EDAM";
            this.btnRender.UseVisualStyleBackColor = true;
            this.btnRender.Click += new System.EventHandler(this.btnRender_Click);
            // 
            // picPreview
            // 
            this.picPreview.Location = new System.Drawing.Point(280, 52);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(510, 370);
            this.picPreview.TabIndex = 2;
            this.picPreview.TabStop = false;
            // 
            // btnRenderESC
            // 
            this.btnRenderESC.Location = new System.Drawing.Point(147, 628);
            this.btnRenderESC.Name = "btnRenderESC";
            this.btnRenderESC.Size = new System.Drawing.Size(128, 23);
            this.btnRenderESC.TabIndex = 3;
            this.btnRenderESC.Text = "Parse using <ESC>";
            this.btnRenderESC.UseVisualStyleBackColor = true;
            this.btnRenderESC.Click += new System.EventHandler(this.btnRenderESC_Click);
            // 
            // txtWipOrder
            // 
            this.txtWipOrder.Location = new System.Drawing.Point(12, 670);
            this.txtWipOrder.Name = "txtWipOrder";
            this.txtWipOrder.Size = new System.Drawing.Size(129, 20);
            this.txtWipOrder.TabIndex = 4;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(147, 669);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Load Wip Order";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblDebug
            // 
            this.lblDebug.AutoSize = true;
            this.lblDebug.Location = new System.Drawing.Point(12, 693);
            this.lblDebug.Name = "lblDebug";
            this.lblDebug.Size = new System.Drawing.Size(42, 13);
            this.lblDebug.TabIndex = 6;
            this.lblDebug.Text = "Debug:";
            // 
            // cbFileTemplates
            // 
            this.cbFileTemplates.FormattingEnabled = true;
            this.cbFileTemplates.Location = new System.Drawing.Point(13, 25);
            this.cbFileTemplates.Name = "cbFileTemplates";
            this.cbFileTemplates.Size = new System.Drawing.Size(114, 21);
            this.cbFileTemplates.TabIndex = 7;
            this.cbFileTemplates.SelectedIndexChanged += new System.EventHandler(this.cbFileTemplate_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Select file template:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 732);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbFileTemplates);
            this.Controls.Add(this.lblDebug);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtWipOrder);
            this.Controls.Add(this.btnRenderESC);
            this.Controls.Add(this.picPreview);
            this.Controls.Add(this.btnRender);
            this.Controls.Add(this.txtSBPL);
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
        private System.Windows.Forms.TextBox txtWipOrder;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblDebug;
        private System.Windows.Forms.ComboBox cbFileTemplates;
        private System.Windows.Forms.Label label1;
    }
}

