using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoDependencyPropertyMarker;
using TransmittalManager.ViewModel;

namespace TransmittalManager.Views
{
    /// <summary>
    /// Interaction logic for FileDisplayView.xaml
    /// </summary>
    public partial class FileDisplayView : UserControl
    {
        public FileDisplayView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        [AutoDependencyProperty]
         public bool ShowChecked { get; set; }
         [AutoDependencyProperty]
         public bool FlatView { get; set; }
        [AutoDependencyProperty]
         public ObservableCollection<FileDataViewModel> Items { get; set; }
    }
}
