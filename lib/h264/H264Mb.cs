using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using org.doubango.thialgou.commonWRAP;
using System.Drawing;
using System.Diagnostics;

namespace thialgou.lib.h264
{
    public class H264Mb : EltMb
    {
        public static Int32[] BLK4x4ToBLK8x8 = { 0, 0, 1, 1, 0, 0, 1, 1, 2, 2, 3, 3, 2, 2, 3, 3 };

        readonly MbMode m_Mode;
	    readonly SliceType m_SliceType;
        readonly UInt32 m_MbType;
	    readonly Int32 m_QP;
	    readonly Int32 m_QPC;
	    readonly Int32 m_CBP;
	    readonly Intra4x4PredMode[] m_Intra4x4PredModes;
        readonly IntraChromaPredMode m_IntraChromaPredMode;
        readonly BlkMode[] m_BlkModes;
        readonly bool m_IsIntra16x16;
        readonly bool m_IsInter8x8;
        readonly bool m_IsIntra;
        readonly I16x16Type m_I16x16Type;
        readonly PType m_PType;
        readonly BType m_BType;
        readonly UInt32 m_NumMbPart;
        readonly Size m_MbPartSize;
        readonly CommonMvVector m_mvd_l0;
        readonly CommonMvVector m_mvd_l1;
        readonly CommonMvVector m_mvL0;
        readonly CommonMvVector m_mvL1;
        readonly Int32[/*4 PartNum(8x8 blk)*/] m_ref_idx_L0;
        readonly Int32[/*4 PartNum(8x8 blk)*/] m_ref_idx_L1;
	    readonly Int32[/*4 PartNum(8x8 blk)*/] m_POC0;
        readonly Int32[/*4 PartNum(8x8 blk)*/] m_POC1;
        
        readonly bool m_IsInCropWindow; // SVC
        readonly bool m_IsResidualPredictionFlag; // SVC
        readonly bool m_IsBLSkippedFlag; // SVC

        public enum I4x4Type
        {
            I_NxN = 0
        };

        /* Table 7-11 – Macroblock types for I slices */
        public enum I16x16Type
        {
            I_16x16_0_0_0 = 1,
            I_16x16_1_0_0 = 2,
            I_16x16_2_0_0 = 3,
            I_16x16_3_0_0 = 4,
            I_16x16_0_1_0 = 5,
            I_16x16_1_1_0 = 6,
            I_16x16_2_1_0 = 7,
            I_16x16_3_1_0 = 8,
            I_16x16_0_2_0 = 9,
            I_16x16_1_2_0 = 10,
            I_16x16_2_2_0 = 11,
            I_16x16_3_2_0 = 12,
            I_16x16_0_0_1 = 13,
            I_16x16_1_0_1 = 14,
            I_16x16_2_0_1 = 15,
            I_16x16_3_0_1 = 16,
            I_16x16_0_1_1 = 17,
            I_16x16_1_1_1 = 18,
            I_16x16_2_1_1 = 19,
            I_16x16_3_1_1 = 20,
            I_16x16_0_2_1 = 21,
            I_16x16_1_2_1 = 22,
            I_16x16_2_2_1 = 23,
            I_16x16_3_2_1 = 24,
        };

        /* Table 7-13 – Macroblock type values 0 to 4 for P and SP slices */
        public enum PType
        {
            P_L0_16x16,
	        P_L0_L0_16x8,
	        P_L0_L0_8x16,
	        P_8x8,
	        P_8x8ref0
        }

