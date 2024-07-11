﻿using System;
using System.Windows;
using System.Windows.Threading;

namespace BlurryControls.Example.Control
{
    /// <summary>
    /// Interaction logic for BlurryUserControl.xaml
    /// </summary>
    public partial class BlurryUserControl
    {
        private readonly DispatcherTimer _timeDispatcherTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(1)};

        public BlurryUserControl()
        {
            InitializeComponent();
            _timeDispatcherTimer.Tick += TimeDispatcherTimerOnTick;
        }

        private void TimeDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            TimeTextBlock.Text = DateTime.Now.ToLongTimeString();
        }

        private void TimeTextBlock_OnLoaded(object sender, RoutedEventArgs e)
        {
            TimeTextBlock.Text = DateTime.Now.ToLongTimeString();
            _timeDispatcherTimer.Start();
        }
    }
}
