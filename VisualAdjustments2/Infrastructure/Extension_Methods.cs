using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.ResourceLinks;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Menu;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.View;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.UI.Controls.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VisualAdjustments2.UI;
using UniRx;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.Visual.Animation.Kingmaker;
using Owlcat.Runtime.UniRx;
using Owlcat.Runtime.Core;
using Kingmaker.Utility;
using Kingmaker.Visual.Animation;
using Owlcat.Runtime.Core.Utils;
using Kingmaker.Visual.Particles;
using Kingmaker.Items.Slots;
using Kingmaker.Items;
using Owlcat.Runtime;
using Kingmaker.Blueprints.Root;

namespace VisualAdjustments2.Infrastructure
{
    public static class Extension_Methods
    {
        public static void SetupFromServicePCView(this VisualWindowsMenuEntityPCView newView, ServiceWindowsMenuEntityPCView old)
        {
            newView.m_Button = old.m_Button;
            newView.m_Button.m_OnLeftClick.RemoveAllListeners();
            newView.m_Label = old.m_Label;
        }
        public static DollData SetupForStoryCompanion(this UnitPartDollData val)
        {
            try
            {
                var state = new DollState();
                state.SetRace(val.Owner.Progression.Race);
                state.SetClass(val.Owner.Progression.GetEquipmentClass());
                state.SetGender(val.Owner.Gender);
                state.CreateWarpaints(default,state.Race.RaceId);
                state.CreateTattos(default);
                //state.Setup(val.Owner);
                val.Owner.SaveDollState(state);
                return state.CreateData();
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
                throw;
            }
        }
        public static ResourceInfo? ToEEInfo(this EquipmentEntity ee)
        {
            if (ResourceLoader.NameToEEInfo.TryGetValue(ee.name, out ResourceInfo val))
            {
                return val;
            }
            return null;
        }
        public static void Setup(this DollState state, UnitEntityData unit)
        {
            // state.m_DefaultSettings = settings.Default;
            state.Gender = unit.Gender;
            state.Race = unit.Progression.Race;
            state.RacePreset = default;
            // state.Head = new DollState.EEAdapter(state.m_DefaultSettings.Head);
            //PregenDollSettings.Entry defaultSettings = state.m_DefaultSettings;
            BlueprintRace race = state.Race;
            // state.CreateWarpaints(defaultSettings, (race != null) ? race.RaceId : Kingmaker.Blueprints.Race.Human);
            /*state.CreateTattos(state.m_DefaultSettings);
            state.Scar = new DollState.EEAdapter(state.m_DefaultSettings.Scar);
            state.Hair = new DollState.EEAdapter(state.m_DefaultSettings.Hair);
            state.Eyebrows = new DollState.EEAdapter(state.m_DefaultSettings.Eyebrows);
            state.Beard = new DollState.EEAdapter(state.m_DefaultSettings.Beard);
            state.Horn = new DollState.EEAdapter(state.m_DefaultSettings.Horn);
            state.HairRampIndex = state.m_DefaultSettings.HairRampIndex;
            state.SkinRampIndex = state.m_DefaultSettings.SkinRampIndex;
            state.EyesColorRampIndex = state.m_DefaultSettings.EyesColorRampIndex;
            state.HornsRampIndex = state.m_DefaultSettings.HornsRampIndex;*/
            ClassData classData = unit.Progression.Classes.FirstItem<ClassData>();
            BlueprintCharacterClass characterClass = (classData != null) ? classData.CharacterClass : null;
            state.CharacterClass = characterClass;
            if (state.CharacterClass != null)
            {
                //state.m_EquipmentIndex.Add(state.CharacterClass, new DollState.EquipmentIndexByClass
                //{
                //    EquipmentRampIndex = state.m_DefaultSettings.EquipmentRampIndex,
                //    EquipmentRampIndexSecondary = state.m_DefaultSettings.EquipmentRampIndexSecondary
                //});
            }
            List<EquipmentEntityLink> clothes;
            if (state.CharacterClass == null)
            {
                clothes = ((state.Gender == Gender.Male) ? DollState.Root.MaleClothes.ToList<EquipmentEntityLink>() : DollState.Root.FemaleClothes.ToList<EquipmentEntityLink>());
            }
            else
            {
                BlueprintCharacterClass characterClass2 = state.CharacterClass;
                Gender gender = state.Gender;
                BlueprintRace race2 = state.Race;
                clothes = characterClass2.GetClothesLinks(gender, (race2 != null) ? race2.RaceId : Kingmaker.Blueprints.Race.Human);
            }
            state.Clothes = clothes;
            BlueprintRace race3 = state.Race;
            state.Scars = DollState.GetScarsList((race3 != null) ? race3.RaceId : Kingmaker.Blueprints.Race.Human);
            state.m_TrackPortrait = false;
            state.UpdateMechanicsEntities(unit.Descriptor);
            state.Validate();
            state.Updated();
        }
        /// <summary>
        /// Verbatim from VA
        /// </summary>
        public static void SetupFromUnitLocal(this DollState state, UnitEntityData unit, SpecialDollType specialDollType = SpecialDollType.None)
        {
            try
            {
                DollData dollData;
                if (specialDollType == SpecialDollType.None)
                {
                    UnitPartDollData unitPartDollData = unit.Get<UnitPartDollData>();
                    dollData = ((unitPartDollData != null) ? unitPartDollData.Default : null);
                }
                else
                {
                    UnitPartDollData unitPartDollData2 = unit.Get<UnitPartDollData>();
                    dollData = ((unitPartDollData2 != null) ? unitPartDollData2.GetSpecial(specialDollType) : null);
                }
                DollData dollData2 = dollData;
                if (dollData2 == null)
                {
                    return;
                }
                state.Gender = dollData2.Gender;
                state.Race = Kingmaker.Game.Instance.BlueprintRoot.Progression.CharacterRaces.First(a => a.Presets.Contains(dollData2.RacePreset));
                state.SetRacePreset(dollData2.RacePreset);
                state.SetRace(state.Race);
                state.SetRacePreset(dollData2.RacePreset);
                state.Scars = DollState.GetScarsList(state.Race.RaceId);
                state.CreateTattos(state?.m_DefaultSettings);
                state.CreateWarpaints(state?.m_DefaultSettings, state.Race.RaceId);
                //state.RacePreset = dollData2.RacePreset;
                ClassData classData = unit.Progression.Classes.FirstOrDefault<ClassData>();
                state.CharacterClass = ((classData != null) ? classData.CharacterClass : null);
                foreach (ClassData classData2 in unit.Progression.Classes)
                {
                    state.m_EquipmentIndex.Add(classData2.CharacterClass, new DollState.EquipmentIndexByClass
                    {
                        EquipmentRampIndex = classData2.CharacterClass.PrimaryColor,
                        EquipmentRampIndexSecondary = classData2.CharacterClass.SecondaryColor
                    });
                }
                CustomizationOptions customizationOptions = (state.Gender == Gender.Male) ? state.Race.MaleOptions : state.Race.FemaleOptions;
                var j = unit?.View?.CharacterAvatar?.EquipmentEntities;
                if (j != null)
                {
                    int i = 0;
                    int y = 0;
                    foreach (var ee in unit?.View?.CharacterAvatar?.EquipmentEntities)
                    {
                        //EquipmentEntity ee = enumerator2.Current;
                        if (customizationOptions.Heads.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Head = new DollState.EEAdapter(customizationOptions.Heads.FirstOrDefault(a => a.Load() == ee));
                            //state.Head = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Eyebrows.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Eyebrows = new DollState.EEAdapter(customizationOptions.Eyebrows.FirstOrDefault(a => a.Load() == ee));
                            ///state.Eyebrows = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Hair.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        //if (ee.name.Contains("Hair") || ee.name.Contains("hair"))
                        {
                            state.Hair = new DollState.EEAdapter(customizationOptions.Hair.FirstOrDefault(a => a.Load() == ee));
                            //state.Hair = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Beards.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Beard = new DollState.EEAdapter(customizationOptions.Beards.FirstOrDefault(a => a.Load() == ee));
                            //state.Beard = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Horns.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Horn = new DollState.EEAdapter(customizationOptions.Horns.FirstOrDefault(a => a.Load() == ee));
                            // state.Horn = new DollState.EEAdapter(ee);
                        }
                        if (state.Scars.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            var NewEEAdapter = new DollState.EEAdapter();
                            NewEEAdapter.m_Link = DollState.GetScarsList(state.Race.RaceId).FirstOrDefault((EquipmentEntityLink link2) => link2.Load(false) == ee);
                            state.Scar = NewEEAdapter;
                        }

                        //if (state.Warprints.Contains((DollState.DollPrint link) => link.PaintEE.Load() == ee))
                        {
                            //Main.Logger.Log("joe biden");
                           // state.Horn = new DollState.EEAdapter(customizationOptions.Horns.FirstOrDefault(a => a.Load() == ee));
                            // state.Horn = new DollState.EEAdapter(ee);
                        }
                       // if (state.Warprints.Contains((DollState.DollPrint link) => link.PaintEE.Load() == ee))
                        {
                            
                            //var NewEEAdapter = new DollState.EEAdapter();
                            var link = DollState.GetWarpaintsList(state.Race.RaceId).FirstOrDefault((EquipmentEntityLink link2) => link2.Load(false) == ee);
                            if (i < 5 && link != null)
                            {
                                //state.Warprints[i].PaintEE = new DollState.EEAdapter(link);
                                state.SetWarpaint(link, i);
                                i++;
                                //Main.Logger.Log(link.Load().name);
                            }
                        }
                        //state.CreateTattos(null);
                        //if (state.Tattoos.Contains((DollState.DollPrint link) => link.PaintEE.Load() == ee))
                        {
                           // var NewEEAdapter = new DollState.EEAdapter();
                            var link = BlueprintRoot.Instance.CharGen.TattoosForCustomization.FirstOrDefault((EquipmentEntityLink link2) => link2.Load(false) == ee);
                            if (y <= 5 && link != null)
                            {
                                state.SetTattoo(link, y);
                                y++;
                               // Main.Logger.Log(link.Load().name);
                            }
                        }
                    }
                }
                /*using (List<EquipmentEntity>.Enumerator enumerator2 = unit.View.CharacterAvatar.EquipmentEntities.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {

                        /* if (state.Warprints.Contains((DollState.DollPrint link) => link.PaintEE.m_Entity == ee))
                         {
                             var NewEEAdapter = new DollState.EEAdapter(ee);
                             NewEEAdapter.m_Link = DollState.GetWarpaintsList(dollData2.RacePreset.RaceId).FirstOrDefault((EquipmentEntityLink link2) => link2.Load(false) == ee);
                             state.SetWarpaint(NewEEAdapter,NewEEAdapter.m_Entity.ramp);
                             // state.Warpaint = new DollState.EEAdapter(ee);
                         }*//*
                    }
                }*/
                List<EquipmentEntityLink> clothes;
                if (state.CharacterClass == null)
                {
                    clothes = ((state.Gender == Gender.Male) ? DollState.Root.MaleClothes.ToList<EquipmentEntityLink>() : DollState.Root.FemaleClothes.ToList<EquipmentEntityLink>());
                }
                else
                {
                    BlueprintCharacterClass characterClass = state.CharacterClass;
                    Gender gender = state.Gender;
                    BlueprintRace race = state.Race;
                    clothes = characterClass.GetClothesLinks(gender, (race != null) ? race.RaceId : Kingmaker.Blueprints.Race.Human);
                }
                state.Clothes = clothes;
                // state.Warpaints = BlueprintRoot.Instance.CharGen.WarpaintsForCustomization.ToList<EquipmentEntityLink>();
                BlueprintRace race2 = state.Race;
                state.Scars = DollState.GetScarsList((race2 != null) ? race2.RaceId : Kingmaker.Blueprints.Race.Human);
                
                if (unit?.View?.CharacterAvatar != null)
                {

                    if (state.Hair.m_Link?.Load() != null)
                    {
                        state.HairRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Hair.Load());
                    }
                    DollState.EEAdapter eeadapter = state.GetSkinEntities().FirstOrDefault<DollState.EEAdapter>();
                    if (eeadapter.m_Link?.Load() != null)
                    {
                        state.SkinRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(eeadapter.Load());
                    }
                    if (state.Horn.m_Link?.Load() != null)
                    {
                        state.HornsRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Horn.Load());
                    }
                    for (int i = 0; i <= 5; i++)
                    {
                        if (state.Warprints.Count >= i && state.Warprints.ElementAtOrDefault(i) != null)
                        {
                            state.Warprints.ElementAt(i).PaintRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Warprints.ElementAt(i).PaintEE.Load());
                        }
                    }
                    for (int i = 0; i <= 5; i++)
                    {
                        if (state.Tattoos.Count >= i && state.Tattoos.ElementAtOrDefault(i) != null)
                        {
                            state.Tattoos.ElementAt(i).PaintRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Tattoos.ElementAt(i).PaintEE.Load());
                        }
                    }
                }
                /* if (state.Warprints != null)
                 {
                     foreach()
                         {
                     state.WarpaintRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Warpaint.Load());
                 }*/
                state.m_TrackPortrait = false;
                state.UpdateMechanicsEntities(unit.Descriptor);
                state.EquipmentRampIndex = dollData2.ClothesPrimaryIndex;
                state.EquipmentRampIndexSecondary = dollData2.ClothesSecondaryIndex;
                //state.Updated();
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
        public static T AddComponent<T>(this GameObject game, T duplicate) where T : Component
        {
            T target = game.AddComponent<T>();
            foreach (PropertyInfo x in typeof(T).GetProperties())
                if (x.CanWrite)
                    x.SetValue(target, x.GetValue(duplicate));
            return target;
        }
        public static ListViewItemPCView ConvertToListPCView(this CharGenFeatureSelectorItemPCView oldview)
        {
            try
            {
                var newcomp = oldview.gameObject.AddComponent<ListViewItemPCView>();
                newcomp.m_Button = oldview.m_Button;
                newcomp.m_DisplayName = oldview.m_FeatureNameText;
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("CollapseButton"));
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("RecommendationPlace"));
                /*foreach(var comp in oldview.transform.Find("IconPlace").GetComponents<Component>().Concat(oldview.transform.Find("IconPlace").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }
                foreach (var comp in oldview.transform.Find("TextContainer/Description").GetComponents<Component>().Concat(oldview.transform.Find("TextContainer/Description").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }

                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("IconPlace"), true);
                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("TextContainer/Description"), true);*/
                var iconplace = oldview.transform.Find("IconPlace");

