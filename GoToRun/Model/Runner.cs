using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoToRun.Model
{
    public class Runner : INotifyPropertyChanged
    {
        private string _name;
        private float _weight;
        private ObservableCollection<Fillup> _fillupHistory;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public float Weight
        {
            get { return _weight; }
            set
            {
                if (_weight == value) return;
                _weight = value;
                NotifyPropertyChanged("Weight");
            }
        }

        public ObservableCollection<Fillup> FillupHistory
        {
            get { return _fillupHistory; }
            set
            {
                if (_fillupHistory == value) return;
                _fillupHistory = value;
                if (_fillupHistory != null)
                {
                    _fillupHistory.CollectionChanged += delegate
                    {
                        NotifyPropertyChanged("AverageFuelEfficiency");
                        NotifyPropertyChanged("LastFillup");
                    };
                }
                NotifyPropertyChanged("FillupHistory");
                NotifyPropertyChanged("AverageFuelEfficiency");

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

