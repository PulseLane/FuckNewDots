using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

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
