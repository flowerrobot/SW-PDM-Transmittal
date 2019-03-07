using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Syncfusion.Windows.Tools.Controls;

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
            //DockCollection.Add(new DockItem() { Header = "Test" });
            var doc = new MyDockItem() { Header = "Search",  State = DockState.Document };
            //var doc = new MyDockItem() { Header = "Search", CanDock = true, CanDocument = true, State = DockState.Document, CanFloatMaximize = true };
            if (searchCount > 0)
            {
                doc.Header += searchCount.ToString();
            }
            searchCount += 1;
            //doc.Content = new Views.SearchView(new SearchViewModel((IDockItem)doc));
            DockCollection.Add(doc);
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

            //DockCollection.Add(new MyDockItem(){ Header = "New Transmittal1", CanDock = false, CanDocument = true, State = DockState.Document, CanFloatMaximize = true });
            //DockCollection.Add(new MyDockItem(){ Header = "New Transmittal2", CanDocument = true, State = DockState.Document, CanFloatMaximize = true });
            //DockCollection.Add(new MyDockItem(){ Header = "New Transmittal3",  State = DockState.Document, CanFloatMaximize = true });
            //DockCollection.Add(new MyDockItem(){ Header = "New Transmittal4", CanDocument = true});
            //DockCollection.Add(new MyDockItem(){ Header = "New Transmittal5", State = DockState.Document});



            var doc = new MyDockItem()
            { Header = "New Transmittal", CanDock = false, CanDocument = true, State = DockState.Document, CanFloatMaximize = true };
            if (transmitalCount > 0)
            {
                doc.Header += transmitalCount.ToString();
            }
            transmitalCount += 1;

            var trans = new Views.TransmittalView( new TransmittalViewModel((IDockItem)doc));
            

            doc.Content = trans;
            DockCollection.Add(doc);
        }


        public ObservableCollection<DockItem> DockCollection { get; } = new ObservableCollection<DockItem>();


        public RelayCommand NewTransmittalCommand { get;  }
        public RelayCommand SearchCommand { get; }
    }
}