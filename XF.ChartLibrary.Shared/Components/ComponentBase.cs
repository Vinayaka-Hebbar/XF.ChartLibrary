using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
    public interface IComponent
    {
        float XOffset { get; }

        float YOffset { get; }

        bool IsEnabled { get; }
    }

    public abstract partial class ComponentBase : IComponent
    {
        protected float xOffset;
        protected float yOffset;

        public float XOffset
        {
            get => xOffset;
            set
            {
#if __ANDROID__ || SKIASHARP
                xOffset = value.DpToPixel();
#else
                xOffset = value;
#endif
            }
        }

        public float YOffset
        {
            get => yOffset;
            set
            {
#if __ANDROID__ || SKIASHARP
                yOffset = value.DpToPixel();
#else
                yOffset = value;
#endif
            }
        }

        public bool IsEnabled { get; set; } = true;
    }
}
