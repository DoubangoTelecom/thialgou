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
using thialgou.lib.model;

namespace thialgou.controls.items
{
    /// <summary>
    /// Interaction logic for ItemHdrItem.xaml
    /// </summary>
    public partial class ItemHdrItem : ItemBase<Elt>
    {
        private Elt m_Elt;

        public ItemHdrItem()
        {
            InitializeComponent();

            this.ValueLoaded += ItemHdrItem_ValueLoaded;
        }

        void ItemHdrItem_ValueLoaded(object sender, EventArgs e)
        {
            if ((m_Elt = this.Value) == null)
            {
                label.Content = "(null)";
                return;
            }

            label.Content = m_Elt.Description;

            switch (m_Elt.Type)
            {
                case Elt.EltType_t.EltType_Control:
                    {
                        this.IsEnabled = (m_Elt as EltControl).IsExpressionValueTrue;
                        break;
                    }
            }
        }
    }
}
