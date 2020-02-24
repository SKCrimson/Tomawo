using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable MemberCanBePrivate.Global

namespace Tomawo.Code
{
    internal class TimeController : INotifyPropertyChanged
    {
        #region Constructors

        internal TimeController(MediaElement media, PlayStopController playStop)
        {
            _state = "wait";
            _timerBrush = (Brush)Application.Current.Resources["BlueBrush"];

            GetSettings();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _playStop = playStop;
            _sounds = new Sounds(media, Quiet);
        }

        #endregion

        #region Event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Variables

        private string _state;

        private Settings _settings;
        private readonly DispatcherTimer _timer;
        private readonly PlayStopController _playStop;
        private readonly Sounds _sounds;
        private InterpolateColors _colors;

        private Brush _timerBrush;
        private Visibility _allSettings;
        private double _count;
        private bool _quiet;
        private bool _checkStopwatch;

        private double _time;
        private double _breakTime;
        private double _longBreak;
        private string _hours;
        private Visibility _colon;
        private double _minutes;
        private string _minutesStr;
        private double _seconds;
        private string _secondsStr;

        #endregion

        #region Properties

        public Brush TimerBrush
        {
            get => _timerBrush;
            set
            {
                _timerBrush = value;
                OnPropertyChanged("TimerBrush");
            }
        }
        public Visibility AllSettings
        {
            get => _allSettings;
            set
            {
                _allSettings = value;
                OnPropertyChanged("AllSettings");
            }
        }
        public double Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }
        public double CountReady { get; set; }
        public bool Quiet
        {
            get => _quiet;
            set
            {
                _quiet = value;
                OnPropertyChanged("Quiet");
                Settings.UpdateQuiet(_quiet);
            }
        }
        public bool CheckStopwatch
        {
            get => _checkStopwatch;
            set
            {
                _checkStopwatch = value;
                OnPropertyChanged("CheckStopwatch");
            }
        }

        public double Time
        {
            get => _time;
            set
            {
                _time = value;
                _seconds = 0;
                OnPropertyChanged("Time");
                ParseTime();
            }
        }
        public double BreakTime
        {
            get => _breakTime;
            set
            {
                _breakTime = value;
                _seconds = 0;
                OnPropertyChanged("BreakTime");
                ParseTime();
            }
        }
        public double LongBreak
        {
            get => _longBreak;
            set
            {
                _longBreak = value;
                _seconds = 0;
                OnPropertyChanged("LongBreak");
                ParseTime();
            }
        }
        public string Hours
        {
            get => _hours;
            set
            {
                _hours = value;
                OnPropertyChanged("Hours");
            }
        }
        public Visibility Colon
        {
            get => _colon;
            set
            {
                _colon = value;
                OnPropertyChanged("Colon");
            }
        }
        public string Minutes
        {
            get => _minutesStr;
            set
            {
                _minutesStr = value;
                OnPropertyChanged("Minutes");
            }
        }
        public string Seconds
        {
            get => _secondsStr;
            set
            {
                _secondsStr = value;
                OnPropertyChanged("Seconds");
            }
        }

        #endregion

        #region Methods

