using HarmonyLib;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuckNewDots.Harmony_Patches
{
    [HarmonyPatch(typeof(StandardLevelDetailView), "SetContent")]
    internal class StandardLevelDetailViewSetContentPatch
    {
        internal static Action<IBeatmapLevel> LevelSelectedEvent;

        private static IBeatmapLevel lastLevel = null;
        private static BeatmapDifficulty lastDefaultDifficulty;
        private static BeatmapCharacteristicSO lastDefaultBeatmapCharacteristic;
        private static PlayerData lastPlayerData;


        private static StandardLevelDetailView standardLevelDetailView = null;

        static void Prefix(StandardLevelDetailView __instance, IBeatmapLevel level, BeatmapDifficulty defaultDifficulty, BeatmapCharacteristicSO defaultBeatmapCharacteristic, PlayerData playerData)
        {
            lastLevel = level;
            lastDefaultDifficulty = defaultDifficulty;
            lastDefaultBeatmapCharacteristic = defaultBeatmapCharacteristic;
            lastPlayerData = playerData;

            standardLevelDetailView = __instance;
            Plugin.config.ChangeEvent -= ConfigChanged;
            Plugin.config.ChangeEvent += ConfigChanged;

            if (!level.levelID.StartsWith("custom_level"))
            {
                return;
            }
            LevelSelectedEvent?.Invoke(level);

            if (!Plugin.config.addCustomCharacteristic)
            {
                //RemoveCustomCharacteristicIfPresent(level);
            }
            else
            {
                AddCustomCharacteristic(level);
            }
        }

        static async void AddCustomCharacteristic(IBeatmapLevel level)
        {
            // Anything with "OldDots" means we've already done this map - no need to recreate
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName.EndsWith("OldDots")))
            {
                return;
            }

            List<IDifficultyBeatmapSet> difficultyBeatmapSets = new List<IDifficultyBeatmapSet>(level.beatmapLevelData.difficultyBeatmapSets);

            foreach (var difficultyBeatmapSet in level.beatmapLevelData.difficultyBeatmapSets)
            {
                CustomDifficultyBeatmapSet customDifficultyBeatmapSet = new CustomDifficultyBeatmapSet(Utils.CreateBeatmapCharacteristicSO(difficultyBeatmapSet.beatmapCharacteristic));
                customDifficultyBeatmapSet.SetCustomDifficultyBeatmaps((await CreateCustomDifficulties(difficultyBeatmapSet.difficultyBeatmaps, customDifficultyBeatmapSet)).ToArray());
                if (customDifficultyBeatmapSet.difficultyBeatmaps.Count > 0)
                {
                    difficultyBeatmapSets.Add(customDifficultyBeatmapSet);
                }

            }

            ((BeatmapLevelData) level.beatmapLevelData).SetField<BeatmapLevelData, IReadOnlyList<IDifficultyBeatmapSet>>("_difficultyBeatmapSets", difficultyBeatmapSets.ToArray());
        }

        static async Task<List<CustomDifficultyBeatmap>> CreateCustomDifficulties(IReadOnlyList<IDifficultyBeatmap> difficultyBeatmaps, CustomDifficultyBeatmapSet beatmapSet)
        {
            List<CustomDifficultyBeatmap> customDifficulties = new List<CustomDifficultyBeatmap>();

            HashSet<int> indices = new HashSet<int>();
            int i = 0;

            foreach (IDifficultyBeatmap difficultyBeatmap in difficultyBeatmaps)
            {
                if (Utils.ContainsDotNotes(difficultyBeatmap))
                {
                    Plugin.logger.Debug($"Dots found in {difficultyBeatmap.difficulty}");
                    indices.Add(i);
                }
                else
                {
                    Plugin.logger.Debug($"No dots in {difficultyBeatmap.difficulty}");
                }
                i++;
            }

            CustomDifficultyBeatmap[] customDifficultyBeatmaps = new CustomDifficultyBeatmap[indices.Count];
            foreach (var index in indices)
            {
                IDifficultyBeatmap difficultyBeatmap = difficultyBeatmaps[index];
                IBeatmapDataBasicInfo beatmapDataBasicInfo = await difficultyBeatmap.GetBeatmapDataBasicInfoAsync();
                customDifficulties.Add(new CustomDifficultyBeatmap(
                    difficultyBeatmap.level,
                    beatmapSet,
                    difficultyBeatmap.difficulty,
                    difficultyBeatmap.difficultyRank,
                    difficultyBeatmap.noteJumpMovementSpeed,
                    difficultyBeatmap.noteJumpStartBeatOffset,
                    difficultyBeatmap.level.beatsPerMinute,
                    ((CustomDifficultyBeatmap) difficultyBeatmap).beatmapSaveData,
                    beatmapDataBasicInfo));
            }

            return customDifficulties;
        }

        static void RemoveCustomCharacteristicIfPresent(IBeatmapLevel level)
        {
            if (level.beatmapLevelData.difficultyBeatmapSets.Any(x => x.beatmapCharacteristic.serializedName.EndsWith("OldDots")))
            {
                ((BeatmapLevelData) level.beatmapLevelData).SetField<BeatmapLevelData, IReadOnlyList<IDifficultyBeatmapSet>>("_difficultyBeatmapSets", level.beatmapLevelData.difficultyBeatmapSets.Where(x => !x.beatmapCharacteristic.serializedName.EndsWith("OldDots")).ToArray());
            }
        }

        static void ConfigChanged()
        {

            standardLevelDetailView.SetContent(lastLevel, standardLevelDetailView.selectedDifficultyBeatmap.difficulty, lastDefaultBeatmapCharacteristic, lastPlayerData);
        }
    }
}
