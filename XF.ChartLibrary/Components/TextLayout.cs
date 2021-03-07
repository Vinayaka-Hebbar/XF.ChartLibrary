using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using XF.ChartLibrary.Utils;

namespace XF.ChartLibrary.Components
{
    public class TextLayout : IDisposable
    {
        public const string NewLine = "\n";

        private TextAlignment verticalAlignment;

        public TextAlignment VerticalAlignment
        {
            get => verticalAlignment;
            set => verticalAlignment = value;
        }

        private TextAlignment horizontalAlign;
        public TextAlignment HorizontalAlign
        {
            get => horizontalAlign;
            set => horizontalAlign = value;
        }

        private SKPaint paint;

        private static IEnumerable<Span> Split(FormattedString text, char c)
        {
            var splits = new char[] { c };
            return text.Spans.SelectMany(r =>
            {
                if (r.Text == null)
                    return new Span[0];

                var returns = r.Text.Split(new[] { c }, StringSplitOptions.None);
                return returns.SelectMany((s, i) =>
                {
                    var result = new List<Span>();
                    if (i > 0)
                    {
                        result.Add(new Span(c.ToString())
                        {
                            TextSize = (float)r.FontSize,
                            LineHeight = (float)r.LineHeight,
                            Foreground = r.TextColor.ToSKColor(),
                            FontName = r.FontFamily,
                            Attributes = r.FontAttributes
                        });
                    }
                    if (!string.IsNullOrEmpty(s))
                    {
                        result.Add(new Span(s)
                        {
                            TextSize = (float)r.FontSize,
                            LineHeight = (float)r.LineHeight,
                            Foreground = r.TextColor.ToSKColor(),
                            Attributes = r.FontAttributes,
                            FontName = r.FontFamily
                        });
                    }
                    return result;
                });
            });
        }

        private static IEnumerable<Span> Split(IEnumerable<Span> spans, char c)
        {
            var splits = new char[] { c };
            return spans.SelectMany(r =>
            {
                if (r.Text == null)
                    return new Span[0];

                var returns = r.Text.Split(splits, StringSplitOptions.None);
                return returns.SelectMany((s, i) =>
                {
                    var result = new List<Span>();
                    if (i > 0)
                    {
                        result.Add(new Span(r)
                        {
                            Text = c.ToString(),
                        });
                    }
                    if (!string.IsNullOrEmpty(s))
                    {
                        result.Add(new Span(r)
                        {
                            Text = s,
                        });
                    }
                    return result;
                });
            });
        }

        public void Draw(SKCanvas canvas, FormattedString text, SKRect available)
        {
            // splittingLines
            var spans = Split(text, '\n');

            // Splitting words
            spans = Split(spans, ' ').ToList();

            var updatedSpans = new List<Span>();

            float y = 0, x = 0;
            SKRect bounds = SKRect.Empty;
            var frame = available.Size;
            int line = 0;
            paint = new SKPaint()
            {
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left
            };
            foreach (var span in spans)
            {
                paint.Typeface = span.Typeface;
                paint.TextSize = span.TextSize.DpToPixel();
                paint.FakeBoldText = span.Attributes.HasFlag(FontAttributes.Bold);

                var previousLine = line;

                if (span.Text == NewLine)
                {
                    var newLineHeight = UpdateLineHeight(line, updatedSpans, span.LineHeight);
                    line++;
                    x = 0;
                    y += newLineHeight;
                }
                else if (span.Text == " ")
                {
                    x += paint.MeasureText(span.Text);
                }
                else if (span.Text != null)
                {
                    if (span.Text.Length > 0)
                    {
                        paint.MeasureText(span.Text, ref bounds);

                        var shouldReturn = x > 0 && x + bounds.Width - bounds.Left > frame.Width + 1;

                        var lineHeight = span.LineHeight;
                        if (lineHeight == -1)
                        {
                            lineHeight = span.TextSize * 1.15f;
                        }
                        if (shouldReturn)
                        {
                            var newLineHeight = UpdateLineHeight(line, updatedSpans, lineHeight.DpToPixel());
                            line++;
                            x = 0;
                            y += newLineHeight;
                        }

                        updatedSpans.Add(new Span()
                        {
                            Text = span.Text,
                            Foreground = span.Foreground == SKColor.Empty ? SKColors.Black : span.Foreground,
                            TextSize = paint.TextSize,
                            Line = line,
                            Typeface = span.Typeface,
                            Bounds = bounds,
                            LayoutFrame = SKRect.Create(x, y, bounds.Width - bounds.Left, lineHeight.DpToPixel()),
                        });

                        x += bounds.Width;
                    }
                }
            }

            if (line == 0)
            {
                UpdateLineHeight(line, updatedSpans, 0);
            }

            var result = updatedSpans.ToArray();

            // Total size
            var h = updatedSpans.Count > 0 ? updatedSpans.Max(s => s.LayoutFrame.Bottom) - updatedSpans.Min(s => s.LayoutFrame.Top) : 0;
            var w = updatedSpans.Count > 0 ? updatedSpans.Max(s => s.LayoutFrame.Right) - updatedSpans.Min(s => s.LayoutFrame.Left) : 0;
            var offset = SKPoint.Empty;

            if (verticalAlignment == TextAlignment.Center)
            {
                offset.Y = available.Height / 2 - h / 2;
            }
            else if (verticalAlignment == TextAlignment.End)
            {
                offset.Y = available.Height - h;
            }
            if (horizontalAlign == TextAlignment.Center)
            {
                offset.X = available.Width / 2 - w / 2;
            }
            else if (horizontalAlign == TextAlignment.End)
            {
                offset.Y = available.Width - w;
            }
            foreach (var span in updatedSpans)
            {
                var area = SKRect.Create(offset.X + available.Left + span.LayoutFrame.Left - span.Bounds.Left, offset.Y + available.Top + span.LayoutFrame.Top, span.LayoutFrame.Width, span.LayoutFrame.Height);
                paint.Color = span.Foreground;
                paint.Typeface = span.Typeface;
                paint.FakeBoldText = span.Attributes.HasFlag(FontAttributes.Bold);
                paint.TextSize = span.TextSize;
                paint.Typeface = string.IsNullOrEmpty(span.FontName) ? SKTypeface.Default : GetTypeface(span.FontName, span.Attributes);
                paint.TextSkewX = span.Attributes.HasFlag(FontAttributes.Italic) ? 0.5f : 0f;
                canvas.DrawText(span.Text, area.Left, area.Bottom, paint);
            }
        }

        static SKTypeface GetTypeface(string fontName, FontAttributes attributes)
        {
            if (attributes == FontAttributes.Bold)
            {
                return SKFontManager.Default.MatchFamily(fontName, SKFontStyle.Bold);
            }
            else if (attributes == FontAttributes.Italic)
            {
                return SKFontManager.Default.MatchFamily(fontName, SKFontStyle.Italic);
            }
            return SKFontManager.Default.MatchFamily(fontName, SKFontStyle.Normal);
        }

        private static float UpdateLineHeight(int line, List<Span> spans, float lineHeight)
        {
            if (line == 0)
            {
                var height = spans.Max(s => -s.Bounds.Top);
                foreach (var span in spans)
                {
                    var f = span.LayoutFrame;
                    span.LayoutFrame = SKRect.Create(f.Left, f.Top, f.Width, height);
                }
                return height;
            }
            return lineHeight;
        }

        public void Dispose()
        {
            paint?.Dispose();
        }
    }
}
