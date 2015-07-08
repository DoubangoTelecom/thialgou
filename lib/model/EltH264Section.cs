using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    internal class EltH264Section : Elt
    {
        readonly CommonH264Section m_Section;
        public EltH264Section(CommonH264Section section)
            : base(section)
        {
            m_Section = section;
        }

        internal CommonH264Section Section
        {
            get
            {
                return m_Section;
            }
        }
    }
}
