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
using thialgou.lib.model;
using thialgou.lib;
using org.doubango.thialgou.commonWRAP;

namespace thialgou.controls.screens
{
    /// <summary>
    /// Interaction logic for ScreenHeader.xaml
    /// </summary>
    public partial class ScreenHeader : ScreenList<Elt>
    {
        Elt[] m_Elts;
        String m_Title = "Unknown";

        public ScreenHeader(Elt[] elts)
        {
            InitializeComponent();

            m_LabelItemDescription.Content = String.Empty;
            m_TreeView.Loaded += new RoutedEventHandler(treeView_Loaded);
            DataSource = new MyObservableCollection<Elt>(true);
            DataView.Filter = delegate(object @event)
            {
                Elt elt = @event as Elt;
                if (elt == null)
                {
                    return false;
                }
                // Do not display MB info in the slice header.
                return elt.Type != Elt.EltType_t.EltType_Mb;
            };
            Elts = elts;
        }

        public Elt[] Elts
        {
            private get
            {
                return m_Elts;
            }
            set
            {
                m_Elts = value;
                DataSource.Clear();
                if (m_Elts != null)
                {
                    foreach (Elt elt in m_Elts)
                    {
                        if (elt.Type == Elt.EltType_t.EltType_Nalu)
                        {
                            switch ((elt as EltH264Nalu).NaluType)
                            {
                                case NalUnitType.NAL_UNIT_SPS:
                                case NalUnitType.NAL_UNIT_SUBSET_SPS:
                                    {
                                        m_Title = "SPS";
                                        break;
                                    }
                                case NalUnitType.NAL_UNIT_PPS:
                                    {
                                        m_Title = "PPS";
                                        break;
                                    }
                                case NalUnitType.NAL_UNIT_CODED_SLICE:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_DATAPART_A:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_DATAPART_B:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_DATAPART_C:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_IDR:
                                case NalUnitType.NAL_UNIT_CODED_SLICE_SCALABLE:
                                    {
                                        m_Title = "Slice";
                                        break;
                                    }
                            }
                        }
                        // We want element at zero to be displayed. This ie why elt.Elements is not used.
                        // The Slice contains Macroblocks which should not be displayed.
                        DataSource.Add(m_Title=="Slice" ? elt.CloneAndFilter(thialgou.lib.model.Elt.EltType_t.EltType_Mb) : elt);
                    }
                }
                //DataView.Refresh();
            }
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
                return ScreenBase.SCREEN_TYPE_HEADER;
            }
        }

        public override ItemsControl ListControl
        {
            get { return m_TreeView; }
        }

        void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Elt item = (m_TreeView.SelectedItem as Elt);
            if (item != null)
            {
                String description = item.Description;
                if (!String.IsNullOrEmpty(item.BitsStart))
                {
                    description += String.Format("\nBits start: {0}", item.BitsStart);
                }
                if (!String.IsNullOrEmpty(item.BitsVal))
                {
                    description += String.Format("\nBits val: {0}", item.BitsVal);
                }
                m_LabelItemDescription.Content = description;
            }
            else
            {
                m_LabelItemDescription.Content = String.Empty;
            }
        }

        void treeView_Loaded(object sender, RoutedEventArgs e)
        {
            if (m_TreeView.Items.Count > 0)
            {
                // Expand first item (most likely the function name)
                TreeViewItem treeItem = m_TreeView.ItemContainerGenerator.ContainerFromItem(m_TreeView.Items[0]) as TreeViewItem;
                if (treeItem != null)
                {
                    treeItem.IsExpanded = true;
                }
            }
        }
    }
}