        private void GetSettings()
        {
            _settings = new Settings();
            Count = _settings.SessionCont;
            Time = _settings.Time;
            BreakTime = _settings.BreakTime;
            LongBreak = _settings.LongBreak;
            Quiet = _settings.Quiet;
        }

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(info));
        }
        private void ParseTime()
        {
            if (_time >= 60)
            {
                _minutes = _time - 60;
                Hours = "1";
                Colon = Visibility.Visible;
            }
            else
            {
                _minutes = _time;
                Hours = String.Empty;
                Colon = Visibility.Hidden;
            }

            if (_minutes <= 0)
                Minutes = "00";
            else if (_minutes < 10)
                Minutes = "0" + _minutes.ToString("##.");
            else
                Minutes = _minutes.ToString("##.");

            if (_seconds <= 0)
                Seconds = "00";
            else if (_seconds < 10)
                Seconds = "0" + _seconds.ToString("##.");
            else
                Seconds = _seconds.ToString("##.");
        }

        internal void SessionStart()
        {
            AllSettings = Visibility.Collapsed;

            if (_state == "stopwatch")
            {
                StopwatchStart();
                return;
            }

            _state = "work";

            TimerBrush = (Brush)Application.Current.Resources["GreenBrush"];
            _colors = new InterpolateColors((int)_time * 60);
            _playStop.SessionStart();

            _timer.Tick += session_Tick;
            _timer.Start();

            _time -= 1;
            _seconds = 59;

            _sounds.StartingTimerSound(_quiet);
        }
        internal void SessionStop()
        {
            _sounds.Stop();
            _timer.Stop();

            switch (_state)
            {
                case "work":
                    _playStop.SessionStop();
                    _timer.Tick -= session_Tick;
                    break;
                case "break":
                    _playStop.BreakStop();
                    _timer.Tick -= break_Tick;
                    break;
                case "long":
                    CountReady = 0;
                    _playStop.LongStop();
                    _timer.Tick -= longBreak_Tick;
                    break;
                case "stopwatch":
                    CheckStopwatch = false;
                    _playStop.StopwatchStop();
                    _timer.Tick -= stopwatch_Tick;
                    break;
                default:
                    CountReady = 0;
                    _playStop.LongStop();
                    _timer.Tick -= session_Tick;
                    _timer.Tick -= break_Tick;
                    _timer.Tick -= longBreak_Tick;
                    _timer.Tick -= stopwatch_Tick;
                    break;
            }
        }
        private void session_Tick(object sender, EventArgs e)
        {
            ParseTime();

            if (_colors.CheckColorBrush())
                TimerBrush = _colors.GetColorBrush();

            _seconds -= 1;

            if (_time == 0)
            {
                if (_seconds == 3)
                    _sounds.EndingTimerSound();
                else if (_seconds == -1)
                {
                    _timer.Stop();
                    _timer.Tick -= session_Tick;
                    CountReady += 1;

                    if (_count == CountReady)
                    {
                        LongBreakStart();
                        return;
                    }

                    BreakStart();
                    return;
                }
            }

            if (_seconds != -1) return;

            if (_time != 0)
            {
                _time -= 1;
                _seconds = 59;
            }
        }

        internal void BreakStart()
        {
            _state = "break";

            TimerBrush = (Brush)Application.Current.Resources["LightBlueBrush"];
            _sounds.EndTimerSound();
            _playStop.BreakStart();

            Time = _breakTime;

            _timer.Tick += break_Tick;
            _timer.Start();

            _time -= 1;
            _seconds = 59;
        }
        internal void BreakEnd()
        {
            _sounds.EndTimerSound();
            _playStop.BreakEnd();
            TimerBrush = (Brush)Application.Current.Resources["BlueBrush"];

            _settings.Session();
            _time = _settings.Time;

            ParseTime();
        }
        private void break_Tick(object sender, EventArgs e)
        {
            ParseTime();
            _seconds -= 1;

            if (_time == 0 && _seconds == -1)
            {
                _timer.Stop();
                _timer.Tick -= break_Tick;

                BreakEnd();
                return;
            }

            if (_seconds != -1) return;

            if (_time != 0)
            {
                _time -= 1;
                _seconds = 59;
            }
        }

        internal void LongBreakStart()
        {
            _state = "long";

            TimerBrush = (Brush)Application.Current.Resources["LightBlueBrush"];
            _sounds.EndTimerSound();
            _playStop.LongStart();

            Time = _longBreak;

            _timer.Tick += longBreak_Tick;
            _timer.Start();

            _time -= 1;
            _seconds = 59;
        }
        internal void LongBreakEnd()
        {
            AllSettings = Visibility.Visible;
            TimerBrush = (Brush)Application.Current.Resources["BlueBrush"];

            _sounds.EndTimerSound();
            CountReady = 0;
            _playStop.LongEnd();

            _settings.Session();
            _time = _settings.Time;

            ParseTime();
        }
        private void longBreak_Tick(object sender, EventArgs e)
        {
            ParseTime();
            _seconds -= 1;

            if (_time == 0 && _seconds == -1)
            {
                _timer.Stop();
                _timer.Tick -= longBreak_Tick;

                LongBreakEnd();
                return;
            }

            if (_seconds != -1) return;

            if (_time != 0)
            {
                _time -= 1;
                _seconds = 59;
            }
        }

        internal void Stopwatch(bool action)
        {
            if (action)
            {
                _state = "stopwatch";
                _playStop.Stopwatch();

                Time = 0;
                _seconds = 0;
            }
            else
            {
                _state = "wait";
                _playStop.BreakEnd();

                GetSettings();
            }

            ParseTime();
        }
        internal void StopwatchStart()
        {
            TimerBrush = (Brush)Application.Current.Resources["Violet"];
            _playStop.StopwatchStart();

            _timer.Tick += stopwatch_Tick;
            _timer.Start();

            _sounds.StartingTimerSound(_quiet);
        }
        private void stopwatch_Tick(object sender, EventArgs e)
        {
            _seconds += 1;
            ParseTime();

            if (_seconds == 60)
            {
                Time += 1;
                _seconds = 0;
            }
        }

        internal void SessionRewind()
        {
            AllSettings = Visibility.Visible;

            _state = "wait";

            _settings.Session();
            Time = _settings.Time;

            Seconds = "00";

            _playStop.SessionRewind();
            TimerBrush = (Brush)Application.Current.Resources["BlueBrush"];
        }

        #endregion
    }
}
