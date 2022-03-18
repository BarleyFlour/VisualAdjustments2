using HarmonyLib;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Persistence;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Class.LevelUp.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace VisualAdjustments2.Infrastructure
{
    [HarmonyPatch]
    internal static class SaveHooker
    {
        [HarmonyPatch(typeof(ZipSaver))]
        [HarmonyPatch("SaveJson"), HarmonyPostfix]
        private static void Zip_Saver(string name, ZipSaver __instance)
        {
            DoSave(name, __instance);
        }

        [HarmonyPatch(typeof(FolderSaver))]
        [HarmonyPatch("SaveJson"), HarmonyPostfix]
        private static void Folder_Saver(string name, FolderSaver __instance)
        {
            DoSave(name, __instance);
        }

        private static void DoSave(string name, ISaver saver)
        {
            if (name != "header")
                return;

            try
            {
                var serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                var writer = new StringWriter();
                serializer.Serialize(writer, GlobalCharacterSettings.Instance);
                writer.Flush();
                saver.SaveJson(LoadHooker.FileName, writer.ToString());
            }
            catch (Exception e)
            {
                Main.Logger.Log(e.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(Game))]
    internal static class LoadHooker
    {
        public const string FileName = "header.json.VisualAdjustments2";

        [HarmonyPatch("LoadGame"), HarmonyPostfix]
        private static void LoadGame(SaveInfo saveInfo)
        {
            using (saveInfo)
            {
                using (saveInfo.GetReadScope())
                {
                    ThreadedGameLoader.RunSafelyInEditor((Action)(() =>
                    {
                        try
                        {
                            string raw;
                            using (ISaver saver = saveInfo.Saver.Clone())
                            {
                                raw = saver.ReadJson(FileName);
                            }
                            if (raw != null)
                            {
                                var serializer = new JsonSerializer();
                                var rawReader = new StringReader(raw);
                                var jsonReader = new JsonTextReader(rawReader);
                                GlobalCharacterSettings.Instance = serializer.Deserialize<GlobalCharacterSettings>(jsonReader);
                            }
                            else
                            {
                                GlobalCharacterSettings.Instance = new GlobalCharacterSettings();
                            }
                        }
                        catch (Exception e) { Main.Logger.Error(e.ToString()); }
                    })).Wait();
                }
            }
        }
    }
    public class GlobalCharacterSettings
    {
        public CharacterSettings ForCharacter(UnitEntityData unit)
        {
            var key = unit.UniqueId;
            if (!PerCharacter.TryGetValue(key, out var record))
            {
                record = new CharacterSettings();
                PerCharacter.Add(key, record);
            }
            return record;
        }

        public Dictionary<string, CharacterSettings> PerCharacter = new Dictionary<string, CharacterSettings>();
        public static GlobalCharacterSettings Instance = new GlobalCharacterSettings();
    }
}