        /* Table 7-14 – Macroblock type values 0 to 22 for B slices */
        public enum BType
        {
            B_Direct_16x16,
            B_L0_16x16,
            B_L1_16x16,
            B_Bi_16x16,
            B_L0_L0_16x8,
            B_L0_L0_8x16,
            B_L1_L1_16x8,
            B_L1_L1_8x16,
            B_L0_L1_16x8,
            B_L0_L1_8x16,
            B_L1_L0_16x8,
            B_L1_L0_8x16,
            B_L0_Bi_16x8,
            B_L0_Bi_8x16,
            B_L1_Bi_16x8,
            B_L1_Bi_8x16,
            B_Bi_L0_16x8,
            B_Bi_L0_8x16,
            B_Bi_L1_16x8,
            B_Bi_L1_8x16,
            B_Bi_Bi_16x8,
            B_Bi_Bi_8x16,
            B_8x8,
        }

        // Table 7-17 – Sub-macroblock types in P macroblocks
        public enum PSubType
        {
            P_L0_8x8,
            P_L0_8x4,
            P_L0_4x8,
            P_L0_4x4
        }

        // Table 7-17 – Sub-macroblock types in P macroblocks
        public enum PSubMode
        {
            Pred_L0,
            /*Pred_L0,
            Pred_L0,
            Pred_L0*/
        }

        // Table 7-18 – Sub-macroblock types in B macroblocks
        public enum BSubType
        {
            B_Direct_8x8,
            B_L0_8x8,
            B_L1_8x8,
            B_Bi_8x8,
            B_L0_8x4,
            B_L0_4x8,
            B_L1_8x4,
            B_L1_4x8,
            B_Bi_8x4,
            B_Bi_4x8,
            B_L0_4x4,
            B_L1_4x4,
            B_Bi_4x4
        }

        // Table 7-18 – Sub-macroblock types in B macroblocks
        public enum BSubMode
        {
            Direct,
            /*Pred_L0,
            Pred_L1,
            BiPred,
            Pred_L0,
            Pred_L0,
            Pred_L1,
            Pred_L1,
            BiPred,
            BiPred,
            Pred_L0,
            Pred_L1,
            BiPred,*/
        }

        // PartNum for both P and SP slices
        // Usage: if (P_SLICE) { int partNum = PartNumP[mb_type]; }
        static UInt32[] PartNumP =
        {
            1,// P_L0_16x16
	        2,// P_L0_L0_16x8
	        2,// P_L0_L0_8x16
	        4,// P_8x8
	        4,// P_8x8ref0
        };
        // PartSize for both P and SP slices
        // Usage: if (P_SLICE) { int partWidth = PartSizeP[mb_type][0]; int partHeight = PartSizeP[mb_type][1]; }
        static UInt32[][] PartSizeP =
        {
            new UInt32[]{16,16},// P_L0_16x16
	        new UInt32[]{16,8},// P_L0_L0_16x8
	        new UInt32[]{8,16},// P_L0_L0_8x16
	        new UInt32[]{8,8},// P_8x8
	        new UInt32[]{8,8},// P_8x8ref0
        };

