using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using System.Diagnostics;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.h264
{
    public class H264SeqParamSet : H264Nalu
    {
        readonly UInt32 m_Id;

        readonly UInt32 m_Width;
        readonly UInt32 m_Height;
        readonly UInt32 m_SizeInMbs;

        public H264SeqParamSet(EltH264Nalu nalu)
            : base(nalu)
        {
            EltSyntax eltSyntax;
            Debug.Assert(nalu.NaluType == NalUnitType.NAL_UNIT_SPS || nalu.NaluType == NalUnitType.NAL_UNIT_SUBSET_SPS);

            eltSyntax = nalu.FindSyntax("seq_parameter_set_id");
            Debug.Assert(eltSyntax != null);
            m_Id = (UInt32)eltSyntax.Value;

            eltSyntax = nalu.FindSyntax("pic_width_in_mbs_minus1");
            Debug.Assert(eltSyntax != null);
            m_Width = ((UInt32)eltSyntax.Value << 4) + 16;

            eltSyntax = nalu.FindSyntax("pic_height_in_map_units_minus1");
            Debug.Assert(eltSyntax != null);
            m_Height = ((UInt32)eltSyntax.Value << 4) + 16;

            m_SizeInMbs = (m_Width * m_Height) >> 8;
        }

        public UInt32 Id
        {
            get
            {
                return m_Id;
            }
        }

        public UInt32 Width
        {
            get
            {
                return m_Width;
            }
        }

        public UInt32 Height
        {
            get
            {
                return m_Height;
            }
        }

        public UInt32 SizeInMbs
        {
            get
            {
                return m_SizeInMbs;
            }
        }
    }
}
