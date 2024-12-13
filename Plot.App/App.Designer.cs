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
            comboBox1 = new System.Windows.Forms.ComboBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            comboBox3 = new System.Windows.Forms.ComboBox();
            groupBox6 = new System.Windows.Forms.GroupBox();
            checkBox2 = new System.Windows.Forms.CheckBox();
            groupBox7 = new System.Windows.Forms.GroupBox();
            button2 = new System.Windows.Forms.Button();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox7.SuspendLayout();
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
            flowLayoutPanel1.Controls.Add(groupBox4);
            flowLayoutPanel1.Controls.Add(groupBox6);
            flowLayoutPanel1.Controls.Add(groupBox7);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(109, 430);
            flowLayoutPanel1.TabIndex = 11;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Location = new System.Drawing.Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(100, 44);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "滚动效果";
            // 
            // comboBox1
            // 
            comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Scrolling", "Sweeping", "Stepping" });
            comboBox1.Location = new System.Drawing.Point(3, 19);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new System.Drawing.Size(94, 25);
            comboBox1.TabIndex = 1;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(comboBox3);
            groupBox4.Location = new System.Drawing.Point(3, 53);
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
            // groupBox6
            // 
            groupBox6.Controls.Add(checkBox2);
            groupBox6.Location = new System.Drawing.Point(3, 108);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new System.Drawing.Size(100, 49);
            groupBox6.TabIndex = 4;
            groupBox6.TabStop = false;
            groupBox6.Text = "网格线";
            // 
            // checkBox2
            // 
            checkBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            checkBox2.Location = new System.Drawing.Point(3, 19);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new System.Drawing.Size(94, 27);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "开启/关闭";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(button2);
            groupBox7.Location = new System.Drawing.Point(3, 163);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new System.Drawing.Size(100, 52);
            groupBox7.TabIndex = 6;
            groupBox7.TabStop = false;
            groupBox7.Text = "创建";
            // 
            // button2
            // 
            button2.Dock = System.Windows.Forms.DockStyle.Fill;
            button2.Location = new System.Drawing.Point(3, 19);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(94, 30);
            button2.TabIndex = 5;
            button2.Text = "创建图表";
            button2.UseVisualStyleBackColor = true;
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
            groupBox4.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox7.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private WinForm.FormPlot formPlot1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}
