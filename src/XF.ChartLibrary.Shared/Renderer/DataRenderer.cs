﻿using System.Collections.Generic;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Utils;


#if NETSTANDARD || SKIASHARP
using Canvas = SkiaSharp.SKCanvas;
#elif __IOS__ || __TVOS__
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Canvas = Android.Graphics.Canvas;
#endif

namespace XF.ChartLibrary.Renderer
{
    public abstract partial class DataRenderer : ChartRenderer
    {
        protected readonly Animation.Animator Animator;

        protected DataRenderer(Animation.Animator animator, ViewPortHandler viewPortHandler) : base(viewPortHandler)
        {
            Animator = animator;
        }

        public abstract void DrawValues(Canvas c);
        public abstract void DrawData(Canvas c);
        public abstract void DrawExtras(Canvas c);


        public abstract void InitBuffers();

        public virtual bool IsDrawingValuesAllowed(IChartDataProvider dataProvider)
        {
            return dataProvider.Data.EntryCount > dataProvider.MaxVisibleCount
                * ViewPortHandler.ScaleX;
        }

        public abstract void DrawHighlighted(Canvas c, IList<Highlight.Highlight> indices);
    }
}
