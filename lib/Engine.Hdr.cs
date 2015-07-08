using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using thialgou.lib.model;
using thialgou.lib.events;

namespace thialgou.lib
{
    partial class _Engine
    {
        private readonly MyObservableCollection<Hdr> m_Hdrs;
        private Hdr m_HdrCurrent;
        private List<HdrItem> m_ItemCurrents = new List<HdrItem>();

        public MyObservableCollection<Hdr> Hdrs
        {
            get
            {
                return m_Hdrs;
            }
        }

        private void HdrParser_onEvent(object sender, HdrEventParseArgs e)
        {
            switch (e.ParseType)
            {
                case HdrEventParseType.BeginHdr:
                    {
                        m_HdrCurrent = new Hdr(e.HdrType, e.GetExtraString(HdrEventParseArgs.EXTRA_FUNC_NAME_STRING));
                        break;
                    }
                case HdrEventParseType.EndHdr:
                case HdrEventParseType.ErrorHdr:
                    {
                        if (m_HdrCurrent == null)
                        {
                            LOG.Error("ErrorHdr/EndHdr: Unexpected code called");
                            return;
                        }
                        m_HdrCurrent.Error = (e.ParseType == HdrEventParseType.ErrorHdr);
                        m_Hdrs.Add(m_HdrCurrent);
                        m_HdrCurrent = null;
                        m_ItemCurrents.Clear();
                        break;
                    }
                case HdrEventParseType.BeginFnc:
                    {
                        if (m_HdrCurrent == null)
                        {
                            LOG.Error("BeginFnc: Unexpected code called");
                            return;
                        }
                        break;
                    }
                case HdrEventParseType.EndFunc:
                    {
                        if (m_HdrCurrent == null)
                        {
                            LOG.Error("EndFunc: Unexpected code called");
                            return;
                        }
                        break;
                    }
                case HdrEventParseType.SyntaxElt:
                    {
                        if (m_HdrCurrent == null)
                        {
                            LOG.Error("SyntaxElt: Unexpected code called");
                            return;
                        }
                        HdrItem item = new HdrItemSyntaxElt(
                            e.GetExtraString(HdrEventParseArgs.EXTRA_SYNTAX_NAME_STRING), 
                            e.GetExtraString(HdrEventParseArgs.EXTRA_SYNTAX_DESCRIPTOR_STRING),
                            e.GetExtraInt32(HdrEventParseArgs.EXTRA_SYNTAX_VALUE_INT32));

                        if (m_ItemCurrents.Count != 0)
                        {
                            m_ItemCurrents[m_ItemCurrents.Count - 1].Items.Add(item);
                        }
                        else
                        {
                            m_HdrCurrent.Items.Add(item);
                        }
                        break;
                    }

                case HdrEventParseType.BeginCtrl:
                    {
                        if (m_HdrCurrent == null)
                        {
                            LOG.Error("BeginCtrl: Unexpected code called");
                            return;
                        }
                        m_ItemCurrents.Add(new HdrItemCtrl.HdrItemCtrlBegin(
                            e.GetExtraString(HdrEventParseArgs.EXTRA_CTRL_NAME_STRING),
                            e.GetExtraString(HdrEventParseArgs.EXTRA_CTRL_EXPRESSION_STRING),
                            e.GetExtraInt32(HdrEventParseArgs.EXTRA_CTRL_VALUE_INT32)
                            ));
                        break;
                    }

                case HdrEventParseType.EndCtrl:
                    {
                        if (m_HdrCurrent == null || m_ItemCurrents.Count == 0)
                        {
                            LOG.Error("EndCtrl: Unexpected code called");
                            return;
                        }
                        
                        /* mItemCurrents[mItemCurrents.Count - 1].Items.Add(new HdrItemCtrl.HdrItemCtrlEnd(
                            e.GetExtraString(HdrEventParseArgs.EXTRA_CTRL_NAME_STRING)
                            ));
                         */
                        m_HdrCurrent.Items.Add(m_ItemCurrents[m_ItemCurrents.Count - 1]);
                        m_ItemCurrents.RemoveAt(m_ItemCurrents.Count - 1);
                        break;
                    }
            }
        }
    }
}
