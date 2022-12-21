using System.IO;
using InscryptionAPI.Ascension;
using UnityEngine;

namespace StarCraftCore.Scripts.Challenges
{
	public class CharRegionOnlyChallenge
	{
		public static ChallengeManager.FullChallenge Challenge;
		public static void Initialize()
		{
			Challenge = ChallengeManager.Add
			(
				Plugin.PluginGuid,
				"Planet Char",
				"You're forced to survive on planet Char",
				15,
				StarCraftCore.Utils.GetTextureFromPath("Artwork/Challenges/ascensionicon_char.png", Plugin.Directory),
				StarCraftCore.Utils.GetTextureFromPath("Artwork/Challenges/ascensionicon_char_activated.png", Plugin.Directory)
			);
		}
	}
}