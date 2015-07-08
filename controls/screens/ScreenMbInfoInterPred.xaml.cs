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
using thialgou.lib.h264;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenMbInfoInterPred.xaml
    /// </summary>
    public partial class ScreenMbInfoInterPred : ScreenBase
    {
        Macroblock m_Mb;
        readonly TextBlock[] m_TextBlocks;

        public ScreenMbInfoInterPred()
        {
            InitializeComponent();

            m_TextBlocks = new TextBlock[]
            {
                m_TextBlock0,
                m_TextBlock1,
                m_TextBlock2,
                m_TextBlock3,
                m_TextBlock4,
                m_TextBlock5,
                m_TextBlock6,
                m_TextBlock7,
                m_TextBlock8,
                m_TextBlock9,
                m_TextBlock10,
                m_TextBlock11,
                m_TextBlock12,
                m_TextBlock13,
                m_TextBlock14,
                m_TextBlock15,
            };
        }

        public void ShowMacroblock(Macroblock mb)
        {
            m_Mb = mb;
            int xFracL, yFracL;

            H264Mb mbH264 = (m_Mb.Mb as H264Mb);
            for (int i = 0; i < 16; ++i)
            {
                xFracL = mbH264.mvL0[i].x & 3;
                yFracL = mbH264.mvL0[i].y & 3;
                m_TextBlocks[i].Text = "---List0---";
                m_TextBlocks[i].Text += String.Format("\nMV({0}, {1})\nMVD({2}, {3})", mbH264.mvL0[i].x, mbH264.mvL0[i].y, mbH264.mvd_l0[i].x, mbH264.mvd_l0[i].y);
                m_TextBlocks[i].Text += String.Format("\nFrac({0}, {1})", xFracL != 0 ? (xFracL == 2 ? "Half" : "Quater") : "Integer", yFracL != 0 ? (xFracL == 2 ? "Half" : "Quater") : "Integer");
                m_TextBlocks[i].Text += String.Format("\nPOC={0}, RefIdx={1}", mbH264.POC0[H264Mb.BLK4x4ToBLK8x8[i]], mbH264.ref_idx_L0[H264Mb.BLK4x4ToBLK8x8[i]]);
            }

            if (mbH264.SliceType == SliceType.B_SLICE)
            {
                for (int i = 0; i < 16; ++i)
                {
                    xFracL = mbH264.mvL1[i].x & 3;
                    yFracL = mbH264.mvL1[i].y & 3;
                    m_TextBlocks[i].Text += "\n---List1---";
                    m_TextBlocks[i].Text += String.Format("\nMV({0}, {1})\nMVD({2}, {3})", mbH264.mvL1[i].x, mbH264.mvL1[i].y, mbH264.mvd_l1[i].x, mbH264.mvd_l1[i].y);
                    m_TextBlocks[i].Text += String.Format("\nFrac({0}, {1})", xFracL != 0 ? (xFracL == 2 ? "Half" : "Quater") : "Integer", yFracL != 0 ? (xFracL == 2 ? "Half" : "Quater") : "Integer");
                    m_TextBlocks[i].Text += String.Format("\nPOC={0}, RefIdx={1}", mbH264.POC1[H264Mb.BLK4x4ToBLK8x8[i]], mbH264.ref_idx_L1[H264Mb.BLK4x4ToBLK8x8[i]]);
                }
            }
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "InterPred";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_MB_INFO_INTERPRED;
            }
        }
    }
}
