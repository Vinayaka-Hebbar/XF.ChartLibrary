namespace XF.ChartLibrary.Utils
{
    public readonly struct ChartSize
    {
        public ChartSize(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; }
        public float Height { get; }
    }
}
