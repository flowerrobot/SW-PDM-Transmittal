using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransmittalManager
{
    public enum FileStatus : int
    {
        Unknown,
        UnReleased,
        Released,
        Outdated
    }

    public enum IssueType : int
    {
        Unknown,
        ForInformation,
        ForApproval,
        ForManufacture,
        ForTender,
        ForMaterialTakeOff
    }

    public enum TransmittalStatus : int
    {
        Unknown,
        Preparing,
        WaitingForApproval,
        Issued,
        Received
    }

    [Flags]
    public enum Groups
    {
        NoPermisions = 1,
        Designer = 2,
        ProjectManager = 4
    }
}
