using SkiaSharp;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Formatter;
using XF.ChartLibrary.Highlight;
using XF.ChartLibrary.Listener;
using XF.ChartLibrary.Utils;

namespace Sample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PieChartSample : IChartValueSelectionListener
    {
        protected readonly string[] parties = new string[] {
            "Party A", "Party B", "Party C", "Party D", "Party E", "Party F", "Party G", "Party H",
            "Party I", "Party J", "Party K", "Party L", "Party M", "Party N", "Party O", "Party P",
            "Party Q", "Party R", "Party S", "Party T", "Party U", "Party V", "Party W", "Party X",
            "Party Y", "Party Z"
        };

        private readonly Random random = new Random();

        public PieChartSample()
        {
            InitializeComponent();
            var chart = Chart;
            chart.UsePercenValuesEnabled = true;
            chart.Description.IsEnabled = false;
            chart.SetExtraOffsets(5, 10, 5, 5);

            chart.DragDecelerationFrictionCoef = 0.95f;

            chart.DrawHoleEnabled = true;
            chart.SetHoleColor(SKColors.White);

            chart.SetTransparentCircleColor(SKColors.White);
            chart.SetTransparentCircleAlpha(110);

            chart.HoleRadius = 58f;
            chart.TransparentCircleRadiusPercent = (61f);

            chart.DrawCenterTextEnabled = true;

            chart.RotationAngle = 0;
            // enable rotation of the chart by touch
            chart.RotationEnabled = true;
            chart.HighlightPerTapEnabled = true;

            // chart.setUnit(" €");
            // chart.setDrawUnitsInChart(true);

            // add a selection listener
            chart.ValueSelectionListener = this;

            var l = chart.Legend;
            l.VerticalAlignment = XF.ChartLibrary.Components.VerticalAlignment.Top;
            l.HorizontalAlignment = XF.ChartLibrary.Components.HorizontalAlignment.Right;
            l.Orientation = XF.ChartLibrary.Components.Orientation.Vertical;
            l.DrawInside = false;
            l.XEntrySpace = 7f;
            l.YEntrySpace = 0f;
            l.YOffset = 0f;

            // entry label styling
            chart.SetEntryLabelTypeface(FontManager.Default);
            chart.SetEntryLabelTextSize(12f);
            SetData(2, 20);
            SetChartTheme(Application.Current.RequestedTheme);
        }

        private readonly XF.ChartLibrary.Components.Path start = new XF.ChartLibrary.Components.Path(Icons.Star, SKStrokeCap.Round, SKStrokeJoin.Round)
        {
            Stroke = SKColors.Blue
        };

        private void SetData(int count, int range)
        {
            IList<PieEntry> entries = new List<PieEntry>();

            // NOTE: The order of the entries when being added to the entries array determines their position around the center of
            // the chart.
            for (int i = 0; i < count; i++)
            {
                entries.Add(new PieEntry((float)((random.Next(range)) + range / 5),
                        parties[i % parties.Length],
                        start));
            }
            PieDataSet dataSet = new PieDataSet(entries, "Election Results")
            {
                IsDrawIconsEnabled = false,

                SliceSpace = 3f,
                IconsOffset = new SKPoint(0, 40),
                SelectionShift = 5f
            };

            // add a lot of colors

            var colors = new List<SKColor>();

            colors.AddRange(ColorTemplate.VordiplomColors);
            colors.AddRange(ColorTemplate.JoyfulColors);
            colors.AddRange(ColorTemplate.LibreryColors);
            colors.AddRange(ColorTemplate.PastelColors);
            colors.AddRange(ColorTemplate.JoyfulColors);
            colors.Add(ColorTemplate.HoleBlue);


            dataSet.Colors = colors;
            //dataSet.setSelectionShift(0f);

            PieData data = new PieData(dataSet);
            data.SetValueFormatter(new PercentFormatter());
            data.SetValueTextSize(11f);
            data.SetValueTextColor(SKColors.White);
            data.SetValueTypeface(FontManager.Default);
            Chart.Data = data;

            // undo all highlights
            Chart.HighlightValues(null);

            Chart.InvalidateSurface();
        }


        public void OnNothingSelected()
        {
        }

        public void OnValueSelected(XF.ChartLibrary.Data.Entry e, Highlight h)
        {
        }

        protected override void OnAppearing()
        {
            Chart.Animator.AnimateY(1400, XF.ChartLibrary.Animation.EasingFunctions.EaseInOutQuad);
            // chart.spin(2000, 0, 360);
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
            chart.Legend.TextColor = textColor;
            chart.Data.SetValueTextColor(SKColors.Black);
            chart.SetEntryLabelColor(SKColors.Black);
        }
    }
}