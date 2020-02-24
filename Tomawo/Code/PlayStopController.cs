using System.ComponentModel;
using System.Windows;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Tomawo.Code
{
    internal class PlayStopController : INotifyPropertyChanged
    {
        #region Constructors

        internal PlayStopController()
        {
            _state = "working sessions";
            _stackCount = Visibility.Visible;
            _playStop = Visibility.Visible;
            _iconPlay = Visibility.Visible;
            _iconStop = Visibility.Collapsed;
            _rewind = Visibility.Collapsed;
        }

        #endregion

        #region Event

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Variables

        private string _state;
        private Visibility _stackCount;
        private Visibility _iconPlay;
        private Visibility _iconStop;
        private Visibility _playStop;
        private Visibility _rewind;

        #endregion

        #region Properties

        public string State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged("State");
            }
        }
        public Visibility StackCount
        {
            get => _stackCount;
            set
            {
                _stackCount = value;
                OnPropertyChanged("StackCount");
            }
        }
        public Visibility IconPlay
        {
            get => _iconPlay;
            set
            {
                _iconPlay = value;
                OnPropertyChanged("IconPlay");
            }
        }
        public Visibility IconStop
        {
            get => _iconStop;
            set
            {
                _iconStop = value;
                OnPropertyChanged("IconStop");
            }
        }
        public Visibility PlayStop
        {
            get => _playStop;
            set
            {
                _playStop = value;
                OnPropertyChanged("PlayStop");
            }
        }
        public Visibility Rewind
        {
            get => _rewind;
            set
            {
                _rewind = value;
                OnPropertyChanged("Rewind");
            }
        }

        #endregion

        #region Methods

        private void OnPropertyChanged(string info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        internal void SessionStart()
        {
            State = "session run ...";
            IconPlay = Visibility.Collapsed;
            IconStop = Visibility.Visible;
        }
        internal void SessionStop()
        {
            State = "session stopped";
            Rewind = Visibility.Visible;
            PlayStop = Visibility.Collapsed;
        }

        internal void SessionRewind()
        {
            State = "working sessions";
            Rewind = Visibility.Collapsed;
            PlayStop = Visibility.Visible;
            IconPlay = Visibility.Visible;
            IconStop = Visibility.Collapsed;
            StackCount = Visibility.Visible;
        }

        internal void BreakStart()
        {
            State = "break run ...";
            StackCount = Visibility.Hidden;
        }
        internal void BreakStop()
        {
            State = "break stopped";
            Rewind = Visibility.Visible;
            PlayStop = Visibility.Collapsed;
        }
        internal void BreakEnd()
        {
            State = "working sessions";
            IconPlay = Visibility.Visible;
            IconStop = Visibility.Collapsed;
        }

        internal void LongStart()
        {
            State = "long break run ...";
            StackCount = Visibility.Hidden;
        }
        internal void LongStop()
        {
            State = "long break stopped";
            Rewind = Visibility.Visible;
            PlayStop = Visibility.Collapsed;
        }
        internal void LongEnd()
        {
            State = "working sessions";
            StackCount = Visibility.Hidden;
            IconPlay = Visibility.Visible;
            IconStop = Visibility.Collapsed;
        }

        internal void Stopwatch()
        {
            State = "stopwatch session";
            StackCount = Visibility.Collapsed;
        }
        internal void StopwatchStart()
        {
            State = "stopwatch run ...";
            IconPlay = Visibility.Collapsed;
            IconStop = Visibility.Visible;
        }
        internal void StopwatchStop()
        {
            State = "stopwatch  stopped";
            Rewind = Visibility.Visible;
            PlayStop = Visibility.Collapsed;
        }

        #endregion
    }
}
