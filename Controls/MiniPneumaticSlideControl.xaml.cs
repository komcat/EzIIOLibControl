using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using System.Linq;
using EzIIOLib;
using Newtonsoft.Json;

namespace EzIIOLibControl.Controls
{
    public partial class MiniPneumaticSlideControl : UserControl
    {
        private MultiDeviceManager deviceManager;
        private PneumaticSlideManager slideManager;

        // Slides Dependency Property
        public static readonly DependencyProperty SlidesProperty =
            DependencyProperty.Register(
                nameof(Slides),
                typeof(ObservableCollection<SlideViewModel>),
                typeof(MiniPneumaticSlideControl),
                new PropertyMetadata(new ObservableCollection<SlideViewModel>()));

        // DeviceManager Dependency Property
        public static readonly DependencyProperty DeviceManagerProperty =
            DependencyProperty.Register(
                nameof(DeviceManager),
                typeof(MultiDeviceManager),
                typeof(MiniPneumaticSlideControl),
                new PropertyMetadata(null, OnDeviceManagerChanged));

        // Title Dependency Property
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(MiniPneumaticSlideControl),
                new PropertyMetadata("Pneumatic Slides"));

        public ObservableCollection<SlideViewModel> Slides
        {
            get => (ObservableCollection<SlideViewModel>)GetValue(SlidesProperty);
            set => SetValue(SlidesProperty, value);
        }

        public MultiDeviceManager DeviceManager
        {
            get => (MultiDeviceManager)GetValue(DeviceManagerProperty);
            set => SetValue(DeviceManagerProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        // Events
        public event EventHandler RefreshRequested;
        public event EventHandler<string> LogEvent;

        public MiniPneumaticSlideControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        private static void OnDeviceManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MiniPneumaticSlideControl;
            control?.InitializeSlidesFromConfig();
        }

        private void InitializeSlidesFromConfig()
        {
            // Clear existing slides
            Slides.Clear();

            if (DeviceManager == null)
                return;

            // Load configuration
            try
            {
                var config = LoadConfiguration();

                // Create slide manager
                slideManager = new PneumaticSlideManager(DeviceManager);
                slideManager.LoadSlidesFromConfig(config);

                // Create view models for each slide
                foreach (var slideName in config.PneumaticSlides.Select(s => s.Name))
                {
                    var slide = slideManager.GetSlide(slideName);
                    var viewModel = new SlideViewModel(slide);
                    Slides.Add(viewModel);
                }

                LogMessage($"Loaded {Slides.Count} pneumatic slides");
            }
            catch (Exception ex)
            {
                LogMessage($"Error loading slides: {ex.Message}");
            }
        }

        private IOConfiguration LoadConfiguration()
        {
            string configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Config",
                "IOConfig.json"
            );

            if (!File.Exists(configPath))
                throw new FileNotFoundException($"Configuration file not found: {configPath}");

            string jsonContent = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<IOConfiguration>(jsonContent);
        }

        private async void ExtendButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string slideName)
            {
                var slideVM = GetSlideViewModel(slideName);
                if (slideVM != null && !slideVM.IsMoving)
                {
                    button.IsEnabled = false;
                    try
                    {
                        LogMessage($"Extending {slideName}...");
                        bool success = await slideVM.Slide.ExtendAsync();
                        LogMessage($"{slideName} extend {(success ? "ok" : "failed")}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error: {ex.Message}");
                    }
                    finally
                    {
                        button.IsEnabled = true;
                    }
                }
            }
        }

        private async void RetractButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string slideName)
            {
                var slideVM = GetSlideViewModel(slideName);
                if (slideVM != null && !slideVM.IsMoving)
                {
                    button.IsEnabled = false;
                    try
                    {
                        LogMessage($"Retracting {slideName}...");
                        bool success = await slideVM.Slide.RetractAsync();
                        LogMessage($"{slideName} retract {(success ? "ok" : "failed")}");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error: {ex.Message}");
                    }
                    finally
                    {
                        button.IsEnabled = true;
                    }
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
            InitializeSlidesFromConfig();
        }

        private SlideViewModel GetSlideViewModel(string slideName)
        {
            return Slides.FirstOrDefault(s => s.Name == slideName);
        }

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[MiniSlideControl] {message}");
            LogEvent?.Invoke(this, message);
        }
    }
}