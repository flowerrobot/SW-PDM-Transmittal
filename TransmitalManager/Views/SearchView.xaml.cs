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
using TransmittalManager.ViewModel;

namespace TransmittalManager.Views
{
    /// <summary>
    /// Interaction logic for SearchView.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView()
        {
            InitializeComponent();
        }

        public SearchView(SearchViewModel searcher)
        {
            InitializeComponent();
            Searcher = searcher;
        }

        public SearchViewModel Searcher
        {
            get => (SearchViewModel)GetValue(SearcherProperty);
            set {
                SetValue(SearcherProperty, value);
                LayoutRoot.DataContext = value;
            }
        }


        public static readonly DependencyProperty SearcherProperty = DependencyProperty.Register("Searcher", typeof(SearchViewModel), typeof(SearchView), new PropertyMetadata(false));
    }
}
