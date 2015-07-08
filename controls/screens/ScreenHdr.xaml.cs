/*
* Copyright (C) 2012 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using thialgou.lib.model;
using thialgou.lib;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for HdrScreen.xaml
    /// </summary>
    public partial class ScreenHdr : ScreenList<Elt>
    {
        readonly MyObservableCollection<Elt> m_Elements;

        public ScreenHdr()
            : this(null)
        {
        }

        public ScreenHdr(Elt elt)
        {
            InitializeComponent();

            m_Elements = new MyObservableCollection<Elt>();
            m_Elements.Add(elt); // we want element at zero to be displayed
            
            DataSource = m_Elements;
            treeView.Loaded += new RoutedEventHandler(treeView_Loaded);
        }

        void treeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (treeView.Items.Count > 0)
            {
                // Expand first item (most likely the function name)
                TreeViewItem treeItem = treeView.ItemContainerGenerator.ContainerFromItem(treeView.Items[0]) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                }
            }
        }

        public override String BaseScreenId
        {
            get
            {
                /*if (m_Elt != null)
                {
                    return GetScreenTitle(m_Elt);
                }
                return base.BaseScreenId;*/
                return "null";
            }
            set
            {
                base.BaseScreenId = value;
            }
        }

        public override string BaseScreenTitle
        {
            get
            {
                return BaseScreenId;
            }
        }

        public override ItemsControl ListControl
        {
            get { return treeView; }
        }

        /*public static String GetScreenTitle(Hdr hdr)
        {
            switch (hdr.Type)
            {
                case Hdr.HdrType.SPS: return Constants.SCREEN_TITLE_SPS;
                case Hdr.HdrType.PPS: return Constants.SCREEN_TITLE_PPS;
                case Hdr.HdrType.SLICE: return Constants.SCREEN_TITLE_SLICE;
                default:
                    {
                        throw new Exception(String.Format("{0} cannot be mapped to a screen title", hdr.Type));
                    }
            }
        }*/

        void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HdrItem item = (treeView.SelectedItem as HdrItem);
            if (item != null)
            {
                labelItemDescription.Content = item.Description;
            }
        }
    }
}
