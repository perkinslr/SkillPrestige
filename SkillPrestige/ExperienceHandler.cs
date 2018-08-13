using System.Collections.Generic;
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

        private static decimal[] LastExperiencePoints { get; set; }

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
            if (DisableExperienceGains || !PerSaveOptions.Instance.UseExperienceMultiplier) return;
            if (!ExperienceLoaded)
            {
                ExperienceLoaded = true;
                LastExperiencePoints = new List<decimal>(Game1.player.experiencePoints.Select(skill_xp => (decimal)skill_xp)).ToArray();
                Logger.LogVerbose("Loaded Experience state.");
                return;

            }
            for (var skillIndex = 0; skillIndex < Game1.player.experiencePoints.Length; skillIndex++)
            {
                var lastExperienceDetected = LastExperiencePoints[skillIndex];
                var currentExperience = Game1.player.experiencePoints[skillIndex];
                var gainedExperience = currentExperience - (int)lastExperienceDetected;
                decimal skillExperienceFactor = 0.0M;
                Prestige activePrestige = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.SingleOrDefault(x => x.SkillType.Ordinal == skillIndex);
                if (activePrestige == null) continue;
                skillExperienceFactor = activePrestige.PrestigePoints * PerSaveOptions.Instance.ExperienceMultiplier;
                if (gainedExperience < 0)
                {
                    /* probably means we prestiged, need to reload the value */
                    LastExperiencePoints[skillIndex] = (decimal)currentExperience;
                    return;
                }
                if (gainedExperience == 0 || skillExperienceFactor <= 0) continue;
                Logger.LogVerbose($"Detected {gainedExperience} experience gained in {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                var extraExperience = (gainedExperience * skillExperienceFactor) + lastExperienceDetected - (int)lastExperienceDetected;
                Logger.LogVerbose($"Adding {extraExperience} experience to {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                LastExperiencePoints[skillIndex] = (int)LastExperiencePoints[skillIndex] + gainedExperience + extraExperience;
                Game1.player.gainExperience(skillIndex, (int)extraExperience);
            }
        }
    }
}
