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
using thialgou.lib.h264;
using thialgou.lib;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenMbInfoSubBlocks.xaml
    /// </summary>
    public partial class ScreenMbInfoSubBlocks : ScreenBase
    {
        Macroblock m_Mb;

        public ScreenMbInfoSubBlocks()
        {
            InitializeComponent();
        }

        public void ShowMacroblock(Macroblock mb)
        {
            m_Mb = mb;

            H264Mb mbH264 = (m_Mb.Mb as H264Mb);
            m_TextBlock0.Text = mbH264.BlkModes[0].ToString();
            m_TextBlock1.Text = mbH264.BlkModes[1].ToString();
            m_TextBlock2.Text = mbH264.BlkModes[2].ToString();
            m_TextBlock3.Text = mbH264.BlkModes[3].ToString();
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "Sub Blocks";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_MB_INFO_SUNBLOCKS;
            }
        }
    }
}
