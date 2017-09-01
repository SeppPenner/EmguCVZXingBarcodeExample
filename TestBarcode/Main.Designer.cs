namespace TestBarcode
{
    partial class Main
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
            this.ButtonStart = new System.Windows.Forms.Button();
            this.RichTextBoxText = new System.Windows.Forms.RichTextBox();
            this.RichTextBoxContent = new System.Windows.Forms.RichTextBox();
            this.PictureBoxImage = new System.Windows.Forms.PictureBox();
            this.TableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.TrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxImage)).BeginInit();
            this.TableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ButtonStart
            // 
            this.ButtonStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonStart.Location = new System.Drawing.Point(2, 2);
            this.ButtonStart.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(71, 77);
            this.ButtonStart.TabIndex = 0;
            this.ButtonStart.Text = "Bild konvertieren";
            this.ButtonStart.UseVisualStyleBackColor = true;
            this.ButtonStart.Click += new System.EventHandler(this.TestImage_Click);
            // 
            // RichTextBoxText
            // 
            this.RichTextBoxText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RichTextBoxText.Location = new System.Drawing.Point(77, 83);
            this.RichTextBoxText.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RichTextBoxText.Name = "RichTextBoxText";
            this.RichTextBoxText.Size = new System.Drawing.Size(71, 77);
            this.RichTextBoxText.TabIndex = 1;
            this.RichTextBoxText.Text = "";
            // 
            // RichTextBoxContent
            // 
            this.RichTextBoxContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RichTextBoxContent.Location = new System.Drawing.Point(77, 2);
            this.RichTextBoxContent.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.RichTextBoxContent.Name = "RichTextBoxContent";
            this.RichTextBoxContent.Size = new System.Drawing.Size(71, 77);
            this.RichTextBoxContent.TabIndex = 2;
            this.RichTextBoxContent.Text = "";
            // 
            // PictureBoxImage
            // 
            this.PictureBoxImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PictureBoxImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureBoxImage.Location = new System.Drawing.Point(152, 164);
            this.PictureBoxImage.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PictureBoxImage.Name = "PictureBoxImage";
            this.PictureBoxImage.Size = new System.Drawing.Size(583, 294);
            this.PictureBoxImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBoxImage.TabIndex = 3;
            this.PictureBoxImage.TabStop = false;
            // 
            // TableLayoutPanelMain
            // 
            this.TableLayoutPanelMain.ColumnCount = 3;
            this.TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.TableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanelMain.Controls.Add(this.ButtonStart, 0, 0);
            this.TableLayoutPanelMain.Controls.Add(this.RichTextBoxContent, 1, 0);
            this.TableLayoutPanelMain.Controls.Add(this.RichTextBoxText, 1, 1);
            this.TableLayoutPanelMain.Controls.Add(this.PictureBoxImage, 2, 2);
            this.TableLayoutPanelMain.Controls.Add(this.TrackBar, 2, 0);
            this.TableLayoutPanelMain.Controls.Add(this.label1, 2, 1);
            this.TableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanelMain.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TableLayoutPanelMain.Name = "TableLayoutPanelMain";
            this.TableLayoutPanelMain.RowCount = 3;
            this.TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.TableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanelMain.Size = new System.Drawing.Size(737, 460);
            this.TableLayoutPanelMain.TabIndex = 4;
            // 
            // TrackBar
            // 
            this.TrackBar.Location = new System.Drawing.Point(152, 2);
            this.TrackBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TrackBar.Maximum = 255;
            this.TrackBar.Minimum = 1;
            this.TrackBar.Name = "TrackBar";
            this.TrackBar.Size = new System.Drawing.Size(496, 45);
            this.TrackBar.TabIndex = 5;
            this.TrackBar.Value = 1;
            this.TrackBar.Scroll += new System.EventHandler(this.TrackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(152, 81);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "label1";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 460);
            this.Controls.Add(this.TableLayoutPanelMain);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "Main";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.PictureBoxImage)).EndInit();
            this.TableLayoutPanelMain.ResumeLayout(false);
            this.TableLayoutPanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.RichTextBox RichTextBoxText;
        private System.Windows.Forms.RichTextBox RichTextBoxContent;
        private System.Windows.Forms.PictureBox PictureBoxImage;
        private System.Windows.Forms.TableLayoutPanel TableLayoutPanelMain;
        private System.Windows.Forms.TrackBar TrackBar;
        private System.Windows.Forms.Label label1;
    }
}

