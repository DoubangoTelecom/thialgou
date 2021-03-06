/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.9
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace org.doubango.thialgou.ioWRAP {

using System;
using System.Runtime.InteropServices;

public class VideoConverter : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal VideoConverter(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(VideoConverter obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~VideoConverter() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          ioWRAPPINVOKE.delete_VideoConverter(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public VideoConverter(VideoChroma_t eChromaInput, VideoChroma_t eChromaOutput) : this(ioWRAPPINVOKE.new_VideoConverter((int)eChromaInput, (int)eChromaOutput), true) {
  }

}

}