        // PartNum for both B and SB slices
        // Usage: if (B_SLICE) { int partNum = PartNumB[mb_type]; }
        static UInt32[] PartNumB =
        {
            1,// B_Direct_16x16,
            1,// B_L0_16x16,
            1,// B_L1_16x16,
            1,// B_Bi_16x16,
            2,// B_L0_L0_16x8,
            2,// B_L0_L0_8x16,
            2,// B_L1_L1_16x8,
            2,// B_L1_L1_8x16,
            2,// B_L0_L1_16x8,
            2,// B_L0_L1_8x16,
            2,// B_L1_L0_16x8,
            2,// B_L1_L0_8x16,
            2,// B_L0_Bi_16x8,
            2,// B_L0_Bi_8x16,
            2,// B_L1_Bi_16x8,
            2,// B_L1_Bi_8x16,
            2,// B_Bi_L0_16x8,
            2,// B_Bi_L0_8x16,
            2,// B_Bi_L1_16x8,
            2,// B_Bi_L1_8x16,
            2,// B_Bi_Bi_16x8,
            2,// B_Bi_Bi_8x16,
            4,// B_8x8,
        };
        // PartSize for both B and SB slices
        // Usage: if (B_SLICE) { int partWidth = PartSizeB[mb_type][0]; int partHeight = PartSizeB[mb_type][1]; }
        static UInt32[][] PartSizeB =
        {
            new UInt32[]{16,16},// B_Direct_16x16,
            new UInt32[]{16,16},// B_L0_16x16,
            new UInt32[]{16,16},// B_L1_16x16,
            new UInt32[]{16,16},// B_Bi_16x16,
            new UInt32[]{16,8},// B_L0_L0_16x8,
            new UInt32[]{8,16},// B_L0_L0_8x16,
            new UInt32[]{16,8},// B_L1_L1_16x8,
            new UInt32[]{8,16},// B_L1_L1_8x16,
            new UInt32[]{16,8},// B_L0_L1_16x8,
            new UInt32[]{8,16},// B_L0_L1_8x16,
            new UInt32[]{16,8},// B_L1_L0_16x8,
            new UInt32[]{8,16},// B_L1_L0_8x16,
            new UInt32[]{16,8},// B_L0_Bi_16x8,
            new UInt32[]{8,16},// B_L0_Bi_8x16,
            new UInt32[]{16,8},// B_L1_Bi_16x8,
            new UInt32[]{8,16},// B_L1_Bi_8x16,
            new UInt32[]{16,8},// B_Bi_L0_16x8,
            new UInt32[]{8,16},// B_Bi_L0_8x16,
            new UInt32[]{16,8},// B_Bi_L1_16x8,
            new UInt32[]{8,16},// B_Bi_L1_8x16,
            new UInt32[]{16,8},// B_Bi_Bi_16x8,
            new UInt32[]{8,16},// B_Bi_Bi_8x16,
            new UInt32[]{8,8},// B_8x8,
        };

        // SubPartNum for both P and SP slices
        // Usage: if (P_SLICE) { int subPartNum = SubPartNumP[sub_mb_type]; }
        static UInt32[] SubPartNumP =
        {
            1,// P_L0_8x8,
            2,// P_L0_8x4,
            2,// P_L0_4x8,
            4,// P_L0_4x4
        };

        // SubPartSize for both P and SP slices
        // Usage: if (P_SLICE) { int subPartWidth = SubPartSizeP[sub_mb_type][0]; int partHeight = SubPartSizeP[sub_mb_type][1]; }
        static UInt32[][] SubPartSizeP =
        {
            new UInt32[]{8,8},// P_L0_8x8
	        new UInt32[]{8,4},// P_L0_8x4
	        new UInt32[]{4,8},// P_L0_4x8
	        new UInt32[]{4,4},// P_L0_4x4
        };

        // SubPartNum for both B and SB slices
        // Usage: if (B_SLICE) { int subPartNum = SubPartNumB[sub_mb_type]; }
        static UInt32[] SubPartNumB =
        {
            4,// B_Direct_8x8,
            1,// B_L0_8x8,
            1,// B_L1_8x8,
            1,// B_Bi_8x8,
            2,// B_L0_8x4,
            2,// B_L0_4x8,
            2,// B_L1_8x4,
            2,// B_L1_4x8,
            2,// B_Bi_8x4,
            2,// B_Bi_4x8,
            4,// B_L0_4x4,
            4,// B_L1_4x4,
            4,// B_Bi_4x4
        };

        // SubPartSize for both B and SB slices
        // Usage: if (B_SLICE) { int subPartWidth = SubPartSizeB[sub_mb_type][0]; int partHeight = SubPartSizeB[sub_mb_type][1]; }
        static UInt32[][] SubPartSizeB =
        {
            new UInt32[]{4,4},// B_Direct_8x8,
            new UInt32[]{8,8},// B_L0_8x8,
            new UInt32[]{8,8},// B_L1_8x8,
            new UInt32[]{8,8},// B_Bi_8x8,
            new UInt32[]{8,4},// B_L0_8x4,
            new UInt32[]{4,8},// B_L0_4x8,
            new UInt32[]{8,4},// B_L1_8x4,
            new UInt32[]{4,8},// B_L1_4x8,
            new UInt32[]{8,4},// B_Bi_8x4,
            new UInt32[]{4,8},// B_Bi_4x8,
            new UInt32[]{4,4},// B_L0_4x4,
            new UInt32[]{4,4},// B_L1_4x4,
            new UInt32[]{4,4},// B_Bi_4x4
        };

