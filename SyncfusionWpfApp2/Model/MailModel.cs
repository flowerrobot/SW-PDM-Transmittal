#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Windows.Tools.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows.Media;

namespace SyncfusionWpfApp2
{
    public class MailDetails : NotificationObject
    {
        private string _subject;
        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                _subject = value;
                RaisePropertyChanged("Subject");
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                RaisePropertyChanged("Content");
            }
        }

        private string _from;
        public string From
        {
            get { return _from; }
            set
            {
                _from = value;
                RaisePropertyChanged("From");
            }
        }

        private string _to;

        public string To
        {
            get { return _to; }
            set
            {
                _to = value;
                RaisePropertyChanged("To");
            }
        }

        private Category _category;
        public Category MailCategory
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged("MailCategory");
            }
        }
        private bool _isNew;
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                RaisePropertyChanged("IsNew");
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                RaisePropertyChanged("Date");
            }
        }

        public enum Category
        {
            Inbox,
            SentItems,
            DeleteItems,
            IT,
            Sports,
            Global,
            HR,
            Drafts

        }
    }
}
