using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.ResourceLinks;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Components;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Kingmaker.Visual.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2
{
    static class ExtensionMethods
    {
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
                    foreach (var ee in unit?.View?.CharacterAvatar?.EquipmentEntities)
                    {
                        //EquipmentEntity ee = enumerator2.Current;
                        if (customizationOptions.Heads.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Head = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Eyebrows.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Eyebrows = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Hair.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        //if (ee.name.Contains("Hair") || ee.name.Contains("hair"))
                        {
                            state.Hair = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Beards.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Beard = new DollState.EEAdapter(ee);
                        }
                        if (customizationOptions.Horns.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            state.Horn = new DollState.EEAdapter(ee);
                        }
                        if (state.Scars.Contains((EquipmentEntityLink link) => link.Load(false) == ee))
                        {
                            var NewEEAdapter = new DollState.EEAdapter(ee);
                            NewEEAdapter.m_Link = DollState.GetScarsList(state.Race.RaceId).FirstOrDefault((EquipmentEntityLink link2) => link2.Load(false) == ee);
                            state.Scar = NewEEAdapter;
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

                    if (state.Hair.m_Entity != null)
                    {
                        state.HairRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Hair.Load());
                    }
                    DollState.EEAdapter eeadapter = state.GetSkinEntities().FirstOrDefault<DollState.EEAdapter>();
                    if (eeadapter.m_Entity != null)
                    {
                        state.SkinRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(eeadapter.Load());
                    }
                    if (state.Horn.m_Entity != null)
                    {
                        state.HornsRampIndex = unit.View.CharacterAvatar.GetPrimaryRampIndex(state.Horn.Load());
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
                state.CreateTattos(state?.m_DefaultSettings);
                state.CreateWarpaints(state?.m_DefaultSettings, state.Race.RaceId);
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
    }
}
