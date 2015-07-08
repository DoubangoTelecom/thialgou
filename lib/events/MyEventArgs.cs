/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thialgou.lib.events
{
    public class MyEventArgs : EventArgs
    {
        private readonly IDictionary<String, Object> extras;

        public MyEventArgs()
            : base()
        {
            this.extras = new Dictionary<String, Object>();
        }

        public Object GetExtra(String key)
        {
            if (this.extras.ContainsKey(key))
            {
                return this.extras[key];
            }
            return null;
        }

        public String GetExtraString(String key)
        {
            Object o = GetExtra(key);
            if (o != null)
            {
                return (o as String);
            }
            return null;
        }

        public Int32? GetExtraInt32(String key)
        {
            Object o = GetExtra(key);
            if (o != null)
            {
                return (o as Int32?);
            }
            return null;
        }

        public MyEventArgs AddExtra(String key, Object value)
        {
            if (!this.extras.ContainsKey(key))
            {
                this.extras.Add(key, value);
            }
            return this;
        }
    }
}
