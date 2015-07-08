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
using thialgou.controls.items.model;

namespace thialgou.controls.items
{
    /// <summary>
    /// Interaction logic for ItemMbBits4x4Row.xaml
    /// </summary>
    public partial class ItemMbBits4x4Row : ItemBase<MbBits4x4Row>
    {
        public ItemMbBits4x4Row()
        {
            InitializeComponent();

            this.ValueLoaded += ItemHdrItem_ValueLoaded;
        }

        void ItemHdrItem_ValueLoaded(object sender, EventArgs e)
        {
            m_ListBox.ItemsSource = Value.Row;
        }
    }
}
