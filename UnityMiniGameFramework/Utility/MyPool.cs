using System;
using System.Collections.Generic;

namespace UnityMiniGameFramework
{
    public class MyPool<T> where T : class
    {
        private Action<T> mReset;
        private Func<T> mNew;
        private Stack<T> stack;

        public MyPool(Func<T> New, Action<T> Reset = null)
        {
            this.mNew = New;
            this.mReset = Reset;
            stack = new Stack<T>();
        }

        public T New()
        {
            if (stack.Count > 0)
            {
                T t = null;
                lock (this)
                {
                    t = stack.Pop();
                }

                mReset?.Invoke(t);
                return t;
            }
            else
            {
                T t = mNew();
                return t;
            }
        }

        public void Store(T t)
        {
            lock (this)
                stack.Push(t);
        }

        public void Clear()
        {
            stack.Clear();
        }
        
        
    }
}