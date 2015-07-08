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
using System.ComponentModel;
using System.Collections;
using thialgou.lib.events;
using thialgou.controls.items;
using System.Diagnostics;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenThumbnails.xaml
    /// </summary>
    public partial class ScreenThumbnails : ScreenBase
    {
        readonly MyObservableCollection<Picture> m_DataSource;
        readonly UInt32 m_LayerId;
        readonly String m_Title;
        public event EventHandler<PictureEventArgs> OnPictureEvent;

        public ScreenThumbnails(UInt32 layerId)
        {
            InitializeComponent();

            m_LayerId = layerId;
            m_Title = String.Format("Thumbnails [DQId = {0}]", m_LayerId);
            
            m_ListBox.SelectionChanged += (sender, e) =>
            {
                if (OnPictureEvent != null)
                {
                    EventHandlerTrigger.TriggerEvent<PictureEventArgs>(OnPictureEvent, this, new PictureEventArgs(PictureEventArgs.PictureEventType_t.PictureEventType_Selected, (m_ListBox.SelectedItem as Picture)));
                }
            };
            m_DataSource = new MyObservableCollection<Picture>(true);
            m_DataSource.onItemPropChanged += (sender, e) =>
            {
            };
            m_ListBox.ItemsSource = m_DataSource;            
        }

        public void AddPicture(Picture picture)
        {
            Debug.Assert(m_LayerId == picture.LayerId);
            m_DataSource.Add(picture);
            if (m_DataSource.Count == 1)
            {
                // Select First
                m_ListBox.SelectedIndex = 0;
                m_ListBox.Focus();
            }
        }

        void m_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        public override String BaseScreenTitle
        {
            get 
            {
                return m_Title; 
            }
        }

        public override int BaseScreenType
        {
            get 
            {
                return ScreenBase.SCREEN_TYPE_THUMBNAILS;
            }
        }

        class ThumnSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                Picture pict1 = x as Picture;
                Picture pict2 = y as Picture;
                if (pict1 != null && pict2 != null)
                {
                    return pict1.CompareTo(pict2);
                }
                return 0;
            }
        }
    }
}
