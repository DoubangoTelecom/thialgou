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
using thialgou.lib.model;
using org.doubango.thialgou.commonWRAP;
using thialgou.controls.items.model;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenMbBits.xaml
    /// </summary>
    public partial class ScreenMbBits : ScreenBase
    {
        Macroblock m_Mb;

        readonly MyObservableCollection<MbBits4x4Row> m_DataSourceY;
        readonly MyObservableCollection<MbBits4x4Row> m_DataSourceU;
        readonly MyObservableCollection<MbBits4x4Row> m_DataSourceV;

        readonly CommonEltMbDataType_t m_DataType;
        readonly String m_Title;

        public ScreenMbBits(CommonEltMbDataType_t dataType)
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

            m_DataSourceY = new MyObservableCollection<MbBits4x4Row>(true);
            m_DataSourceU = new MyObservableCollection<MbBits4x4Row>(true);
            m_DataSourceV = new MyObservableCollection<MbBits4x4Row>(true);

            m_ListBoxY.ItemsSource = m_DataSourceY;
            m_ListBoxU.ItemsSource = m_DataSourceU;
            m_ListBoxV.ItemsSource = m_DataSourceV;
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
                return ScreenBase.SCREEN_TYPE_MB_BITS;
            }
        }

        public void ShowMacroblock(Macroblock mb)
        {
            lock (this)
            {
                if ((m_Mb != mb) && mb != null)
                {
                    m_Mb = mb;
                    Int32[] bits4x4;
                    UInt32 YCount = 0, UCount = 0, VCount = 0;

                    m_DataSourceY.Clear();
                    m_DataSourceU.Clear();
                    m_DataSourceV.Clear();

                    m_TextBlockMacroblockInfo.Text = String.Format("{0} / MB {1}({2}, {3}) / Slice:{4} / Picture:{5} / Layer:{6}", m_Title, mb.Mb.Address, mb.Mb.X << 4, mb.Mb.Y << 4, mb.SliceId, mb.PictureId, mb.LayerId);

                    for (uint y = 0; y < 4; y++)
                    {
                        MbBits4x4Row rowY = new MbBits4x4Row();
                        MbBits4x4Row rowU = new MbBits4x4Row();
                        MbBits4x4Row rowV = new MbBits4x4Row();
                        for (uint x = 0; x < 4; x++)
                        {
                            // Y
                            bits4x4 = m_Mb.Mb.GetMbBits4x4(CommonYuvLine_t.CommonYuvLine_Y, m_DataType, x, y);
                            if (bits4x4 != null)
                            {
                                rowY.Add(new MbBits4x4(YCount++, x, y, bits4x4));
                            }
                            // U
                            bits4x4 = m_Mb.Mb.GetMbBits4x4(CommonYuvLine_t.CommonYuvLine_U, m_DataType, x, y);
                            if (bits4x4 != null)
                            {
                                rowU.Add(new MbBits4x4(UCount++, x, y, bits4x4));
                            }
                            // V
                            bits4x4 = m_Mb.Mb.GetMbBits4x4(CommonYuvLine_t.CommonYuvLine_V, m_DataType, x, y);
                            if (bits4x4 != null)
                            {
                                rowV.Add(new MbBits4x4(VCount++, x, y, bits4x4));
                            }
                        }
                       if (rowY.Row.Count > 0)
                        {
                            m_DataSourceY.Add(rowY);
                        }
                        if (rowU.Row.Count > 0)
                        {
                            m_DataSourceU.Add(rowU);
                        }
                        if (rowV.Row.Count > 0)
                        {
                            m_DataSourceV.Add(rowV);
                        }
                    }
                }
            }
        }
    }
}
