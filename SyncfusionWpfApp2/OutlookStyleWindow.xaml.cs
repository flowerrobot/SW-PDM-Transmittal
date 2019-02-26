#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using Syncfusion.Windows.Shared;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.SfSkinManager;

namespace SyncfusionWpfApp2
{
    /// <summary>
    /// Interaction logic for OutlookStyleWindow.xaml
    /// </summary>
    public partial class OutlookStyleWindow : RibbonWindow
    {
		
        public OutlookStyleWindow()
        {
            InitializeComponent();
            RemoveGroupBarOverFlowButton();
				
        }		
		
        private void exitTab_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public void RemoveGroupBarOverFlowButton()
        {
            foreach (ToggleButton item in VisualUtils.EnumChildrenOfType(groupBar, typeof(ToggleButton)))
            {
                if (item.Name == "PART_OverFlowButton")
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
