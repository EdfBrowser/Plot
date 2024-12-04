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
            checkBox1 = new System.Windows.Forms.CheckBox();
            panel1 = new System.Windows.Forms.Panel();
            comboBox1 = new System.Windows.Forms.ComboBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // formPlot1
            // 
            formPlot1.Dock = System.Windows.Forms.DockStyle.Fill;
            formPlot1.Location = new System.Drawing.Point(0, 31);
            formPlot1.Name = "formPlot1";
            formPlot1.Size = new System.Drawing.Size(615, 399);
            formPlot1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(0, 0);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "开始/暂停";
            button1.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.BackColor = System.Drawing.Color.White;
            checkBox1.Location = new System.Drawing.Point(81, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(75, 21);
            checkBox1.TabIndex = 3;
            checkBox1.Text = "自动缩放";
            checkBox1.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(checkBox1);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(615, 31);
            panel1.TabIndex = 4;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.ItemHeight = 17;
            comboBox1.Items.AddRange(new object[] { "1x", "2x", "4x" });
            comboBox1.Location = new System.Drawing.Point(162, 3);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(76, 25);
            comboBox1.TabIndex = 4;
            // 
            // App
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(615, 430);
            Controls.Add(formPlot1);
            Controls.Add(button1);
            Controls.Add(panel1);
            Name = "App";
            Text = "Form1";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private WinForm.FormPlot formPlot1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}
