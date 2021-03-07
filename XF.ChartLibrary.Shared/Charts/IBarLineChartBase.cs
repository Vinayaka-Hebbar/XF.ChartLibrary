using SkiaSharp;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;


#if NETSTANDARD || SKIASHARP
using Rect = SkiaSharp.SKRect;
using Point = SkiaSharp.SKPoint;
using Color = SkiaSharp.SKColor;
using Paint = SkiaSharp.SKPaint;
#elif SKIASHARP
#elif __IOS__ || __TVOS__
using Rect = CoreGraphics.CGRect;
using Color = UIKit.UIColor;
using Point = CoreGraphics.CGPoint;
#elif __ANDROID__
using Rect = Android.Graphics.RectF;
using Paint = Android.Graphics.Paint;
using Point = Android.Graphics.PointF;
using Color = Android.Graphics.Color;
#endif

#if !NATIVE || NETSTANDARD 
using PlatformColor = Xamarin.Forms.Color;
#elif WPF
using PlatformColor = System.Windows.Media.Color;
#elif __IOS__ || __TVOS__
using PlatformColor = UIKit.UIColor;
#elif __ANDROID__
using PlatformColor = Android.Graphics.Color;
#endif


namespace XF.ChartLibrary.Charts
{
    public interface IBarLineChartBase : IChartBase
    {
        bool AutoScaleMinMaxEnabled { get; set; }
        YAxis AxisLeft { get; }
        YAxisRenderer AxisRendererLeft { get; set; }
        YAxisRenderer AxisRendererRight { get; set; }
        YAxis AxisRight { get; }
        Color BorderColor { get; set; }
        float BorderWidth { get; set; }
        bool ClipDataToContent { get; set; }
        bool ClipValuesToContent { get; set; }
        bool DoubleTapToZoomEnabled { get; set; }
        bool DragXEnabled { get; set; }
        bool DragYEnabled { get; set; }
        bool DrawBorders { get; set; }
#if NETSTANDARD
        Gestures.IChartGesture Gesture { get; } 
#endif
        PlatformColor GridBackgroundColor { get; set; }
        float HighestVisibleX { get; }
        bool HighlightPerDragEnabled { get; set; }
        bool HighlightPerTapEnabled { get; set; }
        bool IsAnyAxisInverted { get; }
        bool IsDragEnabled { get; }
        bool IsDrawGridBackground { get; set; }
        bool KeepPositionOnRotation { get; set; }
        float LowestVisibleX { get; }
        int MaxVisibleCount { get; set; }
        float MinOffset { get; set; }
        bool PinchZoomEnabled { get; set; }
        bool ScaleXEnabled { get; set; }
        bool ScaleYEnabled { get; set; }
        float VisibleXRange { get; }
        float VisibleXRangeMaximum { set; }
        float VisibleXRangeMinimum { set; }
        XAxisRenderer XAxisRenderer { get; set; }
        float YChartMax { get; }
        float YChartMin { get; }

        YAxis GetAxis(YAxisDependency axis);
        Point GetPosition(Data.Entry e, YAxisDependency axis);
        Transformer GetTransformer(YAxisDependency which);
        void Initialize();
        bool IsInverted(YAxisDependency axis);
        void MoveViewTo(float xValue, float yValue, YAxisDependency axis);
        void MoveViewToX(float xValue);
#if SKIASHARP
        void OnPaintSurface(SKSurface surface, SKImageInfo info); 
#endif
        void OnSizeChanged(float w, float h);
        void ResetViewPortOffsets();
#if __ANDROID__ || SKIASHARP
        void SetPaint(Paint p, PaintKind which);
        Paint GetPaint(PaintKind which);
#endif
        void SetVisibleXRange(float minXRange, float maxXRange);
        void SetVisibleYRange(float minYRange, float maxYRange, YAxisDependency axis);
        void SetVisibleYRangeMaximum(float maxYRange, YAxisDependency axis);
        void SetVisibleYRangeMinimum(float minYRange, YAxisDependency axis);
        void StopDeceleration();
        void Zoom(float scaleX, float scaleY, float x, float y);
    }
}