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
using System.Collections.Generic;
using TransmittalManager.Models;

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
        [PreferredConstructor]
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
            SimpleIoc.Default.Register<SearchViewModel>();
            SimpleIoc.Default.Register<DocumentCollectionViewModel>();
            //SimpleIoc.Default.Register<List<RecipientViewModel>>();
        }

        private MainViewModel mv;
        public MainViewModel Main
        {
            get {
                if (mv == null) mv = new MainViewModel();
                return mv;
                //   return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public TransmittalViewModel TransView => new TransmittalViewModel();//ServiceLocator.Current.GetInstance<TransmittalViewModel>();

        public DocumentCollectionViewModel FileData
        {
            get {
#if DEBUG
                DocumentCollectionViewModel fd = new DocumentCollectionViewModel();

                FileDataViewModel i1 = new FileDataViewModel(new Document() { Description = "Hi", Revision = "0", FileState = "Released" });

                fd.Add(i1);
                //  fd.Add(new FileDataViewModel { Name = "two" });
                return fd;
#endif

            }
        }

        public RecipientsSelectionViewModel Recipients
        {
            get {
                var a = new RecipientsSelectionViewModel();
#if DEBUG

                a.AllRecipients.Add(new RecipientViewModel(new Recipient() { Name = "Hello" }));
                a.AllRecipients.Add(new RecipientViewModel(new Recipient { Name = "Hello1", Email = "lol@" }));
                a.AllRecipients.Add(new RecipientViewModel(new Recipient { Name = "Hello2", Email = "lol" }) { IsSelected = true });

                //#else

                foreach (var r in Recipient.AllRecipients)
                {
                    a.AllRecipients.Add(new RecipientViewModel(r));
                }
#endif

                return a;
            }
        }

        public SearchViewModel SearchView => new SearchViewModel();

        public static void Cleanup() { }
    }
}