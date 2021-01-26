using System.Collections.Generic;
using XF.ChartLibrary.Components;

namespace XF.ChartLibrary.Highlight
{
    public interface IHighlighter
    {
        Highlight GetClosestHighlightByPixel(IList<Highlight> closestValues, float x, float y, YAxisDependency axis, float minSelectionDistance);
        Highlight GetHighlight(float x, float y);
    }
}