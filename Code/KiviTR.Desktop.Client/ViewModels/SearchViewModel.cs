using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Input;
using JetBrains.Annotations;
using KiviTR.Common;
using KiviTR.Common.Models;
using Prism.Commands;
using Prism.Mvvm;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace KiviTR.Desktop.Client.ViewModels
{
    [UsedImplicitly]
    public class SearchViewModel : BindableBase
    {
        private bool isEnabled = true;
        private ObservableCollection<Meaning> meaningsA;
        private ObservableCollection<Meaning> meaningsB;
        private string searchValue;

        public SearchViewModel()
        {
            ExecuteSearchDelegateCommand = new DelegateCommand(() => { Search(searchValue); }, CanExecute);


            HotKeyManager.RegisterHotKey(Keys.F12, ModifierKeys.Control);
            HotKeyManager.HotKeyPressed += (sender, args) =>
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() => { Search(Clipboard.GetText()); });
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                }
            };
        }


        private string Url => $"http://tureng.com/en/turkish-english/{SearchValue}";

        public string SearchValue
        {
            get => searchValue;
            set
            {
                SetProperty(ref searchValue, value);
                ExecuteSearchDelegateCommand.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Meaning> MeaningsA
        {
            get => meaningsA;
            set => SetProperty(ref meaningsA, value);
        }

        public ObservableCollection<Meaning> MeaningsB
        {
            get => meaningsB;
            set => SetProperty(ref meaningsB, value);
        }


        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                SetProperty(ref isEnabled, value);
                ExecuteSearchDelegateCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand ExecuteSearchDelegateCommand { get; }

        public void Search(string key)
        {
            SearchValue = key;
            Execute();
        }

        private async void Execute()
        {
            RunFunc(
                () =>
                {
                    MeaningsA = new ObservableCollection<Meaning>();
                    MeaningsB = new ObservableCollection<Meaning>();
                });


            try
            {
                var request = new Request();
                string html = await request.SimpleGetAsync(Url);
                var parser = new TurengHtmlParser(html, SearchValue);
                VocabularyTranslate translated = parser.Translate();

                if (translated.MeaningsA != null)
                {
                    RunFunc(() => { MeaningsA.AddRange(translated.MeaningsA); });
                }

                if (translated.MeaningsB != null)
                {
                    RunFunc(() => { MeaningsB.AddRange(translated.MeaningsB); });
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("HAYVAN GİBİ ŞEY ARIYONUZ AQ.");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private bool CanExecute()
        {
            return IsEnabled && !string.IsNullOrWhiteSpace(SearchValue);
        }

        public static void RunFunc(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}