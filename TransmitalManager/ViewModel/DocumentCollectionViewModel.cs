using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Ioc;

namespace TransmittalManager.ViewModel
{
    public class DocumentCollectionViewModel : ObservableCollection<FileDataViewModel>
    {
        public DocumentCollectionViewModel(List<FileDataViewModel> files) : base(files)
        {}
        [PreferredConstructor]
        public DocumentCollectionViewModel() : base()
        {
        }

        public bool ShowChecked { get; set; } = false;
        public FileDataViewModel SelectedItem { get; set; }

        public bool ShowQty { get; set; } = true;

    }
}
