using APIPlugin;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Card;

namespace StarCraftCore.Scripts.Patches
{
    [HarmonyPatch(typeof (Card), "SetInfo", new System.Type[] {typeof (CardInfo)})]
    public class Card_SetInfo
    {
        public static void Postfix(CardInfo info, Card __instance)
        {
            // Fixes Zerglings portrait not changing from 2 to 4 when buffing the health at the campfire
            foreach (IPortraitChanges portraitChanges in __instance.gameObject.GetComponents<IPortraitChanges>())
            {
                if (portraitChanges.ShouldRefreshPortrait())
                {
                    portraitChanges.RefreshPortrait();
                }
            }
        }
    }
}