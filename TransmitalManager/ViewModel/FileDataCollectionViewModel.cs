using System.Collections.ObjectModel;

namespace TransmittalManager.ViewModel
{
    public class FileDataCollectionViewModel : ObservableCollection<FileDataViewModel>
    {
        public bool FlatView { get; }
        public bool ShowChecked { get; }
    }
}
