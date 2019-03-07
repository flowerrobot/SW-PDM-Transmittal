using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TransmittalManager.ViewModel
{
    public class SearchViewModel : DockViewModelBase
    {
        #region SearchFeilds
        public string FileName { get; set; }
        public string ProjectNo { get; set; }
        public string IssueBy { get; set; }

        public DateTime? SentByDate { get; set; }
        public DateTime? SentAfterDate { get; set; }
        public IssueType? IssueType { get; set; }
        public bool? IssueToWorkshop { get; set; }
        #endregion

        public List<string> Projects { get; } //todo link to common fields
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }


        public string SearchText
        {
            get {
                if (IsSearching) return "Cancel";
                return "Search";
            }
        }
        private bool IsSearching => worker.IsBusy;

        public ObservableCollection<TransmittalViewModel> Results { get; } = new ObservableCollection<TransmittalViewModel>();


        private BackgroundWorker worker;
        [PreferredConstructor]
        public SearchViewModel()
        {
            SearchCommand = new RelayCommand(SearchCommandExecuted, SearchCommandCanExecute);
            ClearCommand = new RelayCommand(ClearCommandExecuted, ClearCommandCanExecute);

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += BeginSearch;
            worker.RunWorkerCompleted += SearchComplete;
            worker.ProgressChanged += ProgressChange;
        }

        public SearchViewModel(DockItem di) : this()
        {
            SearchCommand = new RelayCommand(SearchCommandExecuted, SearchCommandCanExecute);
            ClearCommand = new RelayCommand(ClearCommandExecuted, ClearCommandCanExecute);

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = true;
            worker.DoWork += BeginSearch;
            worker.RunWorkerCompleted += SearchComplete;
            worker.ProgressChanged += ProgressChange;
        }

        private void ProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is TransmittalViewModel vm)
            {
                Results.Add(vm);
            }
        }

        private void SearchComplete(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void BeginSearch(object sender, DoWorkEventArgs e)
        {

        }


        #region Commands




        private bool ClearCommandCanExecute()
        {
            return !IsSearching;
        }

        private bool SearchCommandCanExecute()
        {
            return true;
        }

        private void ClearCommandExecuted()
        {
            FileName = null;
            ProjectNo = null;
            SentByDate = null;
            SentAfterDate = null;
            IssueType = null;
            IssueToWorkshop = null;
            IssueBy = null;
        }
        private void SearchCommandExecuted()
        {

            if (IsSearching)
            {
                worker.CancelAsync();
            }
            else
            {
                Results.Clear();
                worker.RunWorkerAsync();
            }
        }
        #endregion


        public override bool? Closing()
        {
            return false;
        }
    }
}
