using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using InscryptionAPI.Dialogue;
using StarCraftCore.Scripts.Abilities;
using StarCraftCore.Scripts.Backgrounds;
using StarCraftCore.Scripts.Challenges;
using StarCraftCore.Scripts.Regions;
using UnityEngine;

namespace StarCraftCore
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("cyantist.inscryption.api", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
	    public const string PluginGuid = "jamesgames.inscryption.starcraftcore";
	    public const string PluginName = "StarCraft Core";
	    public const string PluginVersion = "1.3.1.0";

        public static string Directory;
        public static ManualLogSource Log;

        private void Awake()
        {
	        Log = Logger;
            Logger.LogInfo($"Loading {PluginName}...");
            Directory = this.Info.Location.Replace("StarCraftCore.dll", "");
            new Harmony(PluginGuid).PatchAll();

            // Backgrounds
            XelNagaBackground.Initialize();
	        
            // Abilities
            AbductAbility.Initialize(typeof(AbductAbility));
            ArmoredAbility.Initialize(typeof(ArmoredAbility));
            AssimilateAbility.Initialize(typeof(AssimilateAbility));
            BloodBankAbility.Initialize(typeof(BloodBankAbility));
            BlinkAbility.Initialize(typeof(BlinkAbility));
            DetectorAbility.Initialize(typeof(DetectorAbility));
            ExplodeAbility.Initialize(typeof(ExplodeAbility));
            HookAbility.Initialize(typeof(HookAbility));
            RandomConsumableOnDeathAbility.Initialize(typeof(RandomConsumableOnDeathAbility));
            RegenerateAbility.Initialize(typeof(RegenerateAbility));
            RicochetAbility.Initialize(typeof(RicochetAbility));
            SplashDamageAbility.Initialize(typeof(SplashDamageAbility));
            SwarmAbility.Initialize(typeof(SwarmAbility));
            MineralMiningAbility.Initialize(typeof(MineralMiningAbility));
            HealingAbility.Initialize(typeof(HealingAbility));

            // Dialogue Colors
            DialogueManager.AddColor(PluginGuid, "purple", new Color(0.3f, 0, 0.7f));
            DialogueManager.AddColor(PluginGuid, "light_green", new Color(0, 0.75f, 0));
        }

        private void Start()
        {
	        // Regions
	        AuirRegion.Initialize();
	        CharRegion.Initialize();
	        
	        // Challenges
	        AuirRegionOnlyChallenge.Initialize();
	        CharRegionOnlyChallenge.Initialize();
		        
	        Logger.LogInfo($"Loaded {PluginName}!");
        }
    }
}
