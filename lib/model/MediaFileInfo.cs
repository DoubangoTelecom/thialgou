/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.CommonTypes;

namespace thialgou.lib.model
{
    /// <summary>
    /// Media File information
    /// </summary>
    public class MediaFileInfo
    {
        private readonly MediaType mMediaType;
        private readonly ContainerType mContainerType;
        private readonly EncodingType mEncondingType;
        private readonly String mPath;

        public MediaFileInfo(MediaType mediaType, ContainerType containerType, EncodingType encondingType, String path)
        {
            mMediaType = mediaType;
            mContainerType = containerType;
            mEncondingType = encondingType;
            mPath = path;
        }

        public MediaType MediaType
        {
            get { return mMediaType; }
        }

        public ContainerType ContainerType
        {
            get { return mContainerType; }
        }

        public EncodingType EncodingType
        {
            get { return mEncondingType; }
        }

        public String Path
        {
            get { return mPath; }
        }

        /*public override string ToString()
        {
            return String.Format("MediaType = {0}, ContainerType = {1}, EncodingType = {2}", MediaType, ContainerType, EncodingType);
        }*/
    }
}
