using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace thialgou.lib.model
{
    public class Thumb : IComparable<Thumb>, INotifyPropertyChanged
    {
        readonly uint m_uiId;
        readonly IntPtr m_pLuma = IntPtr.Zero;
        readonly IntPtr m_pCb = IntPtr.Zero;
        readonly IntPtr m_pCr = IntPtr.Zero;
        readonly uint m_uiWidth;
        readonly uint m_uiHeight;
        readonly uint m_uiStride;

        public Thumb(uint uiId, IntPtr pLuma, IntPtr pCb, IntPtr pCr, uint uiWidth, uint uiHeight, uint uiStride)
        {
            m_uiId = uiId;

            int nSizeLuma = (int)(uiStride * uiHeight);
            int nSizeChroma = (nSizeLuma) >> 2;
            m_pLuma = Marshal.AllocHGlobal(nSizeLuma);
            m_pCb = Marshal.AllocHGlobal(nSizeChroma);
            m_pCr = Marshal.AllocHGlobal(nSizeChroma);

            if (pLuma != IntPtr.Zero)
            {
                NativeMethods.CopyMemory(m_pLuma, pLuma, (uint)nSizeLuma);
            }
            if (pCb != IntPtr.Zero)
            {
                NativeMethods.CopyMemory(m_pCb, pCb, (uint)nSizeChroma);
            }
            if (pCr != IntPtr.Zero)
            {
                NativeMethods.CopyMemory(m_pCr, pCr, (uint)nSizeChroma);
            }
            m_uiWidth = uiWidth;
            m_uiHeight = uiHeight;
            m_uiStride = uiStride;
        }

        ~Thumb()
        {
            if (m_pLuma != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_pLuma);
            }
            if (m_pCb != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_pCb);
            }
            if (m_pCr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(m_pCr);
            }
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

        public int CompareTo(Thumb other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return (other.Id.CompareTo(this.Id));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
