#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Syncfusion.Windows.Tools.Controls;

namespace SyncfusionWpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DockCollections = new ObservableCollection<DockItem>();

            DockCollections.Add(new DockItem() { Header = "ToolBox" });

            DockCollections.Add(new DockItem() { Header = "Integration" });

            DockCollections.Add(new DockItem() { Header = "Features" });

            this.DataContext = this;
        }
        private ObservableCollection<DockItem> _dockcollection;

        public ObservableCollection<DockItem> DockCollections

        {

            get {

                return _dockcollection;

            }

            set {

                _dockcollection = value;

            }

        }
    }
}
