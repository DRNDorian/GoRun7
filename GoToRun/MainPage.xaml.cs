using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using GoToRun.Resources;
using System.Collections;
using System.IO.IsolatedStorage;
using System.IO;
using GoToRun.Model;

namespace GoToRun
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Fillup _currentFillup;
        private const string CURRENT_FILLUP_KEY = "CurrentFillup";
        private bool _hasUnsavedChanges;
        private const string HAS_UNSAVED_CHANGES_KEY = "HasUnsavedChanges";
        double X1 = 0; double Y1 = 0;
        DispatcherTimer timer = new DispatcherTimer();
        bool startS = true;
        double firstX, firstY;
        double RastAll = 0; //Пройденный путь
        double AverageSpeed = 0; // Средняя скорость
        int num = 0; //Пройденное время

        private bool buttomV = true;

        // Конструктор
        public MainPage()
        {
            InitializeComponent();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(00, 0, 5);
            bool enabled = timer.IsEnabled;

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Initialize the page state only if it is not already initialized,
            // and not when the application was deactivated but not tombstoned (returning from being dormant).
            if (DataContext == null)
            {
                InitializePageState();
            }

            // Delete temporary storage to avoid unnecessary storage costs.
            State.Clear();
        }
        private void InitializePageState()
        {

            DataContext = _currentFillup =
                State.ContainsKey(CURRENT_FILLUP_KEY) ?
                (Fillup)State[CURRENT_FILLUP_KEY] :
                new Fillup { Date = DateTime.Now };

            _hasUnsavedChanges = State.ContainsKey(HAS_UNSAVED_CHANGES_KEY) && (bool)State[HAS_UNSAVED_CHANGES_KEY];

        }

        private async void OneShotLocation_Click()
        {
            //переменную следует выбрать откуда-то из хранилища настроек пользователя
            //и установить в реальное значение
            bool isAllow = true;

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            geolocator.ReportInterval = 5000;
            //проверка определения позиции
            if (geolocator.LocationStatus == PositionStatus.Disabled)
            {
                if (MessageBox.Show(
                    "Определение позиции заблокировано. Перейти к настройкам телефона?",
                    "Ошибка настроек", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-location:"));
                }
                return;
            }


            if (!isAllow)
            {
                if (MessageBox.Show(
                    "Разрешить определение позиции устройства для приложения?",
                    "Настройки", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //сохранить выбранное значение для дальнейшего использования
                    //чтобы в следующий раз им заполнять isAllow
                }
                else
                {
                    return;
                }
            }

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(TimeSpan.FromMinutes(0), TimeSpan.FromSeconds(5));
                RastAll += look_Distanse(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);
                AverageSpeed = RastAll / num;

            }
            catch (Exception eh)
            {
                MessageBox.Show("Что-то случилось странное!");
            }
        }

        private void UpdateInfo()
        {
            TimerBox.Text = num.ToString();
            DistanceBox.Text = (Math.Round(RastAll)).ToString();
            SpeedBox.Text = Math.Round(AverageSpeed*1000/3600, 2).ToString();
            CaloryBox.Text = (RastAll * 65 / 1000).ToString();
        }
        void timer_Tick(object sender, object e)
        {
            num = num + 5;
            OneShotLocation_Click();
            UpdateInfo();
        }

        //Вычесление расстояние между двумя точками
        double look_Distanse(double X, double Y)
        {
            if (startS)
            {
                X1 = X; Y1 = Y; startS = false;
            }
            firstX = X; firstY = Y;
            int R = 6372795;
            X *= Math.PI / 180;
            Y *= Math.PI / 180;
            X1 *= Math.PI / 180;
            Y1 *= Math.PI / 180;
            ///
            var cl1 = Math.Cos(X);
            var cl2 = Math.Cos(X1);
            var sl1 = Math.Sin(X);
            var sl2 = Math.Sin(X1);
            var delta = Y - Y1;
            var cdelta = Math.Cos(delta);
            var sdelta = Math.Sin(delta);
            ///
            var y = Math.Sqrt(Math.Pow(cl2 * sdelta, 2) + Math.Pow(cl1 * sl2 - sl1 * cl2 * cdelta, 2));
            var x = sl1 * sl2 + cl1 * cl2 * cdelta;
            var ad = Math.Atan2(y, x);
            double dist = (ad * R); //расстояние между двумя координатами в метрах
            X1 = firstX; Y1 = firstY;
            return Math.Round(dist);
        }
        // Загрузка данных для элементов ViewModel
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (buttomV)
            {
                timer.Start();
                buttomV = false;
            }
            else
            {
                buttomV = true;
                timer.Stop();
                SaveResult result = RunnerData.SaveFillup(_currentFillup,
                delegate
                {
                    MessageBox.Show("There is not enough space on your phone to " +
                    "save your fill-up data. Free some space and try again.");
                });

                if (result.SaveSuccessful)
                {
                    Microsoft.Phone.Shell.PhoneApplicationService.Current
                        .State[Constants.FILLUP_SAVED_KEY] = true;
                    //NavigationService.GoBack();
                }
                else
                {
                    string errorMessages = String.Join(
                        Environment.NewLine + Environment.NewLine,
                        result.ErrorMessages.ToArray());
                    if (!String.IsNullOrEmpty(errorMessages))
                    {
                        MessageBox.Show(errorMessages,
                            "Warning: Invalid Values", MessageBoxButton.OK);
                    }
                }
            }

        }

        private void Buttom_click_1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/History.xaml", UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Acievements.xaml", UriKind.Relative));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Help.xaml", UriKind.Relative));
        }
    }
}