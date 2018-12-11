using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using JetBrains.Annotations;
using KiviTR.Common;
using KiviTR.Common.Models;
using Prism.Commands;
using Prism.Mvvm;

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
            ExecuteSearchDelegateCommand = new DelegateCommand(() => SearchAsync(), CanExecute);
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

        public Task SearchAsync(string key)
        {
            SearchValue = key;
            return Task.Factory.StartNew(Execute);
        }


        public Task SearchAsync()
        {
            return Task.Factory.StartNew(Execute);
        }

        private void Execute()
        {
            try
            {
                Request request = new Request();
                string html = request.SimpleGet(Url);
                TurengHtmlParser parser = new TurengHtmlParser(html, SearchValue);
                VocabularyTranslate translated = parser.Translate();

                MeaningsA = new ObservableCollection<Meaning>();
                MeaningsB = new ObservableCollection<Meaning>();
                if (translated.MeaningsA != null)
                {
                    MeaningsA.AddRange(translated.MeaningsA);
                }

                if (translated.MeaningsB != null)
                {
                    MeaningsB.AddRange(translated.MeaningsB);
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
    }

}