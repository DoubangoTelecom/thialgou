using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;
using System.ComponentModel;
using thialgou.lib.h264;

namespace thialgou.lib.model
{
    public class Elt : IComparable<Elt>, INotifyPropertyChanged
    {
        readonly MyObservableCollection<Elt> m_Elements;
        readonly EltType_t m_Type;
        readonly String m_BitsStart;
        readonly String m_BitsVal;
        readonly uint m_BitsCount;
        String m_Description;

        public enum EltType_t
        {
            EltType_Header = CommonElementType_t.CommonEltType_Header,
            EltType_Function = CommonElementType_t.CommonEltType_Function,
            EltType_Syntax = CommonElementType_t.CommonEltType_Syntax,
            EltType_Control = CommonElementType_t.CommonEltType_Control,
            EltType_Error = CommonElementType_t.CommonEltType_Error,
            EltType_Mb = CommonElementType_t.CommonEltType_Mb,

            /* === H.264 === */
            EltType_Section = CommonElementType_t.CommonEltType_Section,
            EltType_Nalu = CommonElementType_t.CommonEltType_Nalu,
        }

        private Elt(EltType_t type, String bitsStart, String bitsVal, uint bitsCount)
        {
            m_Elements = new MyObservableCollection<Elt>(true);
            m_Type = type;
            m_BitsStart = bitsStart;
            m_BitsVal = bitsVal;
            m_BitsCount = bitsCount;
        }

        protected Elt(CommonElt elt)
        {
            m_Elements = new MyObservableCollection<Elt>(true);
            m_Type = (EltType_t)elt.getType();
            m_BitsStart = elt.getBitsStart().Trim();
            m_BitsVal = elt.getBitsVal().Trim();
            m_BitsCount = elt.getBitsCount();
            
            Explode(elt);
        }
        
        void Explode(CommonElt elt)
        {
            CommonEltVectorPtr elts = elt.getElements();
            Elt _elt;
            foreach (CommonElt e in elts)
            {
                if ((_elt = Elt.New(e)) != null)
                {
                    m_Elements.Add(_elt);
                }
            }
        }

        internal static Elt New(CommonElt elt)
        {
            System.Diagnostics.Debug.Assert(elt != null);
            switch (elt.getType())
            {
                case CommonElementType_t.CommonEltType_Syntax:
                    {
                        return new EltSyntax(SwigHelper.CastTo<CommonSyntax>(elt));
                    }
                case CommonElementType_t.CommonEltType_Control:
                    {
                        return new EltControl(SwigHelper.CastTo<CommonControl>(elt));
                    }
                case CommonElementType_t.CommonEltType_Header:
                    {
                        return new EltHeader(SwigHelper.CastTo<CommonHeader>(elt));
                    }
                case CommonElementType_t.CommonEltType_Function:
                    {
                        return new EltFunction(SwigHelper.CastTo<CommonFunction>(elt));
                    }
                case CommonElementType_t.CommonEltType_Error:
                    {
                        return new EltError(SwigHelper.CastTo<CommonError>(elt));
                    }
                case CommonElementType_t.CommonEltType_Mb:
                    {
                        CommonMb mb = SwigHelper.CastTo<CommonMb>(elt);
                        return mb.getMbType() == CommonEltMbType_t.CommonEltMbType_H264 ? new H264Mb(SwigHelper.CastTo<CommonMbH264>(elt)) : new EltMb(mb);
                    }
                /* === H.264 === */
                case CommonElementType_t.CommonEltType_Nalu:
                    {
                        return new EltH264Nalu(SwigHelper.CastTo<CommonH264Nalu>(elt));
                    }
                case CommonElementType_t.CommonEltType_Section:
                    {
                        return new EltH264Section(SwigHelper.CastTo<CommonH264Section>(elt));
                    }
                default:
                    {
                        throw new NotImplementedException("No constructor for element with type = " + elt.getType());
                    }
            }
        }

        public EltType_t Type
        {
            get
            {
                return m_Type;
            }
        }

        public String BitsStart
        {
            get
            {
                return m_BitsStart;
            }
        }

        public String BitsVal
        {
            get
            {
                return m_BitsVal;
            }
        }

        public uint BitsCount
        {
            get
            {
                return m_BitsCount;
            }
        }

        public MyObservableCollection<Elt> Elements
        {
            get
            {
                return m_Elements;
            }
        }

        public virtual String Description
        {
            get
            {
                return String.IsNullOrEmpty(m_Description) ? Type.ToString() : m_Description;
            }
            private set
            {
                m_Description = value;
            }
        }

        public EltSyntax FindSyntax(String name, bool recursive)
        {
            EltSyntax _elt;
            foreach (Elt e in m_Elements)
            {
                if (e.Type == EltType_t.EltType_Syntax && (_elt = (e as EltSyntax)) != null && _elt.Name == name)
                {
                    return _elt;
                }
                if (recursive)
                {
                    _elt = e.FindSyntax(name, recursive);
                    if (_elt != null)
                    {
                        return _elt;
                    }
                }
            }
            return null;
        }

        public EltSyntax FindSyntax(String descriptor)
        {
            return FindSyntax(descriptor, true);
        }

        public Elt CloneAndFilter(EltType_t typeToExclude)
        {
            Elt elt = new Elt(Type, BitsStart, BitsVal, BitsCount);
            elt.Description = Description;
            foreach(var v in Elements)
            {
                if (v.Type != typeToExclude)
                {
                    elt.m_Elements.Add(v);
                }
            }
            return elt;
        }

        public virtual int CompareTo(Elt other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return (other.Type.CompareTo(this.Type));
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
