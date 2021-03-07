#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
#endif

namespace XF.ChartLibrary.Charts
{
    public interface IPieRadarChartBase : IChartBase
    {
        int MaxVisibleCount { get; set; }
        float MinOffset { get; set; }
        float Radius { get; }
        float RequiredBaseOffset { get; }
        float RequiredLegendOffset { get; }

        int GetIndexForAngle(float angle);
        float DistanceToCenter(float x, float y);
        float GetAngleForPoint(float x, float y);
        Point GetPosition(float centerX, float centerY, float dist, float angle);
        Point GetPosition(Point center, float dist, float angle);
    }
}