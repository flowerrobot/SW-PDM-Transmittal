﻿using DockingAdapterMVVM;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransmittalManager.Interfaces;
using TransmittalManager.Models;

namespace TransmittalManager.ViewModel
{
    public class RecipientsSelectionViewModel : ViewModelBase
    {
        public ObservableCollection<RecipientViewModel> AllRecipients { get; } = new ObservableCollection<RecipientViewModel>();
        public RecipientViewModel SelectedItem { get; set; }

        public List<RecipientViewModel> GetSelectedItems()
        {
            List<RecipientViewModel> items = new List<RecipientViewModel>();


            foreach (var r in AllRecipients)
            {
                if (r.IsSelected) items.Add(r);
            }
            return items;
        }

#if DEBUG
        public RecipientsSelectionViewModel()
        {
            AddNewCommand = new RelayCommand(AddNewCommandExecuted, CanAddNewCommand);
            OkayCommand = new RelayCommand(OkayCommandExecuted, CanOkayCommand, true);
            CancelCommand = new RelayCommand(CancelCommandExecuted, CanCancelCommand, true);
                     
        }
#endif
        public RecipientsSelectionViewModel(List<Recipient> preSelected)
        {
            AddNewCommand = new RelayCommand(AddNewCommandExecuted, CanAddNewCommand);
            OkayCommand = new RelayCommand(OkayCommandExecuted, CanOkayCommand, true);
            CancelCommand = new RelayCommand(CancelCommandExecuted, CanCancelCommand, true);
            //Load a recipient selection
            preSelected.ForEach(r => AllRecipients.Add(new RecipientViewModel(r)));
            Recipient.AllRecipients.ForEach(r => { if (preSelected.Any(ps => ps.Id == r.Id)) AllRecipients.Add(new RecipientViewModel(r)); });
        }
        #region Commands

        public RelayCommand AddNewCommand { get; }
        public RelayCommand OkayCommand { get; }
        public RelayCommand CancelCommand { get; }
        private void CancelCommandExecuted()
        {
            RequestToClose?.Invoke(this, CloseReason.Cancel);
        }
        private bool CanCancelCommand() => true;
        private bool CanOkayCommand() => AllRecipients.Any(t => t.IsSelected);
        private void OkayCommandExecuted()
        {
            RequestToClose?.Invoke(this, CloseReason.ToSave);
        }
        private bool CanAddNewCommand() => false;
        private void AddNewCommandExecuted()
        {
            throw new NotImplementedException();
        }
        #endregion

        public event CloseEventHandler RequestToClose;
    }
}
