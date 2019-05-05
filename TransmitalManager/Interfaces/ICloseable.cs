using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmittalManager.Interfaces
{
    public interface ICloseable
    {
        void Close();
        bool? DialogResult { get; set; }


    }
}
