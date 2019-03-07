using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TransmittalManager.ViewModel
{
    public class TransmittalViewModel : DockViewModelBase
    {


        public int Id { get; }
        public bool IssueToWorkshop { get; set; }
        public string Recipients { get; set; } = "Test";
        public DateTime SentDate { get; set; }
        public IssueType IssueType { get; set; }
        public TransmittalStatus State { get; set; }
        public string Comments { get; set; }
        public string IssueBy { get; set; } = "Test";
        public string ProjectNo { get; set; } = "Test";

        public List<string> Projects { get; } //todo link to common fields

        public ObservableCollection<FileDataViewModel> Files { get; set; }

        public string OkayText
        {
            get {
                switch (State)
                {
                    case TransmittalStatus.Preparing: return "Submit";
                    case TransmittalStatus.WaitingForApproval: return "Approved";
                    case TransmittalStatus.Received:
                    case TransmittalStatus.Issued:
                    default: return "Close";
                }

            }
        }

        public RelayCommand<ICloseable> OkayCommand { get; set; }
        public RelayCommand<ICloseable> CancelCommand { get; set; }

        public RelayCommand AddFileCommand { get; set; }
        public RelayCommand RemoveFileCommand { get; set; }

        public bool IsEnabled { get; internal set; }


       
        public TransmittalViewModel(DockItem di) : base(di)
        {
            OkayCommand = new RelayCommand<ICloseable>(OkayCommandExecute, OkayCommandCanExecute);
            CancelCommand = new RelayCommand<ICloseable>(CancelCommandExecute, CancelCommandCanExecute);
        }

         [PreferredConstructor]
        public TransmittalViewModel()
        {
            OkayCommand = new RelayCommand<ICloseable>(OkayCommandExecute, OkayCommandCanExecute);
            CancelCommand = new RelayCommand<ICloseable>(CancelCommandExecute, CancelCommandCanExecute);

        }



        #region Commands



        private void OkayCommandExecute(ICloseable obj)
        {
            obj.DialogResult = true;
            obj.Close();
        }

        private bool OkayCommandCanExecute(ICloseable arg)
        {
            return true;
        }

        private void CancelCommandExecute(ICloseable obj)
        {
            obj.DialogResult = false;
            obj.Close();
        }

        private bool CancelCommandCanExecute(ICloseable arg) => true;

        #endregion

        public override bool? Closing()
        {
            return false;
        }
    }
}
