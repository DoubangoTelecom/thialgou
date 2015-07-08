/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
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
using thialgou.lib.CommonTypes;
using thialgou.lib.events;
using thialgou.appl.lib;
using System.Threading;
using thialgou.lib;
using thialgou.controls.screens;
using org.doubango.thialgou.commonWRAP;
using System.ComponentModel;
using System.Windows.Threading;
using System.Diagnostics;
using thialgou.lib.model;

namespace thialgou.appl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Engine m_Engine;

        enum H264HeaderEltType_t
        {
            H264HeaderEltType_SliceHeader,
            H264HeaderEltType_SPS,
            H264HeaderEltType_PPS,
        };

        ScreenPictureView[] m_ScreenPictureViews;
        ScreenMbBits[] m_ScreenMbBits;
        ScreenHeader[] m_ScreenHeaders;
        ScreenPictureInfo m_ScreenPictureInfo;
        ScreenMbInfo m_ScreenScreenMbInfo;
        readonly IDictionary<UInt32/*LayerId*/, ScreenThumbnails> m_ScreenThumbnails;

        Picture m_SelectedPicture;
        Macroblock m_SelectedMacroblock;

        public MainWindow()
        {
            InitializeComponent();

            m_ScreenThumbnails = new Dictionary<UInt32/*LayerId*/, ScreenThumbnails>();

            tabControlThumbs.Items.Clear();
            tabControlMacroblockBits.Items.Clear();
            tabControlPictureView.Items.Clear();
            tabControlInfo.Items.Clear();

            
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\toulde\\videos\\AUD_MW_E\\AUD_MW_E.264", "AUD_MW_E.yuv");
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\toulde\\videos\\FM1_BT_B\\FM1_BT_B.264", "FM1_BT_B.yuv"); // multiple slices + deblocking
            m_Engine = Engine.CreateRawH264("C:\\Projects\\toulde\\videos\\SVCBS-1\\SVCBS-1\\SVCBS-1.264", "SVCBS-1.yuv"); // Vidyo
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\toulde\\videos\\freh1_b\\Freh1_B.264", "Freh1_B.yuv");
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\hl78965\\clean\\hartallo\\encoderSVC.264", "encoderSVC.yuv");
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\hl78965\\clean\\hartallo\\encoderAVC.264", "encoder.yuv");
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\hl78965\\clean\\hartallo\\encoder.264", "encoder.yuv"); // New Hartallo
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\hl78965\\clean\\hartallo\\encoder_avc_10frms_noearlyterm.264", "encoder.yuv"); // Old Hartallo
            //m_Engine = Engine.CreateRawH264("C:\\tmp\\x264_104\\output.264", "encoder.yuv"); // x264
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\JM\\output.264", "encoder.yuv"); // JM
            //m_Engine = Engine.CreateRawH264("C:\\Projects\\hartallo\\tests\\TestH264\\bourne.264", "bourne.yuv");// bourne
            

            // Screen Previews
            m_ScreenPictureViews = new ScreenPictureView[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
            for (int k = 0; k < m_ScreenPictureViews.Length; ++k )
            {
                m_ScreenPictureViews[k] = new ScreenPictureView((CommonEltMbDataType_t)k);
                ScreenBase.AddToTabControl(m_ScreenPictureViews[k], tabControlPictureView);
                m_ScreenPictureViews[k].OnMacroblockEvent += (_sender, _e) =>
                {
                    switch (_e.Type)
                    {
                        // Screen Macroblocks
                        case MacroblockEventArgs.MacroblockEventType_t.MacroblockEventType_Selected:
                            {
                                m_SelectedMacroblock = _e.Macroblock;
                                // Macroblock Bits
                                if (m_ScreenMbBits == null)
                                {
                                    m_ScreenMbBits = new ScreenMbBits[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
                                    for (int i = 0; i < m_ScreenMbBits.Length; ++i)
                                    {
                                        m_ScreenMbBits[i] = new ScreenMbBits((CommonEltMbDataType_t)i);
                                        ScreenBase.AddToTabControl(m_ScreenMbBits[i], tabControlMacroblockBits);
                                    }
                                    tabControlMacroblockBits.SelectedIndex = (int)CommonEltMbDataType_t.CommonEltMbDataType_Final;
                                }

                                if (tabControlMacroblockBits.SelectedIndex != -1)
                                {
                                    m_ScreenMbBits[tabControlMacroblockBits.SelectedIndex].ShowMacroblock(_e.Macroblock);
                                }
                                // Macroblock Info
                                if (m_ScreenScreenMbInfo == null)
                                {
                                    m_ScreenScreenMbInfo = new ScreenMbInfo();
                                    ScreenBase.AddToTabControl(m_ScreenScreenMbInfo, tabControlInfo);
                                }
                                m_ScreenScreenMbInfo.ShowMacroblock(_e.Macroblock);
                                // Headers(PPS, SPS, SLICE) associated to this Macroblock
                                ShowHeadersFromSelectedMb(_e.Macroblock);
                                break;
                            }
                    }
                };
            }


            (m_Engine.Decoder as DecoderVideo).OnPictureEvent += (_sender, _e) =>
                {
                    switch (_e.Type)
                    {
                        case PictureEventArgs.PictureEventType_t.PictureEventType_Added:
                            {
                                if (!m_ScreenThumbnails.ContainsKey(_e.Picture.LayerId))
                                {
                                    m_ScreenThumbnails[_e.Picture.LayerId] = new ScreenThumbnails(_e.Picture.LayerId);
                                    ScreenBase.AddToTabControl(m_ScreenThumbnails[_e.Picture.LayerId], tabControlThumbs);
                                    m_ScreenThumbnails[_e.Picture.LayerId].OnPictureEvent += new EventHandler<PictureEventArgs>(Thumbnails_OnPictureEvent);
                                }
                                m_ScreenThumbnails[_e.Picture.LayerId].AddPicture(_e.Picture);
                                break;
                            }
                    }
                };

            // TabControls selection changed events

            tabControlPictureView.SelectionChanged += (_sender, _e) =>
                {
                    m_ScreenPictureViews[tabControlPictureView.SelectedIndex].ShowPicture(m_SelectedPicture);
                };
            tabControlMacroblockBits.SelectionChanged += (_sender, _e) =>
            {
                m_ScreenMbBits[tabControlMacroblockBits.SelectedIndex].ShowMacroblock(m_SelectedMacroblock);
            };
            
            

            //FIXME:
            /*http://stackoverflow.com/questions/5480372/whats-the-best-way-to-update-an-observablecollection-from-another-thread*/

            //this.Dispatcher.BeginInvoke((Action)(() => { mEngine.Decoder.Decode(uint.MaxValue); }));
            // FIXME
            new Thread(delegate()
            {
                m_Engine.Decoder.Decode(/*uint.MaxValue*/200);
            })
            .Start();            
        }

        void Thumbnails_OnPictureEvent(object _sender, PictureEventArgs _e)
        {
            switch (_e.Type)
            {
                case PictureEventArgs.PictureEventType_t.PictureEventType_Selected:
                    {
                        m_SelectedPicture = _e.Picture;
                        // Show Picture view
                        if (tabControlPictureView.SelectedIndex != -1)
                        {
                            m_ScreenPictureViews[tabControlPictureView.SelectedIndex].ShowPicture(_e.Picture);
                        }

                        // Show Info
                        if (m_ScreenPictureInfo == null)
                        {
                            m_ScreenPictureInfo = new ScreenPictureInfo();
                            ScreenBase.AddToTabControl(m_ScreenPictureInfo, tabControlInfo);
                        }
                        m_ScreenPictureInfo.ShowPicture(_e.Picture);

                        // Show headers
                        if (m_ScreenHeaders == null)
                        {
                            if (m_Engine.Decoder.FileInput.EncodingType == EncodingType.H264)
                            {
                                m_ScreenHeaders = new ScreenHeader[Utils.GetMaxEnumValue<H264HeaderEltType_t>()];
                            }
                            else
                            {
                                Debug.Assert(false);
                            }
                        }
                        break;
                    }
            }
        }

        void Decoder_OnEvent(object sender, EltEventArgs e)
        {
           //ScreenHdr screen = new ScreenHdr(e.Elt);
            //TabItem i = ScreenBase.AddToTabControl(screen, tabControlHdrs);
            //i.Tag = screen;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (m_Engine != null)
            {
                m_Engine.Dispose();
                m_Engine = null;
            }
        }

        void ShowHeadersFromSelectedMb(Macroblock selectMb)
        {
            if (m_Engine.Decoder.FileInput.EncodingType == EncodingType.H264)
            {
                H264Picture h264Picture = (m_SelectedPicture as H264Picture);
                Debug.Assert(h264Picture != null);

                H264Slice h264Slice = h264Picture.Slices[selectMb.SliceId];
                Debug.Assert(h264Slice != null);

                ShowH264Headers(new[] { h264Slice.PicParamSet.Nalu }, new[] { h264Slice.SeqParamSet.Nalu }, new[] { h264Slice.Nalu });
            }
        }

        void ShowH264Headers(Elt[] ppsElts, Elt[] spsElts, Elt[] sliceElts)
        {
            if (m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_PPS] == null)
            {
                m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_PPS] = new ScreenHeader(ppsElts);
                ScreenBase.AddToTabControl(m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_PPS], tabControlHdrs);
            }
            else
            {
                m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_PPS].Elts = ppsElts;
            }

            if (m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SPS] == null)
            {
                m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SPS] = new ScreenHeader(spsElts);
                ScreenBase.AddToTabControl(m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SPS], tabControlHdrs);
            }
            else
            {
                m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SPS].Elts = spsElts;
            }

            if (m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SliceHeader] == null)
            {
                m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SliceHeader] = new ScreenHeader(sliceElts);
                ScreenBase.AddToTabControl(m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SliceHeader], tabControlHdrs);
            }
            else
            {
                m_ScreenHeaders[(int)H264HeaderEltType_t.H264HeaderEltType_SliceHeader].Elts = sliceElts;
            }
        }
    }
}
