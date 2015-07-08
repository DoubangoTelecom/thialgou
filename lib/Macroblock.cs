using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ComponentModel;
using org.doubango.thialgou.commonWRAP;
using org.doubango.thialgou.ioWRAP;

namespace thialgou.lib
{
    public class Macroblock : IDisposable, IComparable<Macroblock>, INotifyPropertyChanged
    {
        readonly EltMb m_Mb;
        readonly UInt32 m_LayerId;
        readonly UInt32 m_PictureId;
        readonly UInt32 m_SliceId;
        String m_MD5Y, m_MD5U, m_MD5V;

        public Macroblock(UInt32 layerId, UInt32 pictureId, UInt32 sliceId, EltMb mb)
        {
            m_Mb = mb;
            m_LayerId = layerId;
            m_PictureId = pictureId;
            m_SliceId = sliceId;
        }

        ~Macroblock()
        {
            Dispose();
        }

        public void Dispose()
        {
        }

        public UInt32 LayerId
        {
            get
            {
                return m_LayerId;
            }
        }
        public UInt32 PictureId
        {
            get
            {
                return m_PictureId;
            }
        }
        public UInt32 SliceId
        {
            get
            {
                return m_SliceId;
            }
        }

        public EltMb Mb
        {
            get
            {
                return m_Mb;
            }
        }

        public String MD5Y
        {
            get
            {
                if (String.IsNullOrEmpty(m_MD5Y))
                {
                    m_MD5Y = ComputeMD5(CommonYuvLine_t.CommonYuvLine_Y, CommonEltMbDataType_t.CommonEltMbDataType_Final);
                }
                return m_MD5Y;
            }
        }

        public String MD5U
        {
            get
            {
                if (String.IsNullOrEmpty(m_MD5U))
                {
                    m_MD5U = ComputeMD5(CommonYuvLine_t.CommonYuvLine_U, CommonEltMbDataType_t.CommonEltMbDataType_Final);
                }
                return m_MD5U;
            }
        }

        public String MD5V
        {
            get
            {
                if (String.IsNullOrEmpty(m_MD5V))
                {
                    m_MD5V = ComputeMD5(CommonYuvLine_t.CommonYuvLine_V, CommonEltMbDataType_t.CommonEltMbDataType_Final);
                }
                return m_MD5V;
            }
        }

        String ComputeMD5(CommonYuvLine_t line, CommonEltMbDataType_t type)
        {
            String md5String;
            Int32[] a = Mb.GetMbBits(line, type);
            byte[] b = new byte[a.Length];
            for (int i = 0; i < b.Length; ++i)
            {
                b[i] = /*BitConverter.GetBytes*/(byte)(a[i] & 0xFF);
            }
            IntPtr ptr = Marshal.AllocHGlobal(b.Length);
            Marshal.Copy(b, 0, ptr, b.Length);
            Md5 md5Ctx = new Md5();
            md5String = md5Ctx.compute(ptr, (uint)b.Length);
            Marshal.FreeHGlobal(ptr);
            md5Ctx.Dispose();

            return md5String;
        }

        public int CompareTo(Macroblock other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (LayerId != other.LayerId)
            {
                return LayerId.CompareTo(other.LayerId);
            }
            if (PictureId != other.PictureId)
            {
                return PictureId.CompareTo(other.PictureId);
            }
            if (SliceId != other.SliceId)
            {
                return SliceId.CompareTo(other.SliceId);
            }
            return Mb.CompareTo(other.Mb);
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
