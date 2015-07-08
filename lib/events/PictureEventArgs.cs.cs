using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thialgou.lib.events
{
    public class PictureEventArgs : MyEventArgs
    {
        public enum PictureEventType_t
        {
            PictureEventType_Added,
            PictureEventType_Selected,
            PictureEventType_Updated // e.g. new slice added
        }

        readonly PictureEventType_t m_Type;
        readonly Picture m_Picture;

        public PictureEventArgs(PictureEventType_t type, Picture picture)
            : base()
        {
            m_Type = type;
            m_Picture = picture;
        }

        public PictureEventType_t Type
        {
            get
            {
                return m_Type;
            }
        }

        public Picture Picture
        {
            get
            {
                return m_Picture;
            }
        }
    }
}
