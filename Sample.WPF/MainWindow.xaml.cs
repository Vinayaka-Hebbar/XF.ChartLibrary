﻿using System.Windows;
using XF.ChartLibrary.Charts;
using XF.ChartLibrary.Data;

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var entries = new Entry[]
            {
                new Entry(0,0),
                new Entry(10,13),
                new Entry(15,10),
                new Entry(20,4)
            };
            var dataSets = new LineDataSet[]
            {
                new LineDataSet(entries, "Sample")
                {
                    ValueTextColor = SkiaSharp.SKColors.Black,
                    Mode = LineDataSet.LineMode.CubicBezier,
                    DrawFilled = true,
                    Colors = new SkiaSharp.SKColor[0]
                }
            };
            Root.Content = new LineChart()
            {
                MaxVisibleCount = 2,
                Data = new LineData(dataSets),
                XAxis =
                {
                    Position = XF.ChartLibrary.Components.XAxis.XAxisPosition.Bottom
                },
                AxisRight =
                {
                    IsEnabled = false
                }
            };
        }
    }
}