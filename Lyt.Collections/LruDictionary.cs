namespace Lyt.Collections;

public sealed class LruDictionary<TKey, TValue>(int capacity) : 
    IDictionary<TKey, TValue>  where TKey: notnull
{
    private sealed class Node(TKey key, TValue value)
    {
        public TKey Key { get; } = key;

        public TValue Value { get; set; } = value;
    }

    private readonly int capacity =
            capacity <= 0 || capacity > 8 * 1024 * 1024 ?
                throw new ArgumentOutOfRangeException(nameof(capacity)) :
                capacity;

    private readonly Dictionary<TKey, LinkedListNode<Node>> map = [];
    
    private readonly LinkedList<Node> list = new();

    public int Count => this.map.Count;

    public int Capacity => this.capacity;

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (this.map.TryGetValue(key, out var node))
        {
            // Move to front (most recently used)
            this.list.Remove(node);
            this.list.AddFirst(node);
            value = node.Value.Value;
            return true;
        }

        value = default;
        return false;
    }

    public void Add(TKey key, TValue value)
    {
        if (this.map.TryGetValue(key, out var existingNode))
        {
            // Update value and move to front
            existingNode.Value.Value = value;
            this.list.Remove(existingNode);
            this.list.AddFirst(existingNode);
            return;
        }

        if (this.map.Count >= this.capacity)
        {
            // Remove least recently used (tail)
            var lruNode = this.list.Last;
            if (lruNode != null)
            {
                this.map.Remove(lruNode.Value.Key);
                this.list.RemoveLast();
            }
        }

        var newNode = new LinkedListNode<Node>(new Node(key, value));
        this.list.AddFirst(newNode);
        this.map[key] = newNode;
    }

    public void Clear()
    {
        this.map.Clear();
        this.list.Clear();
    }

    public bool IsReadOnly => false;

    public ICollection<TKey> Keys => this.map.Keys;

    public ICollection<TValue> Values => this.list.Select(node => node.Value).ToList();

    public TValue this[TKey key]
    {
        get
        {
            if (this.map.TryGetValue(key, out var node))
            {
                return node.Value.Value;
            }

            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
        } 

        set
        {
            if (this.map.TryGetValue(key, out var node))
            {
                node.Value.Value = value;
                return;
            }

            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
        }
    }

    public void Add(KeyValuePair<TKey, TValue> item) => this.Add(item.Key, item.Value);

    public bool ContainsKey(TKey key) => this.map.ContainsKey(key) ;

    public bool Contains(KeyValuePair<TKey, TValue> item) => this.ContainsKey(item.Key);

    public IEnumerator<TKey> GetKeysEnumerator() => this.map.Keys.GetEnumerator();

    #region Unsupported IDictionary interface Members

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        Debug.WriteLine("Enumerating KVP's is not supported in LruDictionary. Enumerate keys instead.");

        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }

        return Enumerable.Empty<KeyValuePair<TKey, TValue>>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        Debug.WriteLine("CopyTo is not supported in LruDictionary.");

        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }
    }

    public bool Remove(TKey key)
    {
        Debug.WriteLine("Remove is not supported in LruDictionary. Use Clear() to remove all items.");  

        if ( Debugger.IsAttached)
        {
            Debugger.Break();
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) => this.Remove(item.Key);

    #endregion Unsupported IDictionary interface Members
}