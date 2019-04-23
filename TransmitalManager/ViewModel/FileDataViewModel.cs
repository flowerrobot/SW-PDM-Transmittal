using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using TransmittalManager.Models;

namespace TransmittalManager.ViewModel
{
    public class FileDataViewModel : ViewModelBase
    {

            public FileDataViewModel(Document doc)
        {
            Model = doc;

            Id = doc.Id;
            Name = System.IO.Path.GetFileName(doc.FileName);
            Revision = doc.Revision;
            Version = doc.Version;
            FileState = doc.FileState;
            //Status = doc.

        }

        public Document Model { get; }

        public int Id { get; private set; }
        public string Name { get; private set; } = "A Name";
        public string Description { get; private set; } = "Desc";
        public string Revision { get; private set; } = "A";
        public  int Version { get; private set; }
        public string FileState { get; private set; } = "Test";
        public int TotalQty { get; set; }
       

        //public FileStatus Status { get; set; } = FileStatus.Unknown;
        

        public FileInfo PDFFile { get; set;}
        public ObservableCollection<FileDataViewModel> Children { get; } = new ObservableCollection<FileDataViewModel>();


        public bool IsChecked { get; set;}
    }
}
