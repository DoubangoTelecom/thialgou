using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    public class EltHeader : Elt
    {
        readonly String m_FuncName;

        public EltHeader(CommonHeader header)
            : base(header)
        {
            m_FuncName = header.getFuncName();
        }

        public String FuncName
        {
            get
            {
                return m_FuncName;
            }
        }

        public override String Description
        {
            get
            {
                return String.Format("{0} ()", FuncName);
            }
        }
    }
}
