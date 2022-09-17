using FuckNewDots.Harmony_Patches;
using SiraUtil.Submissions;
using System;
using System.Collections.Generic;
using System.Text;
using Zenject;

namespace FuckNewDots.Managers
{
    internal class OldDotManager : IInitializable, IDisposable
    {
        private Config _config;
        private Submission _siraSubmission;
        private IDifficultyBeatmap _difficultyBeatmap;

        public OldDotManager(Config config, Submission submission, IDifficultyBeatmap difficultyBeatmap)
        {
            Plugin.logger.Debug("olddotmanager ctor");
            _config = config;
            _siraSubmission = submission;

            _difficultyBeatmap = difficultyBeatmap;
        }

        public void Initialize()
        {
            bool usingCharacteristic = _difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName.EndsWith("OldDots");
            Plugin.logger.Debug(_difficultyBeatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName);
            if (_config.alwaysEnabled || usingCharacteristic)
            {
                Plugin.logger.Debug("Old dots enabled");
                GameNoteControllerInitPatch.active = true;
                if (_config.alwaysEnabled && !usingCharacteristic)
                {
                    Plugin.logger.Debug("Score submission disabled");
                    _siraSubmission.DisableScoreSubmission("OldDots", "Using old dot note hitboxes with a nonexempt characteristic");
                }
            }
        }

        public void Dispose()
        {
            GameNoteControllerInitPatch.active = false;
        }
    }
}
