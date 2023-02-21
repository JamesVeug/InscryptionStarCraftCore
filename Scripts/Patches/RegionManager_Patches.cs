using System.Collections.Generic;
using System.Linq;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Regions;
using StarCraftCore.Scripts.Challenges;
using StarCraftCore.Scripts.Regions;

namespace StarCraftCore.Scripts.Patches
{
    // Force region to always be char
    [HarmonyPatch(typeof (RegionManager), "GetAllRegionsForMapGeneration")]
    public class RegionManager_GetAllRegionsForMapGeneration
    {
        public static bool Prefix(ref List<RegionData> __result)
        {
            List<RegionData> overrides = new List<RegionData>();
            if (AscensionSaveData.Data.ChallengeIsActive(CharRegionOnlyChallenge.Challenge.Challenge.challengeType))
            {
                overrides.Add(CharRegion.regionData);
            }
            if (AscensionSaveData.Data.ChallengeIsActive(AuirRegionOnlyChallenge.Challenge.Challenge.challengeType))
            {
                overrides.Add(AuirRegion.regionData);
            }
            if (AscensionSaveData.Data.ChallengeIsActive(MarSaraRegionOnlyChallenge.Challenge.Challenge.challengeType))
            {
                overrides.Add(MarSaraRegion.regionData);
            }

            if (overrides.Count == 0)
            {
                return true;
            }

            overrides.Randomize();
            __result = overrides;

            return false;
        }
    }
}