                //iconplace.localScale = new Vector3((float)0.75, (float)0.75, (float)0.75);
                var bg = iconplace.Find("BG");
                bg.Find("Icon").gameObject.SetActive(false);
                var acronym = bg.Find("Acronym");
                acronym.gameObject.SetActive(true);
                var acronymTMP = acronym.GetComponent<TextMeshProUGUI>();
                acronymTMP.fontSizeMax = 46;
                acronymTMP.fontSize = 46;
                acronymTMP.verticalAlignment = VerticalAlignmentOptions.Geometry;
                acronymTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
                //  newcomp.m_IconText = acronymTMP;
                oldview.transform.Find("TextContainer/Description").gameObject.SetActive(false);
                UnityEngine.Component.Destroy(oldview);

                var canvas = Kingmaker.Game.Instance.UI.Canvas != null ? Kingmaker.Game.Instance.UI.Canvas.transform.Find("ServiceWindowsPCView") : Kingmaker.Game.Instance.UI.GlobalMapCanvas.transform.Find("ServiceWindowsConfig");
                var InstantiatedButton = UnityEngine.GameObject.Instantiate(canvas.Find("InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/NonUsable"), newcomp.transform);
                var icon = InstantiatedButton.Find("Icon");
                icon.gameObject.SetActive(true);
                newcomp.m_AddButton = InstantiatedButton.GetComponent<OwlcatMultiButton>();
                UnityEngine.Component.DestroyImmediate(icon.gameObject.GetComponent<Image>(), true);
                var newtxt = icon.gameObject.AddComponent<TextMeshProUGUI>(acronymTMP);
                newtxt.text = ">>";
                newtxt.fontSizeMin = 45;
                newcomp.m_IconText = newtxt;
                iconplace.gameObject.SetActive(false);

