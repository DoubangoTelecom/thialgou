%module(directors="1") commonWRAP
%include <typemaps.i>
%include <stdint.i>
%include <stl.i>

%feature("director") CommonMain;
%feature("director") CommonMainH264;

%nodefaultctor;
%include "cpp/common_main.h"
%include "cpp/common_main_h264.h"
%include "cpp/common_h264_defs_internal.h"
%clearnodefaultctor;

%{
#include "../cpp/common_main.h"
#include "../cpp/common_main_h264.h"
%}

%feature("director") CommonMain;
%feature("director") CommonMainH264;

namespace std {
   %template(CommonEltVector) vector<CommonElt>;
   %template(CommonEltVectorPtr) vector<CommonElt*>;
   %template(CommonMbVectorPtr) std::vector<CommonMb *>;
   %template(CommonMvVector) std::vector<CommonMv>;
};







