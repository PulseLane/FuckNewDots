using FuckNewDots.Managers;
using FuckNewDots.UI;
using Zenject;

namespace FuckNewDots.Installers
{
    internal class MenuInstaller : Installer
    {
        private readonly Config _config;

        public MenuInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();

            Container.BindInterfacesTo<DotIconManager>().AsSingle().NonLazy();
            Container.BindInterfacesTo<ModifierUI>().AsSingle().NonLazy();
        }
    }
}
