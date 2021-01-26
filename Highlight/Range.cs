namespace XF.ChartLibrary.Highlight
{
    public readonly struct Range
    {
        public readonly float From;
        public readonly float To;

        public Range(float from, float to)
        {
            From = from;
            To = to;
        }

		/// <summary>
		///  Returns true if this range contains (if the value is in between) the given value, false if not.
		/// </summary>
		public bool Contains(float value)
		{
			if (value > From && value <= To)
				return true;
			else
				return false;
		}

		public bool IsLarger(float value)
		{
			return value > To;
		}

		public bool IsSmaller(float value)
		{
			return value < From;
		}
	}
}
