using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace thialgou.lib.model
{
    public class MbBits4x4 : IComparable<MbBits4x4>, INotifyPropertyChanged
    {
        readonly Int32[] m_Bytes;
        readonly UInt32 m_X; 
        readonly UInt32 m_Y;
        readonly UInt32 m_Address;

        public MbBits4x4(UInt32 address, UInt32 x, UInt32 y, Int32[] bytes)
        {
            Debug.Assert(bytes.Length >= 16);
            m_Address = address;
            m_X = x;
            m_Y = y;
            m_Bytes = bytes;
        }

        public Int32[] Bytes
        {
            get
            {
                return m_Bytes;
            }
        }

        public UInt32 Address
        {
            get
            {
                return m_Address;
            }
        }

        public UInt32 X
        {
            get
            {
                return m_X;
            }
        }
        public UInt32 Y
        {
            get
            {
                return m_Y;
            }
        }

        public int CompareTo(MbBits4x4 other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            if (other.Y != Y)
            {
                return other.Y.CompareTo(Y);
            }
            return other.X.CompareTo(X);
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
