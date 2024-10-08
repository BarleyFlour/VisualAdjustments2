﻿using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.Utility;
using Kingmaker.Visual.CharacterSystem;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using HarmonyLib;
using Kingmaker.UI.MVVM._VM.MainMenu;
using Kingmaker.UI.MVVM._PCView.MainMenu;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;
using VisualAdjustments2.Infrastructure;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.View.Animation;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Shields;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Root;
using Kingmaker.Visual.Sound;

namespace VisualAdjustments2
{
    
    public class SerializedResourceList
    {
        [JsonProperty] public string Version;
    }
    public class SerializedEEList : SerializedResourceList
    {
        public SerializedEEList(ResourceInfo[] allees, string version)
        {
            allEEs = allees;
            Version = version;
        }
        [JsonProperty] public ResourceInfo[] allEEs;
    }
    public class SerializedFXList : SerializedResourceList
    {
        public SerializedFXList(ResourceInfo[] allfxs, Dictionary<string, ResourceLoader.FXBlocker> fxblockers, string version)
        {
            allFXs = allfxs;
            Version = version;
            AbilityGuidToFXBlocker = fxblockers;
        }
        [JsonProperty] public ResourceInfo[] allFXs;
        [JsonProperty] public Dictionary<string, ResourceLoader.FXBlocker> AbilityGuidToFXBlocker = new Dictionary<string, ResourceLoader.FXBlocker>();
    }
    public class SerializedEnchantmentList : SerializedResourceList
    {
        public SerializedEnchantmentList(ResourceInfo[] enchantmentsList, string version)
        {
            EnchantmentsList = enchantmentsList;
            Version = version;
        }
        [JsonProperty] public ResourceInfo[] EnchantmentsList;
    }
    public class SerializedWeaponList : SerializedResourceList
    {
        public SerializedWeaponList(Dictionary<WeaponAnimationStyle, List<ResourceInfo>> weaponsDictionary, string version)
        {
            WeaponsDictionary = weaponsDictionary;
            Version = version;
        }
        [JsonProperty] public Dictionary<WeaponAnimationStyle, List<ResourceInfo>> WeaponsDictionary;
    }
    public struct ResourceInfo
    {
        public ResourceInfo(string name, string internalname, string guid, Type bptype)
        {
            Name = name;
            Name_Internal = internalname;
            GUID = guid;
            BPType = bptype;
        }
        [JsonProperty] public readonly string Name;
        [JsonProperty] public readonly string Name_Internal;
        [JsonProperty] public readonly string GUID;
        [JsonProperty] public readonly Type BPType;
    }
    public static class ResourceLoader
    {
        private static Dictionary<BlueprintUnitAsksList,string> m_AllVoices;
        public static Dictionary<BlueprintUnitAsksList,string> AllVoices
        {
            get
            {
                if (m_AllVoices == null)
                {
                    m_AllVoices = new();
                    var voices = BlueprintRoot.Instance.CharGen.m_MaleVoices
                        .Select(a => (BlueprintUnitAsksList)a.GetBlueprint())
                        .Concat(BlueprintRoot.Instance.CharGen.FemaleVoices.Select(a => a));
                    voices = voices.Concat(Kingmaker.Cheats.Utilities.GetAllBlueprints().Entries
                        .Where(a => a.Type == typeof(BlueprintUnitAsksList)).Select(a =>
                            ResourcesLibrary.TryGetBlueprint<BlueprintUnitAsksList>(a.Guid)).Where(b => b.GetComponent<UnitAsksComponent>()?.Selected?.HasBarks != false).ToList());
                    
                    foreach(var voice in voices)
                    {
                        Main.Logger.Log($"{voice.name}, {voice.DisplayName}, {voice.NameSafe()}, {voice.AssetGuid}");
                    }
                    foreach (var voice in voices)
                    {
                        if (m_AllVoices.ContainsKey(voice)) continue;
                        if (voice.DisplayName.IsEmpty())
                        {
                            m_AllVoices.Add(voice,ResourceLoader.ProcessEEName(voice.name));
                        }
                        else
                        {
                            m_AllVoices.Add(voice,voice.DisplayName);
                        }
                    }
                }
                return m_AllVoices;
            }
        }
        public static Sprite LoadImage(string folder, string file, Vector2Int size)
        {
#if true
            return Image2Sprite.Create($"{Main.ModEntry.Path}Assets{Path.DirectorySeparatorChar}{folder}{Path.DirectorySeparatorChar}{file}", size);
#else
            return Image2Sprite.Create($"{Main.modEntry.Path + @"\"}Assets{Path.DirectorySeparatorChar}{folder}{Path.DirectorySeparatorChar}{file}", size);
#endif
        }
        // Loosely based on https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
        public static class Image2Sprite
        {
            public static string icons_folder = "";

