/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace thialgou.controls.items
{
    public class ItemBase<T> : UserControl
    {
        private bool loaded;
        public event EventHandler ValueLoaded;

        public static DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(T), typeof(ItemBase<T>));

        public ItemBase()
            : base()
        {
            this.Loaded += this.BaseItem_Loaded;
        }

        public T Value
        {
            get { return ((T)this.GetValue(ValueProperty)); }
            set { this.SetValue(ValueProperty, value); }
        }

        public void BaseItem_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!this.loaded)
            {
                this.loaded = true;
                if (this.ValueLoaded != null)
                {
                    this.ValueLoaded(sender, EventArgs.Empty);
                }
            }
        }
    }
}
