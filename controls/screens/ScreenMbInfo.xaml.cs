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
using thialgou.lib;
using thialgou.lib.h264;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenMbInfo.xaml
    /// </summary>
    public partial class ScreenMbInfo : ScreenBase
    {
        Macroblock m_Mb;
        ScreenMbInfoGeneral m_ScreenGeneral;
        ScreenMbInfoI4x4PredMode m_ScreenMbInfoI4x4PredMode;
        ScreenMbInfoSubBlocks m_ScreenMbInfoSubBlocks;
        ScreenMbInfoInterPred m_ScreenMbInfoInterPred;
        ScreenMbInfoSVC m_ScreenMbInfoSVC;

        public ScreenMbInfo()
        {
            InitializeComponent();

            tabControl.Items.Clear();
        }

        public void ShowMacroblock(Macroblock mb)
        {
            m_Mb = mb;
            H264Mb mbH264 = m_Mb.Mb as H264Mb;

            if (mbH264 != null) // For now only H.264 is supported
            {
                // General
                if (m_ScreenGeneral == null)
                {
                    m_ScreenGeneral = new ScreenMbInfoGeneral();
                    AddToTabControl(m_ScreenGeneral, tabControl);
                }
                m_ScreenGeneral.ShowMacroblock(m_Mb);

                // Intra4x4PredMode
                if (mbH264.Mode == MbMode.INTRA_4X4)
                {
                    if (m_ScreenMbInfoI4x4PredMode == null)
                    {
                        m_ScreenMbInfoI4x4PredMode = new ScreenMbInfoI4x4PredMode();
                        m_ScreenMbInfoI4x4PredMode.Tag = false;
                    }
                    if (!(m_ScreenMbInfoI4x4PredMode.Tag as Boolean?).Value)
                    {
                        AddToTabControl(m_ScreenMbInfoI4x4PredMode, tabControl);
                        m_ScreenMbInfoI4x4PredMode.Tag = true;
                    }
                    m_ScreenMbInfoI4x4PredMode.ShowMacroblock(m_Mb);
                }
                else
                {
                    if (m_ScreenMbInfoI4x4PredMode != null && (m_ScreenMbInfoI4x4PredMode.Tag as Boolean?).Value)
                    {
                        RemoveFromTabControl(m_ScreenMbInfoI4x4PredMode, tabControl);
                        m_ScreenMbInfoI4x4PredMode.Tag = false;
                    }
                }

                // SubBlocks
                if (mbH264.IsInter8x8)
                {
                    if (m_ScreenMbInfoSubBlocks == null)
                    {
                        m_ScreenMbInfoSubBlocks = new ScreenMbInfoSubBlocks();
                        m_ScreenMbInfoSubBlocks.Tag = false;
                    }
                    if (!(m_ScreenMbInfoSubBlocks.Tag as Boolean?).Value)
                    {
                        AddToTabControl(m_ScreenMbInfoSubBlocks, tabControl);
                        m_ScreenMbInfoSubBlocks.Tag = true;
                    }
                    m_ScreenMbInfoSubBlocks.ShowMacroblock(m_Mb);
                }
                else
                {
                    if (m_ScreenMbInfoSubBlocks != null && (m_ScreenMbInfoSubBlocks.Tag as Boolean?).Value)
                    {
                        RemoveFromTabControl(m_ScreenMbInfoSubBlocks, tabControl);
                        m_ScreenMbInfoSubBlocks.Tag = false;
                    }
                }

                // InterPred
                if (!mbH264.IsIntra)
                {
                    if (m_ScreenMbInfoInterPred == null)
                    {
                        m_ScreenMbInfoInterPred = new ScreenMbInfoInterPred();
                        m_ScreenMbInfoInterPred.Tag = false;
                    }
                    if (!(m_ScreenMbInfoInterPred.Tag as Boolean?).Value)
                    {
                        AddToTabControl(m_ScreenMbInfoInterPred, tabControl);
                        m_ScreenMbInfoInterPred.Tag = true;
                    }
                    m_ScreenMbInfoInterPred.ShowMacroblock(m_Mb);
                }
                else
                {
                    if (m_ScreenMbInfoInterPred != null && (m_ScreenMbInfoInterPred.Tag as Boolean?).Value)
                    {
                        RemoveFromTabControl(m_ScreenMbInfoInterPred, tabControl);
                        m_ScreenMbInfoInterPred.Tag = false;
                    }
                }

                // SVC
                if (m_ScreenMbInfoSVC == null)
                {
                    m_ScreenMbInfoSVC = new ScreenMbInfoSVC();
                    AddToTabControl(m_ScreenMbInfoSVC, tabControl);
                }
                m_ScreenMbInfoSVC.ShowMacroblock(m_Mb);
            }
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "MB Info";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_MB_INFO;
            }
        }
    }
}
