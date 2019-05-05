using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmittalManager.Interfaces
{
    public class Dockable
    {
        public delegate void CloseEventHandler(object viewModel, int Reason);

        public enum CloseReason : int
        {
            Unknown,
            ToSave,
            Close,
            Cancel

        };
    }
}
