using DockingAdapterMVVM;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TransmittalManager.Models;
using DockState = DockingAdapterMVVM.DockState;

namespace TransmittalManager.ViewModel
{
    public class SearchViewModel : ViewModelBase, IDockElement
    {
        private static int count;


        #region SearchFeilds
        public string FileName { get; set; }
        public string ProjectNo { get; set; }
        public string IssueBy { get; set; }

        public DateTime? SentByDate { get; set; }
        public DateTime? SentAfterDate { get; set; }
        public IssueType? IssueType { get; set; }
        public bool? IssueToWorkshop { get; set; }
        #endregion
        private TransmitalManager searchModel = new TransmitalManager();
        public List<Project> Projects => Project.AllProjects;
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand ClearCommand { get; set; }
        public RelayCommand OpenTransmittal { get; set; }

        public string SearchText
        {
            get {
                if (IsSearching) return "Cancel";
                return "Search";
            }
        }
        public Project Project { get; set; }
        private bool IsSearching { get; set; }

        public ObservableCollection<TransmittalViewModel> Results { get; } = new ObservableCollection<TransmittalViewModel>();
        public TransmittalViewModel SelectedItem { get; set; }
        public CancellationTokenSource cts = new CancellationTokenSource();
        private IProgress<Transmittal> pg;

        private MainViewModel _parent;
        [PreferredConstructor]
        public SearchViewModel()
        {
            if (count > 0) Header += count;
            count += 1;
            SearchCommand = new RelayCommand(SearchCommandExecuted, SearchCommandCanExecute);
            ClearCommand = new RelayCommand(ClearCommandExecuted, ClearCommandCanExecute);
            OpenTransmittal = new RelayCommand(OpenTransmittalExecuted);

            pg = new Progress<Transmittal>(SearchChanged);
            Task.Run(() => searchModel.SearchForAnyAsync(pg, cts.Token));
            IsSearching = true;


            //worker = new BackgroundWorker();
            //worker.WorkerSupportsCancellation = true;
            //worker.WorkerReportsProgress = true;
            //worker.DoWork += BeginSearch;
            //worker.RunWorkerCompleted += SearchComplete;
            //worker.ProgressChanged += ProgressChange;
        }

        public SearchViewModel(MainViewModel parent) : this()
        {
            _parent = parent;
        }


        private void SearchChanged(Transmittal obj)
        {
            if (obj == null)
            {
                IsSearching = false;
                return;
            }
            Results.Add(new TransmittalViewModel(obj));
        }

        //public SearchViewModel(DockItem di) : base(di)
        //{
        //    if (count > 0) Header += count;
        //    count += 1;
        //    SearchCommand = new RelayCommand(SearchCommandExecuted, SearchCommandCanExecute);
        //    ClearCommand = new RelayCommand(ClearCommandExecuted, ClearCommandCanExecute);

        //    worker = new BackgroundWorker();
        //    worker.WorkerSupportsCancellation = true;
        //    worker.WorkerReportsProgress = true;
        //    worker.DoWork += BeginSearch;
        //    worker.RunWorkerCompleted += SearchComplete;
        //    worker.ProgressChanged += ProgressChange;
        //}

        #region BackGround Worker

        

        
        //private BackgroundWorker worker;
        //private void BeginSearch(object sender, DoWorkEventArgs e)
        //{

        //}
        //private void ProgressChange(object sender, ProgressChangedEventArgs e)
        //{
        //    if (e.UserState is TransmittalViewModel vm)
        //    {
        //        Results.Add(vm);
        //    }
        //}

        //private void SearchComplete(object sender, RunWorkerCompletedEventArgs e)
        //{

        //}
        #endregion
        #region Commands



        private  void OpenTransmittalExecuted()
        {
            if (SelectedItem != null)
                _parent.OpenTransmittal(SelectedItem);
        }
        [DebuggerStepThrough]
        private bool ClearCommandCanExecute()
        {
            return !IsSearching;
        }
        [DebuggerStepThrough]
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
        private async void SearchCommandExecuted()
        {
            try
            {
                if (IsSearching)
                {
                    cts.Cancel(false);
                    IsSearching = false;
                    //worker.CancelAsync();
                }
                else
                {
                    Results.Clear();
                    //worker.RunWorkerAsync();
                    cts.Dispose();
                    cts = new CancellationTokenSource();
                    IsSearching = true;
                    await searchModel.SearchByParametersAsync(this, pg, cts.Token);
                }
            }
            catch (Exception ex)
            {
                IsSearching = false;
            }
        }
        #endregion


        public CloseReaction Closing()
        {
            return CloseReaction.ProccedClose;
        }

        public event CloseEventHandler RequestToClose;


        public string Header { get; set; } = "Search";

        public DockState State { get; set; } = DockState.Document;
    }
}
