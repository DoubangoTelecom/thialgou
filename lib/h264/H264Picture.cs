using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using org.doubango.thialgou.commonWRAP;
using org.doubango.thialgou.ioWRAP;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using thialgou.lib.model;

namespace thialgou.lib.h264
{
    public class H264Picture : Picture
    {
        readonly H264PicParamSet m_DefaultPicParamSet;
        readonly H264SeqParamSet m_DefaultSeqParamSet;
        readonly IDictionary<UInt32, H264Slice> m_Slices;
        readonly Bitmap[/*CommonEltMbDataType_t*/] m_Images;
        readonly String[/*CommonEltMbDataType_t*/] m_Md5YUV;
        readonly String[/*CommonEltMbDataType_t*/] m_Md5Y;
        readonly String[/*CommonEltMbDataType_t*/] m_Md5U;
        readonly String[/*CommonEltMbDataType_t*/] m_Md5V;
        UInt32 m_MbCount;
        UInt32 m_MbBitsCount;
        bool m_MbBitsCountComputed;

        public H264Picture(UInt32 layerId, UInt32 index, H264PicParamSet defaultPicParamSet, H264SeqParamSet defaultSeqParamSet)
            : base(layerId, index)
        {
            m_DefaultSeqParamSet = defaultSeqParamSet;
            m_DefaultPicParamSet = defaultPicParamSet;
            m_Slices = new Dictionary<UInt32, H264Slice>();
            m_Images = new Bitmap[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
            m_Md5YUV = new String[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
            m_Md5Y = new String[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
            m_Md5U = new String[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
            m_Md5V = new String[Utils.GetMaxEnumValue<CommonEltMbDataType_t>()];
        }

        ~H264Picture()
        {
            Dispose();
        }

        public override void Dispose()
        {
            foreach (Bitmap bmp in m_Images)
            {
                if (bmp != null)
                {
                    bmp.Dispose();
                }
            }
        }

        public override bool IsComplete
        {
            get
            {
                return (m_MbCount >= m_DefaultSeqParamSet.SizeInMbs);
            }
        }

        internal void AddSlice(H264Slice slice)
        {
            if (slice != null)
            {
                m_Slices[slice.Nalu.SliceId] = slice;
                m_MbCount += (UInt32)slice.Macroblocs.Count;
            }
        }

        public IDictionary<UInt32, H264Slice> Slices
        {
            get
            {
                return m_Slices;
            }
        }

        public UInt32 MbBitsCount
        {
            get
            {
                if (!m_MbBitsCountComputed)
                {
                    decimal? v = m_Slices.Values.Sum<H264Slice>((x) =>
                            {
                                return x.Macroblocs.Values.Sum<Macroblock>((y) => 
                                { 
                                    return y.Mb.BitsCount;
                                });
                            }
                        );
                    m_MbBitsCount = v.HasValue ? (UInt32)v.Value : 0;
                    m_MbBitsCountComputed = IsComplete;
                }
                return m_MbBitsCount;
            }
        }

        public override UInt32 Width
        {
            get
            {
                return m_DefaultSeqParamSet.Width;
            }
        }
        public override UInt32 Height
        {
            get
            {
                return m_DefaultSeqParamSet.Height;
            }
        }

        public override UInt32 NumberOfSlices
        {
            get
            {
                return (UInt32)m_Slices.Keys.Count;
            }
        }

        public override String EntropyCodingType
        {
            get
            {
                return m_DefaultPicParamSet.IsCAVLCEncoded ? "CAVLC" : "CABAC";
            }
        }

        public override Macroblock GetMacroblockByAddress(UInt32 sliceId, UInt32 mbAddress)
        {
            if (m_Slices.ContainsKey(sliceId))
            {
                if (m_Slices[sliceId].Macroblocs.ContainsKey(mbAddress))
                {
                    return m_Slices[sliceId].Macroblocs[mbAddress];
                }
            }
            return null;
        }

        public override Macroblock GetMacroblockByLocation(UInt32 x, UInt32 y)
        {
            foreach (var s in m_Slices.Values)
            {
                foreach (var m in s.Macroblocs.Values)
                {
                    if (m.Mb.X == x && m.Mb.Y == y)
                    {
                        return m;
                    }
                }
            }
            return null;
        }

        public override String GetMd5(CommonEltMbDataType_t eType, int iLine/*Y=0,U=1,V=2,YUV=3*/)
        {
            if (iLine == 0) return m_Md5Y[(int)eType];
            else if (iLine == 1) return m_Md5U[(int)eType];
            else if (iLine == 2) return m_Md5V[(int)eType];
            else return m_Md5YUV[(int)eType];
        }

        public override Bitmap GetImage(CommonEltMbDataType_t eType)
        {
            if (m_Images[(int)eType] == null && (m_DefaultSeqParamSet != null) && m_Slices.Count > 0)
            {

                // FIXME: YUV420, and single layer
                int nSizeLuma = (int)(m_DefaultSeqParamSet.Width * m_DefaultSeqParamSet.Height);
                int nOutputSize = (int)((m_DefaultSeqParamSet.Width * m_DefaultSeqParamSet.Height) << 2);
                int nSizeChroma = (nSizeLuma) >> 2;
                IntPtr pY = Marshal.AllocHGlobal(nSizeLuma);
                IntPtr pU = Marshal.AllocHGlobal(nSizeChroma);
                IntPtr pV = Marshal.AllocHGlobal(nSizeChroma);
                m_Images[(int)eType] = new Bitmap((int)m_DefaultSeqParamSet.Width, (int)m_DefaultSeqParamSet.Height, PixelFormat.Format32bppArgb);
                BitmapData bitmapData = m_Images[(int)eType].LockBits(new Rectangle(0, 0, m_Images[(int)eType].Width, m_Images[(int)eType].Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

#if SAVE_TO_FILE_YUV
                FileStream file = new FileStream("C:\\Projects\\thialgou\\trunk\\Final.yuv", FileMode.Create, FileAccess.Write);
                int written = 0;
#endif // SAVE_TO_FILE_YUV

                foreach (var kvp in m_Slices)
                {
                    foreach (var mb in kvp.Value.Macroblocs)
                    {
                        mb.Value.Mb.CopyAsU8(pY, m_DefaultSeqParamSet.Width, m_DefaultSeqParamSet.Height, CommonYuvLine_t.CommonYuvLine_Y, eType);
                        mb.Value.Mb.CopyAsU8(pU, m_DefaultSeqParamSet.Width >> 1, m_DefaultSeqParamSet.Height >> 1, CommonYuvLine_t.CommonYuvLine_U, eType);
                        mb.Value.Mb.CopyAsU8(pV, m_DefaultSeqParamSet.Width >> 1, m_DefaultSeqParamSet.Height >> 1, CommonYuvLine_t.CommonYuvLine_V, eType);
                    }
                }
#if SAVE_TO_FILE_YUV
                NativeMethods.WriteFile(file.Handle, pY, (int)nSizeLuma, out written, IntPtr.Zero);
                NativeMethods.WriteFile(file.Handle, pU, (int)nSizeChroma, out written, IntPtr.Zero);
                NativeMethods.WriteFile(file.Handle, pV, (int)nSizeChroma, out written, IntPtr.Zero);
#endif // SAVE_TO_FILE_YUV

                Boolean ret = VideoConverterYUV420ToRGB32.convert(
                                        pY,
                                        pU,
                                        pV,
                                        m_DefaultSeqParamSet.Width,
                                        m_DefaultSeqParamSet.Height,
                                        m_DefaultSeqParamSet.Width,
                                        bitmapData.Scan0,
                                        (uint)nOutputSize);
                if (ret)
                {
                    //int stride = (((int)m_SeqParamSet.Width * 32 + 31) & ~31) >> 3;
                    //m_Images[(int)eType] = new Bitmap((int)m_SeqParamSet.Width, (int)m_SeqParamSet.Height, stride, PixelFormat.Format32bppArgb, pRGB32);
#if SAVE_TO_FILE_BMP
                    m_Images[(int)eType].Save("C:\\Projects\\thialgou\\trunk\\Final.bmp");
#endif // SAVE_TO_FILE_BMP
                }

#if SAVE_TO_FILE_YUV
                file.Close();
#endif // SAVE_TO_FILE_YUV

                /* MD5 */
                if (eType == Utils.MD5_ELTMB_TYPE && m_Md5YUV[(int)eType] == null) // FIXME: Final only for now
                {
                    Md5 md5Ctx = new Md5();
                    m_Md5Y[(int)eType] = md5Ctx.compute(pY, /*(uint)Math.Min(((0 * 16) + (13 * 16 * 352)), nSizeLuma)*/(uint)nSizeLuma); // FIXME
                    m_Md5U[(int)eType] = md5Ctx.compute(pU, /*(uint)Math.Min(((0 * 8) + (1 * 8 * 176)), nSizeChroma)*/(uint)nSizeChroma); // FIXME
                    m_Md5V[(int)eType] = md5Ctx.compute(pV, (uint)nSizeChroma);

                    md5Ctx.init();
                    md5Ctx.update(pY, (uint)nSizeLuma);
                    md5Ctx.update(pU, (uint)nSizeChroma);
                    md5Ctx.update(pV, (uint)nSizeChroma);
                    m_Md5YUV[(int)eType] = md5Ctx.final();

                    md5Ctx.Dispose();
                }

                Marshal.FreeHGlobal(pY);
                Marshal.FreeHGlobal(pU);
                Marshal.FreeHGlobal(pV);

                m_Images[(int)eType].UnlockBits(bitmapData);
            }

            return m_Images[(int)eType];
        }
    }
}
