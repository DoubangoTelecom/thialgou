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
    /// Interaction logic for ScreenMbInfoGeneral.xaml
    /// </summary>
    public partial class ScreenMbInfoGeneral : ScreenBase
    {
        public ScreenMbInfoGeneral()
        {
            InitializeComponent();
        }

        public void ShowMacroblock(Macroblock mb)
        {
            H264Mb mbH264 = (mb.Mb as H264Mb);
            m_TextBlockAddress.Text = String.Format("{0}({1},{2})", mb.Mb.Address, mb.Mb.X, mb.Mb.Y);
            m_TextBlockLocation.Text = String.Format("({0},{1})", mb.Mb.X << 4, mb.Mb.Y << 4);
            m_TextBlockType.Text = mbH264.TypeDescription;
            m_TextBlockPicture.Text = String.Format("{0} (DQId={1})", mb.PictureId, mb.LayerId);
            m_TextBlockSlice.Text = String.Format("{0}\n{1}", mb.SliceId, mbH264.SliceType);
            m_TextBlockBitsStart.Text = String.IsNullOrEmpty(mb.Mb.BitsStart) ? "-" : mb.Mb.BitsStart;
            m_TextBlockBitsCount.Text = mb.Mb.BitsCount.ToString();
            m_TextBlockQP.Text = mbH264.QP.ToString();
            m_TextBlockQPC.Text = mbH264.QPC.ToString();
            m_TextBlockCBP.Text = String.Format("{0}: L={1},C={2}", mbH264.CBP, mbH264.CBP & 15, mbH264.CBP >> 4);
            m_TextBlockMbPartNum.Text = mbH264.NumMbPart.ToString();
            m_TextBlockMbPartSize.Text = String.Format("({0},{1})", mbH264.MbPartSize.Width, mbH264.MbPartSize.Height);
            if (mbH264.IsIntra)
            {
                m_TextBlockMbIntraChromaMode.Text = String.Format("[{0}] {1}", (int)mbH264.IntraChromaPredMode, mbH264.IntraChromaPredMode);
            }
            else
            {
                m_TextBlockMbIntraChromaMode.Text = "-";
            }
            m_TextBlockMD5.Text = String.Format("Y\t= {0}\nU\t= {1}\nV\t= {2}", mb.MD5Y, mb.MD5U, mb.MD5V).ToUpper();
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "General";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_MB_INFO_GENERAL;
            }
        }
    }
}
