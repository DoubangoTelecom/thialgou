using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    public class EltError : Elt
    {
        readonly String m_Description;

        public EltError(CommonError error)
            : base(error)
        {
            m_Description = error.getDescription();
        }

        public override String Description
        {
            get
            {
                return String.Format("/!\\ {0}", m_Description);
            }
        }
    }
}
