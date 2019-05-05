using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransmittalManager.Models;

namespace TransmittalManager.ViewModel
{
    public class RecipientViewModel : ViewModelBase, ICloneable
    {
        public int Id => Recipient.Id;
        public CompanyViewModel Company => Recipient.Company;
        public string Name => Recipient.Name;
        public string Email => Recipient.Email;

        public bool IsSelected { get; set; }

        public Recipient Recipient { get; }

        public RecipientViewModel(Recipient recipient)
        {
            Recipient = recipient;
        }
        public object Clone() => new RecipientViewModel(Recipient);
    }

}
