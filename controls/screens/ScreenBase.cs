/*
* Copyright (C) 2012 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;

namespace thialgou.controls.screens
{
    public class ScreenBase : UserControl, IComparable<ScreenBase>, INotifyPropertyChanged
    {
        protected String mBaseScreenId;

        public const int SCREEN_TYPE_MB_BITS = 100;
        public const int SCREEN_TYPE_MB_INFO = 101;
        public const int SCREEN_TYPE_MB_INFO_GENERAL = 102;
        public const int SCREEN_TYPE_MB_INFO_I4x4PREDMODE = 103;
        public const int SCREEN_TYPE_MB_INFO_SUNBLOCKS = 104;
        public const int SCREEN_TYPE_MB_INFO_INTERPRED = 105;
        public const int SCREEN_TYPE_MB_SVC = 106;

        public const int SCREEN_TYPE_PICTURE_VIEW = 200;
        public const int SCREEN_TYPE_PICTURE_INFO = 201;

        public const int SCREEN_TYPE_THUMBNAILS = 300;
        public const int SCREEN_TYPE_HEADER = 400;

        public ScreenBase()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Width = Double.NaN;
            Height = Double.NaN;
            this.mBaseScreenId = Guid.NewGuid().ToString();
        }

        public virtual String BaseScreenId
        {
            get { return this.mBaseScreenId; }
            set 
            {
                if (this.mBaseScreenId != value)
                {
                    this.mBaseScreenId = value;
                    this.OnPropertyChanged("BaseScreenId");
                }
            }
        }

        public virtual String BaseScreenTitle
        {
            get { throw new Exception("BaseScreen::ScreenType must be overrided."); }
        }

        public virtual int BaseScreenType
        {
            get { throw new Exception("BaseScreen::ScreenType must be overrided."); }
        }

        public virtual void BeforeLoading()
        {
            
        }

        public virtual void AfterLoading()
        {
            
        }

        public virtual void BeforeUnLoading()
        {
            
        }

        public virtual void AfterUnLoading()
        {
            
        }

        public int CompareTo(ScreenBase other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            return other.BaseScreenId.CompareTo(this.BaseScreenId);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static TabItem AddToTabControl(ScreenBase screen, TabControl tabControl, int index)
        {
            TabItem tabItem = new TabItem();
            tabItem.Header = screen.BaseScreenTitle;
            tabItem.Content = screen;
            screen.Width = Double.NaN;
            screen.Height = Double.NaN;
            {
                (tabItem.Content as ScreenBase).BeforeLoading();

                if (index > 0 && index < tabControl.Items.Count)
                {
                    tabControl.Items.Insert(index, tabItem);
                }
                else
                {
                    tabControl.Items.Add(tabItem);
                }

                if (tabControl.SelectedItem == null)
                {
                    tabControl.SelectedItem = tabItem;
                }
            }

            return tabItem;
        }

        public static TabItem AddToTabControl(ScreenBase screen, TabControl tabControl)
        {
            return AddToTabControl(screen, tabControl, -1);
        }

        public static void RemoveFromTabControl(ScreenBase screen, TabControl tabControl)
        {
            int i = 0;
            foreach (TabItem item in tabControl.Items)
            {
                if (item.Content == screen)
                {
                    tabControl.Items.RemoveAt(i);
                    item.Content = null;
                    break;
                }
                i++;
            }
        }
    }
}
