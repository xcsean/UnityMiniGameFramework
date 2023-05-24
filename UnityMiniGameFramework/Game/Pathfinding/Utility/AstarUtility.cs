using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework
{
    public static class AstarUtility
    {
        // todo:获取活动区域大小
        internal static int[,] GetGrids()
        {
            var rect = UnityGameApp.Inst.MainScene.implMap.ActiveRect;
            return new int[(int) rect.width, (int) rect.height];
        }

        internal static bool CheckGridCanArrivedExcludeEgg(int x, int y)
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            if (cmGame == null)
                return false;
            if (cmGame.Egg.LogicPos.Equals(new Vector2Int(x, y)))
                return true;
            return CheckGridCanArrived(x, y);
        }

        internal static bool CheckGridCanArrived(int x, int y)
        {
            var cmGame = (UnityGameApp.Inst.Game as ChickenMasterGame);
            if (cmGame == null)
                return false;

            var rect = UnityGameApp.Inst.MainScene.implMap.ActiveRect;
            if (x < 0 || y < 0 || x > rect.width || y > rect.height)
                return false;
            return !cmGame.MapLogicObjects.ContainsKey(new Vector2Int(x, y));
        }

        public static Vector2Int GetLogicPos(Vector3 realPos)
        {
            var rect = UnityGameApp.Inst.MainScene.implMap.ActiveRect;
            var pos = rect.position;
            return new Vector2Int((int) Math.Floor(realPos.x) - (int) pos.x, (int) Math.Floor(realPos.z) - (int) pos.y);
        }

        public static Vector3 GetRendererPos(Vector2Int logicPos)
        {
            var rect = UnityGameApp.Inst.MainScene.implMap.ActiveRect;
            var pos = rect.position;
            return new Vector3(logicPos.x + pos.x, 0, logicPos.y + pos.y);
        }

        internal static bool isNodeAtEdge(Vector2Int logicPos)
        {
            Dictionary<Vector2Int, bool> visited = new Dictionary<Vector2Int, bool>();
            var rect = UnityGameApp.Inst.MainScene.implMap.ActiveRect;
            int _width = (int) rect.width;
            int _height = (int) rect.height;
            return dfs(logicPos, visited, _width, _height, logicPos);
        }

        static bool dfs(Vector2Int logicPos, Dictionary<Vector2Int, bool> visited, int width, int height,
            Vector2Int startPos)
        {
            if (visited.ContainsKey(logicPos) && visited[logicPos]) return false;
            visited[logicPos] = true;
            if (logicPos != startPos)
            {
                if (!CheckGridCanArrived(logicPos.x, logicPos.y))
                    return false;
                if (logicPos.x == 0 || logicPos.x == width - 1 || logicPos.y == 0 || logicPos.y == height - 1)
                {
                    return true;
                }
            }

            if (dfs(logicPos + Vector2Int.up, visited, width, height, startPos))
                return true;
            if (dfs(logicPos + Vector2Int.down, visited, width, height, startPos))
                return true;
            if (dfs(logicPos + Vector2Int.left, visited, width, height, startPos))
                return true;
            if (dfs(logicPos + Vector2Int.right, visited, width, height, startPos))
                return true;
            return false;
        }

        static bool isNearPosition(Vector2Int pos, Vector2Int targetPos)
        {
            return Mathf.Abs(pos.x - targetPos.x) + Mathf.Abs(pos.y - targetPos.y) == 1;
        }
    }
}