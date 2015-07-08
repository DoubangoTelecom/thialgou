using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace thialgou.lib
{
    public static class SwigHelper
    {
        public static T CastTo<T>(object o, bool cMemoryOwn)
        {
            System.Reflection.MethodInfo getCPtr = o.GetType().GetMethod("getCPtr", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return getCPtr == null ? default(T) : (T)System.Activator.CreateInstance
            (
                typeof(T),
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                null,
                new object[] { ((System.Runtime.InteropServices.HandleRef)getCPtr.Invoke(null, new object[] { o })).Handle, cMemoryOwn },
                null
            );
        }
        public static T CastTo<T>(object o)
        {
            return CastTo<T>(o, false);
        }
    }
}
