﻿using SkiaSharp;
using Xamarin.Forms;

namespace XF.ChartLibrary.Utils
{
    static partial  class Extensions
    {
		public static Color ToFormsColor(this SKColor color) =>
			new Color(color.Red / 255.0, color.Green / 255.0, color.Blue / 255.0, color.Alpha / 255.0);
		
		public static SKColor ToSKColor(this Color color) =>
			new SKColor((byte)(color.R * 255), (byte)(color.G * 255), (byte)(color.B * 255), (byte)(color.A * 255));
	}
}