using System;
using System.Collections.Generic;
using DockingAdapterMVVM;
using GalaSoft.MvvmLight;

namespace TestApp.ViewModel
{
    public class SearchTest : ViewModelBase, IDockElement
    {
        public string FileName { get; set; }
        public string ProjectNo { get; set; }
        public string IssueBy { get; set; }

        public DateTime? SentByDate { get; set; }
        public DateTime? SentAfterDate { get; set; }
        public int? IssueType { get; set; }
        public bool? IssueToWorkshop { get; set; }


        public List<string> Projects { get; } //todo link to common fields
        //public RelayCommand SearchCommand { get; set; }
        //public RelayCommand ClearCommand { get; set; }


        public string SearchText
        {
            get {
                //if (IsSearching) return "Cancel";
                return "Search";
            }
        }

        public string Header { get; set; } = "I Look";
        public DockState State { get; set; } = DockState.Document;

        private bool IsSearching;
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

            //if (IsSearching)
            //{
            //    worker.CancelAsync();
            //}
            //else
            //{
            //    Results.Clear();
            //    worker.RunWorkerAsync();
            //}
        }
        #endregion

    }
}