                var Layout = oldview.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>();
                Layout.spacing = 10;
                /*newtxt.font = acronymTMP.font;
                newtxt.alignment = TextAlignmentOptions.CenterGeoAligned;
                newtxt.horizontalAlignment = HorizontalAlignmentOptions.Geometry;
                newtxt.verticalAlignment = VerticalAlignmentOptions.Geometry;
                newtxt.fontSizeMax = 46;
                newtxt.material = acronymTMP.material;
                newtxt.fontStyle = FontStyles.Bold;
                newtxt.color = acronymTMP.color;
                newcomp.m_IconText = newtxt;*/

                return newcomp;
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
        public static ClassOutfitSelectorButtonPCView ConvertToClassButtonPCView(this CharGenFeatureSelectorItemPCView oldview)
        {
            try
            {
                var newcomp = oldview.gameObject.AddComponent<ClassOutfitSelectorButtonPCView>();
                newcomp.Button = oldview.m_Button;
                //newcomp.m_DisplayName = oldview.m_FeatureNameText;
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("CollapseButton"));
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("RecommendationPlace"));
                /*foreach(var comp in oldview.transform.Find("IconPlace").GetComponents<Component>().Concat(oldview.transform.Find("IconPlace").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }
                foreach (var comp in oldview.transform.Find("TextContainer/Description").GetComponents<Component>().Concat(oldview.transform.Find("TextContainer/Description").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }

                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("IconPlace"), true);
                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("TextContainer/Description"), true);*/
                var iconplace = oldview.transform.Find("IconPlace");
                iconplace.gameObject.SetActive(false);


