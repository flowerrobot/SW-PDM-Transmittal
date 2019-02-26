/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TransmittalManager"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace TransmittalManager.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<TransmittalViewModel>();
            SimpleIoc.Default.Register<FileDataCollectionViewModel>();
        }

        public MainViewModel Main
        {
            get {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public TransmittalViewModel TransView => ServiceLocator.Current.GetInstance<TransmittalViewModel>();

        public FileDataCollectionViewModel FileData
        {
            get {
                FileDataCollectionViewModel fd = new FileDataCollectionViewModel();

                FileDataViewModel i1 = new FileDataViewModel { Name = "Hi", Revision = "0", FileState = "Released" };

                fd.Add(i1);
                fd.Add(new FileDataViewModel { Name = "two" });

                return fd;
            }
        }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}