using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using Syncfusion.Windows.Tools.Controls;

namespace TransmittalManager.ViewModel
{
    public abstract class DockViewModelBase : ViewModelBase
    {
        protected DockViewModelBase()
        {
        }
        protected DockViewModelBase( DockItem di) :this(di.DockingManager, di)
        {
        }
        protected   DockViewModelBase(DockingManager mgr, DockItem di)
        {
            DockingManager = mgr;
            DockItem = di;

            DockingManager.CloseButtonClick += DockingManager_CloseButtonClick;
        }

        private void DockingManager_CloseButtonClick(object sender, CloseButtonEventArgs e)
        {
            if (Closing() == true)
            {
                e.Cancel = true;
            }
        }

        public DockItem DockItem { get; set; }
        public DockingManager DockingManager { get; set; }

        /// <summary>
        /// Return True to Cancel
        /// </summary>
        /// <returns></returns>
        public abstract bool? Closing();



    }
}