                //iconplace.localScale = new Vector3((float)0.75, (float)0.75, (float)0.75);
                /*var bg = iconplace.Find("BG");
                bg.Find("Icon").gameObject.SetActive(false);
                var acronym = bg.Find("Acronym");
                acronym.gameObject.SetActive(true);
                var acronymTMP = acronym.GetComponent<TextMeshProUGUI>();
                
                acronymTMP.fontSizeMax = 46;
                acronymTMP.fontSize = 46;
                acronymTMP.verticalAlignment = VerticalAlignmentOptions.Geometry;
                acronymTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
                acronym.gameObject.SetActive(false);*/


                //  newcomp.m_IconText = acronymTMP;
                var txtContainer = oldview.transform.Find("TextContainer");
                txtContainer.Find("Description").gameObject.SetActive(false);
                txtContainer.GetComponent<LayoutElement>().minHeight = 45;

                newcomp.Label = txtContainer.Find("Name").GetComponent<TextMeshProUGUI>();
                newcomp.Label.fontSizeMax = 24;
                newcomp.Label.text = "";
                UnityEngine.Component.Destroy(oldview);

                var Layout = oldview.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>();
                Layout.spacing = 10;
                /*newtxt.font = acronymTMP.font;
                newtxt.alignment = TextAlignmentOptions.CenterGeoAligned;
                newtxt.horizontalAlignment = HorizontalAlignmentOptions.Geometry;
                newtxt.verticalAlignment = VerticalAlignmentOptions.Geometry;
                newtxt.fontSizeMax = 46;
                newtxt.material = acronymTMP.material;
                newtxt.fontStyle = FontStyles.Bold;
                newtxt.color = acronymTMP.color;
                newcomp.m_IconText = newtxt;*/

