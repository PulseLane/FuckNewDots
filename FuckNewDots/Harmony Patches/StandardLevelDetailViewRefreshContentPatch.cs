using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuckNewDots.Harmony_Patches
{
    [HarmonyPatch(typeof(StandardLevelDetailView), "RefreshContent")]
    internal class StandardLevelDetailViewRefreshContentPatch
    {
        internal static Action<IBeatmapLevel> LevelSelectedEvent;

        [HarmonyPriority(Priority.Last)]
        static void Postfix(IBeatmapLevel ____level)
        {
            if (!____level.levelID.StartsWith("custom_level"))
            {
                return;
            }
            LevelSelectedEvent?.Invoke(____level);
        }
    }
}
