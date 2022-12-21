using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using UnityEngine;

namespace StarCraftCore.Scripts.Patches
{
    [HarmonyPatch(typeof (DialogueParser), "GetColorFromCode", new System.Type[] {typeof (string), typeof(Color)})]
    public class DialogueParser_GetColorFromCode
    {
        public static bool Prefix(DialogueParser __instance, string code, Color defaultColor, ref Color __result)
        {
            string stringValue = DialogueParser.GetStringValue(code, "c");
            switch (stringValue)
            {
                case "purple":
                    __result = new Color(0.3f, 0, 0.7f);
                    return false;
                case "light_green":
                    __result = new Color(0, 0.75f, 0);
                    return false;
            }

            return true;
        }
    }
}