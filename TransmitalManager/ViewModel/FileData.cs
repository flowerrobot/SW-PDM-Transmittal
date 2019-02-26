using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace TransmittalManager.ViewModel
{
    public class FileDataViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public string Revision { get; set;}
        public string FileState { get; set;}
        public FileStatus Status { get; set;}

        public int PDFStatus { get; set;}
        public FileInfo PDFFile { get; set;}

        public ObservableCollection<FileDataViewModel> Children { get; } = new ObservableCollection<FileDataViewModel>();


        public bool IsChecked { get; set;}
    }
}
