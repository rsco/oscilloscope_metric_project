// ------------------------------------------------------------------
//
// A Winforms app that produces some metrics through
// data from UTFPR Oscilloscope.  Requires .net 4.0.
//
// Author: Rodrigo Cavalcanti
//
// ------------------------------------------------------------------
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace OscilloscopeMetrics
{
    public class OscilloscopeMetricsChart : Form
    {
        private System.ComponentModel.IContainer components = null;
        Chart chart1;

        private const string LINUX0 = "ociloscopio_dataset_linux_0.csv";
        private const string LINUX1 = "ociloscopio_dataset_linux_1.csv";
        private const string LINUX2 = "ociloscopio_dataset_linux_2.csv";
        private const string LINUX3 = "ociloscopio_dataset_linux_3.csv";
        private const string ROS0 = "ociloscopio_dataset_ros_0.csv";
        private const string ROS1 = "ociloscopio_dataset_ros_1.csv";
        private const string ROS2 = "ociloscopio_dataset_ros_2.csv";
        private const string ROS3 = "ociloscopio_dataset_ros_3.csv";
        private const string FILEPATH = @"C:\Users\oliveir\Desktop\Temp\{0}";
        private const string LINUX0_REPORT_NAME = "linux_metric_report_0.csv";
        private const string LINUX1_REPORT_NAME = "linux_metric_report_1.csv";
        private const string LINUX2_REPORT_NAME = "linux_metric_report_2.csv";
        private const string LINUX3_REPORT_NAME = "linux_metric_report_3.csv";
        private const string ROS0_REPORT_NAME = "ros_metric_report_0.csv";
        private const string ROS1_REPORT_NAME = "ros_metric_report_1.csv";
        private const string ROS2_REPORT_NAME = "ros_metric_report_2.csv";
        private const string ROS3_REPORT_NAME = "ros_metric_report_3.csv";

        List<decimal> tWriteList = new List<decimal>();
        List<decimal> tReadList = new List<decimal>();
        bool isWrite = false;

        public OscilloscopeMetricsChart(bool isWrite)
        {
            this.isWrite = isWrite;
            InitializeComponent();
        }

        private decimal PlotData(List<decimal> t, int i)
        {
            var jumpTime = t[i];
            return jumpTime;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GenerateData();
            chart1.Series.Clear();
            var series1 = new Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Line
            };

            this.chart1.Series.Add(series1);

            var operationList = isWrite ? tWriteList : tReadList;
            for (int i = 0; i < operationList.Count; i++)
            {
                series1.Points.AddXY(i, PlotData(operationList, i));
            }
            chart1.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ChartArea chartArea1 = new ChartArea();
            Legend legend1 = new Legend();
            this.chart1 = new Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            //
            // chart1
            //
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 50);
            this.chart1.Name = "chart1";
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Metric Chart";
            this.Load += new EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Responsible to generate data through oscilloscope files.
        /// </summary>
        public void GenerateData()
        {
            int cycle = 0;
            decimal writeSum = 0;
            decimal readSum = 0;
            var csv = new StringBuilder();
            var listResult = new List<string>();
            var filenameArr = new string[] { LINUX0, LINUX1, LINUX2, LINUX3, ROS0, ROS1, ROS2, ROS3 };

            //To change the file you'd like to get metrics,
            //you need just change the index. i.e filenameArr[INDEX]
            string fileName = filenameArr[0]; //LINUX0

            foreach (var line in File.ReadLines(string.Format(FILEPATH, fileName)))
            {
                listResult.Add(line);
            }

            var resultArr = listResult.ToArray();
            decimal writeTime = 0;
            decimal readTime = 0;
            decimal sampleRate = Decimal.Parse(resultArr[8].Split(',')[1].Trim(), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint);
            decimal writeJumps = 0;
            decimal readJumps = 0;

            for (int i = 16; i < resultArr.Count(); i++)
            {
                int number = int.Parse(resultArr[i].Split(',')[1].Trim());

                if (number - 1 >= 0)
                {
                    writeJumps = i;
                    writeTime = writeJumps * sampleRate;
                    tWriteList.Add(writeTime);
                }

                if (number - 2 >= 0)
                {
                    readJumps = i;
                    readTime = readJumps * sampleRate;
                    number -= 2;
                    tReadList.Add(writeTime);
                }

                if (number - 4 >= 0)
                {
                    writeJumps = i;
                    writeTime = writeJumps * sampleRate;
                    number -= 4;
                    tWriteList.Add(writeTime);
                }

                if (number - 8 >= 0)
                {
                    readJumps = i;
                    readTime = readJumps * sampleRate;
                    number -= 8;
                    tReadList.Add(writeTime);
                }

                if (writeTime != 0 || readTime != 0)
                {
                    cycle += 1;
                    writeSum += writeTime;
                    readSum += readTime;
                    var newline = string.Format("Cycle {2} - WriterUp: {0}, Cycle {2} - ReaderUp: {1}", writeTime, readTime, cycle);
                    csv.AppendLine(newline);
                    writeTime = 0;
                    readTime = 0;
                }
            }

            var newline2 = string.Empty;
            csv.AppendLine(newline2);

            var newline3 = string.Format("Writer Average: {0}, Reader Average: {1}", (decimal)writeSum / cycle, (decimal)readSum / cycle);
            csv.AppendLine(newline3);

            string reportName = string.Empty;

            switch (fileName)
            {
                case LINUX0:
                    reportName = LINUX0_REPORT_NAME;
                    break;
                case LINUX1:
                    reportName = LINUX1_REPORT_NAME;
                    break;
                case LINUX2:
                    reportName = LINUX2_REPORT_NAME;
                    break;
                case LINUX3:
                    reportName = LINUX3_REPORT_NAME;
                    break;
                case ROS0:
                    reportName = ROS0_REPORT_NAME;
                    break;
                case ROS1:
                    reportName = ROS1_REPORT_NAME;
                    break;
                case ROS2:
                    reportName = ROS2_REPORT_NAME;
                    break;
                case ROS3:
                    reportName = ROS3_REPORT_NAME;
                    break;
            }

            File.WriteAllText(string.Format(FILEPATH, reportName), csv.ToString());
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OscilloscopeMetricsChart(true));
        }
    }
}