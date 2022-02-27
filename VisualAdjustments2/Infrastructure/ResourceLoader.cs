using Kingmaker.Blueprints;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.Utility;
using Kingmaker.Visual.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace VisualAdjustments2
{
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
        public static ResourcesLibrary.LoadedResource LoadEE(string assetid)
        {
            AssetBundle assetBundle = BundlesLoadService.Instance?.RequestBundleForAsset(assetid);
            var resource = assetBundle.LoadAsset_Internal(assetid, typeof(EquipmentEntity));
            var Loaded = new ResourcesLibrary.LoadedResource();
            Loaded.Resource = resource;
            Loaded.AssetId = assetid;
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
        public static EEInfo[] GetEEs()
        {
#if DEBUG
            var sw = new Stopwatch();
            sw.Start();
#endif
            string ProcessEEName(string ee)
            {
                var stringarray = ee.Split('_');
                bool hassetrace = false;
                var newarray = new List<string>();
                var length = stringarray.Length;
                foreach (var s in stringarray)
                {
                    if (!hassetrace && raceidentifiers.Keys.TryFind(a => s == a, out string raceout))
                    {
                        var RaceString = raceidentifiers[raceout];
                        newarray.Add(RaceString);
                        hassetrace = true;
                        length += RaceString.Length;
                    }
                    else if (s == "M")
                    {
                        newarray.Add("Male");
                        length += 4;
                    }
                    else if (s == "F")
                    {
                        newarray.Add("Female");
                        length += 6;
                    }
                    else if (s == "Any")
                    {
                        newarray.Add("Any");
                        length += 3;
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
                        length += SB.Length;
                        newarray.Add(SB.ToString());
                    }
                }
                length += newarray.Count;
                //var concatchararray = new List<char>(length);
                IEnumerable<char> ConcatCharList = new List<char>();
                foreach (var item in newarray)
                {
                    ConcatCharList = ConcatCharList.Concat(' ').Concat(item);
                }
                var AsString = new string(ConcatCharList.ToArray());
                AsString = AsString.TrimStart();
                AsString = AsString.TrimEnd();
                return AsString;
            }
            bool EEName(string name)
            {
                return (name[0] == 'e' && name[1] == 'e' && name[2] == '_');
            }
            if (GetResourceGuidMap() != null)
            {
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

                return task2.Result.ToArray();
            }
            else throw new Exception("Error in getting EEs");
        }
        private static EEInfo[] m_AllEEs;
        public static EEInfo[] AllEEs
        {
            get
            {
                if (m_AllEEs == null)
                {
                    m_AllEEs = GetEEs();
                }
                return m_AllEEs;
            }
        }
    }
}
