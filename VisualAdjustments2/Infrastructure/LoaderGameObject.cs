using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Shields;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using Kingmaker.View.Animation;
using Kingmaker.Visual.CharacterSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static VisualAdjustments2.ResourceLoader;

namespace VisualAdjustments2.Infrastructure
{
    public class LoaderGameObject : MonoBehaviour
    {
        private TextMeshProUGUI Text;
        public IEnumerator WeaponLoader;
        public IEnumerator FXLoader;
        public IEnumerator EELoader;
        public IEnumerator EnchantLoader;
        public IEnumerator LoadChecker;
        private static bool WeaponsLoaded = false;
        private static bool FXLoaded = false;
        private static bool EELoaded = false;
        private static bool EnchantsLoaded = false;

        public static bool Loaded = false;

        public static void CreateLoaderAndLoad()
        {
            if (!Loaded)
            {
                //var gm = new GameObject("Loader");
                var gm = GameObject.Instantiate(Kingmaker.Game.Instance.UI.MainMenu.transform.Find("MessageBox")).gameObject;
                gm.name = "Loader";
                var textgm = gm.transform.Find("Text");
                gm.transform.SetParent(Kingmaker.Game.Instance.UI.MainMenu.transform);
                textgm.localPosition = Vector3.zero;
                gm.transform.localPosition = Vector3.zero;
                gm.transform.localScale = Vector3.one;
                gm.SetActive(true);
                var newcomp = gm.AddComponent<LoaderGameObject>();
                newcomp.Text = textgm.GetComponent<TextMeshProUGUI>();
                newcomp.Text.fontSize = 28;
                newcomp.Text.horizontalAlignment = HorizontalAlignmentOptions.Left;
                textgm.localPosition = new Vector3(77, 0, 0);
                gm.transform.Find("Close").gameObject.SetActive(false);
                newcomp.Load();
                //Loaded = true;
            }
        }

        public void Load()
        {
            try
            {
                this.WeaponLoader = LoadWeapons();
                StartCoroutine(this.WeaponLoader);
                this.FXLoader = LoadFXs();
                StartCoroutine(this.FXLoader);
                this.EELoader = LoadEEs();
                StartCoroutine(this.EELoader);
                this.EnchantLoader = LoadEnchantments();
                StartCoroutine(this.EnchantLoader);
                this.LoadChecker = IsFinished();
                StartCoroutine(this.LoadChecker);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw;
            }
        }

        IEnumerator IsFinished()
        {
            var i = 1;
            while (!Loaded)
            {
#if DEBUG
                Main.Logger.Log("Still loading");
#endif
                if ((WeaponsLoaded && FXLoaded && EELoaded && EnchantsLoaded))
                {
                    Loaded = true;
                }
                else
                {
                    this.Text.text = "VA2 is Loading Resources, please be patient" + new string('.', i%4);
                    i++;
                    yield return new WaitForSeconds(1);
                }

                // Do disappearing stuff
                //Destroy(this.gameObject);
                
            }
            this.gameObject.SetActive(false);
        }

        IEnumerator LoadWeapons()
        {
            if (GetCachedResources<SerializedWeaponList>(out var deserialized) == true &&
                deserialized.Version == GameVersion.GetVersion())
            {
                ResourceLoader.m_AllWeapons = deserialized.WeaponsDictionary;
                WeaponsLoaded = true;
                //   return deserialized.WeaponsDictionary;
            }
            else
            {
#if DEBUG
                var sw = new Stopwatch();
                sw.Start();
#endif
                var allBPs = Kingmaker.Cheats.Utilities.GetAllBlueprints();
                var weapons = (IEnumerable<BlueprintItemEquipmentHand>)allBPs.Entries
                    .Where(a => a.Type == typeof(BlueprintItemWeapon))
                    .Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintItemWeapon>(b.Guid))
                    .OrderBy((bp) => bp.name);
                weapons = allBPs.Entries.Where(a => a.Type == typeof(BlueprintItemShield))
                    .Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintItemShield>(b.Guid)).OrderBy((bp) => bp.name)
                    .Concat(weapons);
                var UniqueVisualWeapons = new List<BlueprintItemEquipmentHand>();
                var i = 0;
                foreach (var item in weapons)
                {
                    var weapon = ResourcesLibrary.TryGetBlueprint<BlueprintItemEquipmentHand>(item.AssetGuid);
                    if (weapon.VisualParameters?.Model != null && !UniqueVisualWeapons.Any(a =>
                            a.VisualParameters.m_WeaponModel.AssetId == weapon.VisualParameters.m_WeaponModel.AssetId))
                    {
                        UniqueVisualWeapons.Add(weapon);
                    }

                    if (i == 15)
                    {
                        i = 0;
                        yield return null;
                    }

                    i++;
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
                        var inf = new ResourceInfo(weapon.m_DisplayNameText, weapon.NameForAcronym,
                            weapon.AssetGuidThreadSafe, weapon.GetType());
                        DictionaryToReturn[wpn.VisualParameters.AnimStyle].Add(inf);
                    }
                    // else
                    {
                    }
                }

