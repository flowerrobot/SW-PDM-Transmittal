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
        public string Recipients { get; set; } = "Test";
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


        private bool filesLoaded;
        private DocumentCollectionViewModel files = new DocumentCollectionViewModel();
        /// <summary>
        /// THe collection of files, If not loaded it will return an empty collection while it loads, then send a proptery change notify event to say its updated
        /// </summary>
        public DocumentCollectionViewModel Files
        {
            get {
                if (!filesLoaded)
                    if (!Bgw.IsBusy)
                        Bgw.RunWorkerAsync(); //Load files async, the rely on property notify to say its uploaded


                return files;

            }
            private set {
                if (value != null)
                {
                    files = value;
                    filesLoaded = true;
                }
            }
        }

        // Common lookups
        public List<Project> Projects => Models.Project.AllProjects;
        public List<IssueType> IssueTypes => new List<IssueType>(new[] { IssueType.ForManufacture, IssueType.ForInformation, IssueType.ForApproval });

        public string OkayText
        {
            get {
                if (User.ActiveUser.Group == Groups.NoPermisions) return "Close";
                switch (TransmittalStatus)
                {
                    case TransmittalStatus.Preparing:
                        {
                            if (User.ActiveUser.Group.HasFlag(Groups.ProjectManager))
                                return "Issue";
                            else
                                return "Submit to PM";

                        }
                    case TransmittalStatus.WaitingForApproval: return "Issue";
                    case TransmittalStatus.Received:
                    case TransmittalStatus.Issued:
                    default: return "Close";
                }

            }
        }

        public RelayCommand OkayCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        public RelayCommand AddFileCommand { get; set; }
        public RelayCommand RemoveFileCommand { get; set; }

        public bool ShowCloseButton
        {
            get { return IsEnabled; }
        }

        public bool IsEnabled
        {
            get {
                if (!transmittalModel.IsLoadedFromDb) return true;
                if (User.ActiveUser.Group == Groups.NoPermisions) return false;
                if (TransmittalStatus == TransmittalStatus.Issued) return false;
                return true;
            }
        }

        private Transmittal transmittalModel;
        private IProgress<Document> pg;
        //public TransmittalViewModel(DockItem di) : base(di)
        //{
        //    OkayCommand = new RelayCommand<ICloseable>(OkayCommandExecute, OkayCommandCanExecute);
        //    CancelCommand = new RelayCommand<ICloseable>(CancelCommandExecute, CancelCommandCanExecute);
        //}

        [PreferredConstructor]
        public TransmittalViewModel()
        {
            ///This method shouldn't be used other than for debug
            OkayCommand = new RelayCommand(OkayCommandExecute, OkayCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);
            AddFileCommand = new RelayCommand(AddFileExecute, FileEditCanExecute);
            RemoveFileCommand = new RelayCommand(RemoveFileExecute, FileRemoveCanExecute);

            transmittalModel = new Transmittal();
            TransmittalStatus = TransmittalStatus.Preparing;
            //Projects = new List<string>();
            //Models.Projects.AllProjects.ForEach(t => Projects.Add($"{t.Number} {t.Name}"));
            files = new DocumentCollectionViewModel();
        }
        public TransmittalViewModel(Transmittal transmittal)
        {
            OkayCommand = new RelayCommand(OkayCommandExecute, OkayCommandCanExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute, CancelCommandCanExecute);
            pg = new Progress<Document>(DocsUpdated);
            transmittalModel = transmittal;

            //Projects = new List<string>();
            //Models.Projects.AllProjects.ForEach(t => Projects.Add($"{t.Number} {t.Name}"));

            if (transmittalModel.IsLoadedFromDb)
            {
                Id = transmittalModel.Id;
                IssueToWorkshop = transmittalModel.IssueToWorkshop;
                Recipients = transmittalModel.Recipients;
                SentDate = transmittalModel.SentDate;
                IssueType = transmittalModel.IssueType;
                TransmittalStatus = transmittalModel.TransmittalStatus;
                Comments = transmittalModel.Comments;
                IssueBy = transmittalModel.IssueBy;
                CreatedBy = transmittalModel.CreatedBy;
                Project = transmittalModel.Project;

                if (transmittalModel.FilesLoad)
                {
                    filesLoaded = true;
                    transmittalModel.Files.ForEach(f => files.Add(new FileDataViewModel(f)));
                }
            }
            else
            {               
                CreatedBy = transmittalModel.CreatedBy;
                TransmittalStatus = TransmittalStatus.Preparing;
            }
        }

        /// <summary>
        /// Load files if can, used when showing the main form
        /// </summary>
        public List<FileDataViewModel> LoadFiles(CancellationToken cts)
        {
            if (transmittalModel?.FilesLoad ?? true) return null;
            Task<List<Document>> t = transmittalModel.LoadFilesAsync(null, cts);

            List<FileDataViewModel> vv = new List<FileDataViewModel>();
            t.Wait(cts);
            if (t.Result == null) return null;
            foreach (Document v in t.Result)
            {
                vv.Add(new FileDataViewModel(v));
            }

            return vv;
        }

        private CancellationTokenSource cts;
        /// <summary>
        /// Loads files async, but does not add them to the collection.
        /// </summary>
        /// <returns></returns>
        public async Task<List<FileDataViewModel>> LoadFilesAsync(CancellationToken cts)
        {

            if (cts == null) cts = new CancellationTokenSource().Token;

            if (transmittalModel?.FilesLoad ?? true) return null;
            Task<List<Document>> t = transmittalModel.LoadFilesAsync(null, cts);

            List<FileDataViewModel> vv = new List<FileDataViewModel>();
            await t;

            if (t.Result == null) return null;
            foreach (Document v in t.Result)
            {
                vv.Add(new FileDataViewModel(v));
            }

            return vv;
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

        private bool cancel;

        private void OkayCommandExecute()
        {
            cancel = false;
            RequestToClose?.Invoke(this, null);
        }
        [DebuggerStepThrough]
        private bool OkayCommandCanExecute()
        {
            if (Files.Count == 0) return false;
            if (Project == null) return false;
            if (IssueType == 0) return false;
            return true;
        }

        private void CancelCommandExecute()
        {
            cancel = true;
            RequestToClose?.Invoke(this, null);
        }
        [DebuggerStepThrough]
        private bool CancelCommandCanExecute() => true;

        [DebuggerStepThrough]
        private bool FileEditCanExecute()
        {
            return IsEnabled;
        }
        private bool FileRemoveCanExecute()
        {
            return IsEnabled && Files.SelectedItem != null;
        }
        private void RemoveFileExecute()
        {
            if (Files.SelectedItem != null)
                Files.Remove(Files.SelectedItem);
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
        #endregion

        public bool? Closing()
        {
            if (cancel) return false;
            MessageBoxResult res = MessageBox.Show("Save?", "Save changes", MessageBoxButton.YesNoCancel);
            System.Threading.Tasks.Task<bool> a;
            if (res == MessageBoxResult.Yes)
                a = transmittalModel.Save(this);
            else if (res == MessageBoxResult.No)
                if (res == MessageBoxResult.Cancel)
                    return true;
            return false;
        }

        public event EventHandler RequestToClose;

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
            if (e.Result is List<FileDataViewModel> fi)
            {
                Files = new DocumentCollectionViewModel(fi);
                //List<FileDataViewModel> vv = new List<FileDataViewModel>();
                //foreach (Document v in fi)
                //{
                //    vv.Add(new FileDataViewModel(v));
                //}

            }

            filesLoaded = true;
        }

        private void BgwOnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
