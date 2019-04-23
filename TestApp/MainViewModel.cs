using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DockingAdapterMVVM;
using TestApp.ViewModel;
using TransmittalManager.ViewModel;

namespace TestApp
{
    public class MainViewModel
    {
        public ObservableCollection<IDockElement> Names { get; set; } = new ObservableCollection<IDockElement>();


        public MainViewModel()
        {
            Names.Add(new NameViewModel());
            Names.Add(new NameViewModel());
            Names.Add(new NameViewModel());

            Names.Add(new SearchTest());
            Names.Add(new SearchViewModel());
        }
    }


}
