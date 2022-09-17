using BeatSaberMarkupLanguage;
using FuckNewDots.Harmony_Patches;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using static BeatSaberMarkupLanguage.BeatSaberUI;

namespace FuckNewDots.Managers
{
    internal class DotIconManager : IInitializable, IDisposable
    {
        internal static Dictionary<string, int[]> levelsWithDots = new Dictionary<string, int[]>();

        private Config _config;

        private BeatmapCharacteristicSegmentedControlController _beatmapCharacteristicSegmentedControlController;
        private BeatmapDifficultySegmentedControlController _beatmapDifficultySegmentedControlController;

        private readonly IconSegmentedControl _beatmapCharacteristicSegmentedControl;
        private SegmentedControl _beatmapDifficultySegmentedControl;

        private IBeatmapLevel _level;

        public DotIconManager(Config config, StandardLevelDetailViewController standardLevelDetailViewController)
        {
            _config = config;
            ; StandardLevelDetailView standardLevelDetailView = standardLevelDetailViewController.GetField<StandardLevelDetailView, StandardLevelDetailViewController>("_standardLevelDetailView");

            _beatmapCharacteristicSegmentedControlController = standardLevelDetailView.GetField<BeatmapCharacteristicSegmentedControlController, StandardLevelDetailView>("_beatmapCharacteristicSegmentedControlController");
            _beatmapDifficultySegmentedControlController = standardLevelDetailView.GetField<BeatmapDifficultySegmentedControlController, StandardLevelDetailView>("_beatmapDifficultySegmentedControlController");
            _beatmapCharacteristicSegmentedControl = _beatmapCharacteristicSegmentedControlController.GetField<IconSegmentedControl, BeatmapCharacteristicSegmentedControlController>("_segmentedControl");
            _beatmapDifficultySegmentedControl = _beatmapDifficultySegmentedControlController.GetField<TextSegmentedControl, BeatmapDifficultySegmentedControlController>("_difficultySegmentedControl");
        }

        public void Initialize()
        {
            StandardLevelDetailViewSetContentPatch.LevelSelectedEvent += OnLevelUpdated;
            _beatmapCharacteristicSegmentedControl.didSelectCellEvent += OnBeatmapCharacteristicSegmentedControlDidSelectCellEvent;
            //_beatmapDifficultySegmentedControl.didSelectCellEvent += OnBeatmapDifficultySegmentedControlDidSelectCellEvent;
            //BeatmapDifficultySegmentedControlController_SetData.CharacteristicsSegmentedControllerDataSetEvent += BeatmapDifficultySegmentedControlController_CharacteristicsSegmentedControllerDataSetEvent;
            BeatmapDifficultySegmentedControlControllerSetDataPatch.BeatmapDifficultySegmentedControlControllerSetDataEvent += OnBeatmapDifficultySegmentedControlControllerSetDataEvent;

            //_config.ChangeEvent += OnConfigChanged;
        }

        private void OnConfigChanged()
        {
            if (_config.addWarningIcon)
            {
                AddDotIcon();
            }
            else
            {
                RemoveDotIcon();
            }
        }

        private void OnLevelUpdated(IBeatmapLevel level)
        {
            _level = level;
            AddDotIcon();
        }

        private void OnBeatmapCharacteristicSegmentedControlDidSelectCellEvent(SegmentedControl _, int __)
        {
            AddDotIcon();
        }

