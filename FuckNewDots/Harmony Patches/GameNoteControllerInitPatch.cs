using HarmonyLib;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FuckNewDots.Harmony_Patches
{
    [HarmonyPatch(typeof(GameNoteController), "Init")]
    internal class GameNoteControllerInitPatch
    {
        internal static bool active = false;

        public static void Postfix(NoteData noteData, ref BoxCuttableBySaber[] ____bigCuttableBySaberList)
        {
            if (!active)
            {
                return;
            }

            if (noteData.cutDirection == NoteCutDirection.Any)
            {
                foreach (BoxCuttableBySaber boxCuttableBySaber in ____bigCuttableBySaberList)
                {
                    if (Mathf.Approximately(boxCuttableBySaber.colliderSize.y, 0.80f))
                    {
                        BoxCollider collider = boxCuttableBySaber.GetField<BoxCollider, BoxCuttableBySaber>("_collider");
                        collider.SetProperty<BoxCollider, Vector3>("size", new Vector3(collider.size.x, 0.50f, collider.size.z));
                    }
                }
            }
        }
    }
}
