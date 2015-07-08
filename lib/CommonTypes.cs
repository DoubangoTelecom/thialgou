/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thialgou.lib.CommonTypes
{
    /// <summary>
    /// Media Type
    /// </summary>
    [Flags]
    public enum MediaType
    {
        Audio = (0x01 << 0),
        Video = (0x01 << 1),
        AudioVideo = Audio | Video,

        All = 0xFF
    }

    /// <summary>
    /// Encoding Type
    /// </summary>
    public enum EncodingType
    {
        I420,
        H264
    }

    /// <summary>
    /// Container type
    /// </summary>
    public enum ContainerType
    {
        Raw,
        MP4,
        Matroska,
        Ogg
    }
}