                SaveCachedResources<SerializedWeaponList>(DictionaryToReturn);
#if DEBUG
                sw.Stop();
                Main.Logger.Log($"Got all Weapons in {sw.ElapsedMilliseconds}ms");
#endif
                WeaponsLoaded = true;
                //yield return null;
            }
        }

        public static IEnumerator LoadEnchantments()
        {
            if (GetCachedResources<SerializedEnchantmentList>(out var deserialized) == true &&
                deserialized.Version == GameVersion.GetVersion())
            {
                ResourceLoader.m_AllEnchants = deserialized.EnchantmentsList.ToList();
                EnchantsLoaded = true;
                //   return deserialized.WeaponsDictionary;
            }
            else
            {
#if DEBUG
                var sw = new Stopwatch();
                sw.Start();
#endif
                var allBPs = Kingmaker.Cheats.Utilities.GetAllBlueprints();
                var enchants = (IEnumerable<BlueprintWeaponEnchantment>)allBPs.Entries
                    .Where(a => a.Type == typeof(BlueprintWeaponEnchantment))
                    .Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintWeaponEnchantment>(b.Guid))
                    .OrderBy((bp) => bp.name);
                var UniqueEnchants = new List<BlueprintWeaponEnchantment>();
                var i = 0;
                foreach (var item in enchants)
                {
                    var enchant = ResourcesLibrary.TryGetBlueprint<BlueprintWeaponEnchantment>(item.AssetGuid);
                    if (enchant.WeaponFxPrefab != null && !enchant.WeaponFxPrefab.AssetId.IsNullOrEmpty() &&
                        !UniqueEnchants.Any(a => a.WeaponFxPrefab.AssetId == enchant.WeaponFxPrefab.AssetId))
                    {
                        UniqueEnchants.Add(enchant);
                    }

                    if (i == 15)
                    {
                        i = 0;
                        yield return null;
                    }

                    i++;
                }

                var ListToReturn = new List<ResourceInfo>();
                foreach (var enchant in UniqueEnchants)
                {
                    if (enchant?.name?.IsNullOrEmpty() == true &&
                        enchant?.m_EnchantName?.ToString().IsNullOrEmpty() == true) continue;
                    var name = ProcessEEName(enchant?.NameSafe());
                    var inf = new ResourceInfo(name, enchant?.m_EnchantName, enchant?.AssetGuidThreadSafe,
                        enchant?.GetType());
                    ListToReturn.Add(inf);
#if DEBUG
                    Main.Logger.Log(inf.Name + " : " + enchant?.m_EnchantName + " : " + enchant?.name + " : " +
                                    enchant?.NameSafe() + " : " + enchant?.Prefix + " : " + enchant?.Suffix + " : " +
                                    enchant?.Comment);
#endif
                }

                SaveCachedResources<SerializedEnchantmentList>(ListToReturn.ToArray());
                ResourceLoader.m_AllEnchants = ListToReturn;
#if DEBUG
                sw.Stop();
                Main.Logger.Log($"Got all Enchantments in {sw.ElapsedMilliseconds}ms");
#endif
                EnchantsLoaded = true;
                //yield return null;
            }
        }

        IEnumerator LoadEEs()
        {
            if (GetCachedResources<SerializedEEList>(out var deserialized) == true &&
                deserialized.Version == GameVersion.GetVersion())
            {
                foreach (var ee in deserialized.allEEs)
                {
                    NameToEEInfo.Add(ee.Name_Internal, ee);
                }

                ResourceLoader.m_AllEEs = deserialized.allEEs.ToList();
                //return deserialized.allEEs.ToList();
                EELoaded = true;
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
                        foreach (var value in list.Where(b =>
                                     (b.Value[0] == 'e' && b.Value[1] == 'e' && b.Value[2] == '_')))
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
                    int i = 0;
                    foreach (var guid in task.Result)
                    {
                        var obj = LoadEE(guid.Key, guid.Value);
                        if (obj != null)
                        {
                            if (obj.Resource != null)
                                templist.Add(new KeyValuePair<string, string>(guid.Key, obj.Resource.name));
                        }

                        if (i == 15)
                        {
                            i = 0;
                            yield return null;
                        }

                        i++;
                    }
#if DEBUG
                    sw2.Stop();
                    Main.Logger.Log($"Loaded EEs in {sw2.ElapsedMilliseconds}ms");
                    var sw3 = new Stopwatch();
                    sw3.Start();
#endif
                    var eeinfolist = new List<ResourceInfo>();
                    // async Task<List<ResourceInfo>> Stuff()
                    {
                        i = 0;
                        foreach (var loaded in templist)
                        {
                            // Task.Run(() =>
                            // {
                            var eeinfo = new ResourceInfo(ProcessEEName(loaded.Value), loaded.Value, loaded.Key,
                                typeof(EquipmentEntity));
                            eeinfolist.Add(eeinfo);
                            NameToEEInfo.Add(loaded.Value, eeinfo);
                            // }).ConfigureAwait(false);
                            if (i == 15)
                            {
                                i = 0;
                                yield return null;
                            }

                            i++;
                        }
                        // return eeinfolist;
                    }
                    // var task2 = Stuff();
                    //  task2.Wait();
#if DEBUG
                    sw3.Stop();
                    Main.Logger.Log($"Strings processed in {sw3.Elapsed}ms");
#endif
                    ResourcesLibrary.CleanupLoadedCache();
#if DEBUG
                    sw.Stop();
                    Main.Logger.Log($"Got all EE's in {sw.ElapsedMilliseconds}ms");
                    EELoaded = true;
                    Main.Logger.Log(
                        $"String processing took {sw.ElapsedMilliseconds - sw2.ElapsedMilliseconds - sw3.ElapsedMilliseconds}");
#endif
                    eeinfolist.Sort((x, y) => string.Compare(x.Name, y.Name));

                    var ResultToArray = eeinfolist.ToArray();
                    SaveCachedResources<SerializedEEList>(ResultToArray);
                    ResourceLoader.m_AllEEs = eeinfolist;
                    EELoaded = true;
                    //return task2.Result;
                }
                else throw (new Exception("GUID Map null, what? how?"));
            }

            yield return null;
        }

        IEnumerator LoadFXs()
        {
            if (ResourceLoader.GetCachedResources<SerializedFXList>(out var deserializedFX) == true &&
                deserializedFX.Version == GameVersion.GetVersion())
            {
                ResourceLoader.AbilityGuidToFXBlocker = deserializedFX.AbilityGuidToFXBlocker;
                ResourceLoader.m_AllFXs = deserializedFX.allFXs.ToList();
                FXLoaded = true;
                //return deserializedFX.allFXs.ToList();
            }
            else
            {
#if DEBUG
                var sw = new Stopwatch();
                sw.Start();
#endif

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
                var i = 0;
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
                        if (bp.Buff.FxOnRemove?.AssetId?.IsNullOrEmpty() == false)
                            firstMatch.FXGuids.Add(bp.Buff.FxOnRemove.AssetId);
                        if (bp.Buff.FxOnStart?.AssetId?.IsNullOrEmpty() == false)
                            firstMatch.FXGuids.Add(bp.Buff.FxOnStart.AssetId);
                        if (i == 15)
                        {
                            i = 0;
                            yield return null;
                        }

                        i++;
                        // }
                    }
                    else if (bp.m_Buff.guid != null && bp.m_Buff.guid != "" && bp.GetBeneficialBuffs())
                    {
                        // Main.Logger.Log(bp.NameForAcronym + " Activatable");
                        resourceInfList.Add(new ResourceInfo(bp.m_DisplayName, bp.NameForAcronym,
                            bp.AssetGuidThreadSafe, bp.GetType()));
                        var blocker = new FXBlocker(bp);
                        AbilityGuidToFXBlocker[bp.AssetGuidThreadSafe] = blocker;
                        wack2.Add(blocker);
#if DEBUG
                        Main.Logger.Log(bp.NameForAcronym);
#endif
                        if (i == 15)
                        {
                            i = 0;
                            yield return null;
                        }

                        i++;
                    }
                }

                i = 0;
                foreach (var ability in allbp.Entries.Where(b => b.Type == typeof(BlueprintAbility)))
                {
                    if (ability.Guid == "416f867258c24b709bb1b74a2ad1ec89" ||
                        ability.Guid == "0bb76d697b7f4d0faa892df309be3710" ||
                        ability.Guid ==
                        "cbfd9c4f683c4394ba3543058f045682") //Wacky COTW abilities in wrath for some reason.
                    {
                        continue;
                    }

                    var bp = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(ability.Guid);
                    if ((!bp?.HiddenInInspector == true) && (bp?.GetBeneficialBuffs() == true))
                    {
                        //Main.Logger.Log(ability.Name);
                        var firstMatch = wack2.FirstOrDefault(b => b.DisplayName == bp.m_DisplayName);
                        if (firstMatch != null)
                        {
                            void Merge(BlueprintBuff buffBP)
                            {
                                //Main.Logger.Log($"Merged: {bp.NameForAcronym}");
                                firstMatch.AllAbilityGUIDs.Add(bp.AssetGuidThreadSafe);
                                if (buffBP.FxOnRemove?.AssetId?.IsNullOrEmpty() == false)
                                    firstMatch.FXGuids.Add(buffBP.FxOnRemove.AssetId);
                                if (buffBP.FxOnStart?.AssetId?.IsNullOrEmpty() == false)
                                    firstMatch.FXGuids.Add(buffBP.FxOnStart.AssetId);
                            }

                            var actionsList = bp?.GetComponent<AbilityEffectRunAction>()?.Actions?.Actions;
                            if (actionsList != null && actionsList.Length > 0)
                            {
                                var buff = ((ContextActionApplyBuff)actionsList.FirstOrDefault(x =>
                                    x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                                if (buff != null)
                                {
                                    Merge(buff);
                                }

                                var conditionalAction =
                                    ((Conditional)actionsList.FirstOrDefault(x => x?.GetType() == typeof(Conditional)));
                                if (conditionalAction != null)
                                {
                                    if (conditionalAction.IfTrue?.Actions != null)
                                    {
                                        var ifTrueAction =
                                            ((ContextActionApplyBuff)conditionalAction.IfTrue.Actions.FirstOrDefault(
                                                x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
                                        if (ifTrueAction != null)
                                        {
                                            //   Main.Logger.Log($"MergedConditionalIfTrue {bp.NameForAcronym}");
                                            Merge(ifTrueAction);
                                        }
                                    }

                                    if (conditionalAction.IfFalse?.Actions != null)
                                    {
                                        var ifFalseAction =
                                            ((ContextActionApplyBuff)conditionalAction.IfFalse.Actions.FirstOrDefault(
                                                x => x?.GetType() == typeof(ContextActionApplyBuff)))?.Buff;
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
                            resourceInfList.Add(new ResourceInfo(bp.m_DisplayName, bp.NameForAcronym,
                                bp.AssetGuidThreadSafe, bp.GetType()));
                            var blocker = new FXBlocker(bp);
                            AbilityGuidToFXBlocker[bp.AssetGuidThreadSafe] = blocker;
                            wack2.Add(blocker);
                        }

                        if (i == 15)
                        {
                            i = 0;
                            yield return null;
                        }

                        i++;
                    }
                }

                resourceInfList.Sort((x, y) => string.Compare(x.Name, y.Name));
                SaveCachedResources<SerializedFXList>(resourceInfList.ToArray());
                ResourceLoader.m_AllFXs = resourceInfList;
#if DEBUG
                sw.Stop();
                Main.Logger.Log($"Got all FXs in {sw.ElapsedMilliseconds}ms");
#endif
                FXLoaded = true;
                //return resourceInfList;
            }
            //yield return null;
        }
    }
}