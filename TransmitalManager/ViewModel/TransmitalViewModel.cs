using DockingAdapterMVVM;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
#if !BlockPDM
using EPDM.Interop.epdm;
#endif
using TransmittalManager.Models;
using DockState = DockingAdapterMVVM.DockState;

namespace TransmittalManager.ViewModel
{
    public class TransmittalViewModel : ViewModelBase, IDockElement
    {
        public int? Id { get; }
        public bool IssueToWorkshop { get; set; }
        public RecipientsSelectionViewModel Recipients { get; set; }
        public DateTime? SentDate { get; set; }

        public string SentDateString
        {
            get {
                if (SentDate == null) return "TBA";
                return SentDate.ToString();
            }
        }

        public IssueType IssueType { get; set; }
        public TransmittalStatus TransmittalStatus { get; set; }
        public string Comments { get; set; }
        public User IssueBy { get; set; }
        public User CreatedBy { get; set; }
        public Project Project { get; set; }


        private DocumentCollectionViewModel files = new DocumentCollectionViewModel();
        /// <summary>
        /// THe collection of files, If not loaded it will return an empty collection while it loads, then send a proptery change notify event to say its updated
        /// </summary>
        public DocumentCollectionViewModel Files
        {
            get {
                if (!transmittalModel.ExtendedDataLoaded)
                    if (!Bgw.IsBusy)
                        Bgw.RunWorkerAsync(); //Load files async, the rely on property notify to say its uploaded


                return files;

            }
            private set {
                if (value != null)
                {
                    files = value;
                }
            }
        }
        public object ViewOverlay { get; set; }

        // Common lookups
        public List<Project> Projects => Models.Project.AllProjects;
        public List<IssueType> IssueTypes => new List<IssueType>(new[] { IssueType.ForManufacture, IssueType.ForInformation, IssueType.ForApproval });



        /// <summary>
        /// If the other button is a close button, hide this button
        /// </summary>
        public bool ShowCloseButton => ButtonViewType != ButtonType.Cancel;


        public bool IsEnabled => ViewStatus.HasFlag(ViewStatusTypes.Editable);

        private Transmittal transmittalModel;
        private IProgress<Document> pg;

        /// <summary>
        /// This is an internal status controlling the view and what is allowable
        /// </summary>
        public ViewStatusTypes ViewStatus { get; internal set; }
        /// <summary>
        /// This indicates the state the okay\cancel buttons should be.
        /// </summary>
        public ButtonType ButtonViewType
        {
            get {

                switch (TransmittalStatus)
                {
                    case TransmittalStatus.Preparing:
                        {
                            if (User.ActiveUser.Group.HasFlag(Groups.ProjectManager))
                                return ButtonType.Submit;
                            else
                                return ButtonType.Cancel;
                        }
                    case TransmittalStatus.WaitingForApproval:
                        {
                            if (User.ActiveUser.Group.HasFlag(Groups.ProjectManager))
                                return ButtonType.Submit;
                            else
                                return ButtonType.Approval;
                        }

                    case TransmittalStatus.Received:
                    case TransmittalStatus.Issued:
                    default: return ButtonType.Cancel;
                }
            }
        }

