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

        // New property to hold the device manager
        public MultiDeviceManager DeviceManager { get; set; }

        // New property to hold the device name
        public string DeviceName { get; set; }

        public event EventHandler<(string DeviceName, string PinName)> PinClicked;
        public IOPinMonitorControl()
        {
            InitializeComponent();
        }

        private void OutputPin_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string pinName)
            {
                try
                {
                    // If DeviceManager is set, toggle the output
                    if (DeviceManager != null && !string.IsNullOrEmpty(DeviceName))
                    {
                        DeviceManager.ToggleOutput(DeviceName, pinName);
                    }

                    // Raise the event with device name and pin name
                    PinClicked?.Invoke(this, (DeviceName, pinName));
                }
                catch (Exception ex)
                {
                    // Optional: Add error handling
                    MessageBox.Show($"Error toggling pin: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}