using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.ioWRAP;
using log4net;
using thialgou.lib.events.h264;
using thialgou.lib.events;
using thialgou.lib.model;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace thialgou.lib.h264
{
    /// <summary>
    /// HdrParserH264
    /// </summary>
    public class HdrParserH264 : HdrParser
    {
        private static ILog LOG = LogManager.GetLogger(typeof(HdrParserH264));

        private _h264HdrEventParsing mIoEvent;
        public override event EventHandler<HdrEventParseArgs> onEvent;
        public override event EventHandler<FrameEventDrawArgs> onEventData;  // FIXME: to be removed and added to DataParserH264

        public HdrParserH264()
            : base(HdrParserType.H264)
        {
            
        }

        public h264HdrEventParsing IoEvent
        {
            get
            {
                if (mIoEvent == null)
                {
                    mIoEvent = new _h264HdrEventParsing(this);
                }
                return mIoEvent;
            }
        }

        /// <summary>
        /// _h264HdrEventParsing
        /// </summary>
        class _h264HdrEventParsing : h264HdrEventParsing
        {
            private readonly HdrParserH264 mParser;

            public _h264HdrEventParsing(HdrParserH264 parser)
                : base()
            {
                mParser = parser;
            }

            public override Int32 OnBeginHdr(h264HdrType_t eType, String pFuncName)
            {
                lock (mParser)
                {
                    if (mParser.onEvent != null)
                    {
                        HdrEventParseArgsH264 args = new HdrEventParseArgsH264(HdrEventParseType.BeginHdr, eType);
                        args.AddExtra(HdrEventParseArgs.EXTRA_FUNC_NAME_STRING, pFuncName);
                        EventHandlerTrigger.TriggerEvent<HdrEventParseArgs>(mParser.onEvent, mParser, args);
                    }
                }
                return 0;
            }

            public override Int32 OnEndHdr(h264HdrType_t eType)
            {
                lock (mParser)
                {
                    if (mParser.onEvent != null)
                    {
                        EventHandlerTrigger.TriggerEvent<HdrEventParseArgs>(mParser.onEvent, mParser, new HdrEventParseArgsH264(HdrEventParseType.EndHdr, eType));
                    }
                }
                return 0;
            }

            public override Int32 OnErrorHdr(h264HdrType_t eType)
            {
                lock (mParser)
                {
                    if (mParser.onEvent != null)
                    {
                        EventHandlerTrigger.TriggerEvent<HdrEventParseArgs>(mParser.onEvent, mParser, new HdrEventParseArgsH264(HdrEventParseType.ErrorHdr, eType));
                    }
                }
                return 0;
            }

            public override Int32 OnSyntaxElt(String pName, String pDescriptor, Int32 value)
            {
                lock (mParser)
                {
                    if (mParser.onEvent != null)
                    {
                        HdrEventParseArgsH264 args = new HdrEventParseArgsH264(HdrEventParseType.SyntaxElt);
                        args
                            .AddExtra(HdrEventParseArgs.EXTRA_SYNTAX_NAME_STRING, pName)
                            .AddExtra(HdrEventParseArgs.EXTRA_SYNTAX_DESCRIPTOR_STRING, pDescriptor)
                            .AddExtra(HdrEventParseArgs.EXTRA_SYNTAX_VALUE_INT32, value);
                        EventHandlerTrigger.TriggerEvent<HdrEventParseArgs>(mParser.onEvent, mParser, args);
                    }
                }
                return 0;
            }

            public override Int32 OnBeginCtrl(String pName, String pExpression, Int32 value)
            {
                lock (mParser)
                {
                    if (mParser.onEvent != null)
                    {
                        HdrEventParseArgsH264 args = new HdrEventParseArgsH264(HdrEventParseType.BeginCtrl);
                        args
                            .AddExtra(HdrEventParseArgs.EXTRA_CTRL_NAME_STRING, pName)
                            .AddExtra(HdrEventParseArgs.EXTRA_CTRL_EXPRESSION_STRING, pExpression)
                            .AddExtra(HdrEventParseArgs.EXTRA_CTRL_VALUE_INT32, value);
                        EventHandlerTrigger.TriggerEvent<HdrEventParseArgs>(mParser.onEvent, mParser, args);
                    }
                }
                return 0;
            }

            public override Int32 OnEndCtrl(String pName)
            {
                lock (mParser)
                {
                    if (mParser.onEvent != null)
                    {
                        HdrEventParseArgsH264 args = new HdrEventParseArgsH264(HdrEventParseType.EndCtrl);
                        args.AddExtra(HdrEventParseArgs.EXTRA_CTRL_NAME_STRING, pName);
                        EventHandlerTrigger.TriggerEvent<HdrEventParseArgs>(mParser.onEvent, mParser, args);
                    }
                }
                return 0;
            }

            public override Int32 OnFrameDraw(IntPtr pLum, IntPtr pCb, IntPtr pCr, uint uiWidth, uint uiHeight, uint uiStride)
            {
                lock (mParser)
                {
                    if (mParser.onEventData != null)
                    {
                        FrameEventDrawArgs args = new FrameEventDrawArgs(0/*FIXME*/, pLum, pCb, pCr, uiWidth, uiHeight, uiStride);
                        EventHandlerTrigger.TriggerEvent<FrameEventDrawArgs>(mParser.onEventData, mParser, args);
                    }
                }
                return 0;
            }
        }
    }
}
