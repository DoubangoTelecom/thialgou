using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.ioWRAP;
using thialgou.lib.model;

namespace thialgou.lib.events.h264
{
    public class HdrEventParseArgsH264 : HdrEventParseArgs
    {
        public HdrEventParseArgsH264(HdrEventParseType type, h264HdrType_t hdrType)
            : base(type, GetType(hdrType))
        {
        }

        public HdrEventParseArgsH264(HdrEventParseType type)
            : this(type, h264HdrType_t.h264HdrType_None)
        {
        }

        private static Hdr.HdrType GetType(h264HdrType_t hdrType)
        {
            switch (hdrType)
            {
                case h264HdrType_t.h264HdrType_PPS:
                    {
                        return Hdr.HdrType.PPS;
                    }
                case h264HdrType_t.h264HdrType_Slice:
                    {
                        return Hdr.HdrType.SLICE;
                    }
                case h264HdrType_t.h264HdrType_SPS:
                    {
                        return Hdr.HdrType.SPS;
                    }
                default:
                    {
                        return Hdr.HdrType.NONE;
                    }
            }
        }
    }
}
