using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using SiraUtil.Submissions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using Zenject;

namespace FuckNewDots.UI
{
    internal class ModifierUI : IInitializable, IDisposable, INotifyPropertyChanged
    {    
        public event PropertyChangedEventHandler PropertyChanged;
        private Config _config;

        #region Config properties
        [UIValue("alwaysEnabled")]
        private bool alwaysEnabled
        {
            get => _config.alwaysEnabled;
            set
            {
                _config.alwaysEnabled = value;
                _config.Changed();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(alwaysEnabled)));
            }
        }

        [UIValue("characteristic")]
        private bool characteristic
        {
            get => _config.addCustomCharacteristic;
            set
            {
                _config.addCustomCharacteristic = value;
                _config.Changed();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(characteristic)));
            }
        }

        [UIValue("warning")]
        private bool warning
        {
            get => _config.addWarningIcon;
            set
            {
                _config.addWarningIcon = value;
                _config.Changed();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(warning)));
            }
        }

        [UIValue("newDotColor")]
        private Color newDotColor
        {
            get => _config.newDotColor;
            set
            {
                _config.newDotColor = value;
                _config.Changed();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(newDotColor)));
            }
        }

        [UIValue("oldDotColor")]
        private Color oldDotColor
        {
            get => _config.oldDotColor;
            set
            {
                _config.oldDotColor = value;
                _config.Changed();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(oldDotColor)));
            }
        }
        #endregion

        public ModifierUI(Config config)
        {
            _config = config;
        }

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("OldDots", "FuckNewDots.UI.Views.modifierUI.bsml", this, MenuType.Solo);
        }

        public void Dispose()
        {
            if (GameplaySetup.instance != null)
            {
                GameplaySetup.instance.RemoveTab("OldDots");
            }
        }
    }
}
