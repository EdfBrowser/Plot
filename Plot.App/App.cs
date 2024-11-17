using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();

            Text = "Plot.App";



        }

        private void btn_xy_mode(object sender, System.EventArgs e)
        {
            timer1.Enabled = false; // turn off live mode
            //plot1.PlotXY(plot1.Figure.Gen.Sequence(50), plot1.Figure.Gen.RandomWalk(50, 100));
            plot1.AxisAuto();
        }

        private void btn_animated_sine(object sender, System.EventArgs e)
        {
            plot1.Figure.AxisSet(0, .05, -1.1, 1.1); // we know what the limits should be
            //plot1.PlotSignal(plot1.Figure.Gen.SineAnimated(20000), 20000);
            timer1.Enabled = true; // start automatic updates
        }

        private void btn_oneMillionPoints(object sender, EventArgs e)
        {
            timer1.Enabled = false; // turn off live mode
            //plot1.PlotSignal(plot1.Figure.Gen.RandomWalk(1_000_000, startRandom: true), 20_000);
            plot1.AxisAuto();
        }

        private void btn_clear(object sender, EventArgs e)
        {
            timer1.Enabled = false; // turn off live mode
            //plot1.Clear(true);
        }

        private bool busyPlotting = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (busyPlotting) return;
            busyPlotting = true;
            //plot1.Clear();
            //plot1.PlotSignal(plot1.Figure.Gen.SineAnimated(20000), 20000);
            plot1.PlotSignal(
                new List<double[]>
                {
                plot1.Figure.Gen.SineAnimated(20000) ,
                plot1.Figure.Gen.SineAnimated(20000),

                }, 20000);

            Application.DoEvents();
            busyPlotting = false;
        }
    }
}
