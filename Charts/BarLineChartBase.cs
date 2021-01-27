using System;
using System.Collections.Generic;
using System.Text;
using XF.ChartLibrary.Components;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Renderer;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Charts
{
    public abstract partial class BarLineChartBase<TData, TDataSet> : ChartBase<TData, TDataSet> where TData :  IChartData<TDataSet> where TDataSet : IDataSet
    { /**
     * flag that indicates if pinch-zoom is enabled. if true, both x and y axis
     * can be scaled with 2 fingers, if false, x and y axis can be scaled
     * separately
     */
        protected bool mPinchZoomEnabled = false;

        /**
         * flag that indicates if double tap zoom is enabled or not
         */
        protected bool mDoubleTapToZoomEnabled = true;

        /**
         * flag that indicates if highlighting per dragging over a fully zoomed out
         * chart is enabled
         */
        protected bool mHighlightPerDragEnabled = true;

        /**
         * if true, dragging is enabled for the chart
         */
        private bool mDragXEnabled = true;
        private bool mDragYEnabled = true;

        private bool mScaleXEnabled = true;
        private bool mScaleYEnabled = true;


        /// <summary>
        /// the object representing the labels on the left y-axis
        /// </summary>
        protected YAxis mAxisLeft;

        /// <summary>
        /// the object representing the labels on the right y-axis
        /// </summary>
        protected YAxis mAxisRight;

        protected YAxisRenderer mAxisRendererLeft;
        protected YAxisRenderer mAxisRendererRight;

        protected Transformer mLeftAxisTransformer;
        protected Transformer mRightAxisTransformer;

        protected XAxisRenderer mXAxisRenderer;

        public BarLineChartBase()
        {
            mAxisLeft = new YAxis(YAxisDependency.Left);
            mAxisRight = new YAxis(YAxisDependency.Right);

            mLeftAxisTransformer = new Transformer(ViewPortHandler);
            mRightAxisTransformer = new Transformer(ViewPortHandler);

            mAxisRendererLeft = new YAxisRenderer(ViewPortHandler, mAxisLeft, mLeftAxisTransformer);
            mAxisRendererRight = new YAxisRenderer(ViewPortHandler, mAxisRight, mRightAxisTransformer);

            mXAxisRenderer = new XAxisRenderer(ViewPortHandler, xAxis, mLeftAxisTransformer);

        }
    }
}
