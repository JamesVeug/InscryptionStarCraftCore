using System;
using StarCraftCore.Scripts.Data.Sigils;

namespace StarCraftCore.Scripts.Data.Sigils
{
	[Serializable]
	public class StimpackAbilityData : ActivatedAbilityData
	{
		public int damage = 1;
		public int attackBoost = 2;
	}
}