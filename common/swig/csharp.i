/* File : csharp.i 
* http://www.swig.org/Doc1.3/CSharp.html
*/
%include <arrays_csharp.i>

CSHARP_ARRAYS(Intra4x4PredModeFake, Intra4x4PredMode)
%typemap(imtype, inattributes="[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0, ArraySubType=UnmanagedType.I8)]") Intra4x4PredModeFake INPUT[] "Intra4x4PredMode[]"
%apply Intra4x4PredModeFake INPUT[]  { Intra4x4PredModeFake *modes }

CSHARP_ARRAYS(BlkModeFake, BlkMode)
%typemap(imtype, inattributes="[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0, ArraySubType=UnmanagedType.I8)]") BlkModeFake INPUT[] "BlkMode[]"
%apply BlkModeFake INPUT[]  { BlkModeFake *modes }

CSHARP_ARRAYS(CommonMv, CommonMv)
%typemap(imtype, inattributes="[In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0, ArraySubType=UnmanagedType.Struct)]") CommonMv INPUT[] "CommonMv[]"
%apply CommonMv INPUT[]  { CommonMv *values }

CSHARP_ARRAYS(int32_t, int32_t)
%typemap(imtype, inattributes="[In, MarshalAs(UnmanagedType.ByValArray, SizeParamIndex=0, ArraySubType=UnmanagedType.I8)]") int32_t INPUT[] "int[]"
%apply int INPUT[]  { int32_t *values }

%define %cs_marshal_array(TYPE, CSTYPE)
%typemap(ctype) TYPE[] "void*"
%typemap(imtype,
inattributes="[MarshalAs(UnmanagedType.LPArray)]") TYPE[] "CSTYPE[]"
%typemap(cstype) TYPE[] "CSTYPE[]"
%typemap(in) TYPE[] %{ $1 = (TYPE*)$input; %}
%typemap(csin) TYPE[] "$csinput"
%enddef

// Mapping void* as IntPtr
%typemap(ctype)  void * "void *"
%typemap(imtype) void * "IntPtr"
%typemap(cstype) void * "IntPtr"
%typemap(csin)   void * "$csinput"
%typemap(in)     void * %{ $1 = $input; %}
%typemap(out)    void * %{ $result = $1; %}
%typemap(csout)  void * { return $imcall; }
%typemap(csdirectorin) void * "$iminput"

%include common.i