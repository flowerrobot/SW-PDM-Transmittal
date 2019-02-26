using System;
using System.Collections.Generic;
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
    /// Interaction logic for NewTransmittal.xaml
    /// </summary>
    public partial class NewTransmittal : UserControl
    {
        public NewTransmittal()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
        [AutoDependencyProperty]
        public TransmittalViewModel Transmittal { get; set; }
    }
}
