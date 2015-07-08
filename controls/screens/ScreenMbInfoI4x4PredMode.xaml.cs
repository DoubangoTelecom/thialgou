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

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenMbInfoI4x4PredMode.xaml
    /// </summary>
    public partial class ScreenMbInfoI4x4PredMode : ScreenBase
    {
        Macroblock m_Mb;
        readonly TextBlock[] m_TextBlocks;

        public ScreenMbInfoI4x4PredMode()
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

            H264Mb mbH264 = (m_Mb.Mb as H264Mb);
            for (int i = 0; i < 16; ++i)
            {
                m_TextBlocks[i].Text = String.Format("[{0}] {1}", (int)mbH264.Intra4x4PredMode[i], Utils.GetIntra4x4PredModeName(mbH264.Intra4x4PredMode[i]));
            }
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "Intra4x4PredMode";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_MB_INFO_I4x4PREDMODE;
            }
        }
    }
}
