using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DockingAdapterMVVM
{
    public enum CloseReason : int
    {
        Unknown,
        ToSave,
        Close,
        Cancel,
        Other

    };

    public enum CloseReaction  : int
    {
        None,
        CancelClose,
        ProccedClose
    }
    public delegate void CloseEventHandler(object viewModel, CloseReason Reason);
    public interface IDockElement : IWindowElement
    {
        string Header { get; set; }
        DockState State { get; set; }
    }

    public interface IWindowElement
    {
        /// <summary>
        /// Called when about to close, return true to cancel
        /// </summary>
        /// <returns></returns>
        CloseReaction Closing(CloseReason reason);

        /// <summary>
        /// Raise this event if the dialog will close
        /// </summary>
        event CloseEventHandler RequestToClose;
    }

}
