using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GoToRun.Model
{
    public class Fillup : INotifyPropertyChanged
    {
        private DateTime _date;
        private double _averageSpeed;
        private double _totalDistance;
        private int _time;
        private int _calory;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date == value) return;
                _date = value;
                NotifyPropertyChanged("Date");
            }
        }
        //Средняя скорость
        public double AverageSpeed
        {
            get { return _averageSpeed; }
            set
            {
                var roundedValue = (float)Math.Round(value, 0);
                if (_averageSpeed.Equals(roundedValue)) return;
                _averageSpeed = roundedValue;
                NotifyPropertyChanged("AverageSpeed");
            }
        }
        // Полное расстояние за текущую пробежку
        public double TotalDistance
        {
            get { return _totalDistance; }
            set
            {
                var roundedValue = (float)Math.Round(value, 1);
                if (_totalDistance.Equals(roundedValue)) return;
                _totalDistance = roundedValue;
                NotifyPropertyChanged("TotalDistance");
            }
        }
        // Общее время за текущую пробежку
        public int Time
        {
            get { return _time; }
            set
            {
                var roundedValue = value;
                if (_time.Equals(roundedValue)) return;
                _time = roundedValue;
                NotifyPropertyChanged("Time");
            }
        }
        public int Calory
        {
            get { return _calory; }
            set
            {
                var roundedValue = value;
                if (_calory.Equals(roundedValue)) return;
                _calory = roundedValue;
                NotifyPropertyChanged("Calory");
            }
        }
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