        [PreferredConstructor]
        public TransmittalViewModel()
        {
            ///This method shouldn't be used other than for debug
            OkayCommand = new RelayCommand(OkayCommandExecute, OkayCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);
            AddFileCommand = new RelayCommand(AddFileExecute, FileEditCanExecute);
            RemoveFileCommand = new RelayCommand(RemoveFileExecute, FileRemoveCanExecute);
            EditRecipientsCommand = new RelayCommand(EditRecipientExecute, FileRemoveCanExecute);

            transmittalModel = new Transmittal();
            TransmittalStatus = TransmittalStatus.Preparing;
            files = new DocumentCollectionViewModel();

            ViewStatus = ViewStatusTypes.NewlyCreated & ViewStatusTypes.Editable;
        }
        public TransmittalViewModel(Transmittal transmittal)
        {
            OkayCommand = new RelayCommand(OkayCommandExecute, OkayCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);
            AddFileCommand = new RelayCommand(AddFileExecute, FileEditCanExecute);
            RemoveFileCommand = new RelayCommand(RemoveFileExecute, FileRemoveCanExecute);
            EditRecipientsCommand = new RelayCommand(EditRecipientExecute, EditRecipientCanExecute);

            pg = new Progress<Document>(DocsUpdated);
            transmittalModel = transmittal;


            if (transmittalModel.IsLoadedFromDb)
            {
                Id = transmittalModel.Id;
                IssueToWorkshop = transmittalModel.IssueToWorkshop;

                SentDate = transmittalModel.SentDate;
                IssueType = transmittalModel.IssueType;
                TransmittalStatus = transmittalModel.TransmittalStatus;
                Comments = transmittalModel.Comments;
                IssueBy = transmittalModel.IssueBy;
                CreatedBy = transmittalModel.CreatedBy;
                Project = transmittalModel.Project;

                if (transmittalModel.ExtendedDataLoaded)
                {
                    transmittalModel.Files.ForEach(f => files.Add(new FileDataViewModel(f)));

                    Recipients = new RecipientsSelectionViewModel(transmittalModel.Recipients);
                }
            }
            else
            {
                CreatedBy = transmittalModel.CreatedBy;
                TransmittalStatus = TransmittalStatus.Preparing;
            }


            if (!transmittalModel.IsLoadedFromDb) ViewStatus &= ViewStatusTypes.LoadedDb;
            if (User.ActiveUser.Group == Groups.NoPermisions) ViewStatus &= ViewStatusTypes.ViewOnly;
            if (TransmittalStatus == TransmittalStatus.Issued) ViewStatus &= ViewStatusTypes.Approved;

            if (!ViewStatus.HasFlag(ViewStatusTypes.ViewOnly) && !ViewStatus.HasFlag(ViewStatusTypes.Approved)) ViewStatus &= ViewStatusTypes.Editable; //Otherwise it should be editable.
        }


        ImportedData? ImportExtendedData()
        {
            if (!transmittalModel.ExtendedDataLoaded) return null;

            var iEd = new ImportedData();
            iEd.files = new List<FileDataViewModel>();
            iEd.recipients = new List<Recipient>();

            foreach (Document v in transmittalModel.Files)
            {
                iEd.files.Add(new FileDataViewModel(v));
            }

            foreach (var r in transmittalModel.Recipients)
            {
                iEd.recipients.Add(r);
            }

            return iEd;
        }
        private CancellationTokenSource cts;
        /// <summary>
        /// Loads files async, but does not add them to the collection.
        /// </summary>
        /// <returns></returns>
        public async Task<ImportedData?> LoadFilesAsync(CancellationToken cts)
        {

            if (cts == null) cts = new CancellationTokenSource().Token;

            if (transmittalModel?.ExtendedDataLoaded ?? true) return null;
            var t = transmittalModel.LoadExtendedDataAsync(null, cts);

            //List<FileDataViewModel> vv = new List<FileDataViewModel>();
            await t;

            return ImportExtendedData();
            //if (t.Result == null) return null;
            //foreach (Document v in t.Result)
            //{
            //    vv.Add(new FileDataViewModel(v));
            //}
        }

        /// <summary>
        /// Event update from IProgress when looking for new docs
        /// </summary>
        /// <param name="obj"></param>
        private void DocsUpdated(Document obj)
        {
            Files.Add(new FileDataViewModel(obj));
        }

        #region Commands



        public RelayCommand OkayCommand { get; }
        public RelayCommand CancelCommand { get; }

        public RelayCommand AddFileCommand { get; }
        public RelayCommand RemoveFileCommand { get; }
        public RelayCommand EditRecipientsCommand { get; }