                return newcomp;
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
        public static ListViewItemPCView ConvertToEquipmentPCView(this CharGenFeatureSelectorItemPCView oldview)
        {
            try
            {
                var newcomp = oldview.gameObject.AddComponent<ListViewItemPCView>();
                newcomp.m_Button = oldview.m_Button;
                newcomp.m_DisplayName = oldview.m_FeatureNameText;
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("CollapseButton"));
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("RecommendationPlace"));
                /*foreach(var comp in oldview.transform.Find("IconPlace").GetComponents<Component>().Concat(oldview.transform.Find("IconPlace").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }
                foreach (var comp in oldview.transform.Find("TextContainer/Description").GetComponents<Component>().Concat(oldview.transform.Find("TextContainer/Description").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }

                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("IconPlace"), true);
                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("TextContainer/Description"), true);*/
                var iconplace = oldview.transform.Find("IconPlace");

                //iconplace.localScale = new Vector3((float)0.75, (float)0.75, (float)0.75);
                var bg = iconplace.Find("BG");
                bg.Find("Icon").gameObject.SetActive(false);
                var acronym = bg.Find("Acronym");
                acronym.gameObject.SetActive(true);
                var acronymTMP = acronym.GetComponent<TextMeshProUGUI>();
                acronymTMP.fontSizeMax = 46;
                acronymTMP.fontSize = 46;
                acronymTMP.verticalAlignment = VerticalAlignmentOptions.Geometry;
                acronymTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
                //  newcomp.m_IconText = acronymTMP;
                oldview.transform.Find("TextContainer/Description").gameObject.SetActive(false);
                UnityEngine.Component.Destroy(oldview);


