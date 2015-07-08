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
using System.ComponentModel;

namespace thialgou.controls.items
{
    /// <summary>
    /// Interaction logic for ItemHeader.xaml
    /// </summary>
    public partial class ItemHeader : ItemBase<Elt>, INotifyPropertyChanged
    {
        Elt m_Value;

        public ItemHeader()
        {
            InitializeComponent();

            this.ValueLoaded += ItemHdrItem_ValueLoaded;
        }

        void ItemHdrItem_ValueLoaded(object sender, EventArgs e)
        {
            m_Value = Value;

            m_LabelDescription.Content = m_Value.Description;

            switch (m_Value.Type)
            {
                case Elt.EltType_t.EltType_Control:
                    {
                        this.IsEnabled = (m_Value as EltControl).IsExpressionValueTrue;
                        break;
                    }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
