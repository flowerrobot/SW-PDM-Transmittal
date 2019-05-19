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

    /// <summary>
    /// Document issue reason
    /// </summary>
    public enum IssueType : int
    {
        Unknown,
        ForInformation,
        ForApproval,
        ForManufacture,
        ForTender,
        ForMaterialTakeOff
    }

    /// <summary>
    /// Current state the transmitallae is in
    /// </summary>
    public enum TransmittalStatus : int
    {
        Unknown,
        Preparing,
        WaitingForApproval,
        Issued,
        Received
    }

    /// <summary>
    /// Represents the permision groups in the app
    /// </summary>
    [Flags]
    public enum Groups
    {
        NoPermisions = 1,
        Designer = 2,
        ProjectManager = 4
    }

    public enum CloseType
    {

    }

}