                //var InstantiatedButton = UnityEngine.GameObject.Instantiate(StaticCanvas.Instance.transform.Find("ServiceWindowsPCView/InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/NonUsable"), newcomp.transform);
                //var icon = InstantiatedButton.Find("Icon");
                // icon.gameObject.SetActive(true);
                //newcomp.m_AddButton = InstantiatedButton.GetComponent<OwlcatMultiButton>();
                //UnityEngine.Component.DestroyImmediate(icon.gameObject.GetComponent<Image>(), true);
                //var newtxt = icon.gameObject.AddComponent<TextMeshProUGUI>(acronymTMP);
                // newtxt.text = ">>";
                //  newtxt.fontSizeMin = 45;
                //newcomp.m_IconText = newtxt;
                iconplace.gameObject.SetActive(false);

                var Layout = oldview.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>();
                Layout.spacing = 10;
                /*newtxt.font = acronymTMP.font;
                newtxt.alignment = TextAlignmentOptions.CenterGeoAligned;
                newtxt.horizontalAlignment = HorizontalAlignmentOptions.Geometry;
                newtxt.verticalAlignment = VerticalAlignmentOptions.Geometry;
                newtxt.fontSizeMax = 46;
                newtxt.material = acronymTMP.material;
                newtxt.fontStyle = FontStyles.Bold;
                newtxt.color = acronymTMP.color;
                newcomp.m_IconText = newtxt;*/

