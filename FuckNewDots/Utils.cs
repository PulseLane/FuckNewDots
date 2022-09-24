using BeatmapSaveDataVersion3;
using IPA.Loader;
using IPA.Utilities;
using Polyglot;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FuckNewDots
{
    internal static class Utils
    {
        private static Dictionary<string, BeatmapCharacteristicSO> _characteristicMap = new Dictionary<string, BeatmapCharacteristicSO>();

        private static Dictionary<string, Sprite> _spriteMap = new Dictionary<string, Sprite>();
        private static Texture2D _textureAtlas = null;
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
            if (_characteristicMap.ContainsKey(oldBeatmapCharacteristicSO.serializedName))
            {
                return _characteristicMap[oldBeatmapCharacteristicSO.serializedName];
            }
            BeatmapCharacteristicSO beatmapCharacteristicSO = BeatmapCharacteristicSO.Instantiate(oldBeatmapCharacteristicSO);

            Sprite icon = GetIcon(oldBeatmapCharacteristicSO.characteristicNameLocalizationKey);
            if (icon == null)
            {
                Plugin.logger.Error("sprite was null");
            }
            string name = beatmapCharacteristicSO.GetField<string, BeatmapCharacteristicSO>("_serializedName") + "OldDots";
            string description = Localization.Get(beatmapCharacteristicSO.descriptionLocalizationKey) + " using old dot note hitboxes";

            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, Sprite>("_icon", icon);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_characteristicNameLocalizationKey", name);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_descriptionLocalizationKey", description);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_serializedName", name);
            beatmapCharacteristicSO.SetField<BeatmapCharacteristicSO, string>("_compoundIdPartName", name);

            _characteristicMap[oldBeatmapCharacteristicSO.serializedName] = beatmapCharacteristicSO;
            return beatmapCharacteristicSO;
        }

        public static bool ContainsDotNotes(IDifficultyBeatmap difficultyBeatmap)
        {
            BeatmapSaveData beatmapSaveData = ((CustomDifficultyBeatmap) difficultyBeatmap).beatmapSaveData;
            return beatmapSaveData.colorNotes.Any(x => x.cutDirection == NoteCutDirection.Any);
        }

        public static int CountDotNotes(IDifficultyBeatmap difficultyBeatmap)
        {
            BeatmapSaveData beatmapSaveData = ((CustomDifficultyBeatmap) difficultyBeatmap).beatmapSaveData;
            return beatmapSaveData.colorNotes.Where(x => x.cutDirection == NoteCutDirection.Any).Count();
        }

        public static Sprite GetIcon(string characteristicName)
        {
            const string backgroundSpriteName = "NoArrowsIcon";
            string foregroundSpriteName = "";

            bool useTextureAtlas = true;

            switch (characteristicName)
            {
                case "LEVEL_360DEGREE":
                    foregroundSpriteName = "360DegreesIcon";
                    break;
                case "LEVEL_90DEGREE":
                    foregroundSpriteName = "90DegreesIcon";
                    break;
                case "LEVEL_ONE_SABER":
                    foregroundSpriteName = "SingleSaberIcon";
                    break;
                case "LEVEL_NO_ARROWS":
                    foregroundSpriteName = "ProModeIcon";
                    break;
                case "Lightshow":
                    foregroundSpriteName = "SongCore.Icons.Lightshow.png";
                    useTextureAtlas = false;
                    break;
                case "Lawless":
                    foregroundSpriteName = "SongCore.Icons.ExtraDiffsIcon.png";
                    useTextureAtlas = false;
                    break;
                default:
                    foregroundSpriteName = "StandardBeatmapCharacteristicIcon";
                    break;
            }

            return CreateSprite(foregroundSpriteName, backgroundSpriteName, useTextureAtlas);
        }

        private static Sprite CreateSprite(string foregroundSpriteName, string backgroundSpriteName, bool useTextureAtlasForForeground = true)
        {
            if (_spriteMap.ContainsKey(foregroundSpriteName))
            {
                return _spriteMap[foregroundSpriteName];
            }

            Sprite foregroundSprite;
            if (!useTextureAtlasForForeground)
            {
                foregroundSprite = GetExternalSprite(foregroundSpriteName);
            }
            else
            {
                foregroundSprite = FindSpriteCached(foregroundSpriteName);
            }

            Sprite backgroundSprite = FindSpriteCached(backgroundSpriteName);

            if (foregroundSprite && backgroundSprite)
            {
                if (_textureAtlas == null)
                {
                    _textureAtlas = GetReadableTexture(backgroundSprite.texture);
                }

                Texture2D newTexture;
                if (!useTextureAtlasForForeground)
                {
                    newTexture = CreateOldDotsTexture(foregroundSprite.textureRect, backgroundSprite.textureRect, GetReadableTexture(foregroundSprite.texture));
                }
                else
                {
                    newTexture = CreateOldDotsTexture(foregroundSprite.textureRect, backgroundSprite.textureRect);
                }

                Sprite sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(0.5f, 0.5f), foregroundSprite.pixelsPerUnit);
                _spriteMap[foregroundSpriteName] = sprite;
                return sprite;
            }
            else
            {
                if (!foregroundSprite)
                {
                    Plugin.logger.Error($"Could not find sprite: {foregroundSpriteName}");
                }
                if (!backgroundSprite)
                {
                    Plugin.logger.Error($"Could not find sprite: {backgroundSpriteName}");
                }
                return null;
            }
        }

        // Not sure why bsml doesn't expose this function
        private static Sprite FindSpriteCached(string name)
        {
            if (BeatSaberMarkupLanguage.Utilities.spriteCache.TryGetValue(name, out var sprite) && sprite != null)
                return sprite;

            foreach (var x in Resources.FindObjectsOfTypeAll<Sprite>())
            {
                if (x.name.Length == 0)
                    continue;

                if (!BeatSaberMarkupLanguage.Utilities.spriteCache.TryGetValue(x.name, out var a) || a == null)
                    BeatSaberMarkupLanguage.Utilities.spriteCache[x.name] = x;

                if (x.name == name)
                    sprite = x;
            }

            return sprite;
        }

        private static Texture2D GetReadableTexture(Texture2D source)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            UnityEngine.Graphics.Blit(source, renderTexture);

            RenderTexture previousTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D readableTexture = new Texture2D(source.width, source.height);
            readableTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            readableTexture.Apply();

            RenderTexture.active = previousTexture;
            RenderTexture.ReleaseTemporary(renderTexture);

            return readableTexture;
        }

        private static Texture2D CreateOldDotsTexture(Rect foregroundRect, Rect backgroundRect, Texture2D foregroundTexture = null, Texture2D backgroundTexture = null)
        {
            Texture2D texture = new Texture2D((int) backgroundRect.width, (int) backgroundRect.height);
            texture.filterMode = _textureAtlas.filterMode;
            texture.wrapMode = _textureAtlas.wrapMode;

            if (foregroundRect.width < backgroundRect.width)
            {
                foregroundRect.x -= ((backgroundRect.width - foregroundRect.width) / 2);
            }
            if (foregroundRect.height < backgroundRect.height)
            {
                foregroundRect.y -= ((backgroundRect.height - foregroundRect.height) / 2);
            }

            if (foregroundTexture == null)
            {
                foregroundTexture = _textureAtlas;
            }
            if (backgroundTexture == null)
            {
                backgroundTexture = _textureAtlas;
            }

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color foregroundColor;
                    Color backgroundColor;

                    foregroundColor = foregroundTexture.GetPixel(x + (int) foregroundRect.x, y + (int) foregroundRect.y);
                    backgroundColor = backgroundTexture.GetPixel(x + (int) backgroundRect.x, y + (int) backgroundRect.y);

                    texture.SetPixel(x, y, MixColors(foregroundColor, backgroundColor));
                }
            }

            texture.Apply();
            return texture;
        }

        private static Color MixColors(Color colorA, Color colorB, float colorAWeight = 9, float colorBWeight = 1, float colorBColor = 0.6f)
        {
            float combinedAlpha = colorA.a * colorAWeight + colorB.a * colorBWeight;
            float weightedColorA = (colorA.a * colorAWeight) / combinedAlpha;
            float weightedColorB = (colorB.a * colorBWeight) / combinedAlpha;

            return new Color(colorA.r * weightedColorA + colorBColor * weightedColorB, colorA.g * weightedColorA + colorBColor * weightedColorB, colorA.b * weightedColorA + colorBColor * weightedColorB, Mathf.Max(colorA.a, colorB.a));
        }


        #region external sprite loading
        // No need to cache for now -> only used when creating olddots icon, which itself is cached
        private static Sprite GetExternalSprite(string name)
        {
            PluginMetadata metadata = PluginManager.GetPluginFromId(name.Split('.')[0]);
            Assembly assembly = metadata.Assembly;

            return LoadSpriteFromExternalAssembly(assembly, name);
        }

        private static Sprite LoadSpriteFromExternalAssembly(Assembly assembly, string resourcePath, float pixelsPerUnit = 100f)
        {
            byte[] data = GetResource(assembly, resourcePath);
            return LoadSpriteRaw(data, pixelsPerUnit);
        }

        private static byte[] GetResource(Assembly assembly, string resourceName)
        {
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            return ReadStream(stream);
        }

        private static byte[] ReadStream(Stream stream)
        {
            byte[] data = new byte[(int) stream.Length];
            stream.Read(data, 0, (int) stream.Length);
            return data;
        }

        private static Sprite LoadSpriteRaw(byte[] data, float pixelsPerUnit = 100f)
        {
            return LoadSpriteFromTexture(LoadTextureRaw(data), pixelsPerUnit);
        }

        private static Texture2D LoadTextureRaw(byte[] data)
        {
            if (data.Count() > 0)
            {
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(data))
                {
                    return texture;
                }
            }

            return null;
        }

        private static Sprite LoadSpriteFromTexture(Texture2D texture, float pixelsPerUnit)
        {
            if (texture != null)
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), pixelsPerUnit);
            }

            return null;
        }
        #endregion
    }
}
