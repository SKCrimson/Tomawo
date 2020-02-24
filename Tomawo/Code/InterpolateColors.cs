using System.Collections.Generic;
using System.Windows.Media;
// ReSharper disable SuggestVarOrType_Elsewhere

namespace Tomawo.Code
{
    internal class InterpolateColors
    {
        #region Constructors

        internal InterpolateColors(int seconds)
        {
            if (seconds <= 180)
            {
                RgbLinearInterpolate(seconds);
                _pace = 0;
            }
            else
            {
                int divider = 180;

                while (seconds % divider != 0)
                    divider++;

                RgbLinearInterpolate(divider);
                _pace = seconds / divider;
            }

            _number = 1;
            _tick = 1;
        }

        #endregion

        #region Variables

        private List<Color> _colors;
        private readonly int _pace;
        private int _number;
        private int _tick;

        #endregion

        #region Methods

        internal bool CheckColorBrush()
        {
            if (_pace == 0)
                return true;

            if (_tick == _pace)
            {
                _tick = 1;
                return true;
            }

            _tick++;
            return false;
        }
        internal SolidColorBrush GetColorBrush()
        {
            _number++;
            return _number > (_colors.Count - 1) ? new SolidColorBrush(Color.FromArgb(255, 205, 40, 10)) : new SolidColorBrush(_colors[_number]);
        }

        private static List<Color> RgbLinearInterpolate(Color start, Color end, int colorCount)
        {
            List<Color> colors = new List<Color>();

            for (int i = 0; i < colorCount; i++)
            {
                double c = i / (double)(colorCount - 1);
                double ci = 1.0 - c;
                double a = (ci * start.A) + (c * end.A);
                double r = (ci * start.R) + (c * end.R);
                double g = (ci * start.G) + (c * end.G);
                double b = (ci * start.B) + (c * end.B);

                colors.Add(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
            }

            return colors;
        }
        private void RgbLinearInterpolate(int colorCount)
        {
            Color start = Color.FromArgb(255, 90, 150, 5);
            Color middle = Color.FromArgb(255, 200, 170, 10);
            Color end = Color.FromArgb(255, 205, 40, 10);

            if (colorCount % 2 == 0)
                colorCount += 1;

            _colors = new List<Color>();

            int halfCount = (colorCount + 1) / 2;

            List<Color> halfColors = RgbLinearInterpolate(start, middle, halfCount);

            if (halfColors.Count > 0)
                halfColors.RemoveAt(halfColors.Count - 1);

            _colors.AddRange(halfColors);
            _colors.AddRange(RgbLinearInterpolate(middle, end, halfCount));
        }

        #endregion
    }
}
