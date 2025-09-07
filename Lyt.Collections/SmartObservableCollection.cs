// Extended ObservableCollection MIT License which is: 
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Lyt.Collections; 

/// <summary>
/// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
/// implementing INotifyCollectionChanged to notify listeners
/// when items get added, removed or the whole list is refreshed.
/// </summary>
[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class SmartObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
{
    private SimpleMonitor? monitor; // Lazily allocated only when a subclass calls BlockReentrancy() or during serialization. Do not rename (binary serialization)

    [NonSerialized]
    private int blockReentrancyCount;

    [NonSerialized]
    private int pendingCountNofications;

    [NonSerialized]
    private int pendingIndexerNofications;

    [NonSerialized]
    private int pendingChangedNofications;

    /// <summary> Initializes a new instance of ObservableCollection that is empty and has default initial capacity.</summary>
    public SmartObservableCollection() { }

    /// <summary>
    /// Initializes a new instance of the ObservableCollection class that contains
    /// elements copied from the specified collection and has sufficient capacity
    /// to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <remarks>
    /// The elements are copied onto the ObservableCollection in the
    /// same order they are read by the enumerator of the collection.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> collection is a null reference </exception>
    public SmartObservableCollection(IEnumerable<T> collection) : base([.. collection ?? throw new ArgumentNullException(nameof(collection))])
    {
    }

    /// <summary> 
    /// Initializes a new instance of the ObservableCollection class that contains elements copied from the specified list
    /// </summary>
    /// <param name="list">The list whose elements are copied to the new list.</param>
    /// <remarks>
    /// The elements are copied onto the ObservableCollection in the same order they are read by the enumerator of the list.
    /// </remarks>
    /// <exception cref="ArgumentNullException"> list is a null reference </exception>
    public SmartObservableCollection(List<T> list) : base([.. list ?? throw new ArgumentNullException(nameof(list))]) { }

    /// <summary> Returns true if notifications are enabled. </summary>
    /// <remarks> Defaults to true to match ObservableCollection behaviour. </remarks>
    public bool AreNotificationsEnabled { get; private set; } = true;

    /// <summary> The collection will not raise events notifications until ResumeNotifications is invoked. </summary>
    public void SuspendNotifications() => this.AreNotificationsEnabled = false;

    /// <summary> Resumes raising events notifications when collection is changed or reset. </summary>
    /// <remarks> Raises events if any were attempted to be raised while notifications were disabled.</remarks>
    public void ResumeNotifications()
    {
        this.AreNotificationsEnabled = true;

        if (this.pendingCountNofications > 0)
        {
            this.pendingCountNofications = 0; 
            this.OnCountPropertyChanged();
        }

        if (this.pendingIndexerNofications > 0)
        {
            this.pendingIndexerNofications = 0;
            this.OnIndexerPropertyChanged();
        }

        if (this.pendingChangedNofications > 0)
        {
            this.pendingChangedNofications = 0;
            this.OnCollectionReset();
        }
    }

    /// <summary> Move item at oldIndex to newIndex. </summary>
    public void Move(int oldIndex, int newIndex) => this.MoveItem(oldIndex, newIndex);

#pragma warning disable CA1070 // Do not declare event fields as virtual

    /// <summary> Occurs when the collection changes, either by adding or removing an item. </summary>
    [field: NonSerialized]
    public virtual event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary> PropertyChanged event (per <see cref="INotifyPropertyChanged" />). </summary>
    [field: NonSerialized]
    protected virtual event PropertyChangedEventHandler? PropertyChanged;

#pragma warning restore CA1070 // Do not declare event fields as virtual

    /// <summary> PropertyChanged event (per <see cref="INotifyPropertyChanged" />). </summary>
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add => PropertyChanged += value;
        remove => PropertyChanged -= value;
    }

    /// <summary>
    /// Called by base class Collection&lt;T&gt; when the list is being cleared;
    /// If Notifications are Enabled, raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void ClearItems()
    {
        this.CheckReentrancy();
        base.ClearItems();
        this.OnCountPropertyChanged();
        this.OnIndexerPropertyChanged();
        this.OnCollectionReset();
    }

    /// <summary>
    /// Called by base class Collection&lt;T&gt; when an item is removed from list;
    /// If Notifications are Enabled, raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void RemoveItem(int index)
    {
        this.CheckReentrancy();
        T removedItem = this[index];

        base.RemoveItem(index);

        this.OnCountPropertyChanged();
        this.OnIndexerPropertyChanged();
        this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
    }

    /// <summary>
    /// Called by base class Collection&lt;T&gt; when an item is added to list;
    /// If Notifications are Enabled, raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void InsertItem(int index, T item)
    {
        this.CheckReentrancy();
        base.InsertItem(index, item);

        this.OnCountPropertyChanged();
        this.OnIndexerPropertyChanged();
        this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
    }

    /// <summary>
    /// Called by base class Collection&lt;T&gt; when an item is set in list;
    /// If Notifications are Enabled, raises a CollectionChanged event to any listeners.
    /// </summary>
    protected override void SetItem(int index, T item)
    {
        this.CheckReentrancy();
        T originalItem = this[index];
        base.SetItem(index, item);

        this.OnIndexerPropertyChanged();
        this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
    }

    /// <summary>
    /// Called by base class ObservableCollection&lt;T&gt; when an item is to be moved within the list;
    /// If Notifications are Enabled, raises a CollectionChanged event to any listeners.
    /// </summary>
    protected virtual void MoveItem(int oldIndex, int newIndex)
    {
        this.CheckReentrancy();

        T removedItem = this[oldIndex];

        base.RemoveItem(oldIndex);
        base.InsertItem(newIndex, removedItem);

        this.OnIndexerPropertyChanged();
        this.OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
    }

    /// <summary> Raises a PropertyChanged event (per <see cref="INotifyPropertyChanged" />). </summary>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    /// <summary>
    /// If Notifications are Enabled, Raise CollectionChanged event to any listeners.
    /// Properties/methods modifying this ObservableCollection will raise
    /// a collection changed event through this virtual method.
    /// </summary>
    /// <remarks>
    /// When overriding this method, either call its base implementation
    /// or call <see cref="BlockReentrancy"/> to guard against reentrant collection changes.
    /// </remarks>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!this.AreNotificationsEnabled)
        {
            ++this.pendingChangedNofications;
            return;
        }

        NotifyCollectionChangedEventHandler? handler = CollectionChanged;
        if (handler != null)
        {
            // Not calling BlockReentrancy() here to avoid the SimpleMonitor allocation.
            blockReentrancyCount++;
            try
            {
                handler(this, e);
            }
            finally
            {
                blockReentrancyCount--;
            }
        }
    }

    /// <summary>
    /// Disallow reentrant attempts to change this collection. E.g. an event handler
    /// of the CollectionChanged event is not allowed to make changes to this collection.
    /// </summary>
    /// <remarks>
    /// typical usage is to wrap e.g. a OnCollectionChanged call with a using() scope:
    /// <code>
    ///         using (BlockReentrancy())
    ///         {
    ///             CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item, index));
    ///         }
    /// </code>
    /// </remarks>
    protected IDisposable BlockReentrancy()
    {
        blockReentrancyCount++;
        return this.EnsureMonitorInitialized();
    }

    /// <summary> Check and assert for reentrant attempts to change this collection. </summary>
    /// <exception cref="InvalidOperationException"> raised when changing the collection
    /// while another collection change is still being notified to other listeners </exception>
    protected void CheckReentrancy()
    {
        if (blockReentrancyCount > 0)
        {
            // we can allow changes if there's only one listener - the problem
            // only arises if reentrant changes make the original event args
            // invalid for later listeners.  This keeps existing code working
            // (e.g. Selector.SelectedItems).
            NotifyCollectionChangedEventHandler? handler = CollectionChanged;
            if (handler != null && !handler.HasSingleTarget)
            {
                throw new InvalidOperationException("Smart Observable Collection Reentrancy Not Allowed");
            }
        }
    }

    /// <summary> Helper to raise a PropertyChanged event for the Count property </summary>
    private void OnCountPropertyChanged()
    {
        if (!this.AreNotificationsEnabled)
        {
            ++ this.pendingCountNofications;
            return; 
        } 

        this.OnPropertyChanged(EventArgsCache.CountPropertyChanged);
    }

    /// <summary> Helper to raise a PropertyChanged event for the Indexer property </summary>
    private void OnIndexerPropertyChanged()
    {
        if (!this.AreNotificationsEnabled)
        {
            ++this.pendingIndexerNofications;
            return;
        }

        this.OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
    } 

    /// <summary> Helper to raise CollectionChanged event to any listeners </summary>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index)
    {
        if (!this.AreNotificationsEnabled)
        {
            ++this.pendingChangedNofications;
            return;
        }

        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
    }

    /// <summary> Helper to raise CollectionChanged event to any listeners </summary>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object? item, int index, int oldIndex)
    {
        if (!this.AreNotificationsEnabled)
        {
            ++this.pendingChangedNofications;
            return;
        }

        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
    }

    /// <summary> Helper to raise CollectionChanged event to any listeners </summary>
    private void OnCollectionChanged(NotifyCollectionChangedAction action, object? oldItem, object? newItem, int index)
    {
        if (!this.AreNotificationsEnabled)
        {
            ++this.pendingChangedNofications;
            return;
        }

        this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
    }

    /// <summary> Helper to raise CollectionChanged event with action == Reset to any listeners </summary>
    private void OnCollectionReset()
    {
        if (!this.AreNotificationsEnabled)
        {
            ++this.pendingChangedNofications;
            return;
        }

        this.OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
    }

    private SimpleMonitor EnsureMonitorInitialized() => monitor ??= new SimpleMonitor(this);

    [OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
        this.EnsureMonitorInitialized();
        monitor!.busyCount = blockReentrancyCount;
    }

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
        if (monitor != null)
        {
            this.blockReentrancyCount = monitor.busyCount;
            monitor.collection = this;
        }
    }

    // this class helps prevent reentrant calls
    [Serializable]
    private sealed class SimpleMonitor : IDisposable
    {
        internal int busyCount; // Only used during (de)serialization to maintain compatibility with desktop. Do not rename (binary serialization)

        [NonSerialized]
        internal SmartObservableCollection<T> collection;

        public SimpleMonitor(SmartObservableCollection<T> collection)
        {
            Debug.Assert(collection != null);
            this.collection = collection;
        }

        public void Dispose() => collection.blockReentrancyCount--;
    }
}

internal static class EventArgsCache
{
    internal static readonly PropertyChangedEventArgs CountPropertyChanged = new("Count");
    internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new("Item[]");
    internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new(NotifyCollectionChangedAction.Reset);
}
