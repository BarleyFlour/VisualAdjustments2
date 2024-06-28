using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.ResourceLinks;
using Kingmaker.Visual.CharacterSystem;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic.Parts;
using VisualAdjustments2.Infrastructure;
using Kingmaker.UnitLogic.Commands.Base;
using Kingmaker;
using Owlcat.Runtime.Core.Utils;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.UnitLogic.Commands;
using UnityEngine;

namespace VisualAdjustments2
{
    public class SerializedDollState
    {
        public class SerializedDollPrint
        {
            [JsonProperty] public string GUID;
            [JsonProperty] public int colorIndex;
            public SerializedDollPrint(string printGUID, int colorRampIndex)
            {
                GUID = printGUID;
                colorIndex = colorRampIndex;
            }
            public void Apply(DollState state, int index)
            {
                var eel = new EquipmentEntityLink();
                eel.AssetId = GUID;
                state.SetWarpaint(eel, index);
                state.SetWarpaintColor(colorIndex, index);
            }
            public DollState.DollPrint Deserialize()
            {
                var print = new DollState.DollPrint();
                print.PaintRampIndex = colorIndex;
                var eel = new EquipmentEntityLink();
                eel.AssetId = GUID;
                print.PaintEE = new DollState.EEAdapter(eel);
                return print;
            }
        }
        [JsonProperty] public Gender Gender;
        [JsonProperty] public string Race;
        [JsonProperty] public string RacePreset;
        [JsonProperty] public string HeadAssetID;
        [JsonProperty] public string ScarID;
        [JsonProperty] public string EyebrowsID;
        [JsonProperty] public string HairID;
        [JsonProperty] public string BeardID;
        [JsonProperty] public string HornID;
        [JsonProperty] public List<string> ClothesIDs;
        [JsonProperty] public List<string> ScarIDs;
        [JsonProperty] public string CharacterClass;
        [JsonProperty] public List<SerializedDollPrint> Warprints = new List<SerializedDollPrint>(5);
        [JsonProperty] public List<SerializedDollPrint> Tattoos = new List<SerializedDollPrint>(5);
        [JsonProperty] public int HairRampIndex = -1;
        [JsonProperty] public int SkinRampIndex = -1;
        [JsonProperty] public int EyesColorRampIndex = -1;
        [JsonProperty] public int HornsRampIndex = -1;
        [JsonProperty] public int EquipmentRampIndex = -1;
        [JsonProperty] public int EquipmentRampIndexSecondary = -1;
        public SerializedDollState(DollState state)
        {
            SetupFromDollState(state);
        }
        public void SetupFromDollState(DollState state)
        {
            if (state == null) return;
            if (state.Gender != null) Gender = state.Gender;
            if (state.Race != null) Race = state.Race.AssetGuidThreadSafe;
            if (state.RacePreset != null) RacePreset = state.RacePreset.AssetGuidThreadSafe;
            if (state.Head.m_Link != null) HeadAssetID = state.Head.m_Link.AssetId;
            if (state.Scar.m_Link != null) ScarID = state.Scar.m_Link.AssetId;
            if (state.Eyebrows.m_Link != null) EyebrowsID = state.Eyebrows.m_Link.AssetId;
            if (state.Hair.m_Link != null) HairID = state.Hair.m_Link.AssetId;
            if (state.Beard.m_Link != null) BeardID = state.Beard.m_Link.AssetId;
            if (state.Horn.m_Link != null) HornID = state.Horn.m_Link.AssetId;
            if (state.Clothes != null) ClothesIDs = state.Clothes.Select(a => a.AssetId).ToList();
            if (state.Scars != null) ScarIDs = state.Scars.Select(a => a.AssetId).ToList();
            if (state.CharacterClass != null) CharacterClass = state.CharacterClass.AssetGuidThreadSafe;

            if (state.Warprints != null) Warprints = state.Warprints.Where(b => b?.PaintEE.m_Link?.AssetId != null).Select(a => a.SerializeDollPrint()).ToList();
            if (state.Tattoos != null) Tattoos = state.Tattoos.Where(b => b?.PaintEE.m_Link?.AssetId != null).Select(a => a.SerializeDollPrint()).ToList();

            if (state.HairRampIndex != null) HairRampIndex = state.HairRampIndex;
            if (state.SkinRampIndex != null) SkinRampIndex = state.SkinRampIndex;
            if (state.EyesColorRampIndex != null) EyesColorRampIndex = state.EyesColorRampIndex;
            if (state.HornsRampIndex != null) HornsRampIndex = state.HornsRampIndex;
            if (state.EquipmentRampIndex != null) EquipmentRampIndex = state.EquipmentRampIndex;
            if (state.EquipmentRampIndexSecondary != null) EquipmentRampIndexSecondary = state.EquipmentRampIndexSecondary;
        }
        public DollState GetDollState()
        {
            try
            {
                var doll = new DollState();
                if (Race != null && Race != "") doll.SetRace(ResourcesLibrary.TryGetBlueprint<BlueprintRace>(Race));
                if (RacePreset != null && RacePreset != "") doll.SetRacePreset(ResourcesLibrary.TryGetBlueprint<BlueprintRaceVisualPreset>(RacePreset));
                if (Gender != null) doll.SetGender(Gender);

                if (BeardID != null && BeardID != "")
                {
                    var eel = new EquipmentEntityLink();
                    eel.AssetId = BeardID;
                    doll.SetBeard(eel);
                }
                if (HeadAssetID != null && HeadAssetID != "")
                {
                    var eel = new EquipmentEntityLink();
                    eel.AssetId = HeadAssetID;
                    doll.SetHead(eel);
                }
                if (ScarID != null && ScarID != "")
                {
                    var eel = new EquipmentEntityLink();
                    eel.AssetId = ScarID;
                    doll.SetScar(eel);
                }
                if (EyebrowsID != null && EyebrowsID != "")
                {
                    var eel = new EquipmentEntityLink();
                    eel.AssetId = EyebrowsID;
                    doll.Eyebrows = new DollState.EEAdapter(eel);
                }
                if (HairID != null && HairID != "")
                {
                    var eel = new EquipmentEntityLink();
                    eel.AssetId = HairID;
                    doll.SetHair(eel);
                }
                if (HornID != null && HornID != "")
                {
                    var eel = new EquipmentEntityLink();
                    eel.AssetId = HornID;
                    doll.SetHorn(eel);
                }
                if (CharacterClass != null)
                {
                    var bp = ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(CharacterClass);
                    if (bp != null) doll.SetClass(bp);
                }
                if (Warprints != null)
                {
                    var deconstructed = Warprints.Select(a => a.Deserialize()).ToList();
                    doll.Warprints = deconstructed;
                    for (var x = 0; deconstructed.Count > x; x++)
                    {
                        var print = deconstructed[x];
                        if (print != null)
                        {
                            doll.SetWarpaint(print.PaintEE.m_Link, x);
                            doll.SetWarpaintColor(print.PaintRampIndex, x);
                        }
                        else
                        {
                            doll.Tattoos.Add(new DollState.DollPrint());
                        }
                    }
                }
                else
                {
                    doll.CreateWarpaints(default,doll.Race.RaceId);
                }
                if (Tattoos != null)
                {
                    var deconstructed = Tattoos.Select(a => a.Deserialize());
                    doll.Tattoos = deconstructed.ToList();
                    int x = 0;
                    foreach (var print in deconstructed)
                    {
                        if (print != null)
                        {
                            doll.SetTattoo(print.PaintEE.m_Link, x);
                            doll.SetTattooColor(print.PaintRampIndex, x);
                        }
                        else
                        {
                            doll.Tattoos.Add(new DollState.DollPrint());
                        }
                        x++;
                    }
                }
                else
                {
                    doll.CreateTattos(default);
                }
                if (HairRampIndex != null && HairRampIndex != -1)
                {
                    doll.SetHairColor(HairRampIndex);
                }
                if (SkinRampIndex != null && SkinRampIndex != -1)
                {
                    doll.SetSkinColor(SkinRampIndex);
                }
                if (EyesColorRampIndex != null && EyesColorRampIndex != -1)
                {
                    doll.SetEyesColor(EyesColorRampIndex);
                }
                if (HornsRampIndex != null && HornsRampIndex != -1)
                {
                    doll.SetHornsColor(HornsRampIndex);
                }
                if (EquipmentRampIndex != null && EquipmentRampIndex != -1)
                {
                    doll.SetPrimaryEquipColor(EquipmentRampIndex);
                }
                if (EquipmentRampIndexSecondary != null && EquipmentRampIndexSecondary != -1)
                {
                    doll.SetSecondaryEquipColor(EquipmentRampIndexSecondary);
                }
                return doll;
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
    }
    static class DollStateSerialization
    {
        public static DollState GetDollState(this UnitEntityData data)
        {
            try
            {
                var settings = data.GetSettings();
                if (settings.doll != null)
                {
                    return settings.doll.GetDollState();
                }
                else
                {
                    var doll = new DollState();
                    doll.SetupFromUnitLocal(data);
                    return doll;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw e;
            }
        }
        public static void RebuildCharacter(this UnitEntityData data)
        {
            //From Polymorph.RestoreView (slightly modified)
        
            foreach (Buff buff in data.Buffs)
            {
                buff.ClearParticleEffect();
            }
            UnitEntityView view = data.View;
            data.AttachToViewOnLoad(null);
            data.View.transform.SetParent(view.transform.parent, false);
            data.View.transform.position = view.transform.position;
            data.View.transform.rotation = view.transform.rotation;
            data.Commands.InterruptAll((UnitCommand cmd) => !(cmd is UnitMoveTo));
            SelectionManagerBase selectionManagerdata = Game.Instance.UI.SelectionManager.Or(null);
            if (selectionManagerdata != null)
            {
                selectionManagerdata.ForceCreateMarks();
            }
            Physics.SyncTransforms();
            UnityEngine.Object.Destroy(view.gameObject);
            data.Wake(5f, false);
            /*UnitEntityView view = data.View;
            data.AttachToViewOnLoad(null);
            data.View.transform.SetParent(view.transform.parent, false);
            data.View.transform.position = view.transform.position;
            data.View.transform.rotation = view.transform.rotation;
            SelectionManagerBase selectionManagerBase = Game.Instance.UI.SelectionManager.Or(null);
            if (selectionManagerBase != null)
            {
                selectionManagerBase.ForceCreateMarks();
            }
            // Polymorph.VisualTransitionSettings settings = this.HasExternalTransition ? this.m_TransitionExternal.ExitTransition : this.m_ExitTransition;
            UnityEngine.Physics.SyncTransforms();
            if (data.IsViewActive)
            {
                var settings = new Polymorph.VisualTransitionSettings();
                settings.OldPrefabFX = new PrefabLink();
                settings.NewPrefabFX = new PrefabLink();
                //data.View.StartCoroutine(Polymorph.Transition(settings, view, data.View));
            }
            else
            {
                UnityEngine.Object.Destroy(view.gameObject);
            }
            data.Wake(/*settings.ScaleTime*//*);
            //data.ViewReplacement = null;
        
            //data.View.CharacterAvatar.
            //data.AttachToViewOnLoad(null);*/
        }
        public static void SaveDollState(this UnitEntityData data, DollState doll, bool Apply = true)
        {
            try
            {
                var settings = data.GetSettings();
                settings.doll = new SerializedDollState(doll);

                /* var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(ToSerialize);
                 var KVStorage = Kingmaker.Game.Instance.Player.Ensure<EntityPartKeyValueStorage>().GetStorage("VisualAdjustments2");
                 if (KVStorage.ContainsKey(data.UniqueId))
                 {
                     KVStorage[data.UniqueId] = serialized;
                 }
                 else
                 {
                     KVStorage.Add(data.UniqueId, serialized);
                 }*/
                if (Apply)
                {
                    var unitPartDollData = data.Ensure<UnitPartDollData>();
                    if (data.Get<UnitPartDollData>().Default != null)
                    {
                        if (doll.RacePreset != null)
                        {
                            data.View.CharacterAvatar.Skeleton = ((doll.Gender == Gender.Male) ? doll.RacePreset.MaleSkeleton : doll.RacePreset.FemaleSkeleton);
                            data.View.CharacterAvatar.UpdateSkeleton();
                            data.View.CharacterAvatar.AddEquipmentEntities(doll.RacePreset.Skin.GetLinks(doll.Gender, doll.RacePreset.RaceId));
                        }
                        var newDoll = doll.CreateData();
                        unitPartDollData.SetDefault(newDoll);
                        data.View.CharacterAvatar.AddEquipmentEntities(newDoll.EquipmentEntityIds.Select(b => ResourcesLibrary.TryGetResource<EquipmentEntity>(b)));
                        newDoll.ApplyRampIndices(data.View.CharacterAvatar);
                    }
                    else
                    {
                        if (doll.RacePreset != null)
                        {
                            data.View.CharacterAvatar.Skeleton = ((doll.Gender == Gender.Male) ? doll.RacePreset.MaleSkeleton : doll.RacePreset.FemaleSkeleton);
                            data.View.CharacterAvatar.UpdateSkeleton();
                            data.View.CharacterAvatar.AddEquipmentEntities(doll.RacePreset.Skin.GetLinks(doll.Gender, doll.RacePreset.RaceId));
                        }
                        // data.View?.CharacterAvatar?.RemoveEquipmentEntities(unitPartDollData?.Default?.EquipmentEntityIds?.Select(b => ResourcesLibrary.TryGetResource<EquipmentEntity>(b)));
                        var newDoll = doll.CreateData();
                        unitPartDollData.SetDefault(newDoll);

                        data.View.CharacterAvatar.AddEquipmentEntities(newDoll.EquipmentEntityIds.Select(b => ResourcesLibrary.TryGetResource<EquipmentEntity>(b)));
                        newDoll.ApplyRampIndices(data.View.CharacterAvatar);
                    }

                }
#if DEBUG
                Main.Logger.Log($"Saved Doll for {data.CharacterName}");
#endif
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public static SerializedDollState.SerializedDollPrint SerializeDollPrint(this DollState.DollPrint print)
        {
            return new SerializedDollState.SerializedDollPrint(print.PaintEE.AssetId, print.PaintRampIndex);
        }
    }
}
