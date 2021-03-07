namespace XF.ChartLibrary.Charts
{
    interface IGestureController
    {
        bool DragXEnabled { get;  }
        bool DragYEnabled { get;  }
        bool IsDragEnabled { get; }
        bool PinchZoomEnabled { get; }
        bool ScaleXEnabled { get; }
        bool ScaleYEnabled { get;  }
        bool HighlightPerDragEnabled { get; }
    }
}