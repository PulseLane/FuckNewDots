using HarmonyLib;
using IPA;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using IPALogger = IPA.Logging.Logger;

namespace FuckNewDots
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static IPALogger logger;
        internal static Config config;

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
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchSelf();
        }
    }
}