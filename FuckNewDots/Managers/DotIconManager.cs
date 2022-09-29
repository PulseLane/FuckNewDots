using BeatSaberMarkupLanguage;
using FuckNewDots.Harmony_Patches;
using HMUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using static BeatSaberMarkupLanguage.BeatSaberUI;

namespace FuckNewDots.Managers
{
    internal class DotIconManager : IInitializable, IDisposable
    {
        internal static Dictionary<string, int[]> levelsWithDots = new Dictionary<string, int[]>();

        private Config _config;

        private StandardLevelDetailView _standardLevelDetailView;
        private LevelParamsPanel _levelParamsPanel;

        private readonly IconSegmentedControl _beatmapCharacteristicSegmentedControl;

        private bool _warningIconSet = false;

        public DotIconManager(Config config, StandardLevelDetailViewController standardLevelDetailViewController)
        {
            _config = config;
            _standardLevelDetailView = standardLevelDetailViewController.GetField<StandardLevelDetailView, StandardLevelDetailViewController>("_standardLevelDetailView");
            _levelParamsPanel = _standardLevelDetailView.GetField<LevelParamsPanel, StandardLevelDetailView>("_levelParamsPanel");

            BeatmapCharacteristicSegmentedControlController beatmapCharacteristicSegmentedControlController = _standardLevelDetailView.GetField<BeatmapCharacteristicSegmentedControlController, StandardLevelDetailView>("_beatmapCharacteristicSegmentedControlController");
            _beatmapCharacteristicSegmentedControl = beatmapCharacteristicSegmentedControlController.GetField<IconSegmentedControl, BeatmapCharacteristicSegmentedControlController>("_segmentedControl");
        }

        public void Initialize()
        {
            StandardLevelDetailViewSetContentPatch.LevelSelectedEvent += OnLevelUpdated;
            _beatmapCharacteristicSegmentedControl.didSelectCellEvent += OnBeatmapCharacteristicSegmentedControlDidSelectCellEvent;
            BeatmapDifficultySegmentedControlControllerSetDataPatch.BeatmapDifficultySegmentedControlControllerSetDataEvent += OnBeatmapDifficultySegmentedControlControllerSetDataEvent;
        }

        private void OnLevelUpdated(IBeatmapLevel level)
        {
            AddDotIcon();
        }

        private void OnBeatmapCharacteristicSegmentedControlDidSelectCellEvent(SegmentedControl _, int __)
        {
            AddDotIcon();
        }

        private void OnBeatmapDifficultySegmentedControlControllerSetDataEvent()
        {
            AddDotIcon();
        }

        private void AddDotIcon()
        {
            if (!_config.addWarningIcon)
            {
                RemoveDotIcon();
                return;
            }

            IDifficultyBeatmap difficultyBeatmap = _standardLevelDetailView.selectedDifficultyBeatmap;
            if (difficultyBeatmap == null)
            {
                return;
            }

            int dotCount = Utils.CountDotNotes(difficultyBeatmap);
            if (dotCount > 0)
            {
                TextMeshProUGUI noteCountTextMesh = _levelParamsPanel.GetField<TextMeshProUGUI, LevelParamsPanel>("_notesCountText");
                noteCountTextMesh.richText = true;
                noteCountTextMesh.text += $" <size=50%>({dotCount})</size>";

                Transform notesCountTransform = _levelParamsPanel.transform.GetChild(1);
                if (notesCountTransform.name != "NotesCount")
                {
                    return;
                }

                ImageView imageView = notesCountTransform.GetChild(0).GetComponent<ImageView>();
                if (difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName.EndsWith("OldDots"))
                {
                    ResetIcon(imageView);
                }
                else
                {
                    imageView.SetImage("#NoArrowsIcon");

                    Color backgroundColor = _config.alwaysEnabled ? _config.oldDotColor : _config.newDotColor;
                    imageView.color = backgroundColor;
                }
                _warningIconSet = true;
            }
            else
            {
                RemoveDotIcon();
            }
        }

        private void RemoveDotIcon()
        {
            if (!_warningIconSet)
            {
                return;
            }

            IDifficultyBeatmap difficultyBeatmap = _standardLevelDetailView.selectedDifficultyBeatmap;
            if (difficultyBeatmap == null)
            {
                return;
            }

            TextMeshProUGUI noteCountTextMesh = _levelParamsPanel.GetField<TextMeshProUGUI, LevelParamsPanel>("_notesCountText");
            noteCountTextMesh.richText = false;
            noteCountTextMesh.text = noteCountTextMesh.text.Split(' ')[0];

            Transform notesCountTransform = _levelParamsPanel.transform.GetChild(1);
            if (notesCountTransform.name == "NotesCount")
            {
                ImageView imageView = notesCountTransform.GetChild(0).GetComponent<ImageView>();
                ResetIcon(imageView);

                _warningIconSet = false;
            }
        }

        private void ResetIcon(ImageView imageView)
        {
            imageView.SetImage("#GameNoteIcon");
            imageView.color = Color.white;
        }

        public void Dispose()
        {
            StandardLevelDetailViewSetContentPatch.LevelSelectedEvent -= OnLevelUpdated;
            _beatmapCharacteristicSegmentedControl.didSelectCellEvent -= OnBeatmapCharacteristicSegmentedControlDidSelectCellEvent;
            BeatmapDifficultySegmentedControlControllerSetDataPatch.BeatmapDifficultySegmentedControlControllerSetDataEvent -= OnBeatmapDifficultySegmentedControlControllerSetDataEvent;
        }
    }
}
