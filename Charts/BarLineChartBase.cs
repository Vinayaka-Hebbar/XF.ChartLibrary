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
    public abstract class BarLineChartBase<TData, TDataSet> : ChartBase<TData, TDataSet> where TData :  IChartData<TDataSet> where TDataSet : IDataSet
    {
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

            mXAxisRenderer = new XAxisRenderer(ViewPortHandler, mXAxis, mLeftAxisTransformer);


            mChartTouchListener = new BarLineChartTouchListener(this, mViewPortHandler.getMatrixTouch(), 3f);

            mGridBackgroundPaint = new Paint();
            mGridBackgroundPaint.setStyle(Style.FILL);
            // mGridBackgroundPaint.setColor(Color.WHITE);
            mGridBackgroundPaint.setColor(Color.rgb(240, 240, 240)); // light
                                                                     // grey

            mBorderPaint = new Paint();
            mBorderPaint.setStyle(Style.STROKE);
            mBorderPaint.setColor(Color.BLACK);
            mBorderPaint.setStrokeWidth(Utils.convertDpToPixel(1f));
        }
    }
}
