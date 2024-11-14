namespace Plot.WinForm
{
    partial class Plot
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new System.Windows.Forms.PictureBox();
            richTextBox1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = System.Drawing.Color.Navy;
            pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            pictureBox1.Location = new System.Drawing.Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(419, 374);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = System.Drawing.Color.DarkRed;
            richTextBox1.ForeColor = System.Drawing.SystemColors.Window;
            richTextBox1.Location = new System.Drawing.Point(76, 73);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new System.Drawing.Size(265, 112);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "Error Message";
            richTextBox1.Visible = false;
            // 
            // Plot
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(richTextBox1);
            Controls.Add(pictureBox1);
            Name = "Plot";
            Size = new System.Drawing.Size(419, 374);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}
