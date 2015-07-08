using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.events;

namespace thialgou.lib.model
{
    /// <summary>
    /// IHdrParserEvents
    /// </summary>
    public interface IHdrParserEvents
    {
        event EventHandler<HdrEventParseArgs> onEvent;
    }

    /// <summary>
    /// HdrParser
    /// </summary>
    public abstract class HdrParser : IHdrParserEvents
    {
        public abstract event EventHandler<HdrEventParseArgs> onEvent;
        public abstract event EventHandler<FrameEventDrawArgs> onEventData; // FIXME: to be remove and added to DataParser

        /// <summary>
        /// 
        /// </summary>
        public enum HdrParserType
        {
            H264
        }

        private readonly HdrParserType mType;

        public HdrParser(HdrParserType type)
        {
            mType = type;
        }

        public HdrParserType Type 
        {
            get { return mType; }
        }
    }
}
