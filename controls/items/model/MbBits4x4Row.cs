using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using System.ComponentModel;
using thialgou.lib;

namespace thialgou.controls.items.model
{
    public class MbBits4x4Row : IComparable<MbBits4x4Row>, INotifyPropertyChanged
    {
        readonly MyObservableCollection<MbBits4x4> m_Row;

        public MbBits4x4Row()
        {
            m_Row = new MyObservableCollection<MbBits4x4>(true);
        }

        public void Add(MbBits4x4 bits4x4)
        {
            if (bits4x4 != null)
            {
                m_Row.Add(bits4x4);
            }
        }

        public MyObservableCollection<MbBits4x4> Row
        {
            get
            {
                return m_Row;
            }
        }

        public int CompareTo(MbBits4x4Row other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return 0;
        }

        public UInt32 Count
        {
            get
            {
                return (UInt32)m_Row.Count;
            }
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
