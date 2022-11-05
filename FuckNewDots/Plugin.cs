using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using System;
using System.Linq;
using IPALogger = IPA.Logging.Logger;

namespace FuckNewDots
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static IPALogger logger;
        internal static Config config;

        internal static bool isBSLInstalled;

        private readonly HarmonyLib.Harmony _harmony;
        private const string _harmonyID = "dev.PulseLane.FuckNewDots";

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector, IPA.Config.Config conf)
        {
            config = conf.Generated<Config>();
            Plugin.logger = logger;
            _harmony = new Harmony(_harmonyID);

            zenjector.Install<Installers.MenuInstaller>(Location.Menu, config);
            zenjector.Install<Installers.GameInstaller>(Location.StandardPlayer, config);
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll();
            CheckForBSLInstall();
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchSelf();
        }

        private bool CheckForBSLInstall()
        {
            try
            {
                var metadatas = PluginManager.EnabledPlugins.Where(x => x.Id == "BetterSongList");
                isBSLInstalled = metadatas.Count() > 0;
            }
            catch (Exception e)
            {
                logger.Debug($"Error checking for BSL install: {e.Message}");
                isBSLInstalled = false;
            }
            return isBSLInstalled;
        }
    }
}