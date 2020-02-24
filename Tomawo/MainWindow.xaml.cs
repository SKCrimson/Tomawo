using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Tomawo.Code;
using Tomawo.Themes;
// ReSharper disable PossibleNullReferenceException
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace Tomawo
{
    public partial class MainWindow
    {
        #region Variables

        private TimeController _timeController;
        private PlayStopController _playStop;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Window Events

        private void Window_Initialized(object sender, EventArgs e)
        {
            try
            {
                Rect bounds = Properties.Settings.Default.WindowPosition;

                if (bounds.Top == 0 && bounds.Left == 0)
                {
                    DefaultPosition();
                }
                else
                {
                    Window.Top = bounds.Top;
                    Window.Left = bounds.Left;
                }
            }
            catch (Exception)
            {
                DefaultPosition();
            }

            InitializeTimer();
            SetSessionsCount();
        }
        private void DefaultPosition()
        {
            double workHeight = SystemParameters.WorkArea.Height;
            double workWidth = SystemParameters.WorkArea.Width;
            Top = (workHeight - Height) / 2;
            Left = (workWidth - Width) / 2;
        }
        private void InitializeTimer()
        {
            _playStop = new PlayStopController();
            GridPlayStop.DataContext = _playStop;
            _timeController = new TimeController(ClockSound, _playStop);
            GridTimer.DataContext = GridAllSettings.DataContext = _timeController;
        }
        private void SetSessionsCount()
        {
            int count = 0;
            do
            {
                count++;

                CountControl control = new CountControl(count.ToString(), false);
                StackSessions.Children.Add(control);
            } while (count < _timeController.Count);
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowPosition = Window.RestoreBounds;
            Properties.Settings.Default.Save();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void StackSessions_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (StackSessions.Visibility != Visibility.Hidden) return;

            StackSessions.Children.Clear();
            int count = 0;

            do
            {
                count++;

                CountControl control = count <= _timeController.CountReady ?
                    new CountControl(count.ToString(), true) : new CountControl(count.ToString(), false);

                StackSessions.Children.Add(control);
            } while (count < _timeController.Count);

            _playStop.StackCount = Visibility.Visible;
        }

        private void CheckBoxStopwatch_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxStopwatch.IsChecked != null && (bool)CheckBoxStopwatch.IsChecked)
            {
                _timeController.Stopwatch(true);
            }
            else
            {
                _timeController.Stopwatch(false);
            }
        }

        #endregion

        #region Timer Button Events

        private void BtnPlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (IconPlay.Visibility == Visibility.Visible)
            {
                _timeController.SessionStart();
            }
            else
            {
                _timeController.SessionStop();
            }
        }
        private void BtnRewind_Click(object sender, RoutedEventArgs e)
        {
            _timeController.SessionRewind();
        }

        #endregion

        #region Panels

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            if (GridSettings.Visibility != Visibility.Visible)
            {
                Storyboard boardOpen = FindResource("OpenSettings") as Storyboard;
                Storyboard boardClose;

                if (GridUsers.Visibility == Visibility.Visible)
                    boardClose = FindResource("CloseUsers") as Storyboard;
                else
                    boardClose = FindResource("CloseTasks") as Storyboard;

                boardClose.Begin();
                boardOpen.Begin();
            }
        }
        private void ButtonUsers_Click(object sender, RoutedEventArgs e)
        {
            if (GridUsers.Visibility != Visibility.Visible)
            {
                Storyboard boardOpen = FindResource("OpenUsers") as Storyboard;
                Storyboard boardClose;

                if (GridSettings.Visibility == Visibility.Visible)
                    boardClose = FindResource("CloseSettings") as Storyboard;
                else
                    boardClose = FindResource("CloseTasks") as Storyboard;

                boardClose.Begin();
                boardOpen.Begin();
            }
        }
        private void ButtonTasks_Click(object sender, RoutedEventArgs e)
        {
            if (GridTasks.Visibility != Visibility.Visible)
            {
                Storyboard boardOpen = FindResource("OpenTasks") as Storyboard;
                Storyboard boardClose;

                if (GridUsers.Visibility == Visibility.Visible)
                    boardClose = FindResource("CloseUsers") as Storyboard;
                else
                    boardClose = FindResource("CloseSettings") as Storyboard;

                boardClose.Begin();
                boardOpen.Begin();
            }
        }

        #endregion

        #region Sliders Events

        private void SliderSessionLength_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings.UpdateTime(SliderSessionLength.Value);
        }
        private void SliderSessionCount_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            StackSessions.Children.Clear();
            int count = 0;

            do
            {
                count++;

                CountControl control = new CountControl(count.ToString(), false);
                StackSessions.Children.Add(control);
            } while (count < SliderSessionCount.Value);

            Settings.UpdateSessionsCount(SliderSessionCount.Value);
        }
        private void SliderBreakLength_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings.UpdateBreak(SliderBreakLength.Value);
        }
        private void SliderLongBreakLength_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Settings.UpdateLongBreak(SliderLongBreakLength.Value);
        }

        #endregion

        #region Login

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            TimerLoginBorder.Visibility = Visibility.Collapsed;
            MainLoginBorder.Visibility = Visibility.Collapsed;
        }
        private void LoginCancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}