        public H264Mb(CommonMbH264 eltMb)
            : base(eltMb)
        {
            m_Mode = eltMb.getMode();
            m_SliceType = eltMb.getSliceType();
            m_MbType = eltMb.getMbType();
            m_QP = eltMb.getQP();
            m_QPC = eltMb.getQPC();
            m_CBP = eltMb.getCBP();
            m_IsIntra16x16 = eltMb.isIntra16x16();
            m_IsInter8x8 = eltMb.isInter8x8();
            m_IsIntra = eltMb.isIntra();
            m_NumMbPart = 1;
            m_MbPartSize = new Size(16, 16);

            if (m_Mode == MbMode.INTRA_4X4)
            {
                m_Intra4x4PredModes = new Intra4x4PredMode[16];
                eltMb.getIntra4x4PredMode(m_Intra4x4PredModes);
            }
            else if (m_IsIntra16x16)
            {
                m_I16x16Type = (m_SliceType == SliceType.P_SLICE) ? (I16x16Type)(m_MbType - 5) : (I16x16Type)m_MbType;
            }
            else if (m_SliceType == SliceType.P_SLICE)
            {
                if (m_MbType < 5 || m_Mode == MbMode.MODE_SKIP)
                {
                    if (m_Mode == MbMode.MODE_SKIP)
                    {
                        m_NumMbPart = 1;
                        m_MbPartSize = new Size(16, 16);
                    }
                    else
                    {
                        m_PType = (PType)m_MbType;
                        if (m_MbType < PartNumP.Length)
                        {
                            m_NumMbPart = PartNumP[m_MbType];
                        }
                        if (m_MbType < PartSizeP.Length)
                        {
                            m_MbPartSize = new Size((int)PartSizeP[m_MbType][0], (int)PartSizeP[m_MbType][1]);
                        }
                    }
                }
                else if (m_MbType == 5)
                {
                    // I_4x4
                    Debug.Assert(false);
                    m_IsIntra = true;
                }
                else
                {
                    // I_16x16
                    m_I16x16Type = (I16x16Type)(m_MbType - 5);
                    m_IsIntra = true;
                    m_IsIntra16x16 = true;
                }
            }
            else if (m_SliceType == SliceType.B_SLICE)
            {
                m_BType = (BType)m_MbType;
                if (m_MbType < PartNumB.Length)
                {
                    m_NumMbPart = PartNumB[m_MbType];
                }
                if (m_MbType < PartSizeP.Length)
                {
                    m_MbPartSize = new Size((int)PartSizeB[m_MbType][0], (int)PartSizeB[m_MbType][1]);
                }
            }

            if (m_IsInter8x8)
            {
                m_BlkModes = new BlkMode[4];
                eltMb.getBlkModes(m_BlkModes);
            }

            if (m_IsIntra)
            {
                m_IntraChromaPredMode = eltMb.getIntraChromaPredMode();
            }
            else
            {
                m_ref_idx_L0 = new Int32[4];
                m_POC0 = new Int32[4];
                m_mvd_l0 = eltMb.getMvdL0();
                m_mvL0 = eltMb.getMvL0();
                eltMb.getRefIdxL0(m_ref_idx_L0);
                eltMb.getPOC0(m_POC0);
                if (SliceType == SliceType.B_SLICE)
                {
                    m_ref_idx_L1 = new Int32[4];
                    m_POC1 = new Int32[4];
                    m_mvd_l1 = eltMb.getMvdL1();
                    m_mvL1 = eltMb.getMvL1();
                    eltMb.getRefIdxL1(m_ref_idx_L1);
                    eltMb.getPOC1(m_POC1);
                }
            }

            /*** SVC ***/
            m_IsInCropWindow = eltMb.isInCropWindow();
            m_IsResidualPredictionFlag = eltMb.isResidualPredictionFlag();
            m_IsBLSkippedFlag = eltMb.isBLSkippedFlag();
        }

