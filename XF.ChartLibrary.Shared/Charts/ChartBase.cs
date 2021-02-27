using System;
using System.Collections.Generic;
using XF.ChartLibrary.Animation;
using XF.ChartLibrary.Data;
using XF.ChartLibrary.Formatter;
using XF.ChartLibrary.Utils;
using XF.ChartLibrary.Components;
using System.Collections;
using XF.ChartLibrary.Interfaces;
using XF.ChartLibrary.Interfaces.DataSets;
using XF.ChartLibrary.Interfaces.DataProvider;
using XF.ChartLibrary.Renderer;

#if NETSTANDARD || SKIASHARP
using Point = SkiaSharp.SKPoint;
using Canvas = SkiaSharp.SKCanvas;
using Paint = SkiaSharp.SKPaint;
#elif __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Canvas = Android.Graphics.Canvas;
using Paint = Android.Graphics.Paint;
#endif


namespace XF.ChartLibrary.Charts
{

#if __ANDOIRD__ || SKIASHARP
    public enum PaintKind
    {
        GridBackground = 4,
        Info = 7,
        Description = 11,
        Hole = 13,
        CentreText = 14,
        LegendLabel = 18,
    }
#endif

    public abstract partial class ChartBase<TData, TDataSet> : IChartBase, IChartDataProvider, IAnimator
        where TData : IChartData<TDataSet> where TDataSet : IDataSet, IBarLineScatterCandleBubbleDataSet
    {
        /// flag that indicates if offsets calculation has already been done or not
        private bool offsetsCalculated = false;
        private Animator animator;
        private Description description = new Description();

        internal LegendRenderer legendRenderer;

        internal DataRenderer renderer;

        protected Highlight.Highlight LastHighlighted;

        private Listener.IChartSelectionListener selectionListener;

        public Listener.IChartSelectionListener SelectionListener
        {
            get => selectionListener;
            set => selectionListener = value;
        }

        public DataRenderer Renderer
        {
            get => renderer;
            set => renderer = value;
        }

        public LegendRenderer LegendRenderer
        {
            get => legendRenderer;
            set => legendRenderer = value;
        }

        protected readonly IList ViewPortJobs = ArrayList.Synchronized(new List<Jobs.ViewPortJob>());

        protected internal ViewPortHandler ViewPortHandler = new ViewPortHandler();

        internal Highlight.IHighlighter highlighter;

        public Highlight.IHighlighter Highlighter
        {
            get => highlighter;
            set => highlighter = value;
        }

        internal TData data;

        internal IList<Highlight.Highlight> indicesToHighlight;

        public IList<Highlight.Highlight> IndicesToHighlight
        {
            get => indicesToHighlight;
            set
            {
                indicesToHighlight = value;
            }
        }

        float IChartBase.ChartWidth => ViewPortHandler.ChartWidth;
        float IChartBase.ChartHeight => ViewPortHandler.ChartHeight;

        IChartData IChartBase.Data => (IChartData)data;

        ViewPortHandler IChartBase.ViewPortHandler => ViewPortHandler;

        public Animator Animator => animator;

        public bool IsDrawMarkersEnabled { get; set; } = true;

        public float ExtraTopOffset { get; set; }

        public float ExtraLeftOffset { get; set; }

        public float ExtraRightOffset { get; set; }

        public float ExtraBottomOffset { get; set; }

        /// <summary>
        /// default value-formatter, number of digits depends on provided chart-data
        /// </summary>
        protected DefaultValueFormatter DefaultValueFormatter = new DefaultValueFormatter(0);

        private float maxHighlightDistance;


        /// <summary>
        ///  Returns true if there are values to highlight, false if there are no
        /// values to highlight.Checks if the highlight array is null, has a length
        /// of zero or if the first object is null.
        /// </summary>
        public bool ValuesToHighlight
        {
            get
            {
                return indicesToHighlight != null && indicesToHighlight.Count > 0
               && indicesToHighlight[0] != null;
            }
        }

        public string NoDataText { get; set; } = "No Data";

        public abstract float YChartMax { get; }

        public abstract float YChartMin { get; }

        public abstract int MaxVisibleCount { get; set; }

        public float MaxHighlightDistance
        {
            get => maxHighlightDistance;
            set => maxHighlightDistance = value;
        }

        IChartData IChartDataProvider.Data => (IChartData)data;

        public virtual void Initialize()
        {
            maxHighlightDistance = 500f;
            Legend = new Legend();
            animator = new Animator()
            {
                Delegate = this
            };
            legendRenderer = new LegendRenderer(ViewPortHandler, Legend);

        }

#if  SKIASHARP
        /// <summary>
        /// set a new paint object for the specified parameter 
        /// </summary>
        /// <param name="p">the new paint object</param>
        /// <param name="kind">Paint type</param>
        public virtual void SetPaint(Paint p, PaintKind kind)
        {
            switch (kind)
            {
                case PaintKind.Info:
                    InfoPaint = p;
                    break;
                case PaintKind.Description:
                    DescPaint = p;
                    break;
            }
        }
        /// <summary>
        /// Returns the paint object associated with the provided constant.
        /// </summary>
        /// <param name="which">Which paint</param>
        /// <returns></returns>
        public virtual Paint GetPaint(PaintKind which)
        {
            switch (which)
            {
                case PaintKind.Info:
                    return InfoPaint;
                case PaintKind.Description:
                    return DescPaint;
            }

            return null;
        }
#endif

        /// calculates the required number of digits for the values that might be drawn in the chart (if enabled), and creates the default value formatter
        internal void SetUpDefaultFormatter(float min, float max)
        {
            // check if a custom formatter is set or not
            float reference;
            if (data == null || data.DataSets.Count < 2)
            {

                reference = Math.Max(Math.Abs(min), Math.Abs(max));
            }
            else
            {
                reference = Math.Abs(max - min);
            }

            int digits = reference.Digits();

            // setup the formatter with a new number of digits
            DefaultValueFormatter.Setup(digits);
        }

        /// <summary>
        /// Clears the chart from all data (sets it to null) and refreshes it (by
        /// calling invalidate()).
        /// </summary>
        public void Clear()
        {
            Data = default;
            offsetsCalculated = false;
            indicesToHighlight = null;
            LastHighlighted = null;
            this.InvalidateView();
        }

        /// <summary>
        /// Removes all DataSets (and thereby Entries) from the chart. Does not set the data object to null. Also refreshes the
        /// chart by calling invalidate().
        /// </summary>
        public void ClearValues()
        {
            data?.ClearValues();
            this.InvalidateView();
        }

        protected abstract void CalculateOffsets();

        /// <summary>
        /// Calculates the y-min and y-max value and the y-delta and x-delta value
        /// </summary>
        protected abstract void CalcMinMax();

        /// <summary>
        /// Lets the chart know its underlying data has changed and performs all
        /// necessary recalculations.It is crucial that this method is called
        /// everytime data is changed dynamically. Not calling this method can lead
        /// to crashes or unexpected behaviour.
        /// </summary>
        public abstract void NotifyDataSetChanged();

        /// <summary>
        /// draws all MarkerViews on the highlighted positions
        /// </summary>
        /// <param name="canvas">Canvas to draw</param>
        protected void DrawMarkers(Canvas canvas)
        {
            // if there is no marker view or drawing marker is disabled
            if (! (Marker is IMarker marker) || !IsDrawMarkersEnabled || !ValuesToHighlight)
                return;
            var data = Data;
            for (int i = 0; i < indicesToHighlight.Count; i++)
            {

                var highlight = indicesToHighlight[i];

                IDataSet set = data[highlight.DataSetIndex];

                Entry e = data.GetEntryForHighlight(indicesToHighlight[i]);
                int entryIndex = set.EntryIndex(e);

                // make sure entry not null
                if (e == null || entryIndex > set.EntryCount * animator.PhaseX)
                    continue;

                var pos = GetMarkerPosition(highlight);

#if __ANDROID__ && !SKIASHARP
                // check bounds
                if (!ViewPortHandler.IsInBounds(pos[0], pos[1]))
                    continue;

                // callbacks to update the content
                marker.RefreshContent(e, highlight);

                // draw the marker
                marker.Draw(canvas, pos[0], pos[1], this); 
#else
                // check bounds
                if (!ViewPortHandler.IsInBounds((float)pos.X, (float)pos.Y))
                    continue;

                // callbacks to update the content
                marker.RefreshContent(e, highlight);

                // draw the marker
                marker.Draw(canvas, pos, this);
#endif
            }
        }

        /// <summary>
        /// returns the DataSet object displayed at the touched position of the chart
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public IBarLineScatterCandleBubbleDataSet GetDataSetByTouchPoint(float x, float y)
        {
            Highlight.Highlight h = GetHighlightByTouchPoint(x, y);
            if (h != null)
            {
                return data[h.DataSetIndex];
            }
            return null;
        }

        /// <summary>
        /// Returns the Highlight object (contains x-index and DataSet index) of the
        /// selected value at the given touch point inside the Line-, Scatter-, or
        /// CandleStick-Chart.
        /// </summary>
        public virtual Highlight.Highlight GetHighlightByTouchPoint(float x, float y)
        {
            if (data == null)
                return default;
            return highlighter.GetHighlight(x, y);
        }

        /// <summary>
        /// /**
        /// Highlights the value selected by touch gesture.Unlike
        /// highlightValues(...), this generates a callback to the
        /// OnChartValueSelectedListener.
        /// </summary>
        /// <param name="high">the highlight object</param>
        /// <param name="callListener">call the listener</param>
        public void HighlightValue(Highlight.Highlight high, bool callListener)
        {
            Entry e = null;

            if (high == null)
                indicesToHighlight = null;
            else
            {
                e = data.GetEntryForHighlight(high);
                if (e == null)
                {
                    indicesToHighlight = null;
                    high = null;
                }
                else
                {
                    // set the indices to highlight
                    indicesToHighlight = new Highlight.Highlight[]{
                        high
                    };
                }
            }

            SetLastHighlighted(indicesToHighlight);

            if (callListener && selectionListener != null)
            {

                if (!ValuesToHighlight)
                    selectionListener.OnNothingSelected();
                else
                {
                    // notify the listener
                    selectionListener.OnValueSelected(e, high);
                }
            }

            // redraw the chart
            this.InvalidateView();
        }

#if __ANDROID__ && !SKIASHARP
        /// <summary>
        /// Returns the actual position in pixels of the MarkerView for the given
        /// Highlight object.
        protected float[] GetMarkerPosition(Highlight.Highlight value)
        {
            return new float[] { value.DrawX, value.DrawY };
        } 
#else
        /// <summary>
        /// Returns the actual position in pixels of the MarkerView for the given
        /// Highlight object.
        protected Point GetMarkerPosition(Highlight.Highlight value)
        {
            return new Point(value.DrawX, value.DrawY);
        }
#endif

        /// <summary>
        /// Sets the last highlighted value for the touchlistener.
        /// </summary>
        protected void SetLastHighlighted(IList<Highlight.Highlight> highs)
        {
            if (highs == null || highs.Count <= 0 || highs[0] == null)
            {
                LastHighlighted = null;
                //  mChartTouchListener.setLastHighlighted(null);
            }
            else
            {
                LastHighlighted = highs[0];
                // mChartTouchListener.setLastHighlighted(highs[0]);
            }
        }

        /// <summary>
        /// Either posts a job immediately if the chart has already setup it's
        /// dimensions or adds the job to the execution queue.
        /// </summary>
        public void AddViewportJob(Jobs.ViewPortJob job)
        {
            if (ViewPortHandler.HasChartDimens)
            {
                job.DoJob();
            }
            else
            {
                ViewPortJobs.Add(job);
            }
        }

        public void RemoveViewportJob(Jobs.ViewPortJob job)
        {
            ViewPortJobs.Remove(job);
        }

        public void ClearAllViewportJobs()
        {
            ViewPortJobs.Clear();
        }

    }
}
