using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RandomnessChecker
{
    public partial class Form1 : Form
    {
        public Form1(Dictionary<String, List<DateTime>> data)
        {
            InitializeComponent();

            var myList = data.ToList();

            myList.Sort((pair1, pair2) => pair1.Value.Count.CompareTo(pair2.Value.Count));
            foreach (var line in myList)
            {
                this.chart1.Series["Series1"].Points.AddXY(line.Key, line.Value.Count);
            }
        }

        /**
         * 
         */
        private void Form1_Load(object sender, EventArgs e)
        {

            this.chart1.Legends.Clear();
            this.WindowState = FormWindowState.Maximized;
            this.AutoScroll = true;

            // Set mousewheel event
            this.chart1.MouseWheel += chart1_MouseWheel;
            this.chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            this.chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;

            // Set the text that appears when hovering over data
            this.chart1.Series["Series1"].ToolTip = "#VALX, #VAL";
            this.chart1.Width = 7000;
            this.chart1.Height = 1000;
        }

        /**
         * Zoom in on chart using mousewheel, but one down mouse wheel resets
         */
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var yAxis = chart.ChartAreas[0].AxisY;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {
                    xAxis.ScaleView.ZoomReset();
                    yAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var yMin = yAxis.ScaleView.ViewMinimum;
                    var yMax = yAxis.ScaleView.ViewMaximum;

                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 4;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 4;
                    var posYStart = yAxis.PixelPositionToValue(e.Location.Y) - (yMax - yMin) / 4;
                    var posYFinish = yAxis.PixelPositionToValue(e.Location.Y) + (yMax - yMin) / 4;

                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    yAxis.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
