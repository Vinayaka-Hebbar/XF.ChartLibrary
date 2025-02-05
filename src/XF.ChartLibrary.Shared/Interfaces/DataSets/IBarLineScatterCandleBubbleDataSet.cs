﻿
#if NETSTANDARD || SKIASHARP
using Color = SkiaSharp.SKColor;
#elif __IOS__ || __TVOS
using Color = UIKit.UIColor;
#elif __ANDROID__
using Color = Android.Graphics.Color;
#endif
using XF.ChartLibrary.Data;
namespace XF.ChartLibrary.Interfaces.DataSets
{
    public interface IBarLineScatterCandleBubbleDataSet<TEntry> : IBarLineScatterCandleBubbleDataSet, IDataSet<TEntry> where TEntry : Entry
    {
    }

    public interface IBarLineScatterCandleBubbleDataSet : IDataSet
    {
        Color HighLightColor { get; }
    }
}