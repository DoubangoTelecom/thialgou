using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thialgou.lib.events
{
    public class MacroblockEventArgs : MyEventArgs
    {
        public enum MacroblockEventType_t
        {
            MacroblockEventType_Selected,
            MacroblockEventType_Hover
        }

        readonly MacroblockEventType_t m_Type;
        readonly Macroblock m_Macroblock;

        public MacroblockEventArgs(MacroblockEventType_t type, Macroblock macroblock)
            : base()
        {
            m_Type = type;
            m_Macroblock = macroblock;
        }

        public MacroblockEventType_t Type
        {
            get
            {
                return m_Type;
            }
        }

        public Macroblock Macroblock
        {
            get
            {
                return m_Macroblock;
            }
        }
    }
}