            public static Sprite Create(string filePath, Vector2Int size)
            {
#if DEBUG
                Main.Logger.Log("Creating sprite");
#endif
                var bytes = File.ReadAllBytes(icons_folder + filePath);
                var texture = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
                _ = texture.LoadImage(bytes);
                return Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0, 0));
            }
        }
        public static Dictionary<string, List<string>> AbilityGuidToFXGuids = new Dictionary<string, List<string>>();
        public static Dictionary<string, ResourceInfo> NameToEEInfo = new Dictionary<string, ResourceInfo>();
        public static ResourcesLibrary.LoadedResource LoadEE(string assetid, string assetBundleName)
        {
            // var sw = new Stopwatch();
            //sw.Start();
            AssetBundle assetBundle = BundlesLoadService.Instance?.RequestBundle(assetBundleName);
            //  sw.Stop();
            // Main.Logger.Log($"Got Bundle in {sw.ElapsedTicks} ticks");
            // sw.Restart();
            var resource = assetBundle.LoadAsset_Internal(assetid, typeof(EquipmentEntity));
            // sw.Stop();
            // Main.Logger.Log($"Loaded Asset in {sw.ElapsedTicks} ticks");
            // sw.Restart();
            var Loaded = new ResourcesLibrary.LoadedResource();
            Loaded.Resource = resource;
            Loaded.AssetId = assetid;
            //  sw.Stop();
            // Main.Logger.Log($"Made LoadedResource in {sw.ElapsedTicks} ticks");
            return Loaded;
        }
        public static ResourcesLibrary.LoadedResource LoadFX(string assetid, string assetBundleName)
        {
            // var sw = new Stopwatch();
            //sw.Start();
            AssetBundle assetBundle = BundlesLoadService.Instance?.RequestBundle(assetBundleName);
            //  sw.Stop();
            // Main.Logger.Log($"Got Bundle in {sw.ElapsedTicks} ticks");
            // sw.Restart();
            var resource = assetBundle.LoadAsset_Internal(assetid, typeof(GameObject));
            // sw.Stop();
            // Main.Logger.Log($"Loaded Asset in {sw.ElapsedTicks} ticks");
            // sw.Restart();
            var Loaded = new ResourcesLibrary.LoadedResource();
            Loaded.Resource = resource;
            Loaded.AssetId = assetid;
            //  sw.Stop();
            // Main.Logger.Log($"Made LoadedResource in {sw.ElapsedTicks} ticks");
            return Loaded;
        }
        public static Dictionary<string, string> resourceguidmap;
        public static Dictionary<string, string> GetResourceGuidMap()
        {
            LocationList locationList = BundlesLoadService.Instance.m_LocationList;
            if (resourceguidmap == null)
            {
                resourceguidmap = locationList.GuidToBundle;
            }
            return resourceguidmap;
        }
        public static readonly Dictionary<string, string> raceidentifiers = new Dictionary<string, string>
        {
            ["HM"] = "Human",
            ["AA"] = "Aasimar",
            ["HE"] = "Half-Elf",
            ["MM"] = "Mongrel",
            ["SU"] = "Succubus",
            ["DW"] = "Dwarf",
            ["TL"] = "Tiefling",
            ["EL"] = "Elf",
            ["KT"] = "Kitsune",
            ["GN"] = "Gnome",
            ["OD"] = "Oread",
            ["HL"] = "Halfling",
            ["DH"] = "Dhampir",
            ["HO"] = "Half-Orc",
            ["ZB"] = "Zombie",
            ["CB"] = "Cambion",
            ["CM"] = "Cambion",
            ["SN"] = "Skeleton",
            ["DE"] = "Drow"

        };
        public static void SaveCachedResources<T>(ResourceInfo[] array) where T : SerializedResourceList
        {
            string filepath;
            if (typeof(T) == typeof(SerializedEEList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedEEs.json");
            }
            else if (typeof(T) == typeof(SerializedFXList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedFXs.json");
            }
            else if (typeof(T) == typeof(SerializedWeaponList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedWeapons.json");
            }
            else if (typeof(T) == typeof(SerializedEnchantmentList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedEnchants.json");
            }
            else filepath = "";
            var cachepath = Path.Combine(Main.ModEntry.Path,"Cache");
            if (!System.IO.Directory.Exists(Path.Combine(Main.ModEntry.Path, "Cache")))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(Main.ModEntry.Path,"Cache"));
            }
            //var filepath = typeof(T) == typeof(SerializedEEList) ? Path.Combine(Main.ModEntry.Path, "CachedEEs.json") : Path.Combine(Main.ModEntry.Path, "CachedFXs.json");
            try
            {
                JsonSerializer serializer = new JsonSerializer();
#if (DEBUG)
                serializer.Formatting = Formatting.Indented;
#endif
                using (StreamWriter sw = new StreamWriter(filepath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    if (typeof(T) == typeof(SerializedEEList))
                    {
                        serializer.Serialize(writer, new SerializedEEList(array, GameVersion.GetVersion()));
                    }
                    else if(typeof(T) == typeof(SerializedFXList))
                    {
                        serializer.Serialize(writer, new SerializedFXList(array, AbilityGuidToFXBlocker, GameVersion.GetVersion()));
                    }
                    /*else if(typeof(T) == typeof(SerializedWeaponList))
                    {
                        serializer.Serialize(writer, new SerializedWeaponList(array, AbilityGuidToFXBlocker, GameVersion.GetVersion()));
                    }*/
                    else if (typeof(T) == typeof(SerializedEnchantmentList))
                    {
                        serializer.Serialize(writer, new SerializedEnchantmentList(array, GameVersion.GetVersion()));
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Can't save {filepath}.");
                Main.Logger.Error(ex.ToString());
            }
        }
        /*private static IEnumerator LoadAsset(AssetBundle b)
        {
            var request = b.;

            while (!request.isDone)
                yield return new WaitForSeconds((float)0.5); //whatever

            BarleyHandleAssets(request.assetBundle);
            yield return null;
        }*/
        public static void SaveCachedResources<T>(Dictionary<WeaponAnimationStyle,List<ResourceInfo>> array) where T : SerializedResourceList
        {
            string filepath;
            if (typeof(T) == typeof(SerializedEEList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedEEs.json");
            }
            else if (typeof(T) == typeof(SerializedFXList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedFXs.json");
            }
            else if (typeof(T) == typeof(SerializedWeaponList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedWeapons.json");
            }
            else filepath = "";
            var cachepath = Path.Combine(Main.ModEntry.Path, "Cache");
            if (!System.IO.Directory.Exists(Path.Combine(Main.ModEntry.Path, "Cache")))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(Main.ModEntry.Path, "Cache"));
            }
            //var filepath = typeof(T) == typeof(SerializedEEList) ? Path.Combine(Main.ModEntry.Path, "CachedEEs.json") : Path.Combine(Main.ModEntry.Path, "CachedFXs.json");
            try
            {
                JsonSerializer serializer = new JsonSerializer();
#if (DEBUG)
                serializer.Formatting = Formatting.Indented;
#endif
                using (StreamWriter sw = new StreamWriter(filepath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    if (typeof(T) == typeof(SerializedWeaponList))
                    {
                        serializer.Serialize(writer, new SerializedWeaponList(array, GameVersion.GetVersion()));
                    }
                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error($"Can't save {filepath}.");
                Main.Logger.Error(ex.ToString());
            }
        }
        public static bool GetCachedResources<T>(out T info) where T : SerializedResourceList
        {
#if DEBUG
            // info = null;
            //  return false;
#endif
            string filepath;
            var cachepath = Path.Combine(Main.ModEntry.Path, "Cache");
            if (!System.IO.Directory.Exists(Path.Combine(Main.ModEntry.Path, "Cache")))
            {
                System.IO.Directory.CreateDirectory(Path.Combine(Main.ModEntry.Path, "Cache"));
            }
            if (typeof(T) == typeof(SerializedEEList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedEEs.json");
            }
            else if (typeof(T) == typeof(SerializedFXList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedFXs.json");
            }
            else if (typeof(T) == typeof(SerializedWeaponList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedWeapons.json");
            }
            else if (typeof(T) == typeof(SerializedEnchantmentList))
            {
                filepath = Path.Combine(Main.ModEntry.Path, "Cache/CachedEnchants.json");
            }
            else filepath = "";
            if (File.Exists(filepath))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamReader sr = new StreamReader(filepath))
                    using (JsonTextReader reader = new JsonTextReader(sr))
                    {
                        T deserialized = serializer.Deserialize<T>(reader);
                        if (deserialized == null)
                        {
                            info = null;
                            return false;
                        }
                        else
                        {
                            if (deserialized.Version != GameVersion.GetVersion())
                            {
                                info = null;
                                return false;
                            }
                            else
                            {
                                info = deserialized;
                                return true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Main.Logger.Error($"Can't read {filepath}.");
                    Main.Logger.Error(ex.ToString());
                    info = null;
                    return false;
                }
            }
            else { info = null; return false; }
        }




        /*private struct LoadInfo
        {
            public LoadInfo(string guid, int index)
            {
                GUID = guid;
                Index = index;
            }
            private readonly string GUID;
            private readonly int Index;
        }*/
        //use load all but associate guids w/ index beforehand then link them using index
        /// <summary>
        /// 
        /// </summary>
        /*public static void StartEEGetting2()
        {
            m_AllEEs = new List<EEInfo>();
            var sw = new Stopwatch();
            sw.Start();
            var assetbundle = BundlesLoadService.Instance.RequestBundle("equipment");
            var guids = GetResourceGuidMap().Where(b => b.Value[0] == 'e' && b.Value[1] == 'e').ToArray();

            var bundles = guids.Distinct();
            foreach (var bundle in bundles)
            {
                var all = BundlesLoadService.Instance.RequestBundle(bundle.Value).LoadAllAssetsAsync<EquipmentEntity>();
                //sw.Restart();
                all.completed += (AsyncOperation _) => { ProcessEEs(all.allAssets, all.asset); };
                //while(!all.isDone)
                {
                    //Main.Logger.Log();
                }
                //Main.Logger.Log(sw.ElapsedMilliseconds.ToString());
                void ProcessEEs(UnityEngine.Object[] ees, UnityEngine.Object obj)
                {
                    try
                    {

                        for (int i = 0; i > ees.Length; i++)
                        {
                            var ee = ees[i];
                            if ((EquipmentEntity)ee != null)
                            {
                                var guid = guids[i];

                                HandleEEGotten((EquipmentEntity)ee, guid.Key);
                            }
                        }
                        //Main.Logger.Log($"Got all of em in {sw.ElapsedMilliseconds}ms");
                        sw.Stop();
                    }
                    catch (Exception e) { Main.Logger.Error(e.ToString()); }
                }
            }*/
        /*  for (int i = 0; i < guids.Count; i++)
          {
              var guid = guids.ElementAt(i);
              if ()
              {
                  toload.Add(new LoadInfo(guid.Key, i));
              }
          }*//*
         // sw.Stop();
         // Main.Logger.Log($"Generated Load Infos in {sw.ElapsedMilliseconds}ms");

      }*/
        public static bool GetBundleBarley(string bundleName, out AssetBundle bundle)
        {
            BundlesLoadService.BundleData bundleData2;
            if (BundlesLoadService.Instance.m_Bundles.TryGetValue(bundleName, out bundleData2) && bundleData2.Bundle)
            {
                PFLog.Bundles.Log("Requested: {0}, already loaded ({1}) ", new object[]
                {
                    bundleName,
                    bundleData2.RequestCount
                });
                bundleData2.RequestCount++;
                bundle = bundleData2.Bundle;
                return true;
            }
            else
            {
                bundle = null;
                return false;
            }
        }
        public static void StartEEGetting()
        {
            m_AllEEs = new List<ResourceInfo>();
            var sw = new Stopwatch();
            sw.Start();
            foreach (var x in GetResourceGuidMap().Where(b => b.Value[0] == 'e' && b.Value[1] == 'e' && b.Value[2] == '_'))
            {
                var thing = ResourcesLibrary.TryGetResource<EquipmentEntity>(x.Key);
                if (thing != null) HandleEEGotten(thing, x.Key);
            }
            sw.Stop();
#if DEBUG
            Main.Logger.Log($"Got EEs in {sw.ElapsedMilliseconds}ms");
#endif


            return;
            var sw2 = new Stopwatch();
            sw2.Start();
            var SortedGuidMap = GetResourceGuidMap().Where(b => b.Value[0] == 'e' && b.Value[1] == 'e' && b.Value[2] == '_').ToArray();
            //if (AllEEs != null) return;
            //AssetBundle assetBundle = BundlesLoadService.Instance.RequestBundle("equipment");
            foreach (var ToAsyncLoad in SortedGuidMap)
            {
                if (GetBundleBarley(ToAsyncLoad.Value, out var assetBundle))
                {
                    var EERequest = assetBundle.LoadAssetAsync<EquipmentEntity>(ToAsyncLoad.Key);
                    EERequest.priority = 1000;
#if DEBUG
                    EERequest.completed += (AsyncOperation _) =>
                    {
                        Main.Logger.Log(EERequest.asset.name.ToString());
                    };
#endif
                }
                else
                {
                    var LoadedBundle = AssetBundle.LoadFromFileAsync(BundlesLoadService.BundlesPath(ToAsyncLoad.Value));
                    LoadedBundle.priority = 1000;
                    LoadedBundle.completed += (AsyncOperation _) =>
                    {
                        var a = LoadedBundle.assetBundle.LoadAssetAsync<EquipmentEntity>(ToAsyncLoad.Key);
                        a.priority = 1000;
#if DEBUG
                        a.completed += (AsyncOperation __) =>
                        {
                            Main.Logger.Log(a.asset.name.ToString());
                        };
#endif
                    };
                }
                //   d = BundlesLoadService.Instance.RequestBundleAsync(ToAsyncLoad.Value);
            }
            /* while (!d.IsCompleted)
                 yield return new WaitForSeconds(2);
             for (int i = 0; i < SortedGuidMap.Length - 1; i++)
             //foreach(var EEBundle in SortedGuidMap)
             {
                 var EEBundle = SortedGuidMap[i];
                 var bundle = BundlesLoadService.Instance.RequestBundle(EEBundle.Value);
                 var a = bundle.LoadAssetAsync<EquipmentEntity>(EEBundle.Key);
                 if (i == SortedGuidMap.Length - 1)
                 {
                     a.completed += (AsyncOperation _) => { Main.Logger.Log(a.allAssets.Length.ToString() + " " + EEBundle.Value); Main.Logger.Log($"Got EEs in {sw2.ElapsedMilliseconds}ms"); };
                 }
                 else
                 {
                     a.completed += (AsyncOperation _) => { Main.Logger.Log(a.allAssets.Length.ToString() + " " + EEBundle.Value); };
                 }

             }*/
            /*
            //#if DEBUG
            var sw = new Stopwatch();
            sw.Start();

            //#endif
            var count = SortedGuidMap.Count();
           // m_AllEEs = new List<EEInfo>();
            var Keys = SortedGuidMap.Select(b => b.Key).ToArray();
            for (int i = 0; i < count; i++)
            {
                var assetguid = Keys[i];
                var request = assetBundle.LoadAssetAsync<EquipmentEntity>(assetguid);
                //#if DEBUG
                if (i == count - 1)
                {
                    // Main.Logger.Log($"Final index {i}, Length:{Keys.Length}");
                    request.completed += (AsyncOperation _) =>
                    {
                        Main.Logger.Log(request?.asset?.ToString());
                        if (request.asset != null) HandleEEGotten((EquipmentEntity)request.asset, assetguid);
                        sw.Stop();
                        HandleFinalEEGotten();
                        Main.Logger.Log($"Got all EEs in {sw.ElapsedMilliseconds}ms");
                    };
                }
                else
                //#else
                /* if (i == count-1)
                 {
                     request.completed += (AsyncOperation _) => { if (request.asset != null) HandleEEGotten((EquipmentEntity)request.asset, assetguid); sw.Stop(); HandleFinalEEGotten(); };
                 }
                 else*/
            //#endif
            /*
            {
                request.completed += (AsyncOperation _) =>
                {

                    if (request.asset != null) HandleEEGotten((EquipmentEntity)request.asset, assetguid);
                };
            }*/
            //}
            // sw2.Stop();
            // Main.Logger.Log("adsadasd " + sw2.ElapsedMilliseconds);
        }
        public static void HandleFinalEEGotten()
        {
            //m_AllEEs.OrderBy(a => a.Name);
            Main.Logger.Log("Got Final EE");
        }
        public static void HandleEEGotten(EquipmentEntity ee, string guid)
        {
            // Main.Logger.Log($"Got EE {ee.name}");
            m_AllEEs.Add(new ResourceInfo(ProcessEEName(ee.name), ee.name, guid, typeof(EquipmentEntity)));
        }
        public static Type[] FXAbilityTypes = new Type[]
        {
            typeof(BlueprintAbility)
        };
        public class FXBlockerHolder
        {
            [JsonProperty] public HashSet<string> FXGuids = new HashSet<string>();
            [JsonProperty] public HashSet<FXBlocker> FXBlockers = new HashSet<FXBlocker>();
            public void Recache()
            {
                var all = FXBlockers.SelectMany(z => z.FXGuids);
                FXGuids.Clear();
                foreach (var fx in all)
                {
                    FXGuids.Add(fx);
                }
            }
        }
        public class FXBlocker
        {
            [JsonProperty] public string DisplayName;
            [JsonProperty] public string AbilityGUID;
            [JsonProperty] public HashSet<string> AllAbilityGUIDs = new HashSet<string>();
            [JsonProperty] public HashSet<string> FXGuids = new HashSet<string>();
            [JsonProperty] public string FullNameMerged;

            public FXBlocker()
            {

            }
            public FXBlocker(BlueprintAbility ability)
            {
                this.AbilityGUID = ability.AssetGuidThreadSafe;

                void Merge(BlueprintBuff a)
                {
                    // Main.Logger.Log($"Merged: {ability.NameForAcronym}");
                    this.AllAbilityGUIDs.Add(ability.AssetGuidThreadSafe);
                    if(this.FullNameMerged.IsNullOrEmpty())
                    {
                        this.FullNameMerged = a.m_DisplayName;
                    }
                    else
                    {
                        var newstring = ($"{FullNameMerged}, {a.m_DisplayName}");
                        this.FullNameMerged = newstring;
#if DEBUG
                        Main.Logger.Log(this.FullNameMerged + " " + a.NameSafe());
#endif
                    }
                    if (a.FxOnRemove?.AssetId?.IsNullOrEmpty() == false) this.FXGuids.Add(a.FxOnRemove.AssetId);
                    if (a.FxOnStart?.AssetId?.IsNullOrEmpty() == false) this.FXGuids.Add(a.FxOnStart.AssetId);
                }
                var c = ability?.GetComponent<AbilityEffectRunAction>()?.Actions?.Actions;
                if (c != null && c.Length > 0)
                {
                    var a = ((ContextActionApplyBuff)c.FirstOrDefault(x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                    if (a != null)
                    {
                        Merge(a);
                    }
                    var b = ((Conditional)c.FirstOrDefault(x => x?.GetType() == typeof(Conditional)));
                    if (b != null)
                    {
                        if (b.IfTrue?.Actions != null)
                        {
                            var z = ((ContextActionApplyBuff)b.IfTrue.Actions.FirstOrDefault(x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                            if (z != null)
                            {
                                //    Main.Logger.Log($"MergedConditionalIfTrue {ability.NameForAcronym}");
                                Merge(z);
                            }
                        }
                        if (b.IfFalse?.Actions != null)
                        {
                            var z = ((ContextActionApplyBuff)b.IfFalse.Actions.FirstOrDefault(x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                            if (z != null)
                            {
                                //  Main.Logger.Log($"MergedConditionalIfFalse {ability.NameForAcronym}");
                                Merge(z);
                            }
                        }
                    }
                }
                this.AllAbilityGUIDs.Add(ability.AssetGuidThreadSafe);
                //this.FullNameMerged = ability.m_DisplayName;
                this.DisplayName = ability.m_DisplayName;
            }
            public FXBlocker(BlueprintActivatableAbility ability)
            {
                this.AbilityGUID = ability.AssetGuidThreadSafe;

                void Merge(BlueprintBuff a)
                {
                    // Main.Logger.Log($"Merged: {ability.NameForAcronym}");
                    this.AllAbilityGUIDs.Add(ability.AssetGuidThreadSafe);
                    if (a.FxOnRemove?.AssetId?.IsNullOrEmpty() == false) this.FXGuids.Add(a.FxOnRemove.AssetId);
                    if (a.FxOnStart?.AssetId?.IsNullOrEmpty() == false) this.FXGuids.Add(a.FxOnStart.AssetId);
                }
                Merge(ability.Buff);
                this.AllAbilityGUIDs.Add(ability.AssetGuidThreadSafe);
                this.DisplayName = ability.m_DisplayName;
            }
        }
        ///settings contains fxblockers, remove specific fxblockers from UI & recache, have buffhandler check all FXGuids, maybe reactive property nonsense?
        public static Dictionary<string, FXBlocker> AbilityGuidToFXBlocker = new Dictionary<string, FXBlocker>();
        public static List<ResourceInfo> GetFXs()
        {

            // return resourceInfList;
            if (GetCachedResources<SerializedFXList>(out var deserializedFX) == true)
            {
                AbilityGuidToFXBlocker = deserializedFX.AbilityGuidToFXBlocker;
                return deserializedFX.allFXs.ToList();
            }
            else
            {
                /*var list = new List<ResourceInfo>();
                var AbilitiesWithFX = Kingmaker.Cheats.Utilities.GetAllBlueprints().Entries.Where(a => a.Type == typeof(BlueprintBuff));
                foreach (var c in AbilitiesWithFX)
                {
                    var loaded = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(c.Guid);
                    if (((loaded.FxOnStart != null && loaded.FxOnStart.AssetId != "") || (loaded.FxOnRemove != null && loaded.FxOnRemove.AssetId != "")) && loaded.m_Flags != 0 && loaded.m_DisplayName != "" && loaded.NameForAcronym != "") list.Add(new ResourceInfo(loaded.m_DisplayName, loaded.NameForAcronym, c.Guid, c.m_Type));
                    // Main.Logger.Log($"{c.Name} : Type = {c.Type.Name}");

                }*/
                var resourceInfList = new List<ResourceInfo>();
                var allbp = Kingmaker.Cheats.Utilities.GetAllBlueprints();
                var templist = new List<BlueprintAbility>();
                var wack2 = new List<FXBlocker>();
                foreach (var activatable in allbp.Entries.Where(b => b.Type == typeof(BlueprintActivatableAbility)))
                {
                    var bp = ResourcesLibrary.TryGetBlueprint<BlueprintActivatableAbility>(activatable.Guid);
                    var firstMatch = wack2.FirstOrDefault(b => b.DisplayName == bp.m_DisplayName);
                    if (firstMatch != null)
                    {
                        //void Merge(BlueprintBuff a)
                        // {
                        // Main.Logger.Log($"Merged: {bp.NameForAcronym}");
                        firstMatch.AllAbilityGUIDs.Add(bp.AssetGuidThreadSafe);
                        if (bp.Buff.FxOnRemove?.AssetId?.IsNullOrEmpty() == false) firstMatch.FXGuids.Add(bp.Buff.FxOnRemove.AssetId);
                        if (bp.Buff.FxOnStart?.AssetId?.IsNullOrEmpty() == false) firstMatch.FXGuids.Add(bp.Buff.FxOnStart.AssetId);
                        // }

                    }
                    else if (bp.m_Buff.guid != null && bp.m_Buff.guid != "" && bp.GetBeneficialBuffs())
                    {

                        // Main.Logger.Log(bp.NameForAcronym + " Activatable");
                        resourceInfList.Add(new ResourceInfo(bp.m_DisplayName, bp.NameForAcronym, bp.AssetGuidThreadSafe, bp.GetType()));
                        var blocker = new FXBlocker(bp);
                        AbilityGuidToFXBlocker[bp.AssetGuidThreadSafe] = blocker;
                        wack2.Add(blocker);
#if DEBUG
                        Main.Logger.Log(bp.NameForAcronym);
#endif
                    }
                }
                foreach (var ability in allbp.Entries.Where(b => b.Type == typeof(BlueprintAbility)))
                {
                    if (ability.Guid == "416f867258c24b709bb1b74a2ad1ec89" || ability.Guid == "0bb76d697b7f4d0faa892df309be3710" || ability.Guid == "cbfd9c4f683c4394ba3543058f045682") //Wacky COTW abilities in wrath for some reason.
                    {
                        continue;
                    }
                    var bp = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(ability.Guid);
                    if (!bp.HiddenInInspector && bp.GetBeneficialBuffs())
                    {
                        //Main.Logger.Log(ability.Name);
                        var firstMatch = wack2.FirstOrDefault(b => b.DisplayName == bp.m_DisplayName);
                        if (firstMatch != null)
                        {
                            void Merge(BlueprintBuff buffBP)
                            {
                                //Main.Logger.Log($"Merged: {bp.NameForAcronym}");
                                firstMatch.AllAbilityGUIDs.Add(bp.AssetGuidThreadSafe);
                                if (buffBP.FxOnRemove?.AssetId?.IsNullOrEmpty() == false) firstMatch.FXGuids.Add(buffBP.FxOnRemove.AssetId);
                                if (buffBP.FxOnStart?.AssetId?.IsNullOrEmpty() == false) firstMatch.FXGuids.Add(buffBP.FxOnStart.AssetId);
                            }
                            var actionsList = bp?.GetComponent<AbilityEffectRunAction>()?.Actions?.Actions;
                            if (actionsList != null && actionsList.Length > 0)
                            {
                                var buff = ((ContextActionApplyBuff)actionsList.FirstOrDefault(x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                                if (buff != null)
                                {
                                    Merge(buff);
                                }
                                var conditionalAction = ((Conditional)actionsList.FirstOrDefault(x => x?.GetType() == typeof(Conditional)));
                                if (conditionalAction != null)
                                {
                                    if (conditionalAction.IfTrue?.Actions != null)
                                    {
                                        var ifTrueAction = ((ContextActionApplyBuff)conditionalAction.IfTrue.Actions.FirstOrDefault(x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                                        if (ifTrueAction != null)
                                        {
                                            //   Main.Logger.Log($"MergedConditionalIfTrue {bp.NameForAcronym}");
                                            Merge(ifTrueAction);
                                        }
                                    }
                                    if (conditionalAction.IfFalse?.Actions != null)
                                    {
                                        var ifFalseAction = ((ContextActionApplyBuff)conditionalAction.IfFalse.Actions.FirstOrDefault(x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                                        if (ifFalseAction != null)
                                        {
                                            // Main.Logger.Log($"MergedConditionalIfFalse {bp.NameForAcronym}");
                                            Merge(ifFalseAction);
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            resourceInfList.Add(new ResourceInfo(bp.m_DisplayName, bp.NameForAcronym, bp.AssetGuidThreadSafe, bp.GetType()));
                            var blocker = new FXBlocker(bp);
                            AbilityGuidToFXBlocker[bp.AssetGuidThreadSafe] = blocker;
                            wack2.Add(blocker);
                        }
                    }
                }
                resourceInfList.Sort((x, y) => string.Compare(x.Name, y.Name));
                SaveCachedResources<SerializedFXList>(resourceInfList.ToArray());
                return resourceInfList;
            }
            /*{
#if DEBUG
                var sw = new Stopwatch();
                sw.Start();
#endif
                if (GetResourceGuidMap() != null)
                {
#if DEBUG
                    var sw4 = new Stopwatch();
                    sw4.Start();
#endif
                    var templist = new List<KeyValuePair<string, string>>();
                    async Task<List<KeyValuePair<string, string>>> GetEEGuids(Dictionary<string, string> list)
                    {
                        var toload = new List<KeyValuePair<string, string>>();
                        //foreach (var value in list.Where(b => (b.Value[0] == 'e' && b.Value[1] == 'e' && b.Value[2] == '_')))
                        //foreach (var value in list.Where(b => b.Value.Length > 3 && b.Value[b.Value.Length - 1] == 'x' && b.Value[b.Value.Length - 2] == 'f' && b.Value[b.Value.Length - 3] == '.'))
                        foreach(var value in list.Where(b => b.Value.EndsWith(".fx")))
                        {
                            await Task.Run(() => { toload.Add(value); }).ConfigureAwait(false);
                        }
                        return toload;
                    }
                    var task = GetEEGuids(GetResourceGuidMap());
                    task.Wait();
#if DEBUG
                    sw4.Stop();
                    Main.Logger.Log($"Processed KVs in {sw4.ElapsedMilliseconds}ms");
#endif
                    var sw2 = new Stopwatch();
                    sw2.Start();
                    foreach (var guid in task.Result)
                    {
                        Main.Logger.Log(guid.Key);
                        var obj = LoadFX(guid.Key, guid.Value);
                        if (obj != null)
                        {
                            if (obj.Resource != null)
                                templist.Add(new KeyValuePair<string, string>(guid.Key, obj.Resource.name));
                        }
                    }
#if DEBUG
                    sw2.Stop();
                    Main.Logger.Log($"Loaded FXs in {sw2.ElapsedMilliseconds}ms");
                    var sw3 = new Stopwatch();
                    sw3.Start();
#endif
                    var eeinfolist = new List<ResourceInfo>();
                    async Task<List<ResourceInfo>> Stuff()
                    {
                        foreach (var loaded in templist)
                        {
                            await Task.Run(() =>
                            {
                                var eeinfo = new ResourceInfo(ProcessEEName(loaded.Value), loaded.Value, loaded.Key);
                                eeinfolist.Add(eeinfo); NameToEEInfo.Add(loaded.Value, eeinfo);
                            }).ConfigureAwait(false);
                        }
                        return eeinfolist;
                    }
                    var task2 = Stuff();
                    task2.Wait();
#if DEBUG
                    sw3.Stop();
                    Main.Logger.Log($"Strings processed in {sw3.Elapsed}ms");
#endif
                    ResourcesLibrary.CleanupLoadedCache();
#if DEBUG
                    sw.Stop();
                    Main.Logger.Log($"Got all FX's in {sw.ElapsedMilliseconds}ms");
                    Main.Logger.Log($"String processing took {sw.ElapsedMilliseconds - sw2.ElapsedMilliseconds - sw3.ElapsedMilliseconds}");
#endif
                    var ResultToArray = task2.Result.ToArray();
                    SaveCachedResources<SerializedFXList>(ResultToArray);
                    return task2.Result;
                }
                else throw (new Exception("GUID Map null, what? how?"));
            }*/
        }
        //Old slow EE getter
        public static List<ResourceInfo> GetEEs()
        {
            if (GetCachedResources<SerializedEEList>(out var deserialized) == true)
            {
                foreach (var ee in deserialized.allEEs)
                {
                    NameToEEInfo.Add(ee.Name_Internal, ee);
                }
                return deserialized.allEEs.ToList();
            }
            else
            {
#if DEBUG
                var sw = new Stopwatch();
                sw.Start();
#endif
                if (GetResourceGuidMap() != null)
                {
#if DEBUG
                    var sw4 = new Stopwatch();
                    sw4.Start();
#endif
                    var templist = new List<KeyValuePair<string, string>>();
                    async Task<List<KeyValuePair<string, string>>> GetEEGuids(Dictionary<string, string> list)
                    {
                        var toload = new List<KeyValuePair<string, string>>();
                        foreach (var value in list.Where(b => (b.Value[0] == 'e' && b.Value[1] == 'e' && b.Value[2] == '_')))
                        {
                            await Task.Run(() => { toload.Add(value); }).ConfigureAwait(false);
                        }
                        return toload;
                    }
                    var task = GetEEGuids(GetResourceGuidMap());
                    task.Wait();
#if DEBUG
                    sw4.Stop();
                    Main.Logger.Log($"Processed KVs in {sw4.ElapsedMilliseconds}ms");
#endif
                    var sw2 = new Stopwatch();
                    sw2.Start();
                    foreach (var guid in task.Result)
                    {
                        var obj = LoadEE(guid.Key, guid.Value);
                        if (obj != null)
                        {
                            if (obj.Resource != null)
                                templist.Add(new KeyValuePair<string, string>(guid.Key, obj.Resource.name));
                        }
                    }
#if DEBUG
                    sw2.Stop();
                    Main.Logger.Log($"Loaded EEs in {sw2.ElapsedMilliseconds}ms");
                    var sw3 = new Stopwatch();
                    sw3.Start();
#endif
                    var eeinfolist = new List<ResourceInfo>();
                    async Task<List<ResourceInfo>> Stuff()
                    {
                        foreach (var loaded in templist)
                        {
                            await Task.Run(() =>
                            {
                                var eeinfo = new ResourceInfo(ProcessEEName(loaded.Value), loaded.Value, loaded.Key, typeof(EquipmentEntity));
                                eeinfolist.Add(eeinfo); NameToEEInfo.Add(loaded.Value, eeinfo);
                            }).ConfigureAwait(false);
                        }
                        return eeinfolist;
                    }
                    var task2 = Stuff();
                    task2.Wait();
#if DEBUG
                    sw3.Stop();
                    Main.Logger.Log($"Strings processed in {sw3.Elapsed}ms");
#endif
                    ResourcesLibrary.CleanupLoadedCache();
#if DEBUG
                    sw.Stop();
                    Main.Logger.Log($"Got all EE's in {sw.ElapsedMilliseconds}ms");
                    Main.Logger.Log($"String processing took {sw.ElapsedMilliseconds - sw2.ElapsedMilliseconds - sw3.ElapsedMilliseconds}");
#endif
                    task2.Result.Sort((x, y) => string.Compare(x.Name, y.Name));
                    var ResultToArray = task2.Result.ToArray();
                    SaveCachedResources<SerializedEEList>(ResultToArray);
                    return task2.Result;
                }
                else throw (new Exception("GUID Map null, what? how?"));
            }
        }
        public static Dictionary<WeaponAnimationStyle, List<ResourceInfo>> GetWeapons()
        {
            if (GetCachedResources<SerializedWeaponList>(out var deserialized) == true)
            {
                return deserialized.WeaponsDictionary;
            }
            else
            {
                var allBPs = Kingmaker.Cheats.Utilities.GetAllBlueprints();
                var weapons = (IEnumerable<BlueprintItemEquipmentHand>)allBPs.Entries.Where(a => a.Type == typeof(BlueprintItemWeapon)).Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintItemWeapon>(b.Guid)).OrderBy((bp) => bp.name);
                weapons = allBPs.Entries.Where(a => a.Type == typeof(BlueprintItemShield)).Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintItemShield>(b.Guid)).OrderBy((bp) => bp.name).Concat(weapons);
                var UniqueVisualWeapons = new List<BlueprintItemEquipmentHand>();
                foreach (var item in weapons)
                {
                    var weapon = ResourcesLibrary.TryGetBlueprint<BlueprintItemEquipmentHand>(item.AssetGuid);
                    if (weapon.VisualParameters?.Model != null && !UniqueVisualWeapons.Any(a => a.VisualParameters.m_WeaponModel.AssetId == weapon.VisualParameters.m_WeaponModel.AssetId))
                    {
                        UniqueVisualWeapons.Add(weapon);
                    }
                }
                var DictionaryToReturn = new Dictionary<WeaponAnimationStyle, List<ResourceInfo>>();
                foreach (var type in Enum.GetValues(typeof(WeaponAnimationStyle)))
                {
                    DictionaryToReturn[(WeaponAnimationStyle)type] = new List<ResourceInfo>();
                }
                foreach (var weapon in UniqueVisualWeapons)
                {
                    // if(weapon.GetType() == typeof(BlueprintItemWeapon))
                    {
                        var wpn = weapon;
                        var inf = new ResourceInfo(weapon.m_DisplayNameText,weapon.NameForAcronym,weapon.AssetGuidThreadSafe,weapon.GetType());
                        DictionaryToReturn[wpn.VisualParameters.AnimStyle].Add(inf);
                    }
                    // else
                    {

                    }
                }
                SaveCachedResources<SerializedWeaponList>(DictionaryToReturn);
                return DictionaryToReturn;
//#if DEBUG
                /*  var sw = new Stopwatch();
                  sw.Start();
#endif
                        if (GetResourceGuidMap() != null)
                  {
  #if DEBUG
                      var sw4 = new Stopwatch();
                      sw4.Start();
  #endif
                      var templist = new List<KeyValuePair<string, string>>();
                      async Task<List<KeyValuePair<string, string>>> GetEEGuids(Dictionary<string, string> list)
                      {
                          var toload = new List<KeyValuePair<string, string>>();
                          foreach (var value in list.Where(b => (b.Value[0] == 'e' && b.Value[1] == 'e' && b.Value[2] == '_')))
                          {
                              await Task.Run(() => { toload.Add(value); }).ConfigureAwait(false);
                          }
                          return toload;
                      }
                      var task = GetEEGuids(GetResourceGuidMap());
                      task.Wait();
  #if DEBUG
                      sw4.Stop();
                      Main.Logger.Log($"Processed KVs in {sw4.ElapsedMilliseconds}ms");
  #endif
                      var sw2 = new Stopwatch();
                      sw2.Start();
                      foreach (var guid in task.Result)
                      {
                          var obj = LoadEE(guid.Key, guid.Value);
                          if (obj != null)
                          {
                              if (obj.Resource != null)
                                  templist.Add(new KeyValuePair<string, string>(guid.Key, obj.Resource.name));
                          }
                      }
  #if DEBUG
                      sw2.Stop();
                      Main.Logger.Log($"Loaded EEs in {sw2.ElapsedMilliseconds}ms");
                      var sw3 = new Stopwatch();
                      sw3.Start();
  #endif
                      var eeinfolist = new List<ResourceInfo>();
                      async Task<List<ResourceInfo>> Stuff()
                      {
                          foreach (var loaded in templist)
                          {
                              await Task.Run(() =>
                              {
                                  var eeinfo = new ResourceInfo(ProcessEEName(loaded.Value), loaded.Value, loaded.Key, typeof(EquipmentEntity));
                                  eeinfolist.Add(eeinfo); NameToEEInfo.Add(loaded.Value, eeinfo);
                              }).ConfigureAwait(false);
                          }
                          return eeinfolist;
                      }
                      var task2 = Stuff();
                      task2.Wait();
  #if DEBUG
                      sw3.Stop();
                      Main.Logger.Log($"Strings processed in {sw3.Elapsed}ms");
  #endif
                      ResourcesLibrary.CleanupLoadedCache();
  #if DEBUG
                      sw.Stop();
                      Main.Logger.Log($"Got all EE's in {sw.ElapsedMilliseconds}ms");
                      Main.Logger.Log($"String processing took {sw.ElapsedMilliseconds - sw2.ElapsedMilliseconds - sw3.ElapsedMilliseconds}");
  #endif
                      task2.Result.Sort((x, y) => string.Compare(x.Name, y.Name));
                      var ResultToArray = task2.Result.ToArray();
                      SaveCachedResources<SerializedEEList>(ResultToArray);
                      return task2.Result;
                  }
                  else throw (new Exception("GUID Map null, what? how?"));*/
            }
        }
        /* private static EEInfo[] m_AllEEs;
         public static EEInfo[] AllEEs
         {
             get
             {
                 /*if (m_AllEEs == null)
                 {
                     m_AllEEs = GetEEs();
                 }*//*
                 return m_AllEEs;
             }
         }*/
        public static List<ResourceInfo> m_AllEEs;
        public static List<ResourceInfo> AllEEs
        {
            get
            {
                if (m_AllEEs == null) m_AllEEs = GetEEs();
                return m_AllEEs;
            }
        }
        public static List<ResourceInfo> m_AllFXs;
        public static List<ResourceInfo> AllFXs
        {
            get
            {
                if (m_AllFXs == null) m_AllFXs = GetFXs();
                return m_AllFXs;
            }
        }
        public static Dictionary<WeaponAnimationStyle, List<ResourceInfo>> m_AllWeapons;
        public static Dictionary<WeaponAnimationStyle, List<ResourceInfo>> AllWeapons
        {
            get
            {
                if (m_AllWeapons == null) m_AllWeapons = GetWeapons();
                return m_AllWeapons;
            }
        }
        public static List<ResourceInfo> m_AllEnchants;
        public static List<ResourceInfo> AllEnchants
        {
            get
            {

                if (m_AllEnchants == null) 
                {
                    if (GetCachedResources<SerializedEnchantmentList>(out var deserialized) == true && deserialized.Version == GameVersion.GetVersion())
                    {
                        ResourceLoader.m_AllEnchants = deserialized.EnchantmentsList.ToList();
                    }
                    else throw new Exception("Main Menu Load failed");
                }
                return m_AllEnchants;
            }
        }
        public static string ProcessEEName(string ee)
        {
            var stringarray = ee.Split('_');
            bool hassetrace = false;
            var newarray = new List<string>();
            foreach (var s in stringarray)
            {
                if (!hassetrace && raceidentifiers.Keys.TryFind(a => s == a, out string raceout))
                {
                    var RaceString = raceidentifiers[raceout];
                    newarray.Add(RaceString + " ");
                    hassetrace = true;
                }
                else if (s == "M")
                {
                    newarray.Add("Male");
                }
                else if (s == "F")
                {
                    newarray.Add("Female");
                }
                else if (s == "Any")
                {
                    newarray.Add("Any");
                }
                else if ((s != "EE" && s != "KEE" && s != "Buff" && s != "NPC" && s != "CMP") && s.Length > 1)
                {
                    StringBuilder SB = new System.Text.StringBuilder(s);
                    var b = 0;
                    for (int c = 0; c < s.Length; c++)
                    {
                        if (c > 0 && ((Char.IsUpper(s[c]) && (!Char.IsUpper(s[c - 1]) || (c < s.Length || !Char.IsUpper(s[c + 1]))) || (Char.IsNumber(s[c]) && (!Char.IsNumber(s[c - 1]) /*|| !Char.IsNumber(s[c-1])*/)))))
                        {
                            SB.Insert(c + b, ' ');
                            b++;
                        }
                    }
                    newarray.Add(SB.ToString());
                }
            }
            //string concatstring = newarray[0];
            StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < newarray.Count; i++)
            {
                sb.Append(' ' + newarray[i]);
                //  concatstring = concatstring.Concat(' ').Concat(newarray[i]);
            }
            /* foreach (var item in newarray)
             {
                 ConcatCharList = ConcatCharList.Concat(' ').Concat(item);
             }
             var AsString = new string(ConcatCharList.ToArray());
             AsString = AsString.TrimStart();
             AsString = AsString.TrimEnd();*/
            // concatstring = concatstring.Trim();
            return sb.ToString().Trim();//concatstring;
        }
    }
}
