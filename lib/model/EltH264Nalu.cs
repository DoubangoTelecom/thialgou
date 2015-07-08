using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    public class EltH264Nalu : Elt
    {
        readonly NalUnitType m_eNaluType;

       readonly UInt32 m_LayerId, m_PictureId, m_SliceId;

        public EltH264Nalu(CommonH264Nalu nalu)
            : base(nalu)
        {
            m_eNaluType = nalu.getNaluType();
            m_LayerId = nalu.getLayerId();
            m_PictureId = nalu.getPictureId();
            m_SliceId = nalu.getSliceId();
        }

        public NalUnitType NaluType
        {
            get
            {
                return m_eNaluType;
            }
        }

        public UInt32 LayerId
        {
            get
            {
                return m_LayerId;
            }
        }

        public UInt32 PictureId
        {
            get
            {
                return m_PictureId;
            }
        }

        public UInt32 SliceId
        {
            get
            {
                return m_SliceId;
            }
        }

        public override String Description
        {
            get
            {
                return String.Format("NALU [{0}]", NaluType);
            }
        }
    }
}
