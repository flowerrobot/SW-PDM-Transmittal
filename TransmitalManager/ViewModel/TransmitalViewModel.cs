using System;

namespace TransmittalManager.ViewModel
{
    public class TransmittalViewModel
    {
        private bool IssueToWorkshop { get; set; }
        public string Recipients { get; set; }
        public DateTime SentDate { get; set; }
        public IssueType IssueType { get; set; }
        public TransmittalStatus State { get; set; }
        public string Comments { get; set; }

        public FileDataCollectionViewModel Files { get; set; }

        public string OkayText
        {
            get {
                switch (State)
                {
                    case TransmittalStatus.Preparing: return "Submit";
                    case TransmittalStatus.WaitingForApproval: return "Approved";
                    case TransmittalStatus.Received:
                    case TransmittalStatus.Issued:
                    default: return "Okay";
                }

            }
        }
    }
}
