using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.View;
using Kingmaker.Visual.CharacterSystem;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class EquipmentVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public EquipmentVM()
        {
            base.AddDisposable(this);
            base.AddDisposable(m_weaponOverride = new WeaponOverrideVM());
            base.AddDisposable(m_classOutfitSelectorVM = new ClassOutfitSelectorVM());
        }
        public void ApplyColor(Color col, bool PrimOrSec)
        {
#if DEBUG
            Main.Logger.Log("TriedApplyClassCol");
#endif
            var settings = Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings();
            if (PrimOrSec)
            {
                settings.ClassOverride.PrimaryIndex = null;
                settings.ClassOverride.PrimaryCustomCol = new SerializableColor(col);
            }
            else
            {
                settings.ClassOverride.SecondaryIndex = null;
                settings.ClassOverride.SecondaryCustomCol = new SerializableColor(col);
            }
            var unit = Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit;

            {
                Gender gender = unit.Gender;
                var race = UnitEntityView.GetActualRace(unit);
                // var settings = unit.GetSettings();
                var EqClass = unit.Progression.GetEquipmentClass();

                if (EqClass == null) return;
                var links = EqClass?.GetClothesLinks(gender, race).Select(b => b.Load());
                // var links = GetLinks();
                /*List<EquipmentEntity> GetLinks()
                {
                    var list = new List<EquipmentEntity>();
                    foreach(var eel in EqClass.EquipmentEntities)
                    {
                        var ee = eel.Load(gender,race);
                        if (ee != null && (ee.Count() > 0)) list.AddRange(ee);
                    }
                    return list;
                }*/
                foreach (var clothesLink in links)
                {
                    EquipmentEntity ee = clothesLink;//.Load(false, false);
                    if (clothesLink.PrimaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                    {
                        if (settings.ClassOverride.PrimaryIndex is not null or >= 0)
                        {
                            Game.Instance.UI.Common.DollRoom.m_Avatar.SetPrimaryRampIndex(ee, (int)settings.ClassOverride.PrimaryIndex, false);
                        }
                        else if (settings.ClassOverride.PrimaryCustomCol != null)
                        {
                            var coll = settings.ClassOverride.PrimaryCustomCol.Value.ToColor();
                            var firstpixel = ee.PrimaryColorsProfile?.Ramps?.Where(t => t.isReadable).FirstOrDefault(t => t.GetPixel(1, 1) == coll);
                            if (firstpixel == null)
                            {
                                var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                                {
                                    filterMode = FilterMode.Bilinear
                                };
                                tex.SetPixel(1, 1, coll);
                                tex.Apply();
                                ee.PrimaryColorsProfile.Ramps.Add(tex);
                                var index = ee.PrimaryColorsProfile.Ramps.IndexOf(tex);
                                Game.Instance.UI.Common.DollRoom.m_Avatar.SetPrimaryRampIndex(ee, index);
                            }
                            else
                            {
                                var index = ee.PrimaryColorsProfile.Ramps.IndexOf(firstpixel);
                                Game.Instance.UI.Common.DollRoom.m_Avatar.SetPrimaryRampIndex(ee, index);
                            }
                        }
                    }
                    if (clothesLink.SecondaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                    {
                        if (settings.ClassOverride.SecondaryIndex is not null or >= 0)
                        {
                            Game.Instance.UI.Common.DollRoom.m_Avatar.SetSecondaryRampIndex(ee, (int)settings.ClassOverride.SecondaryIndex, false);
                        }
                        else if (settings.ClassOverride.SecondaryCustomCol != null)
                        {
                            var coll = settings.ClassOverride.SecondaryCustomCol.Value.ToColor();
                            var firstpixel = ee.SecondaryColorsProfile.Ramps.Where(t => t.isReadable).FirstOrDefault(t => t.GetPixel(1, 1) == coll);
                            if (firstpixel == null)
                            {
                                var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                                {
                                    filterMode = FilterMode.Bilinear
                                };
                                tex.SetPixel(1, 1, coll);
                                tex.Apply();
                                ee.SecondaryColorsProfile.Ramps.Add(tex);
                                var index = ee.SecondaryColorsProfile.Ramps.IndexOf(tex);
                                Game.Instance.UI.Common.DollRoom.m_Avatar.SetSecondaryRampIndex(ee, index);
                            }
                            else
                            {
                                var index = ee.SecondaryColorsProfile.Ramps.IndexOf(firstpixel);
                                Game.Instance.UI.Common.DollRoom.m_Avatar.SetSecondaryRampIndex(ee, index);
                            }
                        }
                    }
                }
            }

            //var needsreset = (unit.IsStoryCompanion() && settings.ClassOverride.GUID != "");
            //if (needsreset)
            {
                //     unit.View.CharacterAvatar.RemoveAllEquipmentEntities(false);
                //   unit.View.CharacterAvatar.RestoreSavedEquipment();
            }
            /* Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.UpdateClassEquipment();
             Kingmaker.Game.Instance.UI.Common.DollRoom.m_Avatar.UpdateCharacter();
             /*if (needsreset) *//*unit.View.UpdateBodyEquipmentVisibility();

             Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.UpdateClassEquipment();*/

        }
        public void ApplyColor(int col, bool PrimOrSec)
        {
#if DEBUG
            Main.Logger.Log("TriedApplyClassCol");
#endif
            try
            {
                var settings = Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings();
                if (PrimOrSec)
                {
                    settings.ClassOverride.PrimaryIndex = col;
                    settings.ClassOverride.PrimaryCustomCol = null;
                }
                else
                {
                    settings.ClassOverride.SecondaryIndex = col;
                    settings.ClassOverride.SecondaryCustomCol = null;
                }
                var unit = Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit;

                {
                    Gender gender = unit.Gender;
                    var race = UnitEntityView.GetActualRace(unit);
                    // var settings = unit.GetSettings();
                    var EqClass = unit.Progression.GetEquipmentClass();

                    if (EqClass == null) return;
                    var links = EqClass?.GetClothesLinks(gender, race).Select(b => b.Load());
                    // var links = GetLinks();
                    /*List<EquipmentEntity> GetLinks()
                    {
                        var list = new List<EquipmentEntity>();
                        foreach(var eel in EqClass.EquipmentEntities)
                        {
                            var ee = eel.Load(gender,race);
                            if (ee != null && (ee.Count() > 0)) list.AddRange(ee);
                        }
                        return list;
                    }*/
                    foreach (var clothesLink in links)
                    {
                        EquipmentEntity ee = clothesLink;//.Load(false, false);
                        if (clothesLink.PrimaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                        {
                            if (settings.ClassOverride.PrimaryIndex is not null or >= 0)
                            {
                                Game.Instance.UI.Common.DollRoom.m_Avatar.SetPrimaryRampIndex(ee, (int)settings.ClassOverride.PrimaryIndex, false);
                            }
                            else if (settings.ClassOverride.PrimaryCustomCol != null)
                            {
                                var coll = settings.ClassOverride.PrimaryCustomCol.Value.ToColor();
                                var firstpixel = ee.PrimaryColorsProfile?.Ramps?.Where(t => t.isReadable).FirstOrDefault(t => t.GetPixel(1, 1) == coll);
                                if (firstpixel == null)
                                {
                                    var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                                    {
                                        filterMode = FilterMode.Bilinear
                                    };
                                    tex.SetPixel(1, 1, coll);
                                    tex.Apply();
                                    ee.PrimaryColorsProfile.Ramps.Add(tex);
                                    var index = ee.PrimaryColorsProfile.Ramps.IndexOf(tex);
                                    Game.Instance.UI.Common.DollRoom.m_Avatar.SetPrimaryRampIndex(ee, index);
                                }
                                else
                                {
                                    var index = ee.PrimaryColorsProfile.Ramps.IndexOf(firstpixel);
                                    Game.Instance.UI.Common.DollRoom.m_Avatar.SetPrimaryRampIndex(ee, index);
                                }
                            }
                        }
                        if (clothesLink.SecondaryColorsProfile?.Ramps?.Count is >= 0 or not null)
                        {
                            if (settings.ClassOverride.SecondaryIndex is not null or >= 0)
                            {
                                Game.Instance.UI.Common.DollRoom.m_Avatar.SetSecondaryRampIndex(ee, (int)settings.ClassOverride.SecondaryIndex, false);
                            }
                            else if (settings.ClassOverride.SecondaryCustomCol != null)
                            {
                                var coll = settings.ClassOverride.SecondaryCustomCol.Value.ToColor();
                                var firstpixel = ee.SecondaryColorsProfile.Ramps.Where(t => t.isReadable).FirstOrDefault(t => t.GetPixel(1, 1) == coll);
                                if (firstpixel == null)
                                {
                                    var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false)
                                    {
                                        filterMode = FilterMode.Bilinear
                                    };
                                    tex.SetPixel(1, 1, coll);
                                    tex.Apply();
                                    ee.SecondaryColorsProfile.Ramps.Add(tex);
                                    var index = ee.SecondaryColorsProfile.Ramps.IndexOf(tex);
                                    Game.Instance.UI.Common.DollRoom.m_Avatar.SetSecondaryRampIndex(ee, index);
                                }
                                else
                                {
                                    var index = ee.SecondaryColorsProfile.Ramps.IndexOf(firstpixel);
                                    Game.Instance.UI.Common.DollRoom.m_Avatar.SetSecondaryRampIndex(ee, index);
                                }
                            }
                        }
                    }
                }
                //  var needsreset = (unit.IsStoryCompanion() && settings.ClassOverride.GUID != "");
                //if (needsreset)
                {
                    //   unit.View.CharacterAvatar.RemoveAllEquipmentEntities(false);
                    //  unit.View.CharacterAvatar.RestoreSavedEquipment();
                }
                //   unit.View.UpdateBodyEquipmentVisibility();
                //  Kingmaker.Game.Instance.UI.Common.DollRoom.m_Avatar.UpdateCharacter();
                /*if (needsreset) */

                // Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View.UpdateClassEquipment();
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public override void DisposeImplementation()
        {

        }
        public void OnCharacterChanged()
        {

        }
        public WeaponOverrideVM m_weaponOverride;
        public EquipmentListVM m_EquipmentListVM;
        public ClassOutfitSelectorVM m_classOutfitSelectorVM;
    }
}
