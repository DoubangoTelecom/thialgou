using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.doubango.thialgou.commonWRAP;
using thialgou.lib.events;
using thialgou.lib.model;

namespace thialgou.lib.h264
{
    public class MainInstH264 : CommonMainH264
    {
        static MainInstH264 g_MainInstH264 = null;
        public event EventHandler<EltEventArgs> OnEvent;
        System.Windows.Threading.Dispatcher m_Dispatcher;

        protected MainInstH264(System.Windows.Threading.Dispatcher dispatcher)
        {
            m_Dispatcher = dispatcher;
        }

        public static MainInstH264 GetInst(System.Windows.Threading.Dispatcher dispatcher)
        {
            if (g_MainInstH264 == null)
            {
                g_MainInstH264 = new MainInstH264(dispatcher);
            }
            return g_MainInstH264;
        }

        public static MainInstH264 GetInst()
        {
            return MainInstH264.GetInst(null);
        }

        public override int OnElt(CommonElt elt)
        {
            if (OnEvent != null)
            {
                // Because the event is delivered asynchronously we must copy the elt HERE before the native memory (C++) is destroyed.
                Elt element = Elt.New(elt);
                EventHandlerTrigger.TriggerEvent<EltEventArgs>(OnEvent, this, new EltEventArgs(element));
            }
            return 0;
        }

        public System.Windows.Threading.Dispatcher Dispatcher
        {
            get
            {
                return m_Dispatcher;
            }
        }
    }
}
