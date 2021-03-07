using SkiaSharp;
using System;

namespace XF.ChartLibrary.Components
{
    public class BitmapDrawable : SKBitmap, IDrawable
    {
        public BitmapDrawable(SKImageInfo info, int rowBytes) : base(info, rowBytes)
        {
        }

        public BitmapDrawable(int width, int height, bool isOpaque = false) : base(width, height, isOpaque)
        {
        }

        public BitmapDrawable(SKImageInfo info) : base(info)
        {
        }

        public void Draw(SKCanvas canvas, float x, float y)
        {
            canvas.DrawBitmap(this, x, y);
        }

        public static BitmapDrawable From(SKCodec codec)
        {
            if (codec == null)
            {
                throw new ArgumentNullException(nameof(codec));
            }
            var info = codec.Info;
            if (info.AlphaType == SKAlphaType.Unpremul)
            {
                info.AlphaType = SKAlphaType.Premul;
            }
            // for backwards compatibility, remove the colorspace
            info.ColorSpace = null;
            return From(codec, info);
        }

        public static BitmapDrawable From(SKCodec codec, SKImageInfo bitmapInfo)
        {
            if (codec == null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            var bitmap = new BitmapDrawable(bitmapInfo);
            var result = codec.GetPixels(bitmapInfo, bitmap.GetPixels(out var length));
            if (result != SKCodecResult.Success && result != SKCodecResult.IncompleteInput)
            {
                bitmap.Dispose();
                bitmap = null;
            }
            return bitmap;
        }

        public static BitmapDrawable From(System.IO.Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            using (var codec = SKCodec.Create(stream))
            {
                if (codec == null)
                {
                    return null;
                }
                return From(codec);
            }
        }

        public static BitmapDrawable From(SKImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var info = new SKImageInfo(image.Width, image.Height, SKImageInfo.PlatformColorType, image.AlphaType);
            var bmp = new BitmapDrawable(info);
            if (!image.ReadPixels(info, bmp.GetPixels(), info.RowBytes, 0, 0))
            {
                bmp.Dispose();
                bmp = null;
            }
            return bmp;
        }


        public static BitmapDrawable From(SKData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            using (var codec = SKCodec.Create(data))
            {
                if (codec == null)
                {
                    return null;
                }
                return From(codec);
            }
        }
    }
}
