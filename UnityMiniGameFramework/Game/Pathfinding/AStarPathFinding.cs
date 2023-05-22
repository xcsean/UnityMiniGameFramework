using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public class AStarPathFinding
    {
        private static AStarPathFinding instance;

        public static AStarPathFinding GetInstance()
        {
            
            if (instance == null)
            {
                instance = new AStarPathFinding();
            }
            return instance;
        }

        public struct ResultPoint
        {
            public int x, y;

            public ResultPoint(int xPos, int yPos)
            {
                x = xPos;
                y = yPos;
            }
        }

        public struct ThreadResult
        {
            public bool IsDone;
            public List<ResultPoint> Result;

            public ThreadResult(bool isDone = false)
            {
                IsDone = isDone;
                Result = new List<ResultPoint>();
            }

            public void UpdateResult(int x, int y)
            {
                Result.Add(new ResultPoint(x, y));
            }
        }

        public const int OBLIQUE = 14;
        public const int STEP = 10;
        public int[,] MazeArray { get; set; }

        BinaryHeap CloseList;
        BinaryHeap OpenList;
        List<Point> surroundPoints = new List<Point>(4);

        private static Dictionary<int, ThreadResult> m_ThreadDict = new Dictionary<int, ThreadResult>();
        private static int m_CurrThreadId = 0;

        private AStarPathFinding()
        {
            MazeArray = AstarUtility.GetGrids();
            CloseList = new BinaryHeap(MazeArray.Length);
            OpenList = new BinaryHeap(MazeArray.Length);
        }

        private void FoundPoint(Point tempStart, Point point)
        {
            var G = CalcG(tempStart, point);
            if (G > point.G)
            {
                point.SetParentPoint(tempStart.GetId());
                point.G = G;
                point.CalcF();
            }
        }

        private void NotFountPoint(Point tempStart, Point end, Point point)
        {
            point.SetParentPoint(tempStart.GetId());
            point.G = CalcG(tempStart, point);
            point.H = CalcH(end, point);
            point.CalcF();
            PointManager.GetInstance().CachePoint(point);
            OpenList.Enqueue(point.GetId());
        }

        private int CalcG(Point start, Point point)
        {
            int G = Mathf.Abs(point.X - start.X) + Mathf.Abs(point.Y - start.Y) == 2 ? STEP : OBLIQUE;
            var parent = point.ParentPoint();
            int parentG = 0;
            if (parent != null)
                parentG = parent.Value.G;
            return G + parentG;
        }

        private int CalcH(Point end, Point point)
        {
            int step = Mathf.Abs(point.X - end.X) + Mathf.Abs(point.Y - end.Y);
            return step * STEP;
        }

        public List<Point> SurrroundPoints(Point point, bool IsIgnoreCorner)
        {
            instance.surroundPoints.Clear();
            for (int x = point.X - 1; x <= point.X + 1; x++)
            for (int y = point.Y - 1; y <= point.Y + 1; y++)
            {
                if (isNearPosition(point, x, y) && CanArrive(point, x, y))
                    GetInstance().surroundPoints.Add(PointManager.GetInstance().CreatePoint(x, y));
            }

            return instance.surroundPoints;
        }

        bool isNearPosition(Point point, int x, int y)
        {
            return Mathf.Abs(point.X - x) + Mathf.Abs(point.Y - y) == 1;
        }

        private bool CanArrive(int x, int y)
        {
            return AstarUtility.CheckGridCanArrived(x, y);
        }

        private bool CanArrive(Point start, int x, int y)
        {
            if (CloseList.Exists(PointManager.GetInstance().CalPointId(x, y)) || !CanArrive(x, y))
                return false;
            if (Mathf.Abs(x - start.X) + Mathf.Abs(y - start.Y) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Point FindPath(Vector2Int startGrid, Vector2Int endGrid, bool isIgnoreCorner, int threadId)
        {
            Point result;
            lock (instance)
            {
                instance.OpenList.Clear();
                instance.CloseList.Clear();
                PointManager.GetInstance().ClearData();
                Point start = PointManager.GetInstance().CreatePoint(startGrid);
                Point end = PointManager.GetInstance().CreatePoint(endGrid);
                if (startGrid.Equals(endGrid))
                {
                    start.SetParentPoint(end.GetId());
                    result = start;
                }
                else
                {
                    instance.OpenList.Enqueue(start.GetId());
                    while (instance.OpenList.Count != 0)
                    {
                        var tempStart = PointManager.GetInstance().GetPoint(instance.OpenList.Dequeue()).Value;
                        instance.CloseList.Enqueue(tempStart.GetId());
                        instance.SurrroundPoints(tempStart, isIgnoreCorner);
                        foreach (Point point in instance.surroundPoints)
                        {
                            if (instance.OpenList.Exists(point.GetId()))
                            {
                                instance.FoundPoint(tempStart, point);
                            }
                            else
                                instance.NotFountPoint(tempStart, end, point);
                        }

                        if (instance.OpenList.Get(end.GetId()) > 0)
                            break;
                    }

                    result = PointManager.GetInstance().GetPoint(end.GetId()).Value;
                }

                if (threadId > 0)
                    FillResult(threadId, result);
            }

            return result;
        }

        static void FillResult(int threadId, Point? point)
        {
            var result = m_ThreadDict[threadId];
            result.IsDone = true;
            while (point != null)
            {
                result.UpdateResult(point.Value.X, point.Value.Y);
                point = point.Value.ParentPoint();
            }

            m_ThreadDict[threadId] = result;
        }

        static int GetThreadId()
        {
            m_CurrThreadId++;
            if (m_CurrThreadId < 0)
                m_CurrThreadId = 1;
            return m_CurrThreadId;
        }

        public List<ResultPoint> GoToTargetPosSync(Vector2Int startPos, Vector2Int endPos)
        {
            int threadId = GetThreadId();
            m_ThreadDict[threadId] = new ThreadResult(false);
            var task = new Task(() => FindPath(startPos, endPos, false, threadId));
            task.Start();
            task.Wait();
            var result = m_ThreadDict[threadId];
            m_ThreadDict.Remove(threadId);
            return result.Result;
        }
    }
}