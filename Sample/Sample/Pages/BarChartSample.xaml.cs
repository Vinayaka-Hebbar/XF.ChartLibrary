using Sample.Custom;
using SkiaSharp;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Highlight;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Listener;
using XF.ChartLibrary.Utils;

namespace Sample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarChartSample : IChartValueSelectionListener
    {
        private readonly Random random = new Random();

        public BarChartSample()
        {
            InitializeComponent();
            var chart = Chart;
            chart.ValueSelectionListener = this;
            chart.Description.IsEnabled = false;
            // if more than 60 entries are displayed in the chart, no values will be
            // drawn
            chart.MaxVisibleCount = 60;

            // scaling can now only be done on x- and y-axis separately
            chart.PinchZoomEnabled = false;

            chart.IsDrawGridBackground = false;
            // chart.setDrawYLabels(false);

            var xAxisFormatter = new DayAxisValueFormatter(chart);

            XAxis xAxis = chart.XAxis;
            xAxis.Position = XAxis.XAxisPosition.Bottom;
            xAxis.Typeface = FontManager.Default;
            xAxis.DrawGridLines = false;
            xAxis.Granularity = 1f; // only intervals of 1 day
            xAxis.LabelCount = 7;
            xAxis.ValueFormatter = xAxisFormatter;

            var custom = new MyAxisValueFormatter();

            YAxis leftAxis = chart.AxisLeft;
            leftAxis.Typeface = FontManager.Default;
            leftAxis.SetLabelCount(8, false);
            leftAxis.ValueFormatter = custom;
            leftAxis.Position = YAxis.YAxisLabelPosition.OutSideChart;
            leftAxis.SpacePercentTop = 15f;
            leftAxis.AxisMinimum = 0f; // this replaces setStartAtZero(true)

            YAxis rightAxis = chart.AxisRight;
            rightAxis.DrawGridLines = false;
            rightAxis.Typeface = FontManager.Default;
            rightAxis.SetLabelCount(8, false);
            rightAxis.ValueFormatter = custom;
            rightAxis.SpacePercentTop = 15f;
            rightAxis.AxisMinimum = 0f; // this replaces setStartAtZero(true)

            Legend l = chart.Legend;
            l.VerticalAlignment = VerticalAlignment.Bottom;
            l.HorizontalAlignment = HorizontalAlignment.Left;
            l.Orientation = Orientation.Horizontal;
            l.Form = Form.Square;
            l.FormSize = 9f;
            l.TextSize = 11f;
            l.XEntrySpace = 4f;

            var mv = new MarkerViewXY(xAxisFormatter);
            chart.Marker = mv; // Set the marker to the chart
            SetData(10, 20);
            SetChartTheme(Application.Current.RequestedTheme);
        }

        private void SetData(int count, float range)
        {
            float start = 1f;

            IList<BarEntry> values = new List<BarEntry>();

            for (int i = (int)start; i < start + count; i++)
            {
                float val = random.Next((int)range + 1);

                if (random.Next(100) < 25)
                {
                    values.Add(new BarEntry(i, val));
                }
                else
                {
                    values.Add(new BarEntry(i, val));
                }
            }

            BarDataSet set1;

            BarData data = Chart.Data;
            if (data != null &&
                    data.DataSetCount > 0)
            {
                set1 = (BarDataSet)data[0];
                set1.Entries = values;
                data.NotifyDataChanged();
                Chart.NotifyDataSetChanged();

            }
            else
            {
                set1 = new BarDataSet(values, "The year 2017")
                {
                    IsDrawIconsEnabled = false,
                    Colors = ColorTemplate.MaterialColors
                };

                IList<IBarDataSet> dataSets = new List<IBarDataSet>
                {
                    set1
                };

                data = new BarData(dataSets);
                data.SetValueTextSize(10f);
                data.SetValueTypeface(FontManager.Default);
                data.BarWidth = 0.9f;

                Chart.Data = data;
            }
        }

        public void OnNothingSelected() { }

        public void OnValueSelected(XF.ChartLibrary.Data.Entry e, Highlight h)
        {
            if (e == null)
                return;

            var bounds = Chart.GetBarBounds((BarEntry)e);
            var position = Chart.GetPosition(e, YAxisDependency.Left);
            System.Diagnostics.Trace.WriteLine(bounds.ToString(), "bounds");
            System.Diagnostics.Trace.WriteLine(position.ToString(), "position");
            System.Diagnostics.Trace.WriteLine(
                    "low: " + Chart.LowestVisibleX + ", high: "
                            + Chart.HighestVisibleX, "x-index");
        }

        protected override void OnThemeChanged(OSAppTheme theme)
        {
            SetChartTheme(theme);
            Chart.InvalidateSurface();
        }

        void SetChartTheme(OSAppTheme theme)
        {
            var textColor = theme == OSAppTheme.Dark ? SKColors.White : SKColors.Black;
            var chart = Chart;
            chart.AxisLeft.TextColor = textColor;
            chart.AxisRight.TextColor = textColor;
            chart.XAxis.TextColor = textColor;
            chart.Legend.TextColor = textColor;
        }
    }
}