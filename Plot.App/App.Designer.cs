namespace Plot.App
{
    partial class App
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
            formPlot1 = new WinForm.FormPlot();
            button1 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // formPlot1
            // 
            formPlot1.Dock = System.Windows.Forms.DockStyle.Fill;
            formPlot1.Location = new System.Drawing.Point(0, 0);
            formPlot1.Name = "formPlot1";
            formPlot1.Size = new System.Drawing.Size(615, 430);
            formPlot1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(0, 0);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "¿ªÊ¼/ÔÝÍ£";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // App
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(615, 430);
            Controls.Add(button1);
            Controls.Add(formPlot1);
            Name = "App";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion
        private WinForm.FormPlot formPlot1;
        private System.Windows.Forms.Button button1;
    }
}
