using System;

namespace XF.ChartLibrary.Animation
{
    public enum EasingOption
    {
        Linear,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
    }


    public delegate float EasingFunction(float value);

    public static class EasingFunctions
    {
        public static float Linear(float value) => value;

        public static float EaseInQuad(float value)
        {
            return value * value;
        }

        public static float EaseOutQuad(float position)
        {
            return -position * (position - 2.0f);
        }

        public static float EaseInOutQuad(float position)
        {
            position *= 2.0f;
            if (position < 1.0f)
            {
                return 0.5f * position * position;
            }

            return -0.5f * ((position - 1.0f) * (position - 3.0f) - 1.0f);
        }

        public static float EaseInCubic(float position)
        {
            return position * position * position;
        }

        public static float EaseOutCubic(float position)
        {
            position -= 1.0f;
            return (position * position * position + 1.0f);
        }

        public static float EaseInOutCubic(float position)
        {
            position *= 2.0f;
            if (position < 1.0f)
            {
                return 0.5f * position * position * position;
            }
            position -= 2.0f;
            return 0.5f * (position * position * position + 2.0f);
        }


        public static double EaseInQuart(double elapsed, double duration)
        {
            var position = (elapsed / duration);
            return position * position * position * position;
        }

        public static float EaseInQuart(float position)
        {
            return position * position * position * position;
        }

        public static float EaseOutQuart(float position)
        {
            position -= 1.0f;
            return -(position * position * position * position - 1.0f);
        }

        public static float EaseInOutQuart(float position)
        {
            position *= 2.0f;
            if (position < 1.0f)
            {
                return 0.5f * position * position * position * position;
            }
            position -= 2.0f;
            return -0.5f * (position * position * position * position - 2.0f);
        }


        public static float EaseInQuint(float position)
        {
            return position * position * position * position * position;
        }


        public static float EaseOutQuint(float position)
        {
            position -= 1.0f;
            return (position * position * position * position * position + 1.0f);
        }


        public static float EaseInOutQuint(float position)
        {
            position *= 2.0f;
            if (position < 1.0f)
            {
                return 0.5f * position * position * position * position * position;
            }
            else
            {
                position -= 2.0f;
                return 0.5f * (position * position * position * position * position + 2.0f);
            }
        }


        public static float EaseInSine(float position)
        {
            return (-MathF.Cos(position * MathF.PI / 2) + 1.0f);
        }


        public static float EaseOutSine(float position)
        {
            return (MathF.Sin(position * MathF.PI / 2));
        }


        public static float EaseInOutSine(float position)
        {
            return (-0.5f * (MathF.Cos(MathF.PI * position) - 1.0f));
        }


        public static float EaseInExpo(float position)
        {
            return (position == 0) ? 0.0f : (MathF.Pow(2.0f, 10.0f * (position - 1.0f)));
        }


        public static float EaseOutExpo(float position)
        {
            return (position == 1f) ? 1.0f : (-(MathF.Pow(2.0f, -10.0f * position)) + 1.0f);
        }


        public static float EaseInOutExpo(float position)
        {
            if (position == 0)
            {
                return 0.0f;
            }
            if (position == 1f)
            {
                return 1.0f;
            }


            position *= 2.0f;
            if (position < 1.0)
            {
                return (0.5f * MathF.Pow(2.0f, 10.0f * (position - 1.0f)));
            }


            position -= 1.0f;
            return (0.5f * (-MathF.Pow(2.0f, -10.0f * position) + 2.0f));
        }


        public static float EaseInCirc(float position)
        {
            return -((MathF.Sqrt(1.0f - position * position)) - 1.0f);
        }


        public static float EaseOutCirc(float position)
        {
            position -= 1.0f;
            return (MathF.Sqrt(1f - position * position));
        }


        public static float EaseInOutCirc(float position)
        {
            position *= 2.0f;
            if (position < 1.0f)
            {
                return (-0.5f * (MathF.Sqrt(1.0f - position * position) - 1.0f));
            }
            position -= 2.0f;
            return (0.5f * (MathF.Sqrt(1.0f - position * position) + 1.0f));
        }


        public static float EaseInElastic(float position)
        {
            if (position == 0f)
            {
                return 0.0f;
            }
            if (position == 1.0f)
            {
                return 1.0f;
            }


            var p = 0.3f;
            float s = p / MathF.PI * (float)Math.Asin(1f);
            return -((float)Math.Pow(2f, 10f * (position -= 1f))
                    * (float)Math.Sin((position - s) * MathF.PI / p));
        }


