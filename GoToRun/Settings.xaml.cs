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

namespace GoToRun
{
    public partial class Settings : PhoneApplicationPage
    {

        private const string RUNNER_INFO_KEY = "RunnerInfo";
        private const string HAS_UNSAVED_CHANGES_KEY = "HasUnsavedChanges";
        private const string WEIGHT_READONLY_STATE = "WeightReadOnlyState";
        private Runner _runner;
        private bool _hasUnsavedChanges;
        private TextBox _textboxWithFocus;

        /// <summary>
        /// </summary>
        public Settings()
        {
            InitializeComponent();
            WeightText.KeyDown += delegate { _hasUnsavedChanges = true; };
            NameText.KeyDown += delegate { _hasUnsavedChanges = true; };
            GotFocus += RunnerDetailsPage_GotFocus;
        }
        void RunnerDetailsPage_GotFocus(object sender, RoutedEventArgs e)
        {
            _textboxWithFocus = e.OriginalSource as TextBox;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (DataContext == null)
            {
                InitializePageState();
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (e.Uri.OriginalString.Equals("MainPage.xaml") ||
                !_hasUnsavedChanges) return;

            CommitTextBoxWithFocus();
            State[RUNNER_INFO_KEY] = _runner;
            State[WEIGHT_READONLY_STATE] = WeightText.IsReadOnly;
            State[HAS_UNSAVED_CHANGES_KEY] = _hasUnsavedChanges;
        }

        private void InitializePageState()
        {
            if (State.ContainsKey(RUNNER_INFO_KEY))
            {
                _runner = (Runner)State[RUNNER_INFO_KEY];

                WeightText.IsReadOnly = (bool)State[WEIGHT_READONLY_STATE];
                if (!_hasUnsavedChanges) _hasUnsavedChanges =
                    (bool)State[HAS_UNSAVED_CHANGES_KEY];

                State.Clear();
            }
            else
            {
                _runner = RunnerData.Runner;
                WeightText.IsReadOnly = _runner.Weight > 0;
            }
            DataContext = _runner;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CommitTextBoxWithFocus();

            if (string.IsNullOrWhiteSpace(_runner.Name))
            {
                MessageBox.Show("Имя не введено!");
                return;
            }

            if (string.IsNullOrWhiteSpace(WeightText.Text))
            {
                MessageBox.Show("Вес не введен!");
                return;
            }


            RunnerData.Runner.Name = _runner.Name;
            RunnerData.Runner.Weight =
                _runner.Weight;
            RunnerData.SaveRunner(delegate
            {
                MessageBox.Show("There is not enough space on your phone to " +
                    "save your car data. Free some space and try again.");
            });

            NavigationService.GoBack();
        }


        private void CommitTextBoxWithFocus()
        {
            if (_textboxWithFocus == null) return;

            System.Windows.Data.BindingExpression expression =
                _textboxWithFocus.GetBindingExpression(TextBox.TextProperty);
            if (expression != null) expression.UpdateSource();

        }

    }
}