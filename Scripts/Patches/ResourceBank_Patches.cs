using HarmonyLib;
using StarCraftCore.Scripts.Regions;

namespace StarCraftCore.Scripts.Patches
{
    [HarmonyPatch(typeof (ResourceBank), "Awake", new System.Type[] {})]
    public class ResourceBank_Awake
    {
        public static void Postfix()
        {
            CharRegion.LoadGameObjects();
        }
    }
}