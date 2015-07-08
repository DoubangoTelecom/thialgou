using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;

namespace thialgou.lib.h264
{
    public class H264Nalu
    {
        readonly EltH264Nalu m_Nalu;

        public H264Nalu(EltH264Nalu nalu)
        {
            m_Nalu = nalu;
        }

        public EltH264Nalu Nalu
        {
            get
            {
                return m_Nalu;
            }
        }
    }
}