        public static float EaseOutElastic(float position)
        {
            if (position == 0f)
            {
                return 0.0f;
            }

            if (position == 1.0f)
            {
                return 1.0f;
            }

            float p = 0.3f;
            float s = p / MathF.PI * (float)Math.Asin(1f);
            return 1f
                    + (float)Math.Pow(2f, -10f * position)
                    * (float)Math.Sin((position - s) * MathF.PI / p);
        }


        public static float EaseInOutElastic(float position)
        {
            if (position == 0)
            {
                return 0.0f;
            }

            position *= 2.0f;
            if (position == 2.0)
            {
                return 1.0f;
            }


            float p = 1f / 0.45f;
            float s = 0.45f / MathF.PI * MathF.Asin(1f);
            if (position < 1f)
            {
                return -0.5f
                        * ((float)MathF.Pow(2f, 10f * (position -= 1f))
                        * (float)MathF.Sin((position * 1f - s) * MathF.PI * p));
            }
            return 1f + 0.5f
                    * (float)MathF.Pow(2f, -10f * (position -= 1f))
                    * (float)MathF.Sin((position * 1f - s) * MathF.PI * p);
        }

        const float s = 1.70158f;

        public static float EaseInBack(float position)
        {
            return (position * position * ((s + 1.0f) * position - s));
        }


        public static float EaseOutBack(float position)
        {
            position -= 1.0f;
            return (position * position * ((s + 1.0f) * position + s) + 1.0f);
        }


        public static float EaseInOutBack(float position)
        {
            var s = EasingFunctions.s;
            position *= 2.0f;
            if (position < 1.0)
            {
                s *= 1.525f;
                return (0.5f * (position * position * ((s + 1.0f) * position - s)));
            }
            s *= 1.525f;
            position -= 2.0f;
            return (0.5f * (position * position * ((s + 1.0f) * position + s) + 2.0f));
        }


        public static float EaseInBounce(float position)
        {
            return 1.0f - EaseOutBounce(1f - position);
        }


        public static float EaseOutBounce(float position)
        {
            if (position < (1.0f / 2.75f))
            {
                return (7.5625f * position * position);
            }
            else if (position < (2.0f / 2.75f))
            {
                position -= (1.5f / 2.75f);
                return (7.5625f * position * position + 0.75f);
            }
            else if (position < (2.5f / 2.75f))
            {
                position -= (2.25f / 2.75f);
                return (7.5625f * position * position + 0.9375f);
            }
            else
            {
                position -= (2.625f / 2.75f);
                return (7.5625f * position * position + 0.984375f);
            }
        }


        public static float EaseInOutBounce(float position)
        {
            if (position < 0.5f)
            {
                return EaseInBounce(position * 2.0f) * 0.5f;
            }
            return EaseOutBounce(position * 2.0f - 1f) * 0.5f + 0.5f;
        }

        public static EasingFunction EasingFunctionFromOption(EasingOption easing)
        {
            return easing switch
            {
                EasingOption.EaseInQuad => EaseInQuad,
                EasingOption.EaseOutQuad => EaseOutQuad,
                EasingOption.EaseInOutQuad => EaseInOutQuad,
                EasingOption.EaseInCubic => EaseInCubic,
                EasingOption.EaseOutCubic => EaseOutCubic,
                EasingOption.EaseInOutCubic => EaseInOutCubic,
                EasingOption.EaseInQuart => EaseInQuart,
                EasingOption.EaseOutQuart => EaseOutQuart,
                EasingOption.EaseInOutQuart => EaseInOutQuart,
                EasingOption.EaseInQuint => EaseInQuint,
                EasingOption.EaseOutQuint => EaseOutQuint,
                EasingOption.EaseInOutQuint => EaseInOutQuint,
                EasingOption.EaseInSine => EaseInSine,
                EasingOption.EaseOutSine => EaseOutSine,
                EasingOption.EaseInOutSine => EaseInOutSine,
                EasingOption.EaseInExpo => EaseInExpo,
                EasingOption.EaseOutExpo => EaseOutExpo,
                EasingOption.EaseInOutExpo => EaseInOutExpo,
                EasingOption.EaseInCirc => EaseInCirc,
                EasingOption.EaseOutCirc => EaseOutCirc,
                EasingOption.EaseInOutCirc => EaseInOutCirc,
                EasingOption.EaseInElastic => EaseInElastic,
                EasingOption.EaseOutElastic => EaseOutElastic,
                EasingOption.EaseInOutElastic => EaseInOutElastic,
                EasingOption.EaseInBack => EaseInBack,
                EasingOption.EaseOutBack => EaseOutBack,
                EasingOption.EaseInOutBack => EaseInOutBack,
                EasingOption.EaseInBounce => EaseInBounce,
                EasingOption.EaseOutBounce => EaseOutBounce,
                EasingOption.EaseInOutBounce => EaseInOutBounce,
                _ => Linear,
            };
        }
    }
}