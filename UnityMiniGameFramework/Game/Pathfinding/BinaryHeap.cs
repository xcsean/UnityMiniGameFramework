using System;
using System.Collections.Generic;

namespace UnityMiniGameFramework
{
    public struct BinaryHeap
    {
        //默认容量为6
        private const int DEFAULT_CAPACITY = 6;
        private int mCount;
        public int[] mItems;
        Dictionary<int, bool> mItemDict; 
        public int Count
        {
            get { return mCount; }
        }

        public BinaryHeap(int capacity)
        {
            mCount = 0;
            if (capacity < 0)
            {
                throw new IndexOutOfRangeException();
            }
            mItems = new int[capacity];
            mItemDict = new Dictionary<int, bool>();
        }

        /// <summary>
        /// 增加元素到堆，并从后往前依次对各结点为根的子树进行筛选，使之成为堆，直到根结点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Enqueue(int value)
        {
            if (mCount == mItems.Length)
            {
                ResizeItemStore(mItems.Length * 2);
            }

            mItems[mCount++] = value;
            int position = BubbleUp(mCount - 1);
            mItemDict[value] = true;
            return (position == 0);
        }

        /// <summary>
        /// 取出堆的最小值
        /// </summary>
        /// <returns></returns>
        public int Dequeue()
        {
            return Dequeue(false);
        }

        private int Dequeue(bool shrink)
        {
            if (mCount == 0)
            {
                throw new InvalidOperationException();
            }
            int result = mItems[0];
            if (mCount == 1)
            {
                mCount = 0;
                mItems[0] = default(int);
            }
            else
            {
                --mCount;
                //取序列最后的元素放在堆顶
                mItems[0] = mItems[mCount];
                mItems[mCount] = default(int);
                // 维护堆的结构
                BubbleDown();
            }
            if (shrink)
            {
                ShrinkStore();
            }
            mItemDict.Remove(result);
            return result;
        }

        private void ShrinkStore()
        {
            // 如果容量不足一半以上，默认容量会下降。
            if (mItems.Length > DEFAULT_CAPACITY && mCount < (mItems.Length >> 1))
            {
                int newSize = Math.Max(
                    DEFAULT_CAPACITY, (((mCount / DEFAULT_CAPACITY) + 1) * DEFAULT_CAPACITY));

                ResizeItemStore(newSize);
            }
        }

        private void ResizeItemStore(int newSize)
        {
            if (mCount < newSize || DEFAULT_CAPACITY <= newSize)
            {
                return;
            }

            UnityEngine.Profiling.Profiler.BeginSample("FindPath ResizeItemStore");
            int[] temp = new int[newSize];
            Array.Copy(mItems, 0, temp, 0, mCount);
            mItems = temp;
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void Clear()
        {
            mCount = 0;
            mItemDict.Clear();
        }

        /// <summary>
        /// 从前往后依次对各结点为根的子树进行筛选，使之成为堆，直到序列最后的节点
        /// </summary>
        private void BubbleDown()
        {
            int parent = 0;
            int leftChild = (parent * 2) + 1;
            while (leftChild < mCount)
            {
                // 找到子节点中较小的那个
                int rightChild = leftChild + 1;
                var leftPoint = PointManager.GetInstance().GetPoint(mItems[leftChild]).Value;
                int bestChild = (rightChild < mCount && (PointManager.GetInstance().GetPoint(mItems[rightChild]).Value.CompareTo(leftPoint)) < 0) ?
                    rightChild : leftChild;
                if (mItems[bestChild].CompareTo(mItems[parent]) < 0)
                {
                    // 如果子节点小于父节点, 交换子节点和父节点
                    int temp = mItems[parent];
                    mItems[parent] = mItems[bestChild];
                    mItems[bestChild] = temp;
                    parent = bestChild;
                    leftChild = (parent * 2) + 1;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 从后往前依次对各结点为根的子树进行筛选，使之成为堆，直到根结点
        /// </summary>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private int BubbleUp(int startIndex)
        {
            while (startIndex > 0)
            {
                int parent = (startIndex - 1) / 2;
                //如果子节点小于父节点，交换子节点和父节点
                var point = PointManager.GetInstance().GetPoint(mItems[startIndex]).Value;
                var parentPoint = PointManager.GetInstance().GetPoint(mItems[parent]).Value;
                if (point.CompareTo(parentPoint) < 0)
                {
                    int temp = mItems[startIndex];
                    mItems[startIndex] = mItems[parent];
                    mItems[parent] = temp;
                }
                else
                {
                    break;
                }
                startIndex = parent;
            }
            return startIndex;
        }

        public bool Exists(int node)
        {
            return mItemDict.ContainsKey(node);
            //for (int i = 0; i < Count; ++i)
            //{
            //    if (node.Equals(mItems[i]))
            //        return true;
            //}
            //return false;
        }

        public int Get(int node)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (node.Equals(mItems[i]))
                    return mItems[i];
            }
            return -1;
        }
    }
}