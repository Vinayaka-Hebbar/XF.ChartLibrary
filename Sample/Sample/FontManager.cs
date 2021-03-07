using SkiaSharp;
using System;
using System.Reflection;
using System.Threading;

namespace Sample
{
    public static class FontManager
    {
        readonly static object Lock = new object();
        static readonly System.Collections.Hashtable CACHE = new System.Collections.Hashtable();

        static SKTypeface _default;
        public static SKTypeface Default
        {
            get
            {
                if (_default == null)
                {
                    Interlocked.Exchange(ref _default, GetFont("OpenSans-Light"));
                }
                return _default;
            }
        }

        static Assembly assemblyCache;
        public static Assembly ResourceAssembly
        {
            get
            {
                if (assemblyCache == null)
                {
                    assemblyCache = typeof(FontManager).Assembly;
                }
                return assemblyCache;
            }
        }

        public static SKTypeface GetFont(string fontName)
        {
            lock (Lock)
            {
                try
                {
                    if (!CACHE.ContainsKey(fontName))
                    {
                        SKTypeface typeface = CreateFont(fontName + ".ttf");
                        CACHE.Add(fontName, typeface);
                    }
                    return CACHE[fontName] as SKTypeface;
                }
                catch (Exception)
                {
                    return SKTypeface.Default;
                }
            }
        }

        static SKTypeface CreateFont(string file)
        {
            var stream = ResourceAssembly.GetManifestResourceStream("Sample.Assets." + file);
            if (stream == null)
                return SKTypeface.Default;

            return SKTypeface.FromStream(stream);
        }
    }
}
