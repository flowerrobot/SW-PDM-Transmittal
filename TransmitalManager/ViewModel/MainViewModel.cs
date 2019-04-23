using DockingAdapterMVVM;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TransmittalManager.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}


            NewTransmittalCommand = new RelayCommand(NewTransCommandExecute, NewTransCommandCanExecute);
            SearchCommand = new RelayCommand(SearchCommandExecute, SearchCommandCanExecute);

            SearchCommandExecute();

            //DockCollections = new ObservableCollection<DockItem>();
            //DockCollections.Add(new DockItem() { Header = "ToolBox" });
            //DockCollections.Add(new DockItem() { Header = "Integration" });
            //DockCollections.Add(new DockItem() { Header = "Features" });
        }


        private int searchCount = 0;
        private int transmitalCount = 0;
        public void SearchCommandExecute()
        {
            try
            {
                Windows.Add(new SearchViewModel(this));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }
        [DebuggerStepThrough]
        public bool SearchCommandCanExecute()
        {
            return true;
        }
        [DebuggerStepThrough]
        public bool NewTransCommandCanExecute()
        {
            return true;
        }

        public void NewTransCommandExecute()
        {
            try
            {
                OpenTransmittal(new TransmittalViewModel());
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        public void OpenTransmittal(TransmittalViewModel view)
        {
            try
            {
                if (view.Id != 0) //if view already exists make it active
                    foreach (IDockElement window in Windows)
                    {
                        if (window is TransmittalViewModel tran)
                        {
                            if (tran.Id == view.Id)
                            {
                                ActiveDoc = window;
                                return;
                            }
                        }
                    }

                //Otherwise make it presmt
                view.RequestToClose += View_RequestToClose; //Add a close notify
                Windows.Add(view); //add view to collection
                ActiveDoc = view; //activate view
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
            }
        }

        private void View_RequestToClose(object sender, System.EventArgs e)
        {
            if (sender is IDockElement de)
            {
                if (de.Closing() != true)
                    Windows.Remove(de);
            }
        }

        public ObservableCollection<DockItem> DockCollection { get; } = new ObservableCollection<DockItem>();

        public ObservableCollection<IDockElement> Windows { get; } = new ObservableCollection<IDockElement>();


        public RelayCommand NewTransmittalCommand { get; }
        public RelayCommand SearchCommand { get; }

        public IDockElement ActiveDoc { get; set; }
    }
}