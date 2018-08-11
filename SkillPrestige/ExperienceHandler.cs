using System.Linq;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige
{
    /// <summary>
    /// Handles experience adjustments for skills.
    /// </summary>
    public static class ExperienceHandler
    {
        private static bool _disableExperienceGains;

        private static bool ExperienceLoaded { get; set; }

        private static int[] LastExperiencePoints { get; set; }

        public static bool DisableExperienceGains
        {
            private get => _disableExperienceGains;
            set
            {
                if (_disableExperienceGains != value) Logger.LogInformation($"{(value ? "Enabling" : "Disabling")} experience gains from prestige points...");
                _disableExperienceGains = value;
                ResetExperience();
            }
        }

        public static void ResetExperience()
        {
            ExperienceLoaded = false;
            LastExperiencePoints = null;
        }

        public static void UpdateExperience()
        {
            
        }
    }
}
