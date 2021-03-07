using SkiaSharp;

namespace XF.ChartLibrary.Renderer
{
    partial class PieChartRenderer
    {
        protected SKPath DrawCenterTextPathBuffer = new SKPath();

        /// <summary>
        ///  draws the description text in the center of the pie chart makes most
        /// sense when center-hole is enabled
        /// </summary>
        /// <param name="c"></param>
        partial void DrawCenterText(SKCanvas c)
        {

            var centerText = Chart.CenterText;

            if (Chart.DrawCenterTextEnabled && centerText != null)
            {

                var center = Chart.CenterCircleBox;
                var offset = Chart.CenterTextOffset;

                float x = center.X + offset.X;
                float y = center.Y + offset.Y;

                float innerRadius = Chart.DrawHoleEnabled && !Chart.DrawSlicesUnderHoleEnabled
                        ? Chart.Radius * (Chart.HoleRadius / 100f)
                        : Chart.Radius;

                var holeRect = SKRect.Empty;
                holeRect.Left = x - innerRadius;
                holeRect.Top = y - innerRadius;
                holeRect.Right = x + innerRadius;
                holeRect.Bottom = y + innerRadius;
                var boundingRect = holeRect;

                float radiusPercent = Chart.CenterTextRadiusPercent / 100f;
                if (radiusPercent > 0.0)
                {
                    boundingRect = boundingRect.Inset(
                            (boundingRect.Width - boundingRect.Width * radiusPercent) / 2.0f,
                            (boundingRect.Height - boundingRect.Height * radiusPercent) / 2.0f
                    );
                }
                //float layoutWidth = Utils.getStaticLayoutMaxWidth(mCenterTextLayout);

                c.Save();
                var path = DrawCenterTextPathBuffer;
                path.Reset();
                path.AddOval(holeRect, SKPathDirection.Clockwise);
                c.ClipPath(path);

                using (var layout = new Components.TextLayout()
                {
                    VerticalAlignment = Xamarin.Forms.TextAlignment.Center,
                    HorizontalAlign = Xamarin.Forms.TextAlignment.Center
                })
                {
                    layout.Draw(c, centerText, boundingRect);
                }

                c.Restore();
            }
        }
    }
}
