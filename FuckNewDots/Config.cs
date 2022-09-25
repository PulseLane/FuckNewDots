using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace FuckNewDots
{
    internal class Config
    {
        internal Action ChangeEvent;
        public virtual bool alwaysEnabled { get; set; } = false;
        public virtual bool addCustomCharacteristic { get; set; } = true;
        public virtual bool addWarningIcon { get; set; } = true;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color newDotColor { get; set; } = Color.green;

        [UseConverter(typeof(HexColorConverter))]
        public virtual Color oldDotColor { get; set; } = Color.red;

        public virtual void Changed() { ChangeEvent?.Invoke(); }
    }
}
