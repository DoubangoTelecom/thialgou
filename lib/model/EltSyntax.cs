using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    public class EltSyntax : Elt
    {
        readonly String m_Name;
        readonly String m_Descriptor;
        readonly Double m_Value;

        public EltSyntax(CommonSyntax syntax)
            : base(syntax)
        {
            m_Name = syntax.getName();
            m_Descriptor = syntax.getDescriptor();
            m_Value = syntax.getValue();
        }

        public String Name
        {
            get
            {
                return m_Name;
            }
        }

        public String Descriptor
        {
            get
            {
                return m_Descriptor;
            }
        }

        public Double Value
        {
            get
            {
                return m_Value;
            }
        }

        public override String Description
        {
            get
            {
                return String.Format("{0} = {1}", Value, Name);
            }
        }
    }
}
