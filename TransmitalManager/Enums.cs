using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmittalManager
{
    public enum FileStatus
    {
        Unknown,
        UnReleased,
        Released,
        Outdated
    }

    public enum IssueType
    {
        Unknown,
        ForInformation,
        ForApproval,
        ForManufacture
    }

    public enum TransmittalStatus
    {
        Unknown,
        Preparing,
        WaitingForApproval,
        Issued,
        Received
    }
}
