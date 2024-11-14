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
            components = new System.ComponentModel.Container();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            button4 = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            button2 = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            plot1 = new WinForm.Plot();
            timer1 = new System.Windows.Forms.Timer(components);
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            tableLayoutPanel1.Controls.Add(button4, 3, 0);
            tableLayoutPanel1.Controls.Add(button3, 2, 0);
            tableLayoutPanel1.Controls.Add(button2, 1, 0);
            tableLayoutPanel1.Controls.Add(button1, 0, 0);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new System.Drawing.Size(615, 40);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // button4
            // 
            button4.Dock = System.Windows.Forms.DockStyle.Fill;
            button4.Location = new System.Drawing.Point(462, 3);
            button4.Name = "button4";
            button4.Size = new System.Drawing.Size(150, 34);
            button4.TabIndex = 3;
            button4.Text = "random number(1M)";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btn_oneMillionPoints;
            // 
            // button3
            // 
            button3.Dock = System.Windows.Forms.DockStyle.Fill;
            button3.Location = new System.Drawing.Point(309, 3);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(147, 34);
            button3.TabIndex = 2;
            button3.Text = "clear";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btn_clear;
            // 
            // button2
            // 
            button2.Dock = System.Windows.Forms.DockStyle.Fill;
            button2.Location = new System.Drawing.Point(156, 3);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(147, 34);
            button2.TabIndex = 1;
            button2.Text = "live signal (1K)";
            button2.UseVisualStyleBackColor = true;
            button2.Click += btn_animated_sine;
            // 
            // button1
            // 
            button1.Dock = System.Windows.Forms.DockStyle.Fill;
            button1.Location = new System.Drawing.Point(3, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(147, 34);
            button1.TabIndex = 0;
            button1.Text = "X/Y Pairs";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btn_xy_mode;
            // 
            // plot1
            // 
            plot1.Dock = System.Windows.Forms.DockStyle.Fill;
            plot1.Location = new System.Drawing.Point(0, 40);
            plot1.Name = "plot1";
            plot1.Size = new System.Drawing.Size(615, 390);
            plot1.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Interval = 1;
            timer1.Tick += timer1_Tick;
            // 
            // App
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(615, 430);
            Controls.Add(plot1);
            Controls.Add(tableLayoutPanel1);
            Name = "App";
            Text = "Form1";
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private WinForm.Plot plot1;
        private System.Windows.Forms.Timer timer1;
    }
}
