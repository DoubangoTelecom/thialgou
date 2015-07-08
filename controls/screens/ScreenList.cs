/*
* Copyright (C) 2012 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using thialgou.lib;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Data;

namespace thialgou.controls.screens
{
    public /* abstract */ class ScreenList<T> : ScreenBase where T : System.IComparable<T>, INotifyPropertyChanged
    {
        private MyObservableCollection<T> mDataSource;
        private ICollectionView mView;

        protected ScreenList()
            : base()
        {
            
        }

        public virtual ItemsControl ListControl 
        {
            get
            {
                throw new Exception("Must be overrided and implemented");
            }
        }

        public MyObservableCollection<T> DataSource
        {
            get
            {
                return mDataSource;
            }

            set
            {
                mDataSource = value;
                ConfigureDS();
            }
        }

        public ICollectionView DataView
        {
            get
            {
                return mView;
            }
        }

        void ConfigureDS()
        {
            ListControl.ItemsSource = DataSource;
            mView = CollectionViewSource.GetDefaultView(ListControl.ItemsSource);
        }
    }
}
