using System;
using System.Collections;
using System.Collections.Generic;
using DiskCardGame;
using Pixelplacement;
using UnityEngine;
using StarCraftCore.Scripts.Data.Sigils;
using UnityEngine.UIElements;

namespace StarCraftCore.Scripts.Abilities
{
    public class DecayAbility : ACustomAbilityBehaviour<DecayAbility, AbilityData>
    {
	    public override Ability Ability => ability;
	    public static Ability ability = Ability.None;

	    private int turnsRemaining = 3;
	    
        public static void Initialize(Type declaringType)
        {
	        ability = InitializeBase(Plugin.PluginGuid, declaringType, Plugin.Directory);
        }

        public override bool RespondsToTurnEnd(bool playerTurnEnd)
        {
	        return Card.OpponentCard != playerTurnEnd;
        }

        public override IEnumerator OnTurnEnd(bool playerTurnEnd)
        {
	        turnsRemaining -= 1;

	        

	        if (turnsRemaining > 0)
	        {
		        UpdateArt();
	        }
	        else
	        {
		        yield return Card.Die(false);
	        }
        }

        private void UpdateArt()
        {
	        // TODO:
        }
    }
}