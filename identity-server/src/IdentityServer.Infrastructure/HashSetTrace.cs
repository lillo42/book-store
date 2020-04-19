using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer.Infrastructure
{
    public class HashSetTrace<T> : ISet<T>
    {
        private readonly ISet<Trace> _traces;
        private readonly ISet<T> _values;

        public HashSetTrace(ISet<T> hash)
        {
            _values = hash ?? throw new ArgumentNullException(nameof(hash));
            _traces = new HashSet<Trace>();
            
            foreach (var value in hash)
            {
                _traces.Add(new Trace(State.NoChange, value, false));
            }
        }

        public IEnumerable<Trace> Traces => _traces;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _values.GetEnumerator();

        void ICollection<T>.Add(T item)
            => _values.Add(item);

        public void ExceptWith(IEnumerable<T> other)
            => _values.ExceptWith(other);

        public void IntersectWith(IEnumerable<T> other)
            => _values.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<T> other)
            => _values.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other)
            => _values.IsProperSupersetOf(other);
        
        public bool IsSubsetOf(IEnumerable<T> other)
            => _values.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other)
            => _values.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other)
            => _values.Overlaps(other);

        public bool SetEquals(IEnumerable<T> other)
            => _values.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other)
            => _values.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<T> other)
            => _values.UnionWith(other);

        public bool Add(T item)
        {
            var ret = _values.Add(item);
            
            if (ret)
            {
                if(!_traces.Add(new Trace(State.Added, item, true)))
                {
                    foreach (var trace in _traces)
                    {
                        if (!trace.Value.Equals(item))
                        {
                            continue;
                        }
                    
                        trace.State = State.Added;
                        break;
                    }
                }
            }

            return ret;
        }

        public void Clear()
        {
            _values.Clear();
            
            foreach (var trace in _traces)
            {
                trace.State = State.Removed;
            }
        }

        public bool Contains(T item)
            => _values.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
            => _values.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            var ret = _values.Remove(item);

            if (ret)
            {
                foreach (var trace in _traces)
                {
                    if (!trace.Value.Equals(item))
                    {
                        continue;
                    }

                    trace.State = State.Removed;
                    break;
                }
            }
            
            return ret;
        }

        public int Count => _values.Count;

        public bool IsReadOnly =>  _values.IsReadOnly;

        public class Trace : IEquatable<Trace>
        {
            public Trace(State state, T value, bool isNew)
            {
                State = state;
                Value = value;
                IsNew = isNew;
            }

            public State State { get; set; }
            public T Value { get; }
            public bool IsNew { get; }

            public bool Equals(Trace other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                
                return EqualityComparer<T>.Default.Equals(Value, other.Value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }
                return Equals((Trace) obj);
            }

            public override int GetHashCode() 
                => EqualityComparer<T>.Default.GetHashCode(Value);
        }
    }
    
    public enum State
    {
        NoChange,
        Added,
        Removed
    }
}