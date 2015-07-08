using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib.model
{
    public class EltControl : Elt
    {
        readonly String m_Name;
        readonly String m_Expression;
        readonly Double? m_Value;

        public EltControl(CommonControl control)
            : base(control)
        {
            m_Name = control.getName();
            m_Expression = control.getExpression();
            m_Value = control.getValue();
        }

        public String Name
        {
            get
            {
                return m_Name;
            }
        }

        public String Expression
        {
            get
            {
                return m_Expression;
            }
        }

        public Double? Value
        {
            get
            {
                return m_Value;
            }
        }

        public Boolean IsExpressionValueTrue
        {
            get
            {
                return (Value.HasValue && Value.Value != 0);
            }
        }

        public override String Description
        {
            get
            {
                return String.Format("{0}({1}) /* {2} */", Name, Expression, IsExpressionValueTrue);
            }
        }
    }

}
