using System;
using System.Windows;
using System.Windows.Controls;
using EzIIOLib;

namespace EzIIOLibControl.Controls
{
    public partial class IOPinToggleSwitch : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty PinNameProperty =
            DependencyProperty.Register(
                nameof(PinName),
                typeof(string),
                typeof(IOPinToggleSwitch),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty PinNumberProperty =
            DependencyProperty.Register(
                nameof(PinNumber),
                typeof(int),
                typeof(IOPinToggleSwitch),
                new PropertyMetadata(-1));

        public static readonly DependencyProperty PinStateProperty =
            DependencyProperty.Register(
                nameof(PinState),
                typeof(bool),
                typeof(IOPinToggleSwitch),
                new PropertyMetadata(false));

        public static readonly DependencyProperty DeviceNameProperty =
            DependencyProperty.Register(
                nameof(DeviceName),
                typeof(string),
                typeof(IOPinToggleSwitch),
                new PropertyMetadata(string.Empty));
        #endregion

        #region Public Properties
        public string PinName
        {
            get => (string)GetValue(PinNameProperty);
            set => SetValue(PinNameProperty, value);
        }

        public int PinNumber
        {
            get => (int)GetValue(PinNumberProperty);
            set => SetValue(PinNumberProperty, value);
        }

        public bool PinState
        {
            get => (bool)GetValue(PinStateProperty);
            set => SetValue(PinStateProperty, value);
        }

        public string DeviceName
        {
            get => (string)GetValue(DeviceNameProperty);
            set => SetValue(DeviceNameProperty, value);
        }

        public MultiDeviceManager DeviceManager { get; set; }
        #endregion

        #region Events
        public event EventHandler<bool> PinStateChanged;
        public event EventHandler<string> Error;
        #endregion

        public IOPinToggleSwitch()
        {
            InitializeComponent();
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            UpdatePinState(true);
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdatePinState(false);
        }

        private void UpdatePinState(bool newState)
        {
            try
            {
                if (DeviceManager != null && !string.IsNullOrEmpty(DeviceName) && !string.IsNullOrEmpty(PinName))
                {
                    bool success = newState ?
                        DeviceManager.SetOutput(DeviceName, PinName) :
                        DeviceManager.ClearOutput(DeviceName, PinName);

                    if (!success)
                    {
                        Error?.Invoke(this, $"Failed to {(newState ? "set" : "clear")} output for pin {PinName}");
                        // Revert the toggle state if the operation failed
                        PinState = !newState;
                    }
                    else
                    {
                        PinStateChanged?.Invoke(this, newState);
                    }
                }
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, $"Error updating pin state: {ex.Message}");
                // Revert the toggle state on error
                PinState = !newState;
            }
        }

        public void UpdateState(bool state)
        {
            if (PinState != state)
            {
                PinState = state;
                PinStateChanged?.Invoke(this, state);
            }
        }
    }
}