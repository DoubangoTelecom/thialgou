/*
* Copyright (C) 2013 Mamadou DIOP
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
#ifdef THIALGOU_COMMON_H264_DEFS_INTERNAL_H_
#error "Must not include intenal header"
#endif
#define THIALGOU_COMMON_H264_DEFS_INTERNAL_H_

enum NalUnitType
{
  NAL_UNIT_UNSPECIFIED_0            =  0,
  NAL_UNIT_CODED_SLICE              =  1,
  NAL_UNIT_CODED_SLICE_DATAPART_A   =  2,
  NAL_UNIT_CODED_SLICE_DATAPART_B   =  3,
  NAL_UNIT_CODED_SLICE_DATAPART_C   =  4,
  NAL_UNIT_CODED_SLICE_IDR          =  5,
  NAL_UNIT_SEI                      =  6,
  NAL_UNIT_SPS                      =  7,
  NAL_UNIT_PPS                      =  8,
  NAL_UNIT_ACCESS_UNIT_DELIMITER    =  9,
  NAL_UNIT_END_OF_SEQUENCE          = 10,
  NAL_UNIT_END_OF_STREAM            = 11,
  NAL_UNIT_FILLER_DATA              = 12,
  NAL_UNIT_SPS_EXTENSION            = 13,
  NAL_UNIT_PREFIX										= 14,
  NAL_UNIT_SUBSET_SPS               = 15,
  NAL_UNIT_RESERVED_16              = 16,
  NAL_UNIT_RESERVED_17              = 17,
  NAL_UNIT_RESERVED_18              = 18,
  NAL_UNIT_AUX_CODED_SLICE          = 19,
  NAL_UNIT_CODED_SLICE_SCALABLE     = 20,
  NAL_UNIT_RESERVED_21              = 21,
  NAL_UNIT_RESERVED_22              = 22,
  NAL_UNIT_RESERVED_23              = 23
};

enum MbMode
{
  MODE_SKIP         = 0,
  MODE_16x16        = 1,
  MODE_16x8         = 2,
  MODE_8x16         = 3,
  MODE_8x8          = 4,
  MODE_8x8ref0      = 5,
  INTRA_4X4         = 6,
  MODE_PCM          = 25+6,
  INTRA_BL          = 36,
  NOT_AVAILABLE     = 99 //TMM
};

enum BlkMode
{
  BLK_8x8   = 8,
  BLK_8x4   = 9,
  BLK_4x8   = 10,
  BLK_4x4   = 11,
  BLK_SKIP  = 0
};

enum SliceType
{
  P_SLICE             = 0,
  B_SLICE             = 1,
  I_SLICE             = 2,
  SP_SLICE            = 3,
  SI_SLICE            = 4,
  NOT_SPECIFIED_SLICE = 5
};

enum Intra4x4PredMode
{
	Intra_4x4_Vertical,
    Intra_4x4_Horizontal,
    Intra_4x4_DC,
    Intra_4x4_Diagonal_Down_Left,
    Intra_4x4_Diagonal_Down_Right,
    Intra_4x4_Vertical_Right,
    Intra_4x4_Horizontal_Down,
    Intra_4x4_Vertical_Left,
    Intra_4x4_Horizontal_Up,
};

enum IntraChromaPredMode
{
	Intra_Chroma_DC,
	Intra_Chroma_Horizontal,
	Intra_Chroma_Vertical,
	Intra_Chroma_Plane,
};