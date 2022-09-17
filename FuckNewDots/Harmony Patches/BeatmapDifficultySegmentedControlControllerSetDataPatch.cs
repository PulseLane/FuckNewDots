using HarmonyLib;
using System;

namespace FuckNewDots.Harmony_Patches
{
    [HarmonyPatch(typeof(BeatmapDifficultySegmentedControlController), "SetData")]
    internal class BeatmapDifficultySegmentedControlControllerSetDataPatch
    {
        internal static event Action BeatmapDifficultySegmentedControlControllerSetDataEvent;
        static void Postfix()
        {
            BeatmapDifficultySegmentedControlControllerSetDataEvent?.Invoke();
        }
    }
}
