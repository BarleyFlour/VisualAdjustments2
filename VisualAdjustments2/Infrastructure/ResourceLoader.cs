using Kingmaker;
using Kingmaker.Blueprints;
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


namespace VisualAdjustments2
{
    public class SerializedEEList
    {
        public SerializedEEList(EEInfo[] allees, string version)
        {
            allEEs = allees;
            Version = version;
        }
        public EEInfo[] allEEs;
        public string Version;
    }
    public struct EEInfo
    {
        public EEInfo(string name, string internalname, string guid)
        {
            Name = name;
            Name_Internal = internalname;
            GUID = guid;
        }
        public readonly string Name;
        public readonly string Name_Internal;
        public readonly string GUID;
    }
    public static class ResourceLoader
    {
        public static void Save(this EEInfo[] array)
        {

        }
        public static ResourcesLibrary.LoadedResource LoadEE(string assetid)
        {
            var sw = new Stopwatch();
            sw.Start();
            AssetBundle assetBundle = BundlesLoadService.Instance?.RequestBundleForAsset(assetid);
            sw.Stop();
            Main.Logger.Log($"Got Bundle in {sw.ElapsedTicks} ticks");
            sw.Restart();
            var resource = assetBundle.LoadAsset_Internal(assetid, typeof(EquipmentEntity));
            sw.Stop();
            Main.Logger.Log($"Loaded Asset in {sw.ElapsedTicks} ticks");
            sw.Restart();
            var Loaded = new ResourcesLibrary.LoadedResource();
            Loaded.Resource = resource;
            Loaded.AssetId = assetid;
            sw.Stop();
            Main.Logger.Log($"Made LoadedResource in {sw.ElapsedTicks} ticks");
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
            ["SN"] = "Skeleton"
        };
        public static void SaveCachedEEs(EEInfo[] array)
        {
            var filepath = Path.Combine(Main.ModEntry.Path, "CachedEEs.json");
            try
            {
                JsonSerializer serializer = new JsonSerializer();
#if (DEBUG)
                serializer.Formatting = Formatting.Indented;
#endif
                using (StreamWriter sw = new StreamWriter(filepath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, new SerializedEEList(array, GameVersion.GetVersion()));
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
        public static bool GetCachedEEs(out SerializedEEList info)
        {
#if DEBUG
            info = null;
            return false;
#endif
            var filepath = Path.Combine(Main.ModEntry.Path, "CachedEEs.json");
            if (File.Exists(filepath))
            {
                try
                {
                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamReader sr = new StreamReader(filepath))
                    using (JsonTextReader reader = new JsonTextReader(sr))
                    {
                        SerializedEEList deserialized = serializer.Deserialize<SerializedEEList>(reader);
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
        public static void StartEEGetting()
        {
            AssetBundle assetBundle = BundlesLoadService.Instance.RequestBundle("equipment");
            var SortedGuidMap = GetResourceGuidMap().Where(b => b.Value[0] == 'e' && b.Value[1] == 'q');
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            var count = SortedGuidMap.Count();
            AllEEs = new List<EEInfo>();
            var Keys = SortedGuidMap.Select(b => b.Key).ToArray();
            for (int i = 0; i < count; i++)
            {
                var assetguid = Keys[i];
                var request = assetBundle.LoadAssetAsync_Internal(assetguid, typeof(EquipmentEntity));
#if DEBUG
                if (i == count - 1)
                {
                    request.completed += (AsyncOperation _) => { if (request.asset != null) HandleEEGotten((EquipmentEntity)request.asset, assetguid); sw.Stop(); HandleFinalEEGotten(); Main.Logger.Log($"Got all EEs in {sw.ElapsedMilliseconds}ms"); };
                }
                else
#else
                if (i == count-1)
                {
                    request.completed += (AsyncOperation _) => { if (request.asset != null) HandleEEGotten((EquipmentEntity)request.asset, assetguid); sw.Stop(); HandleFinalEEGotten(); };
                }
                else
#endif
                {
                    request.completed += (AsyncOperation _) => { if (request.asset != null) HandleEEGotten((EquipmentEntity)request.asset, assetguid); };
                }
            }
        }
        public static void HandleFinalEEGotten()
        {
            AllEEs.OrderBy(a => a.Name);
        }
        public static void HandleEEGotten(EquipmentEntity ee,string guid)
        {
            AllEEs.Add(new EEInfo(ProcessEEName(ee.name), ee.name, guid));
        }
        //Old slow EE getter
        /*public static EEInfo[] GetEEs()
        {
            if (GetCachedEEs(out var deserialized))
            {
                return deserialized.allEEs;
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
                        foreach (var value in list.Where(b => (b.Value[0] == 'e' && b.Value[1] == 'q')))
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
                        var obj = LoadEE(guid.Key);
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
                    var eeinfolist = new List<EEInfo>();
                    async Task<List<EEInfo>> Stuff()
                    {
                        foreach (var loaded in templist)
                        {
                            await Task.Run(() => { eeinfolist.Add(new EEInfo(ProcessEEName(loaded.Value), loaded.Value, loaded.Key)); }).ConfigureAwait(false);
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
                    var ResultToArray = task2.Result.ToArray();
                    SaveCachedEEs(ResultToArray);
                    return ResultToArray;
                }
                else throw (new Exception("GUID Map null, what? how?"));
            }
        }*/
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
        public static List<EEInfo> AllEEs;
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
                    newarray.Add(RaceString);
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
                else if ((s != "EE" && s != "KEE" && s != "Buff") && s.Length > 1)
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
                sb.Append(newarray[i]);
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
