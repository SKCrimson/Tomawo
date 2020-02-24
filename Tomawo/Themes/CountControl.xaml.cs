using System.Windows.Media;

namespace Tomawo.Themes
{
    public partial class CountControl
    {
        public CountControl(string number, bool ready)
        {
            InitializeComponent();

            Number.Text = number;

            if (!ready) return;

            Indicator.Stroke = Brushes.White;
            Number.Foreground = Brushes.White;
        }
    }
}
