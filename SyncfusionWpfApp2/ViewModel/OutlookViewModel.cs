#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Syncfusion.Windows.Shared;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Tools.Controls;

namespace SyncfusionWpfApp2
{
    public class OutlookViewModel : INotifyPropertyChanged
    {
        public OutlookViewModel()
        {            
            UpdateMail();
        }

        #region Mail

        private ObservableCollection<MailDetails> mailInfo;

        public ObservableCollection<MailDetails> MailInfo
        {
            get { return mailInfo; }
            set
            {
                mailInfo = value;
                OnPropertyChanged("Details");
            }
        }

        private object selectedMail;

        public object SelectedMail
        {
            get { return selectedMail; }
            set
            {
                selectedMail = value;
                OnPropertyChanged("SelectedMail");
                UpdateMailContent(selectedMail);
            }
        }

        private object selectedcategory;

        public object SelectedCategory
        {
            get { return selectedcategory; }
            set
            {
                selectedcategory = value;
                OnPropertyChanged("SelectedCategory");
                UpdateMailList();
            }
        }

        private ObservableCollection<MailDetails> _CategoryMails;

        public ObservableCollection<MailDetails> CategoryMails
        {
            get { return _CategoryMails; }
            set
            {
                _CategoryMails = value;
                OnPropertyChanged("CategoryMails");
            }
        }

        private string from;

        public string From
        {
            get { return from; }
            set
            {
                from = value;
                OnPropertyChanged("From");
            }
        }

        private string to;

        public string To
        {
            get { return to; }
            set
            {
                to = value;
                OnPropertyChanged("To");
            }
        }

        private string subject;

        public string Subject
        {
            get { return subject; }
            set
            {
                subject = value;
                OnPropertyChanged("Subject");
            }
        }

        private string mailContent;

        public string MailContent
        {
            get { return mailContent; }
            set
            {
                mailContent = value;
                OnPropertyChanged("MailContent");
            }
        }

        private bool isReadOnly = true;

        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                isReadOnly = value;
                OnPropertyChanged("IsReadOnly");
            }
        }
        public void UpdateMail()
        {
            MailInfo = new ObservableCollection<MailDetails>();
            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "ross@syncfusion.com",
                To = "john@syncfusion.com",
                Content = "Can we schedule Meeting Appointment for today?",
                Subject = "Meeting",
                MailCategory = MailDetails.Category.Inbox
            });
            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "arya@syncfusion.com",
                To = "john@syncfusion.com",
                Content = "Hope you doing well. Can you send me the list of features available with your product?",
                Subject = "Features",
                MailCategory = MailDetails.Category.Inbox
            });
            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "chad@syncfusion.com",
                To = "john@syncfusion.com",
                Content = "Thank you for inquiring about our new enterprise application. A team member will contact you tomorrow with a detailed explanation of the product that fits your business need.",
                Subject = "Enterprise Application",
                MailCategory = MailDetails.Category.Inbox
            });
            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "tywinl@syncfusion.com",
                To = "john@syncfusion.com",
                Content = "Thanks for subscribe our Add-In. We keep sending you the update of our Add-In. Feel free to contact us if you have any other queries.",
                Subject = "Addin purchase",
                MailCategory = MailDetails.Category.Inbox
            });
            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "jacob@syncfusion.com",
                To = "john@syncfusion.com",
                Content = "Thanks for downloading Essential Studio. We keep sending you the update of our Add-In. Feel free to contact us if you have any other queries.",
                Subject = "Essential Studio",
                MailCategory = MailDetails.Category.Inbox
            });

            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "arya@syncfusion.com",
                To = "davids@syncfusion.com",
                Content = "I have attached the purchase details, kindly go through it and reply your feedback",
                Subject = "Purchase details",
                MailCategory = MailDetails.Category.SentItems
            });
            MailInfo.Add(new MailDetails()
            {
                Date = DateTime.Now,
                From = "arya@syncfusion.com",
                To = "joey@syncfusion.com",
                Content = "I will not be available by today. Can you schedule the meeting by tomorrow? Reply your convenient time",
                Subject = "Meeting",
                MailCategory = MailDetails.Category.SentItems
            });
        }
        public void UpdateMailDetails(string MailCategory)
        {
            CategoryMails = new ObservableCollection<MailDetails>(MailInfo.Where(e => e.MailCategory.ToString() == MailCategory));
        }
        private void UpdateMailList()
        {
            UpdateMailDetails((SelectedCategory as TreeViewItemAdv).Header.ToString().Replace(" ", ""));
        }
        public void UpdateMailContent(object selectedMail)
        {
            if (selectedMail is MailDetails)
            {
                MailDetails maildetail = selectedMail as MailDetails;
                string sender = maildetail.From.Substring(0, maildetail.From.IndexOf('@'));
                string reciever = maildetail.To.Substring(0, maildetail.To.IndexOf('@'));
                StringBuilder MyStrBuilder = new StringBuilder("<table border=\"0\"><tbody>");
                MyStrBuilder.Append("<tr><td>&nbsp;</td></tr>");
                MyStrBuilder.Append("<tr><td>Hi " + reciever + ",</td></tr>");
                MyStrBuilder.Append("<tr><td>&nbsp;</td></tr>");
                MyStrBuilder.Append("<tr><td colspan=\"2\" align=\"left\">" + maildetail.Content + "</td></tr>");
                MyStrBuilder.Append("<tr><td>&nbsp;</td></tr>");
                MyStrBuilder.Append("<tr><td>Regards,</td></tr>");
                MyStrBuilder.Append("<tr><td>" + sender + "</td></tr>");
                MyStrBuilder.Append("</tbody></table>");

                MailContent = MyStrBuilder.ToString();
                From = maildetail.From;
                To = maildetail.To;
                Subject = maildetail.Subject;
            }
        }
        #endregion

        #region PropertyHandler
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        #endregion
    }
}
