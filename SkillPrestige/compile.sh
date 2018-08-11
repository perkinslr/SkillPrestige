mkdir -p ../build/SkillPrestige
mcs -unsafe `find . | grep cs$` -target:library -out:../build/SkillPrestige/SkillPrestige.dll -r:../stardew_valley/StardewValley.exe -r:../stardew_valley/StardewModdingAPI.exe -r:../stardew_valley/MonoGame.Framework.dll -r:../stardew_valley/Newtonsoft.Json.dll
cp *.png ../build/SkillPrestige
cp *.json ../build/SkillPrestige
mkdir -p ../build/SkillPrestige/data

