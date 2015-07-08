using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib
{
    public abstract class Picture : IDisposable, IComparable<Picture>, INotifyPropertyChanged
    {
        readonly UInt32 m_Index;
        readonly UInt32 m_LayerId;

        public Picture(UInt32 layerId, UInt32 index)
        {
            m_Index = index;
            m_LayerId = layerId;
        }

        ~Picture()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
        }

        public UInt32 Index
        {
            get
            {
                return m_Index;
            }
        }

        public UInt32 LayerId
        {
            get
            {
                return m_LayerId;
            }
        }

        public abstract UInt32 Width { get; }
        public abstract UInt32 Height { get; }
        public abstract UInt32 NumberOfSlices { get; }
        public abstract String EntropyCodingType { get; }
        public abstract bool IsComplete { get; }

        public abstract Macroblock GetMacroblockByAddress(UInt32 sliceId, UInt32 mbAddress);
        public abstract Macroblock GetMacroblockByLocation(UInt32 x, UInt32 y);
        public abstract String GetMd5(CommonEltMbDataType_t eType, int iLine/*Y=0,U=1,V=2,YUV=3*/);
        public abstract Bitmap GetImage(CommonEltMbDataType_t eType);

        public int CompareTo(Picture other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (LayerId != other.LayerId)
            {
                return LayerId.CompareTo(other.LayerId);
            }
            return Index.CompareTo(other.Index);
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
