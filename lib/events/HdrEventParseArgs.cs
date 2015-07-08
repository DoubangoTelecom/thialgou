/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;

namespace thialgou.lib.events
{
    public enum HdrEventParseType
    {
        BeginHdr,
        EndHdr,
        ErrorHdr,
        BeginFnc,
        EndFunc,
        SyntaxElt,
        BeginCtrl,
        EndCtrl,
        DrawFrame
    }

    public class HdrEventParseArgs : MyEventArgs
    {
        public const String EXTRA_FUNC_NAME_STRING = "HdrEventParseArgs::EXTRA_FUNC_NAME";

        public const String EXTRA_SYNTAX_NAME_STRING = "HdrEventParseArgs::EXTRA_SYNTAX_NAME_STRING";
        public const String EXTRA_SYNTAX_DESCRIPTOR_STRING = "HdrEventParseArgs::EXTRA_SYNTAX_DESCRIPTOR_STRING";
        public const String EXTRA_SYNTAX_VALUE_INT32 = "HdrEventParseArgs::EXTRA_SYNTAX_VALUE_INT32";

        public const String EXTRA_CTRL_NAME_STRING = "HdrEventParseArgs::EXTRA_CTRL_NAME_STRING";
        public const String EXTRA_CTRL_EXPRESSION_STRING = "HdrEventParseArgs::EXTRA_CTRL_EXPRESSION_STRING";
        public const String EXTRA_CTRL_VALUE_INT32 = "HdrEventParseArgs::EXTRA_CTRL_VALUE_INT32";

        private readonly HdrEventParseType mParseType;
        private readonly Hdr.HdrType mHdrType;

        public HdrEventParseArgs(HdrEventParseType parseType, Hdr.HdrType hdrType)
            : base()
        {
            mParseType = parseType;
            mHdrType = hdrType;
        }

        public HdrEventParseArgs(HdrEventParseType parseType)
            : this(parseType, Hdr.HdrType.NONE)
        {
        }

        public HdrEventParseType ParseType
        {
            get
            {
                return mParseType;
            }
        }

        public Hdr.HdrType HdrType
        {
            get
            {
                return mHdrType;
            }
        }
    }
}
