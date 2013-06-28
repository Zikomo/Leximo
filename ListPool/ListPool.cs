using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ListPool
{

    public class Pool<T>
    {
        private Queue<List<T>> listPool;
        private Queue<T> regularPool;
        public Pool()
        {
            Allocate(20);
        }

        public Pool(int capacity)
        {
            Allocate(capacity);
        }

        private void Allocate(int capacity)
        {
            listPool = new Queue<List<T>>(capacity);
            regularPool = new Queue<T>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                listPool.Enqueue(new List<T>(capacity));                
            }
        }

        public void ClearLists()
        {
            listPool.Clear();
        }

        public void ClearObjects()
        {
            regularPool.Clear();
        }

        public List<T> GetList()
        {
            lock (listPool)
            {
                List<T> retVal = null;
                if (listPool.Count > 0)
                {
                    retVal = listPool.Dequeue();
                    retVal.Clear();
                }
                else
                {
                    retVal = new List<T>();
                }
                return retVal;
            }
        }

        public int RegularCount
        {
            get
            {
                return regularPool.Count;
            }
        }

        public int ListCount
        {
            get
            {
                return listPool.Count;
            }
        }

        public void ReturnList(List<T> list)
        {
            
            lock (listPool)
            {
                list.Clear();
                listPool.Enqueue(list);
            }
        }

        public T Get()
        {
            if (regularPool.Count > 0)
            {                
                return regularPool.Dequeue();
            }
            return default(T) ;            
        }

        public void Return(T item)
        {            
            regularPool.Enqueue(item);
        }
    }
}
