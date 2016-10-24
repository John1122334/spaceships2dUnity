using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbCom
{
//    static class Util
//    {
//        static public double safe_clamp(double value, double min, double max)
//        {
//            if (double.IsNaN(value))
//                return min;
//
//            else if (value < min)
//                return min;
//
//            else if (value > max)
//                return max;
//
//            else
//                return value;
//        }
//        public static double saturate(double value)
//        {
//            if (value < 0.0)
//                return 0.0;
//
//            else if (value > 1.0)
//                return 1.0;
//
//            else return value;
//        }
//    }
//    public class Pair<T, U>
//    {
//        public Pair()
//        {
//        }
//
//        public Pair(T first, U second)
//        {
//            this.first = first;
//            this.second = second;
//        }
//
//        public T first;
//        public U second;
//    }
//    public class Indexer<T> : IEnumerable<T>
//    {
//        private Func<int, T> getter;
//        private Action<int, T> setter;
//        private Func<int, bool> validator;
//        public Indexer(Func<int, T> getter, Action<int, T> setter, Func<int, bool> validator)
//        {
//            this.getter = getter;
//            this.setter = setter;
//            this.validator = validator;
//        }
//        public Indexer(IList<T> list)
//        {
//            this.getter = (int i) => list[i];
//            this.setter = (int i, T v) => list[i] = v;
//            this.validator = (int i) => i < list.Count();
//        }
//        public T this[int index]
//        {
//            get
//            {
//                return getter(index);
//            }
//            set
//            {
//                setter(index, value);
//            }
//        }
//
//        private class Enumerator : IEnumerator<T>
//        {
//            private Indexer<T> parent;
//            private int index = -1;
//            public Enumerator(Indexer<T> parent)
//            {
//                this.parent = parent;
//            }
//
//            public T Current
//            {
//                get { return parent[index]; }
//            }
//
//            object System.Collections.IEnumerator.Current
//            {
//                get { return parent[index]; }
//            }
//
//            public bool MoveNext()
//            {
//                index++;
//                return parent.validator(index);
//            }
//
//            public void Reset()
//            {
//                index = -1;
//            }
//
//            public void Dispose()
//            {
//            }
//        }
//
//        public IEnumerator<T> GetEnumerator()
//        {
//            return new Enumerator(this);
//        }
//
//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return new Enumerator(this);
//        }
//    }

//    public class ReadOnlyIndexer<T> : IEnumerable<T>
//    {
//        private Func<int, T> getter;
//        private Func<int, bool> validator;
//        public ReadOnlyIndexer(Func<int, T> getter, Func<int, bool> validator)
//        {
//            this.getter = getter;
//            this.validator = validator;
//        }
//        public ReadOnlyIndexer(IList<T> list)
//        {
//            this.getter = (int i) => list[i];
//            this.validator = (int i) => i < list.Count();
//        }
//        public T this[int index]
//        {
//            get
//            {
//                return getter(index);
//            }
//        }
//
//        private class Enumerator : IEnumerator<T>
//        {
//            private ReadOnlyIndexer<T> parent;
//            private int index = -1;
//            public Enumerator(ReadOnlyIndexer<T> parent)
//            {
//                this.parent = parent;
//            }
//
//            public T Current
//            {
//                get { return parent[index]; }
//            }
//
//            object System.Collections.IEnumerator.Current
//            {
//                get { return parent[index]; }
//            }
//
//            public bool MoveNext()
//            {
//                index++;
//                return parent.validator(index);
//            }
//
//            public void Reset()
//            {
//                index = -1;
//            }
//
//            public void Dispose()
//            {
//            }
//        }
//
//        public IEnumerator<T> GetEnumerator()
//        {
//            return new Enumerator(this);
//        }
//
//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            return new Enumerator(this);
//        }
//    }
    public class ReadOnlyIndexer<K, V> : IEnumerable<V>
    {
        private Func<K, V> getter;
        private IEnumerable<K> keys;
        public ReadOnlyIndexer(Func<K, V> getter, IEnumerable<K> keys)
        {
            this.getter = getter;
            this.keys = keys;
        }
        public V this[K key]
        {
            get
            {
                return getter(key);
            }
        }

        private class Enumerator : IEnumerator<V>
        {
            private ReadOnlyIndexer<K, V> parent;
            private IEnumerator<K> key_i;
            public Enumerator(ReadOnlyIndexer<K, V> parent)
            {
                this.parent = parent;
                this.key_i = parent.keys.GetEnumerator();
            }

            public V Current
            {
                get { return parent[key_i.Current]; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return parent[key_i.Current]; }
            }

            public bool MoveNext()
            {
                return key_i.MoveNext();
            }

            public void Reset()
            {
                key_i = parent.keys.GetEnumerator();
            }

            public void Dispose()
            {
            }
        }

        public IEnumerator<V> GetEnumerator()
        {
            return new Enumerator(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }

}
