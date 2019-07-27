using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GoToRun.Model;
using GoToRun.ViewModels;

namespace GoToRun
{
    public partial class History : PhoneApplicationPage
    {
        public History()
        {
            InitializeComponent();
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
        }
        private void InitializePageState()
        {

            RunnerData.RunnerUpdated += RunnerData_RunnerUpdated;
            DataContext = RunnerData.Runner;

        }
        void RunnerData_RunnerUpdated(object sender, EventArgs e)
        {
            DataContext = RunnerData.Runner;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ListBox.SelectedIndex)
            {
                case 0: NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    return;
            }
        }
    }
}