        public String TypeDescription
        {
            get
            {
                if (m_IsIntra16x16)
                {
                    return String.Format("{0} ({1})", m_I16x16Type, (int)MbType);
                }
                else if (m_Mode == MbMode.MODE_SKIP)
                {
                    return "P_Skip";
                }
                else if (m_Mode == MbMode.MODE_PCM)
                {
                    return "I_PCM";
                }
                else if (m_SliceType == SliceType.P_SLICE)
                {
                    return String.Format("{0} ({1})", m_PType, (int)MbType);
                }
                else if (m_SliceType == SliceType.B_SLICE)
                {
                    return String.Format("{0} ({1})", m_BType, (int)MbType);
                }
                else
                {
                    return String.Format("{0} ({1})", m_Mode, (int)MbType);
                }
            }
        }

        public MbMode Mode
        {
            get
            {
                return m_Mode;
            }
        }

        public SliceType SliceType
        {
            get
            {
                return m_SliceType;
            }
        }

        public UInt32 MbType
        {
            get
            {
                return m_MbType;
            }
        }

        public Int32 QP
        {
            get
            {
                return m_QP;
            }
        }

        public Int32 QPC
        {
            get
            {
                return m_QPC;
            }
        }

        public Int32 CBP
        {
            get
            {
                return m_CBP;
            }
        }

        public bool IsIntra16x16
        {
            get
            {
                return m_IsIntra16x16;
            }
        }

        public bool IsInter8x8
        {
            get
            {
                return m_IsInter8x8;
            }
        }

        public bool IsIntra
        {
            get
            {
                return m_IsIntra;
            }
        }

        public Intra4x4PredMode[] Intra4x4PredMode
        {
            get
            {
                return m_Intra4x4PredModes;
            }
        }

        public IntraChromaPredMode IntraChromaPredMode
        {
            get
            {
                return m_IntraChromaPredMode;
            }
        }

        public BlkMode[] BlkModes
        {
            get
            {
                return m_BlkModes;
            }
        }

        public UInt32 NumMbPart
        {
            get
            {
                return m_NumMbPart;
            }
        }

        public Size MbPartSize
        {
            get
            {
                return m_MbPartSize;
            }
        }

        public CommonMvVector mvd_l0
        {
            get
            {
                return m_mvd_l0;
            }
        }

        public CommonMvVector mvd_l1
        {
            get
            {
                return m_mvd_l1;
            }
        }

        public CommonMvVector mvL0
        {
            get
            {
                return m_mvL0;
            }
        }

        public CommonMvVector mvL1
        {
            get
            {
                return m_mvL1;
            }
        }

        public Int32[/*4 PartNum(8x8 blk)*/] ref_idx_L0
        {
            get
            {
                return m_ref_idx_L0;
            }
        }

        public Int32[/*4 PartNum(8x8 blk)*/] ref_idx_L1
        {
            get
            {
                return m_ref_idx_L1;
            }
        }

        public Int32[/*4 PartNum(8x8 blk)*/] POC0
        {
            get
            {
                return m_POC0;
            }
        }

        public Int32[/*4 PartNum(8x8 blk)*/] POC1
        {
            get
            {
                return m_POC1;
            }
        }

        // SVC
        public bool IsInCropWindow
        {
            get
            {
                return m_IsInCropWindow;
            }
        }

        public bool IsResidualPredictionFlag
        {
            get
            {
                return m_IsResidualPredictionFlag;
            }
        }

        public bool IsBLSkippedFlag
        {
            get
            {
                return m_IsBLSkippedFlag;
            }
        }
    }
}
