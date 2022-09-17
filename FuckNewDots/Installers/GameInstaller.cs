using System;
using System.Collections.Generic;
using System.Text;
using FuckNewDots.Managers;
using Zenject;

namespace FuckNewDots.Installers
{
    internal class GameInstaller : Installer
    {
        private readonly Config _config;

        public GameInstaller(Config config)
        {
            _config = config;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_config).AsSingle();

            Container.BindInterfacesTo<OldDotManager>().AsSingle().NonLazy();
        }
    }
}
