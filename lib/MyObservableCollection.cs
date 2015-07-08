/*
* Copyright (C) 2013 Doubango Telecom <http://www.doubango.org>
* License: GPLv3
* This file is part of Open Source Thialgou project <http://code.google.com/p/thialgou/>
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using thialgou.lib.events;
using System.Windows.Threading;

namespace thialgou.lib
{
    public class MyObservableCollection<T> : ObservableCollection<T>
        where T : IComparable<T>, INotifyPropertyChanged
    {
        private readonly bool trackItemsPropChanges;
        public event EventHandler<StringEventArgs> onItemPropChanged;
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        public MyObservableCollection()
            : this(false)
        {

        }

        public MyObservableCollection(bool trackItemsPropChanges)
            : base()
        {
            this.trackItemsPropChanges = trackItemsPropChanges;
        }

        public MyObservableCollection(IEnumerable<T> collection, bool trackItemsPropChanges)
            : base(collection)
        {
            this.trackItemsPropChanges = trackItemsPropChanges;
        }

        public void RemoveRange(IEnumerable<T> collection)
        {
            foreach (var i in collection)
            {
                this.Items.Remove(i);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Replace(T item)
        {
            this.ReplaceRange(new T[] { item });
        }

        public void ReplaceRange(IEnumerable<T> collection)
        {
            this.Items.Clear();

            foreach (var i in collection)
            {
                this.Items.Add(i);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (var i in collection)
            {
                this.Items.Add(i);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public List<T> FindAll(Predicate<T> matcher)
        {
            List<T> items = this.Items as List<T>;
            return items.FindAll(matcher);
        }

        public int RemoveAll(Predicate<T> match)
        {
            List<T> items = this.Items as List<T>;
            return items.RemoveAll(match);
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.trackItemsPropChanges)
            {
                // http://stackoverflow.com/questions/2104614/updating-an-observablecollection-in-a-separate-thread
                  var eh = CollectionChanged;
                  if (eh != null)
                  {
                      Dispatcher dispatcher = (from NotifyCollectionChangedEventHandler nh in eh.GetInvocationList()
                                               let dpo = nh.Target as DispatcherObject
                                               where dpo != null
                                               select dpo.Dispatcher).FirstOrDefault();

                      if (dispatcher != null && dispatcher.CheckAccess() == false)
                      {
                          dispatcher.Invoke(DispatcherPriority.DataBind, (Action)(() => OnCollectionChanged(e)));
                      }
                      else
                      {
                          foreach (NotifyCollectionChangedEventHandler nh in eh.GetInvocationList())
                              nh.Invoke(this, e);
                      }
                  }
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        {
                            foreach (T item in e.NewItems)
                            {
                                item.PropertyChanged += this.item_PropertyChanged;
                            }
                            break;
                        }

                    case NotifyCollectionChangedAction.Remove:
                        {
                            foreach (T item in e.OldItems)
                            {
                                item.PropertyChanged -= this.item_PropertyChanged;
                            }
                            break;
                        }

                    case NotifyCollectionChangedAction.Reset:
                        {
                            foreach (T item in this.Items)
                            {
                                item.PropertyChanged += this.item_PropertyChanged;
                            }
                            break;
                        }
                }
            }

            base.OnCollectionChanged(e);
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EventHandlerTrigger.TriggerEvent<StringEventArgs>(this.onItemPropChanged, this,
                new StringEventArgs(e.PropertyName));
        }
    }
}
