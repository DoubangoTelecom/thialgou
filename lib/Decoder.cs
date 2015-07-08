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
using thialgou.lib.CommonTypes;
using thialgou.lib.model;
using System.Diagnostics;
using thialgou.lib.events;

namespace thialgou.lib
{
    /// <summary>
    /// Base class for all decoders
    /// </summary>
    public abstract class Decoder
    {
        private readonly MediaFileInfo mFileInput;
        private readonly MediaFileInfo mFileOutput;

        public abstract event EventHandler<EltEventArgs> OnEvent;

        protected Decoder(MediaFileInfo fileInput, MediaFileInfo fileOutput)
        {
            mFileInput = fileInput;
            mFileOutput = fileOutput;
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

        public abstract Boolean IsInitialized { get; }
        public abstract HdrParser HdrParser { get; }
        public abstract Boolean Decode(UInt32 frameCount);
        public abstract void Dispose();
    }
    
    /// <summary>
    /// Base class for all video decoders
    /// </summary>
    public abstract class DecoderVideo : Decoder
    {
        protected readonly IDictionary<UInt32/*LayerId*/, IDictionary<UInt32/*Index*/, Picture>> m_Pictures;

        public event EventHandler<PictureEventArgs> OnPictureEvent;

        protected DecoderVideo(MediaFileInfo fileInput, MediaFileInfo fileOutput)
            : base(fileInput, fileOutput)
        {
            m_Pictures = new Dictionary<UInt32, IDictionary<UInt32, Picture>>();
        }

        public IDictionary<UInt32, IDictionary<UInt32, Picture>> Pictures
        {
            get
            {
                return m_Pictures;
            }
        }

        protected void AddPicture(Picture picture, bool wasComplete)
        {
            bool notifyNewPict = false;
            KeyValuePair<UInt32, IDictionary<UInt32, Picture>> picturesKVP = m_Pictures.FirstOrDefault((x) => { return x.Key == picture.LayerId; });
            if (picturesKVP.Equals(default(KeyValuePair<UInt32, IDictionary<UInt32, Picture>>)))
            {
                IDictionary<UInt32, Picture> pictures = new Dictionary<UInt32, Picture>();
                pictures[picture.Index] = picture;
                m_Pictures[picture.LayerId] = pictures;
            }
            else
            {
                picturesKVP.Value[picture.Index] = picture;
            }

            notifyNewPict = (picture.IsComplete && !wasComplete);
            if (OnPictureEvent != null && notifyNewPict)
            {
                EventHandlerTrigger.TriggerEvent<PictureEventArgs>(OnPictureEvent, this, new PictureEventArgs(PictureEventArgs.PictureEventType_t.PictureEventType_Added, picture));
            }
        }

        public Picture FindPicture(uint layerId, uint pictureIndex)
        {
            if (m_Pictures.ContainsKey(layerId))
            {
                KeyValuePair<UInt32, Picture> pictureKVP = m_Pictures[layerId].FirstOrDefault((x) =>
                {
                    return x.Value.Index == pictureIndex;
                });
                if (!pictureKVP.Equals(default(KeyValuePair<UInt32, Picture>)))
                {
                    return pictureKVP.Value;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Base class for all audio decoders
    /// </summary>
    public abstract class DecoderAudio : Decoder
    {
        protected DecoderAudio(MediaFileInfo fileInput, MediaFileInfo fileOutput)
            : base(fileInput, fileOutput)
        {
        }
    }
}
