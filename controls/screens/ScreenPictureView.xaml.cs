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
using thialgou.controls.adorners;
using thialgou.lib.events;
using org.doubango.thialgou.commonWRAP;
using thialgou.lib.h264;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenPictureView.xaml
    /// </summary>
    public partial class ScreenPictureView : ScreenBase
    {
        Picture m_Picture;
        int m_numberOfMbInRow = 0;
        int m_numberOfMbInCol = 0;
        int m_numberOfMb = 0;
        int m_LastHoverMacroblockAddress = -1, m_LastSelectedMacroblockAddress = -1;

        AdornerSelectMacroblock m_AdornerSelectMacroblock;
        AdornerLayer m_AdornerLayerImage;
        public event EventHandler<MacroblockEventArgs> OnMacroblockEvent;

        readonly CommonEltMbDataType_t m_DataType;
        readonly String m_Title;

        public ScreenPictureView(CommonEltMbDataType_t dataType)
        {
            InitializeComponent();

            m_DataType = dataType;

            switch (m_DataType)
            {
                case CommonEltMbDataType_t.CommonEltMbDataType_Final:
                    {
                        m_Title = "Reconstructed";
                        break;
                    }
                case CommonEltMbDataType_t.CommonEltMbDataType_Predicted:
                    {
                        m_Title = "Predicted";
                        break;
                    }
                case CommonEltMbDataType_t.CommonEltMbDataType_IDCTCoeffsQuant:
                    {
                        m_Title = "Coeffs. (Quantized)";
                        break;
                    }
                case CommonEltMbDataType_t.CommonEltMbDataType_IDCTCoeffsScale:
                    {
                        m_Title = "Coeffs. (Scaled)";
                        break;
                    }
                case CommonEltMbDataType_t.CommonEltMbDataType_Residual:
                    {
                        m_Title = "Residual (Final)";
                        break;
                    }
                case CommonEltMbDataType_t.CommonEltMbDataType_ResidualBase:
                    {
                        m_Title = "Residual (SVC Base)";
                        break;
                    }
                case CommonEltMbDataType_t.CommonEltMbDataType_PreFilter:
                    {
                        m_Title = "Pre-Filter";
                        break;
                    }
                default:
                    {
                        m_Title = "Unknown";
                        break;
                    }
            }
        }

        private void ScreenBase_Loaded(object sender, RoutedEventArgs e)
        {
            m_AdornerLayerImage = AdornerLayer.GetAdornerLayer(m_Image);
            m_AdornerSelectMacroblock = new AdornerSelectMacroblock(m_Image, false);
        }


        public void ShowPicture(Picture picture)
        {
            lock (this)
            {
                if (m_Picture != picture && picture != null)
                {
                    m_Picture = picture;
                    System.Drawing.Bitmap bitmap = m_Picture.GetImage(m_DataType);
                    if (bitmap != null)
                    {
                        m_Image.Source = Utils.CreateBitmapSourceFromBitmap(bitmap);
                        m_numberOfMbInRow = bitmap.Width >> 4;
                        m_numberOfMbInCol = bitmap.Height >> 4;
                        m_numberOfMb = (m_numberOfMbInRow * m_numberOfMbInCol);

                        if (m_LastSelectedMacroblockAddress != -1)
                        {
                            double mbWidth, mbHeight;
                            int mbX, mbY;
                            GetMbPosition(m_LastSelectedMacroblockAddress, out mbX, out mbY, out mbWidth, out mbHeight);
                            SelectMB(mbX, mbY, m_LastSelectedMacroblockAddress, mbWidth, mbHeight, true, true);
                        }
                    }
                }
            }
        }

        public Picture Picture
        {
            get
            {
                return m_Picture;
            }
        }

        public override String BaseScreenTitle
        {
            get
            {
                return m_Title;
            }
        }

        public override int BaseScreenType
        {
            get
            {
                return ScreenBase.SCREEN_TYPE_PICTURE_VIEW;
            }
        }

        void RaiseMbSelectionEvent(UInt32 mbX, UInt32 mbY, MacroblockEventArgs.MacroblockEventType_t selectionType)
        {
            lock (this)
            {
                if (OnMacroblockEvent != null)
                {
                    Macroblock mbObject = m_Picture.GetMacroblockByLocation((UInt32)mbX, (UInt32)mbY);
                    if (mbObject != null)
                    {
                        EventHandlerTrigger.TriggerEvent<MacroblockEventArgs>(OnMacroblockEvent, this, new MacroblockEventArgs(selectionType, mbObject));
                    }
                }
            }
        }

        void SelectMB(int mbX, int mbY, int mbAddress, double mbWidth, double mbHeight, bool force, bool raiseEvent)
        {
            m_AdornerLayerImage.Remove(m_AdornerSelectMacroblock);
            m_AdornerSelectMacroblock.MbRect = new Rect((mbX * mbWidth), (mbY * mbHeight), mbWidth, mbHeight);
            m_AdornerLayerImage.Add(m_AdornerSelectMacroblock);

            if (force || m_LastSelectedMacroblockAddress != mbAddress)
            {
                RaiseMbSelectionEvent((UInt32)mbX, (UInt32)mbY, MacroblockEventArgs.MacroblockEventType_t.MacroblockEventType_Selected);
                m_LastSelectedMacroblockAddress = mbAddress;
            }
            if (raiseEvent)
            {
                RaiseMbSelectionEvent((UInt32)mbX, (UInt32)mbY, MacroblockEventArgs.MacroblockEventType_t.MacroblockEventType_Selected);
            }
        }

        void SelectMB(int mbX, int mbY, int mbAddress, double mbWidth, double mbHeight)
        {
            SelectMB(mbX, mbY, mbAddress, mbWidth, mbHeight, false, false);
        }

        void m_Image_MouseMove(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (m_Picture != null && m_Image.Source != null && m_numberOfMbInRow > 0 && m_numberOfMbInCol > 0)
                {
                    double mbWidth, mbHeight;
                    int mbX, mbY, mbAddress;
                    Point mousePosition = e.GetPosition(m_Image);
                    GetMbPosition(mousePosition, out mbX, out mbY, out mbAddress, out mbWidth, out mbHeight);
                    Macroblock mb = m_Picture.GetMacroblockByLocation((UInt32)mbX, (UInt32)mbY);

                    m_TextBlockImageToolTip.Text = String.Format("MB {0}({1},{2})", mbAddress, mbX, mbY);
                    if (mb != null)
                    {
                        m_TextBlockImageToolTip.Text += String.Format("\nSlice: {0}, {1}", mb.SliceId, (mb.Mb as H264Mb).SliceType);
                        m_TextBlockImageToolTip.Text += String.Format("\nType: {0}", (mb.Mb as H264Mb).TypeDescription);
                    }
                    m_FloatingToolTip.HorizontalOffset = mousePosition.X + 20;
                    m_FloatingToolTip.VerticalOffset = mousePosition.Y;
                    m_FloatingToolTip.IsOpen = true;

                    if (m_LastHoverMacroblockAddress != mbAddress)
                    {
                        RaiseMbSelectionEvent((UInt32)mbX, (UInt32)mbY, MacroblockEventArgs.MacroblockEventType_t.MacroblockEventType_Hover);
                        m_LastHoverMacroblockAddress = mbAddress;
                    }
                }
            }
        }

        void m_Image_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void m_Image_MouseLeave(object sender, MouseEventArgs e)
        {
            m_FloatingToolTip.IsOpen = false;
            m_LastHoverMacroblockAddress = -1;
        }

        private void m_Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                if (m_Picture != null && m_Image.Source != null && m_numberOfMbInRow > 0 && m_numberOfMbInCol > 0)
                {
                    double mbWidth, mbHeight;
                    int mbX, mbY, mbAddress;
                    Point mousePosition = e.GetPosition(m_Image);
                    GetMbPosition(mousePosition, out mbX, out mbY, out mbAddress, out mbWidth, out mbHeight);
                    SelectMB(mbX, mbY, mbAddress, mbWidth, mbHeight);
                }
            }
        }

        private void m_Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_FloatingToolTip.IsOpen = false;
            m_LastHoverMacroblockAddress = -1;
            // m_LastSelectedMacroblockAddress = -1;
        }


        void GetMbPosition(Point mousePosition, out int mbX, out int mbY, out int mbAddress, out double mbWidth, out double mbHeight)
        {
            mbWidth = m_Image.ActualWidth / m_numberOfMbInRow;
            mbHeight = m_Image.ActualHeight / m_numberOfMbInCol;
            mbX = (int)Utils.Clamp(0, (mousePosition.X / mbWidth), m_numberOfMbInRow - 1);
            mbY = (int)Utils.Clamp(0, (mousePosition.Y / mbHeight), m_numberOfMbInCol - 1);
            mbAddress = (mbY * m_numberOfMbInRow) + mbX;
        }

        void GetMbPosition(int mbAddress, out int mbX, out int mbY, out double mbWidth, out double mbHeight)
        {
            mbWidth = m_Image.ActualWidth / m_numberOfMbInRow;
            mbHeight = m_Image.ActualHeight / m_numberOfMbInCol;
            if (mbAddress >= m_numberOfMb) // layer switch (e.g. CIF -> QCIF)
            {
                mbX = 0;
                mbY = 0;
            }
            else
            {
                mbX = mbAddress % m_numberOfMbInRow;
                mbY = (mbAddress / m_numberOfMbInRow);
            }
        }
    }
}
