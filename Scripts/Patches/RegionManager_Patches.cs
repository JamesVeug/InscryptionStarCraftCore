using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Regions;
using StarCraftCore.Scripts.Challenges;
using StarCraftCore.Scripts.Regions;

namespace StarCraftCore.Scripts.Patches
{
    // Force region to always be char
    [HarmonyPatch(typeof (RegionManager), "GetRandomRegionFromTier", typeof(int))]
    public class RegionManager_GetRandomRegionFromTier
    {
        public static bool Prefix(int tier, ref RegionData __result)
        {
            if (AscensionSaveData.Data.ChallengeIsActive(CharRegionOnlyChallenge.Challenge.Challenge.challengeType))
            {
                __result = CharRegion.regionData;
                return false;
            }

            return true;
        }
    }
}