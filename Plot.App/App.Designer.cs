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
            panel1 = new System.Windows.Forms.Panel();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            nud_sec = new System.Windows.Forms.NumericUpDown();
            PlotSignalBtn = new System.Windows.Forms.Button();
            PlotLineBtn = new System.Windows.Forms.Button();
            plot1 = new WinForm.Plot();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nud_sec).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(nud_sec);
            panel1.Controls.Add(PlotSignalBtn);
            panel1.Controls.Add(PlotLineBtn);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(800, 100);
            panel1.TabIndex = 1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(549, 41);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(142, 17);
            label3.TabIndex = 5;
            label3.Text = "seconds of 500Hz data";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(549, 24);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(142, 17);
            label2.TabIndex = 4;
            label2.Text = "seconds of 500Hz data";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(549, 7);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(142, 17);
            label1.TabIndex = 3;
            label1.Text = "seconds of 500Hz data";
            // 
            // nud_sec
            // 
            nud_sec.Location = new System.Drawing.Point(465, 7);
            nud_sec.Name = "nud_sec";
            nud_sec.Size = new System.Drawing.Size(78, 23);
            nud_sec.TabIndex = 2;
            nud_sec.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // PlotSignalBtn
            // 
            PlotSignalBtn.Location = new System.Drawing.Point(375, 7);
            PlotSignalBtn.Name = "PlotSignalBtn";
            PlotSignalBtn.Size = new System.Drawing.Size(84, 25);
            PlotSignalBtn.TabIndex = 1;
            PlotSignalBtn.Text = "PlotSignal";
            PlotSignalBtn.UseVisualStyleBackColor = true;
            // 
            // PlotLineBtn
            // 
            PlotLineBtn.Location = new System.Drawing.Point(285, 7);
            PlotLineBtn.Name = "PlotLineBtn";
            PlotLineBtn.Size = new System.Drawing.Size(84, 22);
            PlotLineBtn.TabIndex = 0;
            PlotLineBtn.Text = "PlotLine";
            PlotLineBtn.UseVisualStyleBackColor = true;
            // 
            // plot1
            // 
            plot1.Dock = System.Windows.Forms.DockStyle.Fill;
            plot1.Location = new System.Drawing.Point(0, 100);
            plot1.Name = "plot1";
            plot1.Size = new System.Drawing.Size(800, 350);
            plot1.TabIndex = 2;
            // 
            // App
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(plot1);
            Controls.Add(panel1);
            Name = "App";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nud_sec).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button PlotSignalBtn;
        private System.Windows.Forms.Button PlotLineBtn;
        private System.Windows.Forms.NumericUpDown nud_sec;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private WinForm.Plot plot1;
    }
}
