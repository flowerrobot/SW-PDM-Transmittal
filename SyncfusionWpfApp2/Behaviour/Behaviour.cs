#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;
using Syncfusion.UI.Xaml.Grid.Helpers;

namespace SyncfusionWpfApp2
{
    public class SelectionBehaviour:Behavior<Syncfusion.UI.Xaml.Grid.SfDataGrid>
    {
        protected override void OnAttached()
        {
            this.AssociatedObject.SelectionChanging += AssociatedObject_SelectionChanging;
            this.AssociatedObject.CellTapped += AssociatedObject_CellTapped;         
            this.AssociatedObject.PreviewMouseUp += AssociatedObject_PreviewMouseUp;
            this.AssociatedObject.QueryRowHeight += AssociatedObject_QueryRowHeight;       
        }

        private void AssociatedObject_QueryRowHeight(object sender, QueryRowHeightEventArgs e)
        {
            if (e.RowIndex == 0)
            {
                //RowHeight 0 for HeadeRow
                e.Height = 0;
                e.Handled = true;
            }
            else if (e.RowIndex == 1)
            {
                //RowHeight 25 for CaptionSummaryRow
                e.Height = 25;
                e.Handled = true;
            }
            else
            {
                e.Height = 60;
                e.Handled = true;
            }
        }

        public int columnindex;
        object selectedItem;
        object currentItem;    
        private void AssociatedObject_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var position = e.GetPosition(this.AssociatedObject);
            var visualContainer = this.AssociatedObject.GetVisualContainer();
            columnindex = (visualContainer.PointToCellRowColumnIndex(position)).ColumnIndex;            
        }      

        private void AssociatedObject_SelectionChanging(object sender, Syncfusion.UI.Xaml.Grid.GridSelectionChangingEventArgs e)
        {          
            if (columnindex == 0)
            {
                var IsNew = (this.AssociatedObject.CurrentItem as MailDetails).IsNew;
                if (IsNew)
                    (this.AssociatedObject.CurrentItem as MailDetails).IsNew = false;
                else
                    (this.AssociatedObject.CurrentItem as MailDetails).IsNew = true;                        
            }                   
            selectedItem = this.AssociatedObject.CurrentItem;
            this.AssociatedObject.SelectedItems.Clear();

        }

        private void AssociatedObject_CellTapped(object sender, Syncfusion.UI.Xaml.Grid.GridCellTappedEventArgs e)
        {
            if (currentItem != null && currentItem != selectedItem)
            {
                var isNew = (currentItem as MailDetails).IsNew;
                if (isNew)
                    (currentItem as MailDetails).IsNew = false;              
            }
            currentItem = selectedItem;           
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.SelectionChanging -= AssociatedObject_SelectionChanging;
            this.AssociatedObject.CellTapped -= AssociatedObject_CellTapped;
            this.AssociatedObject.PreviewMouseUp -= AssociatedObject_PreviewMouseUp;
            this.AssociatedObject.QueryRowHeight -= AssociatedObject_QueryRowHeight;
        }
    }
}
