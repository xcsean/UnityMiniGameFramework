using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class PointManager
    {
        private Dictionary<int, Point> pointDict;

        static PointManager instance;

        public static PointManager GetInstance()
        { 
            return instance ?? (instance = new PointManager());
        }

        private PointManager()
        {
            pointDict = new Dictionary<int, Point>();
        }

        public void CachePoint(Point point)
        {
            var id = point.GetId();
            pointDict[id] = point;
        }

        public void ClearData()
        {
            pointDict.Clear();
        }


        public Point? GetPoint(int id)
        {
            return pointDict[id];
        }

        public Point CreatePoint(int x, int y)
        {
            var id = CalPointId(x, y);
            if (!pointDict.ContainsKey(id))
            {
                var point = new Point(x, y);
                pointDict.Add(id, point);
            }

            return pointDict[id];
        }

        public Point CreatePoint(Vector2Int vec)
        {
            return CreatePoint(vec.x, vec.y);
        }


        public int CalPointId(int x, int y)
        {
            return x * 1000 + y;
        }
    }

    [Serializable]
    public struct Point
    {
        public Point? ParentPoint()
        {
            return mParentId != int.MinValue ? PointManager.GetInstance().GetPoint(mParentId) : null;
        }

        public void SetParentPoint(int parentId)
        {
            mParentId = parentId;
        }

        int mParentId, mId;

        public int F { get; set; } //F=G+H
        public int G { get; set; }
        public int H { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int GetId()
        {
            return mId;
        }

        public Point(Vector2Int v2) : this(v2.x, v2.y)
        {
        }

        public Point(int x, int y)
        {
            mParentId = int.MinValue;
            F = G = H = 0;
            this.X = x;
            this.Y = y;
            mId = PointManager.GetInstance().CalPointId(X, Y);
        }

        public void CalcF()
        {
            this.F = this.G + this.H;
        }

        //计算两点距离
        public float Distance(Point point)
        {
            return Mathf.Sqrt((X - point.X) * (X - point.X) + (Y - point.Y) * (Y - point.Y));
        }

        // todo：
        public Vector3 GetPosition()
        {
            return Vector3.zero;
        }

        public int CompareTo(Point rhs)
        {
            return this.F - rhs.F;
        }

        public bool Equals(Point other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
    }
}