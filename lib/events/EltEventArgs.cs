/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;

namespace thialgou.lib.events
{
    public class EltEventArgs : MyEventArgs
    {
        private readonly Elt m_Elt;

        public EltEventArgs(Elt elt)
            : base()
        {
            m_Elt = elt;
        }

        public Elt Elt
        {
            get
            {
                return m_Elt;
            }
        }
    }
}