        public string OkayText
        {
            get {

                switch (ButtonViewType)
                {

                    case ButtonType.Approval:
                        return "Submit to PM";
                    case ButtonType.Submit:
                        return "Issue";
                    case ButtonType.Cancel:
                    default: return "Close";
                }
            }
        }
        private void OkayCommandExecute()
        {
            switch (ButtonViewType)
            {
                case ButtonType.Approval:
                case ButtonType.Submit:
                    RequestToClose?.Invoke(this, CloseReason.ToSave);
                    break;

                case ButtonType.Cancel:
                    RequestToClose?.Invoke(this, CloseReason.Cancel);
                    break;
                default:
                    RequestToClose?.Invoke(this, CloseReason.Unknown);
                    break;
            }
        }
        //[DebuggerStepThrough]
        private bool OkayCommandCanExecute()
        {
            if (ButtonViewType == ButtonType.Cancel) return true;

            //Verify data
            if (Files.Count == 0) return false;
            if (Project == null) return false;
            if (IssueType == 0) return false;

            return true;
        }

        private void CancelCommandExecute()
        {
            // cancel = true;
            RequestToClose?.Invoke(this, CloseReason.Cancel);
        }
        [DebuggerStepThrough]
        private bool CancelCommandCanExecute() => true;

        [DebuggerStepThrough]
        private bool FileEditCanExecute()
        {
            return IsEnabled;
        }
        private void AddFileExecute()
        {
#if BlockPDM

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "Files (*.dxf,*.Pdf)|*.dxf;*.pdf";
            if (ofd.ShowDialog() == true)
            {
                foreach (string fileName in ofd.FileNames)
                {
                    Files.Add(new FileDataViewModel(new Document(fileName) { FileId = 1, Description = "Test" }));
                }
            }
#else



            IEdmVault11 vault = (IEdmVault11)new EdmVault5();
            EdmStrLst5 res = vault.BrowseForFile(0, (int)EdmBrowseFlag.EdmBws_ForOpen + (int)EdmBrowseFlag.EdmBws_PermitVaultFiles,
                "Select Files", "All Files (*.pdf,*.dxf,*.step)|*.pdf;*.dxf;*.step||", "", "", "Select Files");
            if (res != null)
            {
                IEdmPos5 pos = res.GetHeadPosition();
                List<string> paths = new List<string>();
                IEdmBatchListing4 bli = vault.CreateUtility(EdmUtility.EdmUtil_BatchList);
                while (pos.IsNull)
                {

                    //paths.Add(res.GetNext(pos));
                    bli.AddFile(res.GetNext(pos), default(DateTime), 0, 0);
                }

                string Names = "Description\nRevision";
                EdmListCol[] cols = null;
                EdmListFile2[] files = null;
                bli.CreateListEx(Names, (int)EdmCreateListExFlags.Edmclef_Nothing, ref cols, null);
                bli.GetFiles2(ref files);

                foreach (var file in files)
                {
                    var doc = new Document(file.mbsLockPath);
                    doc.FileId = file.mlFileID;
                    doc.Version = file.mlLatestVersion;
                    doc.Revision = file.mbsRevisionName;
                    doc.FileState = file.moCurrentState.mbsStateName;
                    doc.Description = (file.moColumnData as object[])?[0] as string ?? "Not found";
                    Files.Add(new FileDataViewModel(doc));
                }

            }

            //IEdmVault11 vault = (IEdmVault11)new EdmVault5();
            //EdmStrLst5 res = vault.BrowseForFile(0, (int)EdmBrowseFlag.EdmBws_ForOpen + (int)EdmBrowseFlag.EdmBws_PermitVaultFiles,
            //    "Select Files", "All Files (*.pdf,*.dxf,*.step)|*.pdf;*.dxf;*.step||", "", "", "Select Files");
            //if (res != null)
            //{
            //    IEdmPos5 pos = res.GetHeadPosition();
            //    List<string> paths = new List<string>();
            //    IEdmBatchListing4 bli = vault.CreateUtility(EdmUtility.EdmUtil_BatchList);
            //    while (pos.IsNull)
            //    {

            //        paths.Add(res.GetNext(pos));
            //        //bli.AddFile(res.GetNext(pos), default(DateTime), 0, 0);
            //    }

            //    string Names = "Description\nRevision";
            //    EdmListCol[] cols = null;
            //    EdmListFile2[] files = null;
            //    bli.CreateListEx(Names, (int)EdmCreateListExFlags.Edmclef_GetDrawings + (int)EdmCreateListExFlags.Edmclef_GetReferences + (int)EdmCreateListExFlags.Edmclef_ReturnReferences, ref cols, null);
            //    bli.GetFiles2(ref files);

            //    foreach (var file in files)
            //    {
            //        var doc = new Document(file.mbsLockPath);
            //        doc.FileId = file.mlFileID;
            //        doc.Version = file.mlLatestVersion;
            //        doc.Revision = file.mbsRevisionName;
            //        doc.FileState = file.moCurrentState.mbsStateName;
            //        doc.Description = (file.moColumnData as object[])?[0] as string ?? "Not found";
            //        Files.Add(new FileDataViewModel(doc));
            //    }

            //}
#endif
        }
        [DebuggerStepThrough]
        private bool FileRemoveCanExecute()
        {
            return IsEnabled && Files.SelectedItem != null;
        }
        private void RemoveFileExecute()
        {
            if (Files.SelectedItem != null)
                Files.Remove(Files.SelectedItem);
        }

