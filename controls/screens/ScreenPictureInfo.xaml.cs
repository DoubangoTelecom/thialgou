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
using org.doubango.thialgou.commonWRAP;
using thialgou.lib.h264;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenPictureInfo.xaml
    /// </summary>
    public partial class ScreenPictureInfo : ScreenBase
    {
        Picture m_Picture;

        public ScreenPictureInfo()
        {
            InitializeComponent();
        }

        public override String BaseScreenTitle
        {
            get
            {
                return "Picture Info";
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_PICTURE_INFO;
            }
        }

        public void ShowPicture(Picture picture)
        {
            lock (this)
            {
                m_Picture = picture;
                if (m_Picture != null)
                {
                    m_TextBlockDQId.Text = m_Picture.LayerId.ToString();
                    m_TextBlockPictureNumber.Text = m_Picture.Index.ToString();
                    m_TextBlockSize.Text = String.Format("{0}x{1}", m_Picture.Width, m_Picture.Height);
                    m_TextBlockNumberOfSlices.Text = m_Picture.NumberOfSlices.ToString();
                    m_EntropyCodingType.Text = m_Picture.EntropyCodingType;
                    m_TextBlockMbBitsCount.Text = (m_Picture as H264Picture).MbBitsCount.ToString();
                    m_TextBlockMD5.Text = String.Format("Y\t= {0}\nU\t= {1}\nV\t= {2}\nYUV\t= {3}",
                        m_Picture.GetMd5(Utils.MD5_ELTMB_TYPE, 0),
                        m_Picture.GetMd5(Utils.MD5_ELTMB_TYPE, 1),
                        m_Picture.GetMd5(Utils.MD5_ELTMB_TYPE, 2),
                        m_Picture.GetMd5(Utils.MD5_ELTMB_TYPE, 3)).ToUpper();
                }
                else
                {
                    // FIXME: show error image
                    Debug.Assert(false);
                }
            }
        }

        void ScreenBase_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var c in m_Grid.RowDefinitions)
            {
                c.Height = new GridLength(0.0, GridUnitType.Auto);
            }
        }
    }
}
