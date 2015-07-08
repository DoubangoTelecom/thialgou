/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace thialgou.lib.model
{
    /// <summary>
    /// Hdr
    /// </summary>
    public class Hdr : IComparable<Hdr>, INotifyPropertyChanged
    {
        private readonly HdrType mType;
        private readonly String mName;
        private readonly String mMainFuncName;
        private readonly MyObservableCollection<HdrItem> mItems;
        private Boolean mActive;
        private Boolean mError;

        public enum HdrType
        {
            NONE,
            SPS,
            SLICE,
            PPS
        }

        public Hdr(HdrType type, Boolean active, String name, String mainFuncName)
        {
            mType = type;
            mActive = active;
            mName = name;
            mMainFuncName = mainFuncName;
            mItems = new MyObservableCollection<HdrItem>(true);
        }

        public Hdr(HdrType type, Boolean active, String mainFuncName)
            : this(type, active, null, mainFuncName)
        {
            mName = type.ToString();
        }

        public Hdr(HdrType type, String mainFuncName)
            : this(type, false, mainFuncName)
        {
            
        }

        public HdrType Type
        {
            get
            {
                return mType;
            }
        }

        public Boolean Active
        {
            get
            {
                return mActive;
            }
            set
            {
                if (mActive != value)
                {
                    mActive = value;
                    this.OnPropertyChanged("Active");
                }
            }
        }

        public Boolean Error
        {
            get
            {
                return mError;
            }
            set
            {
                if (mError != value)
                {
                    mError = value;
                    this.OnPropertyChanged("Error");
                }
            }
        }

        public String Name
        {
            get
            {
                return mName;
            }
        }

        public String MainFuncName
        {
            get
            {
                return mMainFuncName;
            }
        }

        public MyObservableCollection<HdrItem> Items
        {
            get
            {
                return mItems;
            }
        }

        public int CompareTo(Hdr other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return (other.Type.CompareTo(this.Type) + other.Name.CompareTo(this.Name));
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


    /// <summary>
    /// Item
    /// </summary>
    public abstract class HdrItem : IComparable<HdrItem>, INotifyPropertyChanged
    {
        public enum ItemType
        {
            Func,
            SyntaxElt,
            Ctrl
        }

        private readonly ItemType mType;
        private readonly String mDescription;
        private readonly MyObservableCollection<HdrItem> mItems;

        protected HdrItem(ItemType type, String description)
        {
            mType = type;
            mDescription = description;
            mItems = new MyObservableCollection<HdrItem>(true);
        }

        protected HdrItem(ItemType type)
            : this(type, null)
        {
        }

        public ItemType Type
        {
            get
            {
                return mType;
            }
        }

        public virtual String Description
        {
            get
            {
                return mDescription;
            }
        }

        public MyObservableCollection<HdrItem> Items
        {
            get
            {
                return mItems;
            }
        }

        public int CompareTo(HdrItem other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return (other.Type.CompareTo(this.Type) + other.Description.CompareTo(this.Description));
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

    /// <summary>
    /// ItemFunc
    /// </summaryItemFunc
    public class HdrItemFunc : HdrItem
    {
        public HdrItemFunc(String description)
            : base(ItemType.Func, description)
        {
        }
    }

    /// <summary>
    /// ItemSyntaxElt
    /// </summary>
    public class HdrItemSyntaxElt : HdrItem
    {
        private readonly String mName;
        private readonly String mDescriptor;
        private readonly Int32? mValue;

        public HdrItemSyntaxElt(String name, String descriptor, Int32? value)
            : base(ItemType.SyntaxElt)
        {
            mName = name;
            mDescriptor = descriptor;
            mValue = value;
        }

        public String Name
        {
            get
            {
                return mName;
            }
        }

        public Int32? Value
        {
            get
            {
                return mValue;
            }
        }

        public String Descriptor
        {
            get
            {
                return mDescriptor;
            }
        }

        public override String Description
        {
            get
            {
                return String.Format("{0} = {1}", Value, Name);
            }
        }
    }

    

    /// <summary>
    /// HdrItemCtrl
    /// </summary>
    public abstract class HdrItemCtrl : HdrItem
    {
        private readonly String mName;

        public HdrItemCtrl(String name)
            : base(ItemType.Ctrl)
        {
            mName = name;
        }

        public String Name
        {
            get
            {
                return mName;
            }
        }

        /// <summary>
        /// HdrItemCtrlStart
        /// </summary>
        public class HdrItemCtrlBegin : HdrItemCtrl
        {
            private readonly String mExpression;
            private readonly Int32? mValue;

            public HdrItemCtrlBegin(String name, String expression, Int32? value)
                : base(name)
            {
                mExpression = expression;
                mValue = value;
            }

            public String Expression
            {
                get
                {
                    return mExpression;
                }
            }

            public Int32? Value
            {
                get
                {
                    return mValue;
                }
            }

            public Boolean IsExpressionValueTrue
            {
                get
                {
                    return (Value.HasValue && Value.Value != 0);
                }
            }

            public override String Description
            {
                get
                {
                    return String.Format("{0}({1}){{ /* {2} */", Name, Expression, IsExpressionValueTrue);
                }
            }
        }

        /// <summary>
        /// HdrItemCtrlEnd
        /// </summary>
        public class HdrItemCtrlEnd : HdrItemCtrl
        {
            public HdrItemCtrlEnd(String name)
                : base(name)
            {
            }

            public override String Description
            {
                get
                {
                    return String.Format("}}//{0}", Name);
                }
            }
        }
    }
}

