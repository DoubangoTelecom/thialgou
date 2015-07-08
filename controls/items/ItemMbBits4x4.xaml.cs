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
using thialgou.lib;
using System.Diagnostics;
using thialgou.lib.model;

namespace thialgou.controls.items
{
    /// <summary>
    /// Interaction logic for ItemMbBits4x4.xaml
    /// </summary>
    public partial class ItemMbBits4x4 : ItemBase<MbBits4x4>
    {
        readonly TextBlock[] m_TexBlocks;
        static Int32[] Inverse4x4LumaBlockScanOrder = new Int32[]{
            0,	1,	4,	5,
            2,	3,	6,	7,
            8,	9,	12,	13,
            10,	11,	14,	15
        };

        public ItemMbBits4x4()
        {
            InitializeComponent();

            m_TexBlocks = new TextBlock[]{
                    textBlock00,
                    textBlock01,
                    textBlock02,
                    textBlock03,
                    textBlock10,
                    textBlock11,
                    textBlock12,
                    textBlock13,
                    textBlock20,
                    textBlock21,
                    textBlock22,
                    textBlock23,
                    textBlock30,
                    textBlock31,
                    textBlock32,
                    textBlock33
                    };

            this.ValueLoaded += ItemHdrItem_ValueLoaded;
        }

        void ItemHdrItem_ValueLoaded(object sender, EventArgs e)
        {
            Background = new SolidColorBrush(((Value.X & 1) != (Value.Y & 1)) ? Colors.Transparent : Colors.AliceBlue);

            for (int i = 0; i < 16; ++i)
            {
                m_TexBlocks[i].Text =  Value.Bytes[i].ToString();
                m_TexBlocks[i].ToolTip = String.Format("@{0} ({1}, {2})", Inverse4x4LumaBlockScanOrder[Value.Address], Value.X, Value.Y);
            }
        }
    }
}
