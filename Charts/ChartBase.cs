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

#if __IOS__ || __TVOS__
using Point = CoreGraphics.CGPoint;
using Canvas = CoreGraphics.CGContext;
#elif __ANDROID__
using Point = Android.Graphics.PointF;
using Canvas = Android.Graphics.Canvas;
#elif NETSTANDARD
using Point = SkiaSharp.SKPoint;
using Canvas = SkiaSharp.SKCanvas;
#endif


namespace XF.ChartLibrary.Charts
{
    public abstract partial class ChartBase<TData, TDataSet> : IChartBase, IChartDataProvider where TData : IChartData<TDataSet> where TDataSet : IDataSet
    {
        /// flag that indicates if offsets calculation has already been done or not
        private bool offsetsCalculated = false;
        private Animator animator;
        private Description description = new Description();

        private Legend legend;

        protected XAxis xAxis;

        private Highlight.Highlight lastHighlighted;

        private IMarker marker;

        private TData data;

        private Listener.IChartSelectionListener selectionListener;

        protected readonly IList ViewPortJobs = ArrayList.Synchronized(new List<Jobs.ViewPortJob>());

        protected ViewPortHandler ViewPortHandler = new ViewPortHandler();

        protected Highlight.IHighlighter highlighter;

        public Highlight.IHighlighter Highlighter
        {
            get => highlighter;
            set => highlighter = value;
        }

        private IList<Highlight.Highlight> indicesToHighlight;

        public IList<Highlight.Highlight> IndicesToHighlight
        {
            get => indicesToHighlight;
            set
            {
                indicesToHighlight = value;
            }
        }

        public bool IsDrawMarkersEnabled { get; set; }

        /// <summary>
        /// default value-formatter, number of digits depends on provided chart-data
        /// </summary>
        protected DefaultValueFormatter DefaultValueFormatter = new DefaultValueFormatter(0);

        private float maxHighlightDistance;

        public TData Data
        {
            get => data;
            set
            {
                data = value;
                offsetsCalculated = false;
                if (value == null)
                    return;
                SetUpDefaultFormatter(value.YMin, value.YMax);
                foreach (TDataSet set in value.DataSets)
                {
                    if (set.NeedsFormatter || set.ValueFormatter == DefaultValueFormatter)
                        set.ValueFormatter = DefaultValueFormatter;
                }
                NotifyDataSetChanged();
            }
        }


        public bool ValuesToHighlight
        {
            get
            {
                return indicesToHighlight != null && indicesToHighlight.Count > 0
               && indicesToHighlight[0] != null;
            }
        }

        public string NoDataText { get; set; }

        public abstract float YChartMax { get; }

        public abstract float YChartMin { get; }

        public abstract int MaxVisibleCount { get; }

        public float MaxHighlightDistance
        {
            get => maxHighlightDistance;
            set => maxHighlightDistance = value;
        }

        IChartData<IDataSet> IChartDataProvider.Data => (IChartData<IDataSet>)data;

        public virtual void Initialize()
        {
            maxHighlightDistance = 500f;
            xAxis = new XAxis();
            legend = new Legend();
        }

        /// calculates the required number of digits for the values that might be drawn in the chart (if enabled), and creates the default value formatter
        internal void SetUpDefaultFormatter(float min, float max)
        {
            // check if a custom formatter is set or not
            float reference;
            if (data == null || data.DataSets.Count < 2)
            {

                reference = MathF.Max(MathF.Abs(min), MathF.Abs(max));
            }
            else
            {
                reference = MathF.Abs(max - min);
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
            data = default;
            offsetsCalculated = false;
            indicesToHighlight = null;
            lastHighlighted = null;
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
        /// Lets the chart know its underlying data has changed and performs all
        /// necessary recalculations.It is crucial that this method is called
        /// everytime data is changed dynamically. Not calling this method can lead
        /// to crashes or unexpected behaviour.
        /// </summary>
        public abstract void NotifyDataSetChanged();

        /**
             * draws all MarkerViews on the highlighted positions
             */
        protected void DrawMarkers(Canvas canvas)
        {

            // if there is no marker view or drawing marker is disabled
            if (marker == null || !IsDrawMarkersEnabled || !ValuesToHighlight)
                return;

            for (int i = 0; i < indicesToHighlight.Count; i++)
            {

                var highlight = indicesToHighlight[i];

                IDataSet set = data[highlight.DataSetIndex];

                Entry e = data.GetEntryForHighlight(indicesToHighlight[i]);
                int entryIndex = set.EntryIndex(e);

                // make sure entry not null
                if (e == null || entryIndex > set.EntryCount * animator.PhaseX)
                    continue;

                float[] pos = GetMarkerPosition(highlight);

                // check bounds
                if (!ViewPortHandler.IsInBounds(pos[0], pos[1]))
                    continue;

                // callbacks to update the content
                marker.RefreshContent(e, highlight);

                // draw the marker
                marker.Draw(canvas, pos[0], pos[1]);
            }
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

        /// <summary>
        /// Returns the actual position in pixels of the MarkerView for the given
        /// Highlight object.
        protected float[] GetMarkerPosition(Highlight.Highlight value)
        {
            return new float[] { value.DrawX, value.DrawY };
        }

        /// <summary>
        /// Sets the last highlighted value for the touchlistener.
        /// </summary>
        protected void SetLastHighlighted(IList<Highlight.Highlight> highs)
        {
            if (highs == null || highs.Count <= 0 || highs[0] == null)
            {
                lastHighlighted = null;
                //  mChartTouchListener.setLastHighlighted(null);
            }
            else
            {
                lastHighlighted = highs[0];
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
