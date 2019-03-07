using Kasay;
using System.Windows;
using System.Windows.Controls;
using TransmittalManager.ViewModel;

namespace TransmittalManager.Views
{
    /// <summary>
    /// Interaction logic for NewTransmittal.xaml
    /// </summary>
    public partial class TransmittalView : UserControl
    {
        public TransmittalView()
        {
            InitializeComponent();
            LayoutRoot.DataContext = Transmittal;
        }

        public TransmittalView(TransmittalViewModel transmittal) : this()
        {
            Transmittal = transmittal;
            LayoutRoot.DataContext = Transmittal;
        }

        //[AutoDependencyProperty]
        //[Bind]
        public TransmittalViewModel Transmittal
        {
            get => (TransmittalViewModel)GetValue(TransmittalProperty);
            set {
                SetValue(TransmittalProperty, value);
                LayoutRoot.DataContext = value;
            }
        }


        public static readonly DependencyProperty TransmittalProperty = DependencyProperty.Register("Transmittal", typeof(TransmittalViewModel), typeof(TransmittalView), new PropertyMetadata(null));

    }
}