using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    public class EltFunction : Elt
    {
        readonly String m_Name;

        public EltFunction(CommonFunction function)
            : base(function)
        {
            m_Name = function.getFuncName();
        }

        public String Name
        {
            get
            {
                return m_Name;
            }
        }

        public override String Description
        {
            get
            {
                return String.Format("{0}()", Name);
            }
        }
    }
}
