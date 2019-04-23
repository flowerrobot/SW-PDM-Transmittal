using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockingAdapterMVVM
{
    public interface IDockElement
    {
        string Header { get; set; }

        DockState State { get; set; }

        /// <summary>
        /// Called when about to close, return true to cancel
        /// </summary>
        /// <returns></returns>
        bool? Closing();

        /// <summary>
        /// Raise this event if the dialog will close
        /// </summary>
        event EventHandler RequestToClose;

    }
}