        private void AddDotIcon()
        {
            if (!_config.addWarningIcon)
            {
                return;
            }

            Plugin.logger.Debug("beatmap characteristic segmented control did select cell event");
            if (_beatmapCharacteristicSegmentedControlController.selectedBeatmapCharacteristic == null)
            {
                return;
            }

            if (_level != null && !_beatmapCharacteristicSegmentedControlController.selectedBeatmapCharacteristic.serializedName.EndsWith("OldDots"))
            {
                Plugin.logger.Debug(_beatmapCharacteristicSegmentedControlController.selectedBeatmapCharacteristic.serializedName);
                foreach (IDifficultyBeatmapSet difficultyBeatmapSet in _level.beatmapLevelData.difficultyBeatmapSets)
                {
                    Plugin.logger.Debug(difficultyBeatmapSet.beatmapCharacteristic.serializedName);
                    if (difficultyBeatmapSet.beatmapCharacteristic.serializedName == _beatmapCharacteristicSegmentedControlController.selectedBeatmapCharacteristic.serializedName)
                    {
                        HashSet<BeatmapDifficulty> difficultiesToStrike = new HashSet<BeatmapDifficulty>();
                        foreach (IDifficultyBeatmap difficultyBeatmap in difficultyBeatmapSet.difficultyBeatmaps)
                        {
                            Plugin.logger.Debug(difficultyBeatmap.difficulty.ToString());
                            if (Utils.ContainsDotNotes(difficultyBeatmap))
                            {
                                difficultiesToStrike.Add(difficultyBeatmap.difficulty);
                            }
                        }

                        if (difficultiesToStrike.Count > 0)
                        {
                            List<SegmentedControlCell> difficultyCells = _beatmapDifficultySegmentedControl.GetField<List<SegmentedControlCell>, SegmentedControl>("_cells");

                            foreach (BeatmapDifficulty difficulty in _beatmapDifficultySegmentedControlController.GetField<List<BeatmapDifficulty>, BeatmapDifficultySegmentedControlController>("_difficulties"))
                            {
                                if (difficultiesToStrike.Contains(difficulty))
                                {
                                    Plugin.logger.Debug("finding cell");
                                    SegmentedControlCell cell = difficultyCells[_beatmapDifficultySegmentedControlController.GetClosestDifficultyIndex(difficulty)];

                                    Transform background = cell.transform.GetChild(0);
                                    background.gameObject.SetActive(true);

                                    Plugin.logger.Debug("grabbing image view");
                                    var imageView = background.GetComponent<ImageView>();
                                    if (imageView)
                                    {
                                        // Grab the background color of background housing the difficulty labels
                                        Color backgroundColor = _config.alwaysEnabled ? _config.oldDotColor : _config.newDotColor;

                                        imageView.color0 = backgroundColor;
                                        imageView.color1 = backgroundColor;

                                        Plugin.logger.Debug("text width");
                                        float textWidth = Math.Min(((RectTransform) cell.transform).sizeDelta.x, cell.transform.GetChild(1).GetComponent<CurvedTextMeshPro>().preferredWidth);
                                        background.localPosition = new Vector3(-(textWidth / 2) - 3f, 0, 0);

                                        Plugin.logger.Debug("resizing");
                                        imageView.preserveAspect = true;
                                        imageView.SetImage("#NoArrowsIcon");
                                        background.localScale = new Vector3(0.25f / 3f * difficultyCells.Count, 0.625f, 1.0f);
                                        Plugin.logger.Debug("done");

                                    }
                                    else
                                    {
                                        Plugin.logger.Error("no image view");
                                    }

                                }
                            }
                        }

                    }
                }
            }
        }

        private void RemoveDotIcon()
        {
            if (_level != null)
            {
                foreach (IDifficultyBeatmapSet difficultyBeatmapSet in _level.beatmapLevelData.difficultyBeatmapSets)
                {
                    List<SegmentedControlCell> difficultyCells = _beatmapDifficultySegmentedControl.GetField<List<SegmentedControlCell>, SegmentedControl>("_cells");

                    foreach (BeatmapDifficulty difficulty in _beatmapDifficultySegmentedControlController.GetField<List<BeatmapDifficulty>, BeatmapDifficultySegmentedControlController>("_difficulties"))
                    {
                        SegmentedControlCell cell = difficultyCells[_beatmapDifficultySegmentedControlController.GetClosestDifficultyIndex(difficulty)];

                        Transform background = cell.transform.GetChild(0);
                        background.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnBeatmapDifficultySegmentedControlDidSelectCellEvent(SegmentedControl arg1, int arg2)
        {
            throw new NotImplementedException();
        }

        private void OnBeatmapDifficultySegmentedControlControllerSetDataEvent()
        {
            AddDotIcon();
        }

        public void Dispose()
        {
            StandardLevelDetailViewSetContentPatch.LevelSelectedEvent -= OnLevelUpdated;
            _beatmapCharacteristicSegmentedControl.didSelectCellEvent -= OnBeatmapCharacteristicSegmentedControlDidSelectCellEvent;
            //_beatmapDifficultySegmentedControl.didSelectCellEvent -= OnBeatmapDifficultySegmentedControlDidSelectCellEvent;
            BeatmapDifficultySegmentedControlControllerSetDataPatch.BeatmapDifficultySegmentedControlControllerSetDataEvent -= OnBeatmapDifficultySegmentedControlControllerSetDataEvent;
            //_config.ChangeEvent -= OnConfigChanged;
        }
    }
}
