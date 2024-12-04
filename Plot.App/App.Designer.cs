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
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            groupBox1 = new System.Windows.Forms.GroupBox();
            checkBox1 = new System.Windows.Forms.CheckBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            comboBox1 = new System.Windows.Forms.ComboBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            comboBox2 = new System.Windows.Forms.ComboBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            comboBox3 = new System.Windows.Forms.ComboBox();
            groupBox5 = new System.Windows.Forms.GroupBox();
            button1 = new System.Windows.Forms.Button();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // formPlot1
            // 
            formPlot1.Dock = System.Windows.Forms.DockStyle.Fill;
            formPlot1.Location = new System.Drawing.Point(109, 0);
            formPlot1.Name = "formPlot1";
            formPlot1.Size = new System.Drawing.Size(506, 430);
            formPlot1.TabIndex = 5;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Controls.Add(groupBox3);
            flowLayoutPanel1.Controls.Add(groupBox4);
            flowLayoutPanel1.Controls.Add(groupBox5);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(109, 430);
            flowLayoutPanel1.TabIndex = 11;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(100, 44);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "自动缩放";
            // 
            // checkBox1
            // 
            checkBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            checkBox1.Location = new System.Drawing.Point(3, 19);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new System.Drawing.Size(94, 22);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "开启/关闭";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(comboBox1);
            groupBox2.Location = new System.Drawing.Point(3, 53);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(97, 47);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "速率";
            // 
            // comboBox1
            // 
            comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "1", "2", "4" });
            comboBox1.Location = new System.Drawing.Point(3, 19);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(91, 25);
            comboBox1.TabIndex = 0;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(comboBox2);
            groupBox3.Location = new System.Drawing.Point(3, 106);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(100, 68);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "帧数";
            // 
            // comboBox2
            // 
            comboBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "15", "30", "60" });
            comboBox2.Location = new System.Drawing.Point(3, 19);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new System.Drawing.Size(94, 25);
            comboBox2.TabIndex = 0;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(comboBox3);
            groupBox4.Location = new System.Drawing.Point(3, 180);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(100, 49);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "通道数量";
            // 
            // comboBox3
            // 
            comboBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBox3.FormattingEnabled = true;
            comboBox3.Items.AddRange(new object[] { "1", "4", "10", "20", "30" });
            comboBox3.Location = new System.Drawing.Point(3, 19);
            comboBox3.Name = "comboBox3";
            comboBox3.Size = new System.Drawing.Size(94, 25);
            comboBox3.TabIndex = 0;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(button1);
            groupBox5.Location = new System.Drawing.Point(3, 235);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new System.Drawing.Size(100, 52);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = "启动";
            // 
            // button1
            // 
            button1.Dock = System.Windows.Forms.DockStyle.Fill;
            button1.Location = new System.Drawing.Point(3, 19);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(94, 30);
            button1.TabIndex = 5;
            button1.Text = "开始/暂停";
            button1.UseVisualStyleBackColor = true;
            // 
            // App
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(615, 430);
            Controls.Add(formPlot1);
            Controls.Add(flowLayoutPanel1);
            MinimumSize = new System.Drawing.Size(631, 469);
            Name = "App";
            Text = "Form1";
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private WinForm.FormPlot formPlot1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button button1;
    }
}
