﻿using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Utils;
#if __IOS__ || __TVOS__
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Canvas = Android.Graphics.Canvas;
#elif NETSTANDARD
using Canvas = SkiaSharp.SKCanvas;
#endif

namespace XF.ChartLibrary.Renderer
{
    public abstract partial class DataRenderer : ChartRenderer
    {
        protected DataRenderer(ViewPortHandler viewPortHandler) : base(viewPortHandler)
        {
            Initialize();
        }

        partial void Initialize();

        public abstract void DrawValues(Canvas c);
        public abstract void DrawData(Canvas c);
        public abstract void DrawExtras(Canvas c);

        public bool IsDrawingValuesAllowed(IChartProvider dataProvider)
        {
            return dataProvider.GetData().EntryCount > dataProvider.MaxVisibleCount
                * ViewPortHandler.ScaleX;
        }

        public abstract void DrawHighlighted(Canvas c, Highlight.Highlight[] indices);
    }
}
