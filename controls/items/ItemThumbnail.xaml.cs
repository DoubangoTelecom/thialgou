using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using thialgou.lib;
using System.Diagnostics;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.controls.items
{
    /// <summary>
    /// Interaction logic for ItemThumbnail.xaml
    /// </summary>
    public partial class ItemThumbnail : ItemBase<Picture>
    {
        private Picture m_Picture;

        public ItemThumbnail()
        {
            InitializeComponent();

            this.ValueLoaded += ItemHdrItem_ValueLoaded;
        }

        void ItemHdrItem_ValueLoaded(object sender, EventArgs e)
        {
            if ((m_Picture = this.Value) == null)
            {
                Debug.Assert(false);
                return;
            }

            System.Drawing.Bitmap bitmap = m_Picture.GetImage(CommonEltMbDataType_t.CommonEltMbDataType_Final);
            if (bitmap == null)
            {
                // Displace error image
                Debug.Assert(false);
                return;
            }
            else
            {
                m_image.Source = Utils.CreateBitmapSourceFromBitmap(bitmap);
            }

            m_labelIndex.Content = m_Picture.Index;
            m_labelLayerId.Content = String.Format("DQId = {0}", m_Picture.LayerId);
        }

        public Picture Picture
        {
            get
            {
                return m_Picture;
            }
        }
    }
}
