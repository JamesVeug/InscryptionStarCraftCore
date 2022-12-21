using System;
using System.Collections.Generic;
using DiskCardGame;
using UnityEngine;
using StarCraftCore.Scripts.Abilities;

namespace StarCraftCore.Scripts.Patches
{
    public static class Utils
    {
        public static bool OpponentHasADetector(PlayableCard card)
        {
            List<CardSlot> opponentSlots = Singleton<BoardManager>.Instance.GetSlots(card.OpponentCard);
            foreach (CardSlot slot in opponentSlots)
            {
                if (slot != null && slot.Card != null && slot.Card.HasAbility(DetectorAbility.ability))
                {
                    return true;
                }
            }

            return false;
        }
    }
}