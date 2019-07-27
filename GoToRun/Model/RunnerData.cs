using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Media.Imaging;

namespace GoToRun.Model
{
         public static class RunnerData
    {
        private const string Runner_PHOTO_DIR_NAME = "GotoRun";
        private const string Runner_KEY = "GotoRun.Runner";
        private static readonly IsolatedStorageSettings appSettings =
            IsolatedStorageSettings.ApplicationSettings;
        private static Runner _runner;

        public static event EventHandler RunnerUpdated;
        public static Runner Runner
        {
            get
            {
                if (_runner == null)
                {
                    if (appSettings.Contains(Runner_KEY))
                    {
                        _runner = (Runner)appSettings[Runner_KEY];
                    }
                    else
                    {
                        _runner = new Runner
                        {
                            FillupHistory = new ObservableCollection<Fillup>()
                        };
                    }
                }
                return _runner;
            }
            set
            {
                _runner = value;
                NotifyRunnerUpdated();
            }
        }

        public static void SaveRunner(Action errorCallback)
        {
            try
            {
                appSettings[Runner_KEY] = Runner;
                appSettings.Save();
                NotifyRunnerUpdated();
            }
            catch (IsolatedStorageException)
            {
                errorCallback();
            }
        }
        public static void DeleteRunner()
        {
            appSettings.Remove(Runner_KEY);
            appSettings.Save();
            Runner = null;
            NotifyRunnerUpdated();
        }


        private static void DeletePhoto(String fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var path = Path.Combine(Runner_PHOTO_DIR_NAME, fileName);
                if (store.FileExists(path)) store.DeleteFile(path);
            }
        }

        public static SaveResult SaveFillup(Fillup fillup, Action errorCallback)
        {

            var saveResult = new SaveResult();

            Runner.FillupHistory.Insert(0, fillup);
            saveResult.SaveSuccessful = true;
            SaveRunner(delegate
            {
                saveResult.SaveSuccessful = false;
                errorCallback();
            });
            return saveResult;
        }

        private static void NotifyRunnerUpdated()
        {
            var handler = RunnerUpdated;
            if (handler != null) handler(null, null);
        }
    }
}
