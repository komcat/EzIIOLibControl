using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using EzIIOLib;

namespace EzIIOLibControl.Controls
{
    public partial class IOPinMonitorControl : UserControl
    {
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(IOPinMonitorControl),
                new PropertyMetadata("Pins"));

        public static readonly DependencyProperty PinsSourceProperty =
            DependencyProperty.Register("PinsSource", typeof(ObservableCollection<PinStatus>),
                typeof(IOPinMonitorControl), new PropertyMetadata(null));

        public static readonly DependencyProperty IsOutputProperty =
            DependencyProperty.Register("IsOutput", typeof(bool), typeof(IOPinMonitorControl),
                new PropertyMetadata(false));

        public string HeaderText
        {
            get => (string)GetValue(HeaderTextProperty);
            set => SetValue(HeaderTextProperty, value);
        }

        public ObservableCollection<PinStatus> PinsSource
        {
            get => (ObservableCollection<PinStatus>)GetValue(PinsSourceProperty);
            set => SetValue(PinsSourceProperty, value);
        }

        public bool IsOutput
        {
            get => (bool)GetValue(IsOutputProperty);
            set => SetValue(IsOutputProperty, value);
        }

        public event EventHandler<string> PinClicked;

        public IOPinMonitorControl()
        {
            InitializeComponent();
        }

        private void OutputPin_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string pinName)
            {
                PinClicked?.Invoke(this, pinName);
            }
        }
    }
}