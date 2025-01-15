namespace Sandbox.WinForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.figureForm1 = new Plot.WinForm.FigureForm();
            this.SuspendLayout();
            // 
            // figureForm1
            // 
            this.figureForm1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.figureForm1.Location = new System.Drawing.Point(0, 0);
            this.figureForm1.Name = "figureForm1";
            this.figureForm1.Size = new System.Drawing.Size(800, 450);
            this.figureForm1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.figureForm1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private Plot.WinForm.FigureForm figureForm1;
    }
}
