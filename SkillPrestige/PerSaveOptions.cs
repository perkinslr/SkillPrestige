using System;
using System.IO;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using SkillPrestige.Logging;

namespace SkillPrestige
{
    /// <summary>
    /// Represents options for this mod per save file.
    /// </summary>
    [Serializable]
    public class PerSaveOptions
    {
        /// <summary>
        /// Whether or not to reset the recipes of a skill on load.
        /// </summary>
        public bool ResetRecipesOnPrestige { get; set; }

        public bool UseExperienceMultiplier { get; set; }

        public decimal ExperienceMultiplier { get; private set; }

        public int CostOfTierOnePrestige { get; set; }

        public int CostOfTierTwoPrestige { get; set; }

        /// <summary>
        /// Number of prestige points gained per prestige of a skill.
        /// </summary>
        public int PointsPerPrestige { get; set; }
        
        /// <summary>
        /// A mode where the player pays for prestige via the points gained after reaching level 10, never resetting to 0.
        /// </summary>
        public bool PainlessPrestigeMode { get; set; }

        public int ExperienceNeededPerPainlessPrestige { get; set; }

        private PerSaveOptions() { }
        private static PerSaveOptions _instance;

        public static PerSaveOptions Instance 
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new PerSaveOptions();
                LoadPerSaveOptions();
                return _instance;
            }
        }

        private static void LoadPerSaveOptions()
        {
            Logger.LogInformation($"per save options file path: {SkillPrestigeMod.PerSaveOptionsPath}");
            _instance = SkillPrestigeMod.ModHelper.ReadJsonFile<PerSaveOptions>(SkillPrestigeMod.PerSaveOptionsPath) ?? SetupPerSaveOptionsFile();
        }

        private static PerSaveOptions SetupPerSaveOptionsFile()
        {
            Logger.LogInformation("Creating new options file...");
            try
            {
                Instance.ResetRecipesOnPrestige =  true;
                Instance.UseExperienceMultiplier =  false;
                Instance.ExperienceMultiplier =  0.1m;
                Instance.CostOfTierOnePrestige =  1;
                Instance.CostOfTierTwoPrestige =  2;
                Instance.PointsPerPrestige =  1;
                Instance.ExperienceNeededPerPainlessPrestige = 15000;
                Save();
            }
            catch(Exception exception)
            {
                Logger.LogError($"Error while attempting to create a per save options file. {Environment.NewLine} {exception}");
                throw;
            }
            Logger.LogInformation("Successfully created new per save options file.");
            return Instance;
        }

        public static void Save()
        {
            SkillPrestigeMod.ModHelper.WriteJsonFile(SkillPrestigeMod.PerSaveOptionsPath, _instance);
            Logger.LogInformation("Per save options file saved.");
        }

        public static void ClearLoadedPerSaveOptionsFile()
        {
            _instance = null;
        }

        /// <summary>
        /// Empty procedure to force the lazy load of the instance.
        /// </summary>
        // ReSharper disable once MemberCanBeMadeStatic.Global - the whole point of this is to force the load of the instance.
        public void Check() { }
    }
}
