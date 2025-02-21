using System;
using System.ComponentModel;
using EzIIOLib;

namespace EzIIOLibControl.Controls
{
    public class SlideViewModel : INotifyPropertyChanged
    {
        private readonly PneumaticSlide slide;
        private bool isMoving;
        private SlidePosition position;
        private bool extendedSensorActive;
        private bool retractedSensorActive;

        public PneumaticSlide Slide => slide;
        public string Name => slide.Name;

        public bool IsMoving
        {
            get => isMoving;
            private set
            {
                if (isMoving != value)
                {
                    isMoving = value;
                    OnPropertyChanged(nameof(IsMoving));
                }
            }
        }

        public SlidePosition Position
        {
            get => position;
            private set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        public bool ExtendedSensorActive
        {
            get => extendedSensorActive;
            private set
            {
                if (extendedSensorActive != value)
                {
                    extendedSensorActive = value;
                    OnPropertyChanged(nameof(ExtendedSensorActive));
                }
            }
        }

        public bool RetractedSensorActive
        {
            get => retractedSensorActive;
            private set
            {
                if (retractedSensorActive != value)
                {
                    retractedSensorActive = value;
                    OnPropertyChanged(nameof(RetractedSensorActive));
                }
            }
        }

        public SlideViewModel(PneumaticSlide slide)
        {
            this.slide = slide ?? throw new ArgumentNullException(nameof(slide));

            // Subscribe to slide events
            slide.PositionChanged += OnPositionChanged;
            slide.SensorStateChanged += OnSensorStateChanged;

            // Initialize current state
            Position = slide.Position;
            UpdateSensorStates(slide.GetSensorStates());
        }

        private void OnPositionChanged(object sender, SlidePosition newPosition)
        {
            Position = newPosition;
            IsMoving = newPosition == SlidePosition.Moving;
        }

        private void OnSensorStateChanged(object sender, SensorState sensorState)
        {
            UpdateSensorStates(sensorState);
        }

        private void UpdateSensorStates(SensorState sensorState)
        {
            ExtendedSensorActive = sensorState.ExtendedSensor;
            RetractedSensorActive = sensorState.RetractedSensor;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}