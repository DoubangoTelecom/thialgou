using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using System.Diagnostics;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.h264
{
    public class H264PicParamSet : H264Nalu
    {
        readonly UInt32 m_Id;
        readonly UInt32 m_SeqParamSetId;
        readonly H264SeqParamSet m_SeqParamSet;
        readonly bool m_IsCAVLCEncoded;

        public H264PicParamSet(EltH264Nalu nalu, H264Decoder decoder)
            : base(nalu)
        {
            EltSyntax eltSyntax;
            Debug.Assert(nalu.NaluType == NalUnitType.NAL_UNIT_PPS);

            eltSyntax = nalu.FindSyntax("pic_parameter_set_id");
            Debug.Assert(eltSyntax != null);
            m_Id = (UInt32)eltSyntax.Value;

            eltSyntax = nalu.FindSyntax("seq_parameter_set_id");
            Debug.Assert(eltSyntax != null);
            m_SeqParamSetId = (UInt32)eltSyntax.Value;

            eltSyntax = nalu.FindSyntax("entropy_coding_mode_flag");
            Debug.Assert(eltSyntax != null);
            m_IsCAVLCEncoded = (eltSyntax.Value == 0);

            m_SeqParamSet = decoder.FindSeqParamSet(nalu.LayerId, m_SeqParamSetId);
            Debug.Assert(m_SeqParamSet != null);
        }

        public UInt32 Id
        {
            get
            {
                return m_Id;
            }
        }

        public bool IsCAVLCEncoded
        {
            get
            {
                return m_IsCAVLCEncoded;
            }
        }

        public UInt32 SeqParamSetId
        {
            get
            {
                return m_SeqParamSetId;
            }
        }

        public H264SeqParamSet SeqParamSet
        {
            get
            {
                return m_SeqParamSet;
            }
        }
    }
}
