using System.Collections;
using DiskCardGame;

namespace StarCraftCore.Scripts.Utils
{
    public interface IMoreTriggers
    {
        bool RespondsToOtherSacrificed(PlayableCard playableCard);
        IEnumerator OnOtherSacrificed(PlayableCard playableCard);
    }
}