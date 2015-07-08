/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using thialgou.lib.model;
using thialgou.lib.CommonTypes;
using thialgou.lib.h264;

namespace thialgou.lib
{
    /// <summary>
    /// _Engine
    /// </summary>
    public abstract partial class _Engine : IDisposable
    {
        private static ILog LOG = LogManager.GetLogger(typeof(_Engine));

        private readonly MediaFileInfo mFileInput;
        private readonly MediaFileInfo mFileOutput;
        private Decoder mDecoder;

        protected _Engine(MediaType mediaType, ContainerType inputFileContainerType, EncodingType inputFileEncondingType, String inputFilePath, String outputFilePath)
        {
            if (mediaType != MediaType.Video || inputFileContainerType != ContainerType.Raw || inputFileEncondingType != EncodingType.H264)
            {
                throw new NotSupportedException("Requested media type not implemented yet");
            }

            mFileInput = new MediaFileInfo(mediaType, inputFileContainerType, inputFileEncondingType, inputFilePath);
            mFileOutput = new MediaFileInfo(mediaType, ContainerType.Raw, EncodingType.I420, outputFilePath);
            m_Hdrs = new MyObservableCollection<Hdr>(true);
            m_Thumbs = new MyObservableCollection<Thumb>(true);

            mDecoder = new H264Decoder(mFileInput, mFileOutput);
            mDecoder.HdrParser.onEvent += HdrParser_onEvent;
            mDecoder.HdrParser.onEventData += HdrParser_onEventData;
        }

        public void Dispose()
        {
            if (mDecoder != null)
            {
                mDecoder.Dispose();
                mDecoder = null;
            }
        }

        public MediaFileInfo FileInput
        {
            get
            {
                return mFileInput;
            }
        }

        public MediaFileInfo FileOutput
        {
            get
            {
                return mFileOutput;
            }
        }

        public Decoder Decoder
        {
            get
            {
                return mDecoder;
            }
        }

        public virtual Boolean MultiInstance
        {
            get
            {
                return false;
            }
        }
    }
}
