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
    /// Interaction logic for ScreenMbInfoSVC.xaml
    /// </summary>
    public partial class ScreenMbInfoSVC : ScreenBase
    {
        public ScreenMbInfoSVC()
        {
            InitializeComponent();
        }

        public void ShowMacroblock(Macroblock mb)
        {
            H264Mb mbH264 = (mb.Mb as H264Mb);
            m_TextBlockInCropWindow.Text = mbH264.IsInCropWindow ? "True" : "False";
            m_TextBlockResidualPredictionFlag.Text = mbH264.IsResidualPredictionFlag ? "1" : "0";
            m_TextBlockBLSkippedFlag.Text = mbH264.IsBLSkippedFlag ? "1" : "0";
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "SVC";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_MB_SVC;
            }
        }
    }
}
