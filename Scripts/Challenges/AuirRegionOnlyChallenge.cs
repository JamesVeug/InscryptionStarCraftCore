using System.IO;
using InscryptionAPI.Ascension;
using UnityEngine;

namespace StarCraftCore.Scripts.Challenges
{
	public class AuirRegionOnlyChallenge
	{
		public static ChallengeManager.FullChallenge Challenge;
		public static void Initialize()
		{
			Challenge = ChallengeManager.Add
			(
				Plugin.PluginGuid,
				"My life for Auir",
				"You're sent to Auir to defeat all bosses.",
				5,
				StarCraftCore.Utils.GetTextureFromPath("Artwork/Challenges/ascensionicon_auir.png", Plugin.Directory),
				StarCraftCore.Utils.GetTextureFromPath("Artwork/Challenges/ascensionicon_auir_activated.png", Plugin.Directory)
			);
		}
	}
}