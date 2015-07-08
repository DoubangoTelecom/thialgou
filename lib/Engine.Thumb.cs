using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;

namespace thialgou.lib
{
    partial class _Engine
    {
        private readonly MyObservableCollection<Thumb> m_Thumbs;

        public MyObservableCollection<Thumb> Thumbs
        {
            get
            {
                return m_Thumbs;
            }
        }

        void HdrParser_onEventData(object sender, thialgou.lib.events.FrameEventDrawArgs e)
        {
            Thumb thumb = new Thumb(e.Id, e.Luma, e.ChromaU, e.ChromaV, e.Width, e.Height, e.Stride);
            m_Thumbs.Add(thumb);
        }
    }
}
