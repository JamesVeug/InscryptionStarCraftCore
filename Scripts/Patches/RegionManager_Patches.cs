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
    [HarmonyPatch(typeof (RegionManager), "GetRandomRegionFromTier", typeof(int))]
    public class RegionManager_GetRandomRegionFromTier
    {
        public static bool Prefix(int tier, ref RegionData __result)
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

            if (overrides.Count > 0)
            {
                int randomseed = 0;
                if (SaveManager.SaveFile != null && RunState.Run != null && (!SaveFile.IsAscension || AscensionSaveData.Data != null))
                {
                    randomseed = SaveManager.SaveFile.randomSeed + (SaveFile.IsAscension ? AscensionSaveData.Data.currentRunSeed : (SaveManager.SaveFile.pastRuns.Count * 1000)) + (RunState.Run.regionTier + 1) * 100;
                }
                __result = overrides[SeededRandom.Range(0, overrides.Count, randomseed)];
                return false;
            }

            return true;
        }
    }
}