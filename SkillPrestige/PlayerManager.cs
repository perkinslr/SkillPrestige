using SkillPrestige.Logging;
using SkillPrestige.SkillTypes;
using StardewValley;

namespace SkillPrestige
{
    /// <summary>
    /// A class to manage aspects of the player.
    /// </summary>
    public static class PlayerManager
    {
        public static void CorrectStats(Skill skillThatIsReset)
        {
            if (skillThatIsReset.Type != SkillType.Combat)
            {
                Logger.LogVerbose("Player Manager - no stats reset.");
            }
            else
            {
                Logger.LogVerbose($"Player Manager- Combat reset. Deducting MaxHP Level Bonus.");
                Game1.player.maxHealth -= 40;   
            }
            
        }

    }
}