                return newcomp;
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
        public static BuffButtonPCView ConvertToBuffButtonPCView(this CharGenFeatureSelectorItemPCView oldview)
        {
            try
            {
                var newcomp = oldview.gameObject.AddComponent<BuffButtonPCView>();
                newcomp.m_Button = oldview.m_Button;
                newcomp.m_FeatureAcronymText = oldview.m_FeatureAcronymText;
                newcomp.m_FeatureImage = oldview.m_FeatureImage;
                newcomp.m_FeatureNameText = oldview.m_FeatureNameText;
                newcomp.m_NotAvailableLabel = oldview.m_NotAvailableLabel;
                newcomp.m_FeatureDescription = oldview.transform.Find("TextContainer/Description").GetComponent<TextMeshProUGUI>();
                //newcomp.m_DisplayName = oldview.m_FeatureNameText;
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("CollapseButton"));
                // UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("RecommendationPlace"));
                /*foreach(var comp in oldview.transform.Find("IconPlace").GetComponents<Component>().Concat(oldview.transform.Find("IconPlace").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }
                foreach (var comp in oldview.transform.Find("TextContainer/Description").GetComponents<Component>().Concat(oldview.transform.Find("TextContainer/Description").GetComponentInChildren<Component>(true)))
                {
                    UnityEngine.Component.DestroyImmediate(comp, true);
                }

                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("IconPlace"), true);
                UnityEngine.GameObject.DestroyImmediate(oldview.transform.Find("TextContainer/Description"), true);*/
                var iconplace = oldview.transform.Find("IconPlace");
                var vertlayoutworkaround = iconplace.GetComponent<VerticalLayoutGroupWorkaround>();
                vertlayoutworkaround.padding.left = 10;
                //iconplace.localScale = new Vector3((float)0.75, (float)0.75, (float)0.75);
                var bg = iconplace.Find("BG");
                //bg.Find("Icon").gameObject.SetActive(false);
                var acronym = bg.Find("Acronym");
                acronym.gameObject.SetActive(true);
                var acronymTMP = acronym.GetComponent<TextMeshProUGUI>();
                acronymTMP.fontSizeMax = 46;
                acronymTMP.fontSize = 46;
                acronymTMP.verticalAlignment = VerticalAlignmentOptions.Geometry;
                acronymTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
                //  newcomp.m_IconText = acronymTMP;
                //oldview.transform.Find("TextContainer/Description").gameObject.SetActive(false);
                UnityEngine.Component.Destroy(oldview);

                var canvas = Kingmaker.Game.Instance.UI.Canvas != null ? Kingmaker.Game.Instance.UI.Canvas.transform.Find("ServiceWindowsPCView") : Kingmaker.Game.Instance.UI.GlobalMapCanvas.transform.Find("ServiceWindowsConfig");
                var InstantiatedButton = UnityEngine.GameObject.Instantiate(canvas.Find("InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/NonUsable"), newcomp.transform);
                var icon = InstantiatedButton.Find("Icon");
                icon.gameObject.SetActive(true);
                newcomp.m_AddButton = InstantiatedButton.GetComponent<OwlcatMultiButton>();
                UnityEngine.Component.DestroyImmediate(icon.gameObject.GetComponent<Image>(), true);
                var newtxt = icon.gameObject.AddComponent<TextMeshProUGUI>(acronymTMP);
                newtxt.text = "";
                newtxt.fontSizeMin = 45;
                newcomp.m_IconText = newtxt;
                // iconplace.gameObject.SetActive(false);

                var Layout = oldview.gameObject.GetComponent<HorizontalLayoutGroupWorkaround>();
                Layout.spacing = 10;
                Layout.padding.left = 10;
                newtxt.font = acronymTMP.font;
                newtxt.alignment = TextAlignmentOptions.CenterGeoAligned;
                newtxt.horizontalAlignment = HorizontalAlignmentOptions.Geometry;
                newtxt.verticalAlignment = VerticalAlignmentOptions.Geometry;
                newtxt.fontSizeMax = 46;
                newtxt.material = acronymTMP.material;
                newtxt.fontStyle = FontStyles.Bold;
                newtxt.color = acronymTMP.color;
                newcomp.m_IconText = newtxt;

                return newcomp;
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
        public static void SetupFromVisualSettings(this EEColorPickerView newcomp, CharacterVisualSettingsView oldcomp)
        {
            newcomp.m_SettingsBtn = oldcomp.m_SettingsBtn;
            newcomp.m_Title = oldcomp.m_Title;
            newcomp.m_Window = oldcomp.m_Window;
            newcomp.m_WindowShow = oldcomp.m_WindowShow;
            newcomp.m_OnClickDispose = oldcomp.m_OnClickDispose;
            var windowcontainer = newcomp.transform.Find("WindowContainer");
            var layout = windowcontainer.gameObject.GetComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childScaleWidth = true;
            layout.spacing = -15;
            var sizefitter = windowcontainer.gameObject.GetComponent<ContentSizeFitterExtended>();
            sizefitter.m_VerticalFit = ContentSizeFitterExtended.FitMode.MinSize;
            windowcontainer.Find("ShowBackpackContainer").gameObject.SetActive(false);
            windowcontainer.Find("ShowHelmContainer").gameObject.SetActive(false);
            windowcontainer.Find("ShowClothContainer").gameObject.SetActive(false);
        }
        public static void SetupFromSlideSelector(this BarleySlideSelectorPCView newcomp, SlideSelectorPCView oldcomp)
        {
            newcomp.m_ButtonNext = oldcomp.m_ButtonNext;
            newcomp.m_ButtonPrevious = oldcomp.m_ButtonPrevious;
            newcomp.m_CalculateHandleSize = oldcomp.m_CalculateHandleSize;
            newcomp.m_Counter = oldcomp.m_Counter;
            newcomp.m_Label = oldcomp.m_Label;
            newcomp.m_Slider = oldcomp.m_Slider;
            newcomp.m_SliderRect = oldcomp.m_SliderRect;
            newcomp.m_SliderSlideArea = oldcomp.m_SliderSlideArea;
            newcomp.m_Value = oldcomp.m_Value;

            UnityEngine.Component.Destroy(oldcomp);
        }
        public static CharacterSettings GetSettings(this UnitEntityData unit)
        {
            return GlobalCharacterSettings.Instance.ForCharacter(unit);
        }
        public static void SetupInfoBar(this DollRoom __instance, UnitEntityData player, bool force = false, BlueprintClassAdditionalVisualSettings additionalVisualSettings = null)
        {
            PFLog.Default.Log("SetupInfo", Array.Empty<object>());
            if (((player != null) ? player.View : null) == null)
            {
                return;
            }
            if (additionalVisualSettings == null)
            {
                ClassData visualSettingsProvider = player.Progression.GetVisualSettingsProvider();
                additionalVisualSettings = ((visualSettingsProvider != null) ? visualSettingsProvider.CharacterClass.GetAdditionalVisualSettings(visualSettingsProvider.Level) : null);
            }
            UnitEntityView unitEntityView = player.View.Or(null);
            Character character = (unitEntityView != null) ? unitEntityView.CharacterAvatar : null;
            if (__instance.m_Unit == player && __instance.m_OriginalAvatar != null && __instance.m_OriginalAvatar == character && !force)
            {
                __instance.m_Avatar.SetAdditionalVisualSettings(additionalVisualSettings);
                __instance.UpdateCharacter();
                return;
            }
            __instance.Cleanup();
            __instance.m_Unit = player;
            __instance.m_OriginalAvatar = character;
            if (__instance.m_OriginalAvatar == null)
            {
                UnitEntityView unitEntityView2 = __instance.SetupSimpleAvatar(player);
                __instance.SetupAnimationManager(unitEntityView2.GetComponentInChildren<UnitAnimationManager>());
                return;
            }
            Character character2 = __instance.CreateAvatar(__instance.m_OriginalAvatar, __instance.Unit.Gender, __instance.Unit.Progression.Race.RaceId, __instance.m_Unit.ToString(), additionalVisualSettings);
            __instance.SetAvatar(character2);
            character2.transform.localScale = player.View.transform.localScale;
            Vector3 localScale = new Vector3(player.View.OriginalScale.x / player.View.transform.localScale.x, player.View.OriginalScale.y / player.View.transform.localScale.y, player.View.OriginalScale.z / player.View.transform.localScale.z);
            character2.transform.parent.localScale = localScale;
            if (player.View.OverrideDollRoomScale != Vector3.zero)
            {
                Vector3 localScale2 = new Vector3(character2.transform.parent.localScale.x * player.View.OverrideDollRoomScale.x, character2.transform.parent.localScale.y * player.View.OverrideDollRoomScale.y, character2.transform.parent.localScale.z * player.View.OverrideDollRoomScale.z);
                character2.transform.parent.localScale = localScale2;
            }
            IKController component = __instance.Unit.View.GetComponent<IKController>();
            IKController ikcontroller = __instance.m_Avatar.gameObject.AddComponent<IKController>();
            ikcontroller.DollRoom = __instance;
            ikcontroller.CharacterSystem = __instance.m_Avatar;
            ikcontroller.Settings = ((component != null) ? component.Settings : null);
            UnitEntityView component2 = __instance.m_OriginalAvatar.GetComponent<UnitEntityView>();
            if (component2 != null)
            {
                ikcontroller.CharacterUnitEntity = component2;
            }
            __instance.Update(false);
            __instance.SetupAnimationManager(character2.AnimationManager);
            character2.AnimationManager.Tick();
            character2.AnimationManager.LocoMotionHandle.Action.OnUpdate(character2.AnimationManager.LocoMotionHandle, 0.1f);
        }
        public static void DestroyFx(GameObject FxObject)
        {
            if (FxObject)
            {
                FxHelper.Destroy(FxObject);
                FxObject = null;
            }
        }
        public static void VAUpdate(this UnitEntityView view)
        {
            view.UpdateAdditionalVisualSettings();
            view.UpdateBodyEquipmentVisibility();
        }
    }
}
