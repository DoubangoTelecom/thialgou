using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace thialgou.controls.adorners
{
    public class AdornerSelectMacroblock : Adorner
    {
        Rect m_MbRect = Rect.Empty;

        readonly SolidColorBrush m_Brush;
        readonly Pen m_Pen;

        public AdornerSelectMacroblock(UIElement adornedElement, bool hover)
            : base(adornedElement)
        {
            m_Brush = new SolidColorBrush(Colors.Transparent);
            m_Pen = new Pen(new SolidColorBrush(hover ? Colors.AliceBlue : Colors.Black), hover ? 0.7 : 1.0);
            m_Pen.DashStyle = hover ? DashStyles.Dot : DashStyles.Dash;
            IsHitTestVisible = hover;
        }

        public Rect MbRect
        {
            get
            {
                return m_MbRect;
            }
            set
            {
                m_MbRect = value;
            }
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (!MbRect.IsEmpty)
            {
                drawingContext.DrawRectangle(m_Brush, m_Pen, m_MbRect);
            }
        }
    }
}
