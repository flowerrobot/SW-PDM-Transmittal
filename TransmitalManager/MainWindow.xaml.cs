using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
using System.Windows;
using TransmittalManager.ViewModel;

namespace TransmittalManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //  docking.DataContext = this;
            MV = (DataContext as MainViewModel);
            dockPan.DataContext = MV;

  
  
        }
        public MainViewModel MV { get; }


        private void Docking_OnCloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