        [DebuggerStepThrough]
        private bool EditRecipientCanExecute()
        {
            return ButtonViewType == ButtonType.Approval || ButtonViewType == ButtonType.Submit;
        }
        private void EditRecipientExecute()
        {
            ViewOverlay = Recipients;
            //Recipients.RequestToClose
        }
        #endregion

        public CloseReaction Closing(CloseReason reason)
        {
            //IF form wasn't editable they just close
            if (!IsEnabled) return CloseReaction.ProccedClose;
            if (reason == CloseReason.Cancel) return CloseReaction.ProccedClose; //this should be the same

            MessageBoxResult res = MessageBox.Show("Save?", "Save changes", MessageBoxButton.YesNoCancel);
            System.Threading.Tasks.Task<bool> a;
            if (res == MessageBoxResult.Yes)
            {
                if (ButtonViewType == ButtonType.Submit && (reason == CloseReason.Close || reason == CloseReason.Other))
                {
                    a = transmittalModel.Save(this, true);
                }
                else
                { a = transmittalModel.Save(this, false); }
                return CloseReaction.ProccedClose;
            }
            else if (res == MessageBoxResult.No)
                return CloseReaction.ProccedClose;
            else if (res == MessageBoxResult.Cancel)
                return CloseReaction.CancelClose;
            return CloseReaction.ProccedClose;
        }

        public event DockingAdapterMVVM.CloseEventHandler RequestToClose;

        public string Header { get; set; } = "Trans";
        public DockingAdapterMVVM.DockState State { get; set; } = DockState.Document;

        #region Background worker

        private BackgroundWorker bgw;

        internal BackgroundWorker Bgw
        {
            get {
                if (bgw != null) return bgw;
                bgw = new BackgroundWorker();
                bgw.WorkerReportsProgress = true;
                bgw.WorkerSupportsCancellation = true;
                bgw.ProgressChanged += BgwOnProgressChanged;
                bgw.RunWorkerCompleted += BgwOnRunWorkerCompleted;
                bgw.DoWork += BgwOnDoWork;

                return bgw;
            }
        }

        private async void BgwOnDoWork(object sender, DoWorkEventArgs e)
        {
            cts = new CancellationTokenSource();

            //Task<List<Document>> files = Task.Run(LoadFilesAsync);
            //e.Result = files.Result;
            //e.Result = LoadFiles(cts.Token);

            e.Result = await LoadFilesAsync(cts.Token);
        }

        private void BgwOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result is ImportedData fi)
            {

                Files = new DocumentCollectionViewModel(fi.files);
                Recipients = new RecipientsSelectionViewModel(fi.recipients);
            }
        }

        private void BgwOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        [Flags]
        public enum ViewStatusTypes
        {
            NewlyCreated = 1,
            LoadedDb = 2,
            Editable = 4,
            ViewOnly = 8,
            Wait4Approval = 16,
            Approved = 32
        }
        public enum ButtonType
        {
            Cancel,
            Approval,
            Submit
        }
    }


    public struct ImportedData
    {
        public List<Recipient> recipients;
        public List<FileDataViewModel> files;
    }
}
