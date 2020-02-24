using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Tomawo.Code
{
    internal class Sounds
    {
        #region Constructors

        internal Sounds(MediaElement media, bool quiet)
        {
            _media = media;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

            SoundsVolume(quiet);
        }

        #endregion

        #region Variables

        private readonly MediaElement _media;
        private int _seconds;
        private readonly DispatcherTimer _timer;

        private double _tickVolume;
        private double _alarmVolume;
        private double _pace;

        #endregion

        #region Methods

        private void SoundsVolume(bool quiet)
        {
            if (quiet)
            {
                _tickVolume = 0.1;
                _alarmVolume = 0.2;
                _pace = 0.025;
            }
            else
            {
                _tickVolume = 0.3;
                _alarmVolume = 0.6;
                _pace = 0.075;
            }
        }

        internal void StartingTimerSound(bool quiet)
        {
            SoundsVolume(quiet);

            _media.Source = new Uri("Sounds/ticking.wma", UriKind.Relative);
            _media.Volume = _tickVolume;
            _media.Play();

            _seconds = 4;
            _timer.Tick += timer_TickStarting;
            _timer.Start();

        }
        private void timer_TickStarting(object sender, EventArgs e)
        {
            if (_seconds != 0)
            {
                _media.Volume -= _pace;
                _seconds -= 1;
            }
            else
            {
                _media.Stop();
                _media.Close();
                _timer.Tick -= timer_TickStarting;
            }
        }

        internal void EndingTimerSound()
        {
            _media.Source = new Uri("Sounds/ticking.wma", UriKind.Relative);
            _media.Volume = 0.0;
            _media.Play();

            _seconds = 3;
            _timer.Tick += timer_TickEnding;
            _timer.Start();

        }
        private void timer_TickEnding(object sender, EventArgs e)
        {
            if (_seconds != 0)
            {
                _media.Volume += _pace;
                _seconds -= 1;
            }
            else
            {
                _media.Stop();
                _media.Close();
                _timer.Tick -= timer_TickEnding;
            }
        }

        internal void EndTimerSound()
        {
            _media.Source = new Uri("Sounds/alarm.wma", UriKind.Relative);
            _media.Volume = _alarmVolume;
            _media.Play();

            _seconds = 2;
            _timer.Tick += timer_TickEnd;
            _timer.Start();

        }
        private void timer_TickEnd(object sender, EventArgs e)
        {
            if (_seconds != 0)
            {
                _seconds -= 1;
            }
            else
            {
                _media.Stop();
                _media.Close();
                _timer.Tick -= timer_TickEnd;
            }
        }

        internal void Stop()
        {
            _media.Stop();
            _media.Close();
        }

        #endregion
    }
}
