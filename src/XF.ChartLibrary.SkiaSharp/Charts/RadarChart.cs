using SkiaSharp;

namespace XF.ChartLibrary.Charts
{
    partial class RadarChart
    {
        public override void OnPaintSurface(SKSurface surface, SKImageInfo e)
        {
            base.OnPaintSurface(surface, e);
            if (Data is null)
                return;
            var canvas = surface.Canvas;
            var xaxis = XAxis;
            if(xaxis.IsEnabled)
                XAxisRenderer.ComputeAxis(xaxis.axisMinimum, xaxis.axisMaximum, false);

            XAxisRenderer.RenderAxisLabels(canvas);

            if (DrawWeb)
                Renderer.DrawExtras(canvas);

            var yAxis = YAxis;
            if (yAxis.IsEnabled && yAxis.DrawLimitLinesBehindData)
                YAxisRenderer.RenderLimitLines(canvas);

            Renderer.DrawData(canvas);

            if (ValuesToHighlight)
                Renderer.DrawHighlighted(canvas, indicesToHighlight);

            if (yAxis.IsEnabled && !yAxis.DrawLimitLinesBehindData)
                YAxisRenderer.RenderLimitLines(canvas);

            YAxisRenderer.RenderAxisLabels(canvas);

            Renderer.DrawValues(canvas);

            LegendRenderer.RenderLegend(canvas);

            DrawDescription(canvas);

            DrawMarkers(canvas);
        }
    }
}
