using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using SkillPrestige.Logging;
using StardewValley;
using Newtonsoft.Json.Linq;
using SkillPrestige.SkillTypes;

namespace SkillPrestige
{
    /// <summary>
    /// Represents the save file data for the Skill Prestige Mod.
    /// </summary>
    [Serializable]
    public class PrestigeSaveData
    {
        private const string DataFileName = @"Data.json";
        
        public static PrestigeSet CurrentlyLoadedPrestigeSet;

        private static PrestigeSaveData _instance;

        /// <summary>
        /// Set of prestige data saved per save file unique ID.
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global - no, it can't be made private or it won't be serialized.
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global - setter used by deserializer.
       
        private PrestigeSaveData()
        {
            CurrentlyLoadedPrestigeSet = PrestigeSet.CompleteEmptyPrestigeSet;
        }

        // ReSharper disable once MemberCanBePrivate.Global - used publically, resharper is wrong.
        public static PrestigeSaveData Instance
        {
            get => _instance ?? (_instance = new PrestigeSaveData());
            // ReSharper disable once UnusedMember.Global - used by deseralizer.
            set => _instance = value;
        }

        // ReSharper disable once MemberCanBeMadeStatic.Global - removing this removes lazy load in accessor for the instance. 
        public void Save()
        {
            Logger.LogInformation("Writing prestige save data to disk...");
            SkillPrestigeMod.ModHelper.WriteJsonFile(SkillPrestigeMod.PerSaveDataPath, CurrentlyLoadedPrestigeSet);
            Logger.LogInformation("Prestige save data written to disk.");
        }

        public void Read()
        {
            _instance = new PrestigeSaveData();
            Logger.LogInformation("Deserializing prestige save data... "+SkillPrestigeMod.PerSaveDataPath);
            
            String abspath = Path.Combine(SkillPrestigeMod.ModHelper.DirectoryPath, SkillPrestigeMod.PerSaveDataPath);
            if (File.Exists(abspath)) {
                var json = File.ReadAllText(abspath);
                CurrentlyLoadedPrestigeSet = PrestigeSet.CompleteEmptyPrestigeSet;
                JObject parser = JObject.Parse(json);
                List<Prestige> prestiges = new List<Prestige>();
                foreach (JObject child in parser["Prestiges"].Children()) {
                    Prestige prestige = new Prestige();
                    prestige.PrestigePoints = child["PrestigePoints"].ToObject<int>();
                    var st = child["SkillType"];
                    prestige.SkillType = new SkillType(st["Name"].ToObject<String>(), st["Ordinal"].ToObject<int>());
                    IList<int> professionSelected = prestige.PrestigeProfessionsSelected;
                    foreach (var profession in child["PrestigeProfessionsSelected"]) {
                        professionSelected.Add(profession.ToObject<int>());
                    }
                    if (child["Bonuses"].Type == JTokenType.Null) {
                        prestige.Bonuses = null;
                    }
                    prestiges.Add(prestige);
                }
                CurrentlyLoadedPrestigeSet.Prestiges = prestiges;
            }
            
            
            
            
            
            
            Logger.LogInformation("Prestige save data loaded.");
        }

        private void UpdatePrestigeSkillsForCurrentFile()
        {
            Logger.LogInformation("Checking for missing prestige data...");
            var missingPrestiges = PrestigeSet.CompleteEmptyPrestigeSet.Prestiges.Where(x => !CurrentlyLoadedPrestigeSet.Prestiges.Select(y => y.SkillType).Contains(x.SkillType)).ToList();
            if (!missingPrestiges.Any()) return;
            Logger.LogInformation("Missing Prestige data found. Loading new prestige data...");
            var prestiges = new List<Prestige>(CurrentlyLoadedPrestigeSet.Prestiges);
            prestiges.AddRange(missingPrestiges);
            CurrentlyLoadedPrestigeSet.Prestiges = prestiges;
            Save();
            Logger.LogInformation("Missing Prestige data loaded.");
        }

        public void UpdateCurrentSaveFileInformation() {
            this.Read();
        }

    }
}
