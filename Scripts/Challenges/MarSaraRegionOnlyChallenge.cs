using System.IO;
using InscryptionAPI.Ascension;
using UnityEngine;

namespace StarCraftCore.Scripts.Challenges
{
	public class MarSaraRegionOnlyChallenge
	{
		public static ChallengeManager.FullChallenge Challenge;
		public static void Initialize()
		{
			Challenge = ChallengeManager.Add
			(
				Plugin.PluginGuid,
				"Planet Mar Sara",
				"You're forced to survive on the desert planet Mar Sara",
				5,
				StarCraftCore.Utils.GetTextureFromPath("Artwork/Challenges/ascensionicon_marsara.png", Plugin.Directory),
				StarCraftCore.Utils.GetTextureFromPath("Artwork/Challenges/ascensionicon_marsara_activated.png", Plugin.Directory)
			);
		}
	}
}