using IPA.Utilities;
using Polyglot;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FuckNewDots
{
    internal static class Utils
    {
        #region Icons
        private static string ICON_360 = "FuckNewDots.Resources.dot360.png";
        private static string ICON_90 = "FuckNewDots.Resources.dot90.png";
        private static string ICON_STANDARD = "FuckNewDots.Resources.dotstandard.png";
        private static string ICON_SINGLE = "FuckNewDots.Resources.dotsingle.png";
        private static string ICON_LIGHTSHOW = "FuckNewDots.Resources.dotlightshow.png";
        private static string ICON_LAWLESS = "FuckNewDots.Resources.dotlawless.png";
        private static string ICON_NO_ARROWS = "FuckNewDots.Resources.dotnoarrow.png";

        private static Sprite _icon360;
        private static Sprite _icon90;
        private static Sprite _iconStandard;
        private static Sprite _iconSingle;
        private static Sprite _iconLightshow;
        private static Sprite _iconLawless;
        private static Sprite _iconNoArrows;

        private static Sprite Icon360
        {
            get
            {
                if (_icon360 == null)
                {
                    _icon360 = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_360);
                }

                return _icon360;
            }
        }

        private static Sprite Icon90
        {
            get
            {
                if (_icon90 == null)
                {
                    _icon90 = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_90);
                }

                return _icon90;
            }
        }

        private static Sprite IconStandard
        {
            get
            {
                if (_iconStandard == null)
                {
                    _iconStandard = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_STANDARD);
                }

                return _iconStandard;
            }
        }

        private static Sprite IconSingle
        {
            get
            {
                if (_iconSingle == null)
                {
                    _iconSingle = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_SINGLE);
                }

                return _iconSingle;
            }
        }

        private static Sprite IconLightshow
        {
            get
            {
                if (_iconLightshow == null)
                {
                    _iconLightshow = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_LIGHTSHOW);
                }

                return _iconLightshow;
            }
        }

        private static Sprite IconLawless
        {
            get
            {
                if (_iconLawless == null)
                {
                    _iconLawless = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_LAWLESS);
                }

                return _iconLawless;
            }
        }

        private static Sprite IconNoArrows
        {
            get
            {
                if (_iconNoArrows == null)
                {
                    _iconNoArrows = SongCore.Utilities.Utils.LoadSpriteFromResources(ICON_NO_ARROWS);
                }

                return _iconNoArrows;
            }
        }
        #endregion

        public static BeatmapCharacteristicSO CreateBeatmapCharacteristicSO(BeatmapCharacteristicSO oldBeatmapCharacteristicSO)
        {
            BeatmapCharacteristicSO beatmapCharacteristicSO =  BeatmapCharacteristicSO.Instantiate(oldBeatmapCharacteristicSO);

            Sprite icon = GetIcon(oldBeatmapCharacteristicSO.characteristicNameLocalizationKey);
            string name = beatmapCharacteristicSO.GetField<string, BeatmapCharacteristicSO>("_serializedName") + "OldDots";
            string description = Localization.Get(beatmapCharacteristicSO.descriptionLocalizationKey) + " using old dot note hitboxes";

            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, Sprite>("_icon", icon);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_characteristicNameLocalizationKey", name);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_descriptionLocalizationKey", description);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_serializedName", name);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_compoundIdPartName", name);

            return beatmapCharacteristicSO;
        }

        private static Sprite GetIcon(string characteristicName)
        {
            switch (characteristicName)
            {
                case "LEVEL_360DEGREE":
                    return Icon360;
                case "LEVEL_90DEGREE":
                    return Icon90;
                case "LEVEL_ONE_SABER":
                    return IconSingle;
                case "Lightshow":
                    return IconLightshow;
                case "Lawless":
                    return IconLawless;
                case "LEVEL_NO_ARROWS":
                    return IconNoArrows;
                default:
                    return IconStandard;
            }
        }
    }
}
