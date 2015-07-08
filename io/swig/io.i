%module(directors="1") ioWRAP
%include "typemaps.i"
%include <stdint.i>

%feature("director") h264HdrEventParsing;

%nodefaultctor;
%include "../../jsvm/JSVM/H264Extension/include/H264AVCCommonLib/IoEvent.h"
%include "cpp/h264/h264_decoder.h"
%include "cpp/common/io_converter_video.h"
%include "cpp/common/io_md5.h"
%clearnodefaultctor;


%{
#include "H264AVCCommonLib/IoEvent.h"
#include "cpp/common/io_converter_video.h"
#include "cpp/common/io_md5.h"
#include "cpp/h264/h264_decoder.h"
%}


