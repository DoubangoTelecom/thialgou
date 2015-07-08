using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace thialgou.lib.events
{
    public class FrameEventDrawArgs : MyEventArgs
    {
        readonly uint m_uiId;
        readonly IntPtr m_pLuma = IntPtr.Zero;
        readonly IntPtr m_pCb = IntPtr.Zero;
        readonly IntPtr m_pCr = IntPtr.Zero;
        readonly uint m_uiWidth;
        readonly uint m_uiHeight;
        readonly uint m_uiStride;

        public FrameEventDrawArgs(uint uiId, IntPtr pLuma, IntPtr pCb, IntPtr pCr, uint uiWidth, uint uiHeight, uint uiStride)
        {
            m_uiId = uiId;           
            m_pLuma = pLuma;
            m_pCb = pCb;
            m_pCr = pCr;
            m_uiWidth = uiWidth;
            m_uiHeight = uiHeight;
            m_uiStride = uiStride;
        }

        public uint Id
        {
            get
            {
                return m_uiId;
            }
        }

        public IntPtr Luma
        {
            get
            {
                return m_pLuma;
            }
        }

        public IntPtr ChromaU
        {
            get
            {
                return m_pCb;
            }
        }

        public IntPtr ChromaV
        {
            get
            {
                return m_pCr;
            }
        }

        public uint Width
        {
            get
            {
                return m_uiWidth;
            }
        }
        public uint Height
        {
            get
            {
                return m_uiHeight;
            }
        }
        public uint Stride
        {
            get
            {
                return m_uiStride;
            }
        }
    }
}
