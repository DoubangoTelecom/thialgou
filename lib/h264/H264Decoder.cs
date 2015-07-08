/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using System.Diagnostics;
using thialgou.lib.CommonTypes;
using org.doubango.thialgou.ioWRAP;
using log4net;
using thialgou.lib.events;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.h264
{
    /// <summary>
    /// H.264 AVC/SVC decoder
    /// </summary>
    public class H264Decoder : DecoderVideo
    {
        private static ILog LOG = LogManager.GetLogger(typeof(H264Decoder));

        private readonly h264Decoder m_DecoderInst;
        private HdrParserH264 m_HdrParser;
        public override event EventHandler<EltEventArgs> OnEvent;
        static MainInstH264 g_MainInstH264 = MainInstH264.GetInst();

        readonly IDictionary<UInt32/*LayerId*/, IDictionary<UInt32/*SeqId*/, H264SeqParamSet>> m_SeqParamSets;
        readonly IDictionary<UInt32/*SeqId*/, H264PicParamSet> m_PicParamSets;

        public H264Decoder(MediaFileInfo fileInput, MediaFileInfo fileOutput)
            : base(fileInput, fileOutput)
        {
            m_DecoderInst = new h264Decoder(FileInput.Path, FileOutput.Path);
            g_MainInstH264.OnEvent += new EventHandler<EltEventArgs>(DecoderH264_OnEvent);

            m_SeqParamSets = new Dictionary<UInt32, IDictionary<UInt32, H264SeqParamSet>>();
            m_PicParamSets = new Dictionary<UInt32/*SeqId*/, H264PicParamSet>();
        }

        void DecoderH264_OnEvent(object sender, EltEventArgs e)
        {
            if (g_MainInstH264.Dispatcher.CheckAccess() == false)
            {
                g_MainInstH264.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.DataBind, (Action)(() => DecoderH264_OnEvent(sender, e)));
                return;
            }

            Debug.Assert(e.Elt.Type == Elt.EltType_t.EltType_Section);

            foreach (Elt elt in e.Elt.Elements)
            {
                switch (elt.Type)
                {
                    case Elt.EltType_t.EltType_Nalu:
                        {
                            EltH264Nalu nalu = (elt as EltH264Nalu);
                            switch (nalu.NaluType)
                            {
                                case NalUnitType.NAL_UNIT_SPS:
                                case NalUnitType.NAL_UNIT_SUBSET_SPS:
                                    {
                                        AddSeqParamSet(new H264SeqParamSet(nalu));
                                        break;
                                    }
                                case NalUnitType.NAL_UNIT_PPS:
                                    {
                                        AddPicParamSet(new H264PicParamSet(nalu, this));
                                        break;
                                    }
                                case NalUnitType.NAL_UNIT_CODED_SLICE:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_IDR:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_SCALABLE:
                                    {
                                        H264Slice slice = new H264Slice(nalu, this);
                                        Debug.Assert(slice != null);
                                        H264Picture picture = FindPicture(nalu.LayerId, nalu.PictureId) as H264Picture;
                                        if (picture == null)
                                        {
                                            picture = new H264Picture(nalu.LayerId, nalu.PictureId, slice.PicParamSet, slice.SeqParamSet);
                                        }
                                        bool wasComplete = picture.IsComplete;
                                        picture.AddSlice(slice);
                                        AddPicture(picture, wasComplete);
                                        break;
                                    }

                                case NalUnitType.NAL_UNIT_ACCESS_UNIT_DELIMITER:
                                    {
                                        break;
                                    }
                            }
                            break;
                        }
                }
            }

            if (OnEvent != null)
            {
                EventHandlerTrigger.TriggerEvent<EltEventArgs>(OnEvent, this, e);
            }
        }

        public override bool IsInitialized
        {
            get 
            { 
                return m_DecoderInst.IsInitialized(); 
            }
        }

        public override HdrParser HdrParser 
        {
            get
            {
                if (m_HdrParser == null)
                {
                    m_HdrParser = new HdrParserH264();
                    m_DecoderInst.SetIoEventHdr(m_HdrParser.IoEvent);
                }
                return m_HdrParser;
            }
        }

        public override void Dispose()
        {
            if (m_DecoderInst != null)
            {
                m_DecoderInst.Dispose();
            }
        }

        public override Boolean Decode(UInt32 frameCount)
        {
            return m_DecoderInst.Decode(frameCount);
        }


        private void AddSeqParamSet(H264SeqParamSet seqParamSet)
        {
            KeyValuePair<UInt32, IDictionary<UInt32, H264SeqParamSet>> seqParamSetsKVP = m_SeqParamSets.FirstOrDefault((x) => { return x.Key == seqParamSet.Nalu.LayerId; });
            if (seqParamSetsKVP.Equals(default(KeyValuePair<UInt32, IDictionary<UInt32, H264SeqParamSet>>)))
            {
                IDictionary<UInt32, H264SeqParamSet> seqParamSets = new Dictionary<UInt32, H264SeqParamSet>();
                seqParamSets[seqParamSet.Id] = seqParamSet;
                m_SeqParamSets[seqParamSet.Nalu.LayerId] = seqParamSets;
            }
            else
            {
                seqParamSetsKVP.Value[seqParamSet.Id] = seqParamSet;
            }
        }

        internal H264SeqParamSet FindSeqParamSet(uint layerId, uint seqParamSetId)
        {
            if(m_SeqParamSets.ContainsKey(layerId))
            {
                KeyValuePair<UInt32, H264SeqParamSet> seqParamSetKVP = m_SeqParamSets[layerId].FirstOrDefault((x) =>
                    {
                        return x.Value.Id == seqParamSetId;
                    });
                if (!seqParamSetKVP.Equals(default(KeyValuePair<UInt32, H264SeqParamSet>)))
                {
                    return seqParamSetKVP.Value;
                }
            }
            return null;
        }

        void AddPicParamSet(H264PicParamSet picParamSet)
        {
            m_PicParamSets[picParamSet.Id] = picParamSet;
        }

        internal H264PicParamSet FindPicParamSet(uint picParamSetId)
        {
            if (m_PicParamSets.ContainsKey(picParamSetId))
            {
                return m_PicParamSets[picParamSetId];
            }
            return null;
        }
    }
}
