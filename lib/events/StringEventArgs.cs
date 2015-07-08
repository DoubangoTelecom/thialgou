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
    public class StringEventArgs : MyEventArgs
    {
        private readonly String value;

        public StringEventArgs(String value)
            : base()
        {
            this.value = value;
        }

        public String Value
        {
            get { return this.value; }
        }
    }
}
