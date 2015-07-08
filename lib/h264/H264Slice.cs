using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using System.Diagnostics;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.h264
{
    public class H264Slice : H264Nalu
    {
        readonly UInt32 m_PicParamSetId;
        readonly H264PicParamSet m_PicParamSet;
        readonly H264SeqParamSet m_SeqParamSet;
        readonly IDictionary<UInt32, Macroblock> m_Macroblocs;

        public H264Slice(EltH264Nalu nalu, H264Decoder decoder)
            : base(nalu)
        {
            switch (nalu.NaluType)
            {
                case NalUnitType.NAL_UNIT_CODED_SLICE:
                case NalUnitType.NAL_UNIT_AUX_CODED_SLICE:
                case NalUnitType.NAL_UNIT_CODED_SLICE_DATAPART_A:
                case NalUnitType.NAL_UNIT_CODED_SLICE_DATAPART_B:
                case NalUnitType.NAL_UNIT_CODED_SLICE_DATAPART_C:
                case NalUnitType.NAL_UNIT_CODED_SLICE_IDR:
                case NalUnitType.NAL_UNIT_CODED_SLICE_SCALABLE:
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            EltSyntax eltSyntax;

            eltSyntax = nalu.FindSyntax("pic_parameter_set_id");
            Debug.Assert(eltSyntax != null);
            m_PicParamSetId = (UInt32)eltSyntax.Value;
            m_PicParamSet = decoder.FindPicParamSet(m_PicParamSetId);
            Debug.Assert(m_PicParamSet != null);
            m_SeqParamSet = m_PicParamSet.SeqParamSet;
            Debug.Assert(m_SeqParamSet != null);

            m_Macroblocs = new Dictionary<UInt32, Macroblock>();
            ExplodeMacroblocs(nalu);
        }

        void ExplodeMacroblocs(Elt elt)
        {
            foreach (Elt _elt in elt.Elements)
            {
                if (_elt.Type == Elt.EltType_t.EltType_Mb)
                {
                    EltMb eltMb = (_elt as EltMb);
                    m_Macroblocs[eltMb.Address] = new Macroblock(Nalu.LayerId, Nalu.PictureId, Nalu.SliceId, eltMb);
                    ExplodeMacroblocs(_elt);
                }
            }
        }

        public UInt32 PicParamSetId
        {
            get
            {
                return m_PicParamSetId;
            }
        }

        public H264PicParamSet PicParamSet
        {
            get
            {
                return m_PicParamSet;
            }
        }

        public H264SeqParamSet SeqParamSet
        {
            get
            {
                return m_SeqParamSet;
            }
        }

        public IDictionary<UInt32, Macroblock> Macroblocs
        {
            get
            {
                return m_Macroblocs;
            }
        }
    }
}
