using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.lib
{
    public static class Utils
    {
        public const CommonEltMbDataType_t MD5_ELTMB_TYPE = CommonEltMbDataType_t.CommonEltMbDataType_Final;

        public static int GetMaxEnumValue<T>() where T : struct, IConvertible
        {
            return Enum.GetValues(typeof(T)).Cast<Int32>().Max() + 1 /* Zero-Based index */;
        }

        public static int Clamp(int nMin, int nVal, int nMax)
        {
            return ((nVal) > (nMax)) ? (nMax) : (((nVal) < (nMin)) ? (nMin) : (nVal));
        }

        public static double Clamp(double nMin, double nVal, double nMax)
        {
            return ((nVal) > (nMax)) ? (nMax) : (((nVal) < (nMin)) ? (nMin) : (nVal));
        }

        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource source;

            try
            {
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                // Must be destroyed according to http://msdn.microsoft.com/en-us/library/1dz311e4.aspx
                NativeMethods.DeleteObject(hBitmap);
            }

            return source;
        }

        public static String GetIntra4x4PredModeName(Intra4x4PredMode mode)
        {
            switch (mode)
            {
                case Intra4x4PredMode.Intra_4x4_Vertical: return "Vert";
                case Intra4x4PredMode.Intra_4x4_Horizontal: return "Horiz";
                case Intra4x4PredMode.Intra_4x4_DC: return "DC";
                case Intra4x4PredMode.Intra_4x4_Diagonal_Down_Left: return "Down_Left";
                case Intra4x4PredMode.Intra_4x4_Diagonal_Down_Right: return "Diag_Down_Right";
                case Intra4x4PredMode.Intra_4x4_Vertical_Right: return "Vert_Right";
                case Intra4x4PredMode.Intra_4x4_Horizontal_Down: return "Horiz_Down";
                case Intra4x4PredMode.Intra_4x4_Vertical_Left: return "Vert_Left";
                case Intra4x4PredMode.Intra_4x4_Horizontal_Up: return "Horiz_Up";
                default: return "Unknown";
            }
        }
    }
}
