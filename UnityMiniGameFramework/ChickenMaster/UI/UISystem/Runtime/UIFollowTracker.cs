using System.Collections.Generic;
using UnityEngine;

namespace UnityMiniGameFramework.UISystem
{
    internal static class UIFollowTracker
    {
        private static HashSet<UIFollow> s_Tracked = new HashSet<UIFollow>();
        private static HashSet<UIFollowSorter> s_Sorters = new HashSet<UIFollowSorter>();

        public static void TrackFollow(UIFollow follow)
        {
            if (s_Tracked.Contains(follow))
                return;
            if (s_Tracked.Count == 0)
                Canvas.willRenderCanvases += RenderCanvases;
            s_Tracked.Add(follow);
        }

        public static void UntrackFollow(UIFollow follow)
        {
            s_Tracked.Remove(follow);
            if (s_Tracked.Count == 0)
                Canvas.willRenderCanvases -= RenderCanvases;
        }

        public static void TrackSorter(UIFollowSorter sorter)
        {
            if (s_Sorters.Contains(sorter))
                return;
            s_Sorters.Add(sorter);
        }

        public static void UntrackSorter(UIFollowSorter sorter)
        {
            s_Sorters.Remove(sorter);
        }

        private static void RenderCanvases()
        {
            foreach (var follow in s_Tracked)
            {
                if (!follow || !follow.IsActive())
                    continue;
                follow.OnCanvasesRender();
            }

            foreach (var sorter in s_Sorters)
            {
                if (!sorter || !sorter.IsActive())
                    continue;
                sorter.OnCanvasesRender();
            }
        }
    }
}