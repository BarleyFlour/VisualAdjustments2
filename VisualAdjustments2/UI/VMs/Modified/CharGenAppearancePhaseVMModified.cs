﻿using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.ResourceLinks;
using Kingmaker.UI.Common;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.Party;
using Kingmaker.UI.MVVM._VM.CharGen.Phases;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Appearance;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._VM.Party;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UniRx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;

namespace VisualAdjustments2
{
    public class CharGenAppearancePhaseVMModified : CharGenPhaseBaseVM
    {
        // Token: 0x17000CC9 RID: 3273
        // (get) Token: 0x06004E8F RID: 20111 RVA: 0x000336A8 File Offset: 0x000318A8
        public override int OrderPriority
        {
            get
            {
                return 0;
                //return base.GetBaseOrderPriority(CharGenPhaseBaseVM.ChargenPhasePriority.Appearance) + ((this.DollState != base.LevelUpController.Doll) ? 1 : 0);
            }
        }
        public static CharGenAppearancePhaseDetailedPCViewModified pcview;
        public static DollCharacterController charController;
        public CharGenAppearancePhaseVMModified(LevelUpController levelUpController, DollState dollState, bool isAlternative) : base(levelUpController)
        {
            this.DollState = dollState;
            dollState.m_OnUpdateAction += () => { levelUpController.Unit.SaveDollState(dollState); };
            this.IsAlternative = isAlternative;

            base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(this.OnUnitChanged));
            charController.Bind(dollState);
            base.AddDisposable(this.RefreshView?.Subscribe(() => { this.Change(); }));
            /*base.AddDisposable(Game.Instance.SelectionCharacter.SelectionCharacterUpdated.Subscribe(delegate (Unit _)
            {
                this.OnUnitChanged(Game.Instance.SelectionCharacter.SelectedUnit.Value);
            }));*/
            // this.AddDisposable(PartyVM.SelectedUnit.Subscribe((UnitDescriptor desc) => { this.OnUnitChanged(desc); }));
        }
        public void OnUnitChanged(UnitDescriptor descriptor)
        {
            try
            {
                if (descriptor.Unit.UniqueId != this.LevelUpController.Unit.UniqueId)
                {
                    var unit = descriptor.Unit;
                    var doll = unit.GetDollState();
                    
                    if (doll.Race == null)
                    {
                        Main.Logger.Log($"{unit.CharacterName}'s doll was null");
                        this.Dispose();
                    }
                    else
                    {
                        var lvlcontroller = new LevelUpController(unit, false, LevelUpState.CharBuildMode.SetName);

                        lvlcontroller.Doll = doll;

                        this.Dispose();

                        var vmnew = new CharGenAppearancePhaseVMModified(lvlcontroller, doll, false);

                        pcview.Bind(vmnew);
                        Main.Logger.Log("bound");
                    }

                }
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
        // Token: 0x06004E91 RID: 20113 RVA: 0x001B76B8 File Offset: 0x001B58B8
        public override void DisposeImplementation()
        {
            StringSequentialSelectorVM beardSelectorVM = this.m_BeardSelectorVM;
            if (beardSelectorVM != null)
            {
                beardSelectorVM.Dispose();
            }
            StringSequentialSelectorVM bodySelectorVM = this.m_BodySelectorVM;
            if (bodySelectorVM != null)
            {
                bodySelectorVM.Dispose();
            }
            StringSequentialSelectorVM hairSelectorVM = this.m_HairSelectorVM;
            if (hairSelectorVM != null)
            {
                hairSelectorVM.Dispose();
            }
            StringSequentialSelectorVM headSelectorVM = this.m_HeadSelectorVM;
            if (headSelectorVM != null)
            {
                headSelectorVM.Dispose();
            }
            StringSequentialSelectorVM scarsSelectorVM = this.m_ScarsSelectorVM;
            if (scarsSelectorVM != null)
            {
                scarsSelectorVM.Dispose();
            }
            List<StringSequentialSelectorVM> warpaintSelectorVMList = this.m_WarpaintSelectorVMList;
            if (warpaintSelectorVMList != null)
            {
                warpaintSelectorVMList.ForEach(delegate (StringSequentialSelectorVM d)
                {
                    d.Dispose();
                });
            }
            StringSequentialSelectorVM hornSelectorVM = this.m_HornSelectorVM;
            if (hornSelectorVM != null)
            {
                hornSelectorVM.Dispose();
            }
            if(charController != null)
            {
                charController.Unbind();
            }
            this.m_BeardSelectorVM = null;
            this.m_BodySelectorVM = null;
            this.m_HairSelectorVM = null;
            this.m_HeadSelectorVM = null;
        }

        // Token: 0x06004E92 RID: 20114 RVA: 0x000336CA File Offset: 0x000318CA
        public override bool CheckIsCompleted()
        {


            return false;
            //This is just levelupstuff
            //return this.SelectionStateIsCompleted(base.LevelUpController) || base.IsInDetailedView.Value;
        }

        // Token: 0x06004E93 RID: 20115 RVA: 0x000094BF File Offset: 0x000076BF
        private bool SelectionStateIsCompleted(LevelUpController controller)
        {
            return true;
        }

        // Token: 0x06004E94 RID: 20116 RVA: 0x001B7780 File Offset: 0x001B5980
        public override void OnBeginDetailedView()
        {
            if (!this.m_Subscribed)
            {
                //Check whatever this does
                base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Gender, true).Subscribe(delegate (Gender _)
                {
                    this.Change();
                }));
                base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Race, true).Subscribe(delegate (BlueprintRace _)
                {
                    this.Change();
                }));
                //My subscriptions
                {
                    /*base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.RacePreset, true).Subscribe(delegate (BlueprintRaceVisualPreset _)
					{
						this.Change();
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Beard, true).Subscribe(delegate (DollState.EEAdapter _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.CharacterClass, true).Subscribe(delegate (BlueprintCharacterClass _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.EquipmentRampIndex, true).Subscribe(delegate (int _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.EquipmentRampIndexSecondary, true).Subscribe(delegate (int _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Eyebrows, true).Subscribe(delegate (DollState.EEAdapter _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.EyesColorRampIndex, true).Subscribe(delegate (int _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Hair, true).Subscribe(delegate (DollState.EEAdapter _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.HairRampIndex, true).Subscribe(delegate (int _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Head, true).Subscribe(delegate (DollState.EEAdapter _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Horn, true).Subscribe(delegate (DollState.EEAdapter _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.HornsRampIndex, true).Subscribe(delegate (int _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Scar, true).Subscribe(delegate (DollState.EEAdapter _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Scars, true).Subscribe(delegate (List<EquipmentEntityLink> _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.SkinRampIndex, true).Subscribe(delegate (int _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Tattoos, true).Subscribe(delegate (List<DollState.DollPrint> _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));
					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Warprints, true).Subscribe(delegate (List<DollState.DollPrint> _)
					{
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));

					base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => dirt.dirtthing(base.LevelUpController), true).Subscribe(delegate (bool fg)
                    {
						this.LevelUpController.Unit.SaveDollState(this.DollState);
					}));*/
                }
                this.m_Subscribed = true;
            }
            this.Change();
        }

        // Token: 0x06004E95 RID: 20117 RVA: 0x001B7800 File Offset: 0x001B5A00
        public void Change()
        {
            if (this.m_BodySelectorVM != null)
            {
                StringSequentialEntity current;
                List<StringSequentialEntity> bodyList = this.GetBodyList(this.DollState, out current);
                StringSequentialSelectorVM bodySelectorVM = this.m_BodySelectorVM;
                if (bodySelectorVM != null)
                {
                    bodySelectorVM.SetValues(bodyList, current);
                }
            }
            if (this.m_HandSelectorVM != null)
            {
                StringSequentialEntity current2;
                List<StringSequentialEntity> handList = this.GetHandList(this.DollState, out current2);
                StringSequentialSelectorVM handSelectorVM = this.m_HandSelectorVM;
                if (handSelectorVM != null)
                {
                    handSelectorVM.SetValues(handList, current2);
                }
            }
            if (this.m_HeadSelectorVM != null)
            {
                StringSequentialEntity current3;
                List<StringSequentialEntity> headsList = this.GetHeadsList(this.DollState, out current3);
                StringSequentialSelectorVM headSelectorVM = this.m_HeadSelectorVM;
                if (headSelectorVM != null)
                {
                    headSelectorVM.SetValues(headsList, current3);
                }
            }
            if (this.m_WarpaintSelectorVMList != null)
            {
                for (int index = 0; index < this.m_WarpaintSelectorVMList.Count; index++)
                {
                    int index4 = index;
                    StringSequentialEntity current4;
                    List<StringSequentialEntity> warpaintList = this.GetWarpaintList(this.DollState, index4, out current4);
                    this.m_WarpaintSelectorVMList[index].SetValues(warpaintList, current4);
                }
            }
            if (this.m_ScarsSelectorVM != null)
            {
                StringSequentialEntity current5;
                List<StringSequentialEntity> scarsList = this.GetScarsList(this.DollState, out current5);
                StringSequentialSelectorVM scarsSelectorVM = this.m_ScarsSelectorVM;
                if (scarsSelectorVM != null)
                {
                    scarsSelectorVM.SetValues(scarsList, current5);
                }
            }
            if (this.m_HairSelectorVM != null)
            {
                StringSequentialEntity current6;
                List<StringSequentialEntity> hairList = this.GetHairList(this.DollState, out current6);
                StringSequentialSelectorVM hairSelectorVM = this.m_HairSelectorVM;
                if (hairSelectorVM != null)
                {
                    hairSelectorVM.SetValues(hairList, current6);
                }
            }
            if (this.m_BeardSelectorVM != null)
            {
                StringSequentialEntity current7;
                List<StringSequentialEntity> beardList = this.GetBeardList(this.DollState, out current7);
                StringSequentialSelectorVM beardSelectorVM = this.m_BeardSelectorVM;
                if (beardSelectorVM != null)
                {
                    beardSelectorVM.SetValues(beardList, current7);
                }
            }
            if (this.m_HornSelectorVM != null)
            {
                StringSequentialEntity current8;
                List<StringSequentialEntity> hornList = this.GetHornList(this.DollState, out current8);
                StringSequentialSelectorVM hornSelectorVM = this.m_HornSelectorVM;
                if (hornSelectorVM != null)
                {
                    hornSelectorVM.SetValues(hornList, current8);
                }
            }
            if (this.m_BodyColorSelectorVM != null)
            {
                this.UpdateBodyColorList(this.DollState, this.m_BodyColorSelectorVM);
            }
            if (this.m_EyesColorSelectorVM != null)
            {
                this.UpdateEyesColorList(this.DollState, this.m_EyesColorSelectorVM);
            }
            if (this.m_HairColorSelectorVM != null)
            {
                this.UpdateHairColorList(this.DollState, this.m_HairColorSelectorVM);
            }
            if (this.m_WarpaintColorSelectorVMList != null)
            {
                for (int index2 = 0; index2 < this.m_WarpaintColorSelectorVMList.Count; index2++)
                {
                    int index3 = index2;
                    this.UpdateWarpaintsColorList(this.DollState, this.m_WarpaintColorSelectorVMList[index2], index3);
                }
            }
            if (this.m_HornColorSelectorVM != null)
            {
                this.UpdateHornColorList(this.DollState, this.m_HornColorSelectorVM);
            }
            if (this.m_PrimaryOutfitColorVM != null)
            {
                this.UpdatePrimaryOutfitColors(this.DollState, this.m_PrimaryOutfitColorVM);
            }
            if (this.m_SecondaryOutfitColorVM != null)
            {
                this.UpdateSecondaryOutfitColors(this.DollState, this.m_SecondaryOutfitColorVM);
            }
            this.LevelUpController.Unit.SaveDollState(this.DollState);
            Main.Logger.Log("changed");
        }

        // Token: 0x06004E96 RID: 20118 RVA: 0x001B7A70 File Offset: 0x001B5C70
        private static void GetTextureSelectorItemVM(ReactiveCollection<TextureSelectorItemVM> valueList, int i, Texture2D item, Action setter)
        {
            if (valueList.Count > i)
            {
                valueList[i].UpdateTextureAndSetter(item, setter);
                return;
            }
            TextureSelectorItemVM textureSelectorItemVm = new TextureSelectorItemVM(item, setter, i);
            valueList.Add(textureSelectorItemVm);
        }

        // Token: 0x17000CCA RID: 3274
        // (get) Token: 0x06004E97 RID: 20119 RVA: 0x001B7AA8 File Offset: 0x001B5CA8
        public SelectionGroupRadioVM<TextureSelectorItemVM> BodyColorSelectorVM
        {
            get
            {
                return this.m_BodyColorSelectorVM = (this.m_BodyColorSelectorVM ?? this.CreateBodyColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x17000CCB RID: 3275
        // (get) Token: 0x06004E98 RID: 20120 RVA: 0x001B7AD4 File Offset: 0x001B5CD4
        public StringSequentialSelectorVM BodySelectorVM
        {
            get
            {
                return this.m_BodySelectorVM = (this.m_BodySelectorVM ?? this.GetBodySelectorVM(this.DollState));
            }
        }

        // Token: 0x06004E99 RID: 20121 RVA: 0x001B7B00 File Offset: 0x001B5D00
        private StringSequentialSelectorVM GetBodySelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetBodyList(dollState, out current), current, true);
        }

        // Token: 0x06004E9A RID: 20122 RVA: 0x001B7B20 File Offset: 0x001B5D20
        private List<StringSequentialEntity> GetBodyList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            for (int i = 0; i < dollState.Race.Presets.Length; i++)
            {
                BlueprintRaceVisualPreset racePreset = dollState.Race.Presets[i];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.BodyConstitution, i + 1),
                    Setter = delegate ()
                    {
                        dollState.SetRacePreset(racePreset);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (racePreset == dollState.RacePreset)
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x06004E9B RID: 20123 RVA: 0x001B7C04 File Offset: 0x001B5E04
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateBodyColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateBodyColorList(dollState, selector);
            return selector;
        }

        // Token: 0x06004E9C RID: 20124 RVA: 0x001B7C28 File Offset: 0x001B5E28
        private void UpdateBodyColorList(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> skinRamps = dollState.GetSkinRamps();
            bool[] rampsAvailability = dollState.GetSkinRampsAvailability();
            int num = 0;
            for (int index = 0; index < skinRamps.Count; index++)
            {
                int i1 = index;
                Texture2D texture2D = skinRamps[index];
                if (rampsAvailability[index])
                {
                    CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, num, texture2D, delegate
                    {
                        dollState.SetSkinColor(i1);
                    });
                    entitiesCollection[num].IsSelected.Value = (dollState.SkinRampIndex == index);
                    num++;
                }
            }
            selector.ClearFromIndex(skinRamps.Count);
        }

        // Token: 0x17000CCC RID: 3276
        // (get) Token: 0x06004E9D RID: 20125 RVA: 0x001B7CF4 File Offset: 0x001B5EF4
        public SelectionGroupRadioVM<TextureSelectorItemVM> EyesColorSelectorVM
        {
            get
            {
                return this.m_EyesColorSelectorVM = (this.m_EyesColorSelectorVM ?? this.CreateEyesColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004E9E RID: 20126 RVA: 0x001B7D20 File Offset: 0x001B5F20
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateEyesColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateEyesColorList(dollState, selector);
            return selector;
        }

        // Token: 0x06004E9F RID: 20127 RVA: 0x001B7D44 File Offset: 0x001B5F44
        private void UpdateEyesColorList(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> eyesColorRamps = dollState.GetEyesColorRamps();
            bool[] colorAvailability = dollState.GetEyesColorAvailability();
            int num = 0;
            for (int index = 0; index < eyesColorRamps.Count; index++)
            {
                int i1 = index;
                Texture2D texture2D = eyesColorRamps[index];
                if (colorAvailability[index])
                {
                    Action setter = delegate ()
                    {
                        dollState.SetEyesColor(i1);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    };
                    CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, num, texture2D, setter);
                    if (dollState.EyesColorRampIndex == index)
                    {
                        selector.TrySelectEntity(entitiesCollection[num]);
                    }
                    num++;
                }
            }
            selector.ClearFromIndex(eyesColorRamps.Count);
        }

        // Token: 0x17000CCD RID: 3277
        // (get) Token: 0x06004EA0 RID: 20128 RVA: 0x001B7E10 File Offset: 0x001B6010
        public StringSequentialSelectorVM ScarsSelectorVM
        {
            get
            {
                return this.m_ScarsSelectorVM = (this.m_ScarsSelectorVM ?? this.GetScarsSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EA1 RID: 20129 RVA: 0x001B7E3C File Offset: 0x001B603C
        private StringSequentialSelectorVM GetScarsSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetScarsList(dollState, out current), current, true);
        }

        // Token: 0x06004EA2 RID: 20130 RVA: 0x001B7E5C File Offset: 0x001B605C
        private List<StringSequentialEntity> GetScarsList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            List<EquipmentEntityLink> scars = dollState.Scars;
            for (int index = 0; index < scars.Count; index++)
            {
                EquipmentEntityLink scar = scars[index];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.Scar, index + 1),
                    Setter = delegate ()
                    {
                        dollState.SetScar(scar);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (scar.Load(false) == dollState.Scar.Load())
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CCE RID: 3278
        // (get) Token: 0x06004EA3 RID: 20131 RVA: 0x001B7F3C File Offset: 0x001B613C
        public StringSequentialSelectorVM HandSelectorVM
        {
            get
            {
                return this.m_HandSelectorVM = (this.m_HandSelectorVM ?? this.GetHandSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EA4 RID: 20132 RVA: 0x001B7F68 File Offset: 0x001B6168
        private StringSequentialSelectorVM GetHandSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHandList(dollState, out current), current, true);
        }

        // Token: 0x06004EA5 RID: 20133 RVA: 0x000336E7 File Offset: 0x000318E7
        private List<StringSequentialEntity> GetHandList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> result = new List<StringSequentialEntity>();
            current = null;
            return result;
        }

        // Token: 0x17000CCF RID: 3279
        // (get) Token: 0x06004EA6 RID: 20134 RVA: 0x001B7F88 File Offset: 0x001B6188
        public StringSequentialSelectorVM BeardSelectorVM
        {
            get
            {
                return this.m_BeardSelectorVM = (this.m_BeardSelectorVM ?? this.GetBeardSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EA7 RID: 20135 RVA: 0x001B7FB4 File Offset: 0x001B61B4
        private StringSequentialSelectorVM GetBeardSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetBeardList(dollState, out current), current, true);
        }

        // Token: 0x06004EA8 RID: 20136 RVA: 0x001B7FD4 File Offset: 0x001B61D4
        private List<StringSequentialEntity> GetBeardList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            CustomizationOptions customizationOptions = (dollState.Gender == Gender.Male) ? dollState.Race.MaleOptions : dollState.Race.FemaleOptions;
            for (int index = 0; index < customizationOptions.Beards.Length; index++)
            {
                EquipmentEntityLink beard = customizationOptions.Beards[index];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.Beard, index + 1),
                    Setter = delegate ()
                    {
                        dollState.SetBeard(beard);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (beard.Load(false) == dollState.Beard.Load())
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CD0 RID: 3280
        // (get) Token: 0x06004EA9 RID: 20137 RVA: 0x001B80DC File Offset: 0x001B62DC
        public StringSequentialSelectorVM HornSelectorVM
        {
            get
            {
                return this.m_HornSelectorVM = (this.m_HornSelectorVM ?? this.GetHornSelectorVM(this.DollState));
            }
        }

        // Token: 0x17000CD1 RID: 3281
        // (get) Token: 0x06004EAA RID: 20138 RVA: 0x001B8108 File Offset: 0x001B6308
        public SelectionGroupRadioVM<TextureSelectorItemVM> HornColorSelectorVM
        {
            get
            {
                return this.m_HornColorSelectorVM = (this.m_HornColorSelectorVM ?? this.CreateHornColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EAB RID: 20139 RVA: 0x001B8134 File Offset: 0x001B6334
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateHornColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateHornColorList(dollState, selector);
            return selector;
        }

        // Token: 0x06004EAC RID: 20140 RVA: 0x001B8158 File Offset: 0x001B6358
        private void UpdateHornColorList(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> hornsRamps = dollState.GetHornsRamps();
            for (int index = 0; index < hornsRamps.Count; index++)
            {
                int i1 = index;
                Texture2D texture2D = hornsRamps[index];
                Action setter = delegate ()
                {
                    dollState.SetHornsColor(i1);
                    this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index, texture2D, setter);
                if (dollState.HairRampIndex == index)
                {
                    selector.TrySelectEntity(entitiesCollection[index]);
                }
            }
            selector.ClearFromIndex(hornsRamps.Count);
        }

        // Token: 0x06004EAD RID: 20141 RVA: 0x001B81F8 File Offset: 0x001B63F8
        private StringSequentialSelectorVM GetHornSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHornList(dollState, out current), current, true);
        }

        // Token: 0x06004EAE RID: 20142 RVA: 0x001B8218 File Offset: 0x001B6418
        private List<StringSequentialEntity> GetHornList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            CustomizationOptions customizationOptions = (dollState.Gender == Gender.Male) ? dollState.Race.MaleOptions : dollState.Race.FemaleOptions;
            for (int index = 0; index < customizationOptions.Horns.Length; index++)
            {
                EquipmentEntityLink horn = customizationOptions.Horns[index];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.Horns, index + 1),
                    Setter = delegate ()
                    {
                        dollState.SetHorn(horn);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (horn.Load(false) == dollState.Horn.Load())
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CD2 RID: 3282
        // (get) Token: 0x06004EAF RID: 20143 RVA: 0x001B8320 File Offset: 0x001B6520
        public StringSequentialSelectorVM HeadSelectorVM
        {
            get
            {
                return this.m_HeadSelectorVM = (this.m_HeadSelectorVM ?? this.GetHeadSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EB0 RID: 20144 RVA: 0x001B834C File Offset: 0x001B654C
        private StringSequentialSelectorVM GetHeadSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHeadsList(dollState, out current), current, true);
        }

        // Token: 0x06004EB1 RID: 20145 RVA: 0x001B836C File Offset: 0x001B656C
        private List<StringSequentialEntity> GetHeadsList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            CustomizationOptions customizationOptions = (dollState.Gender == Gender.Male) ? dollState.Race.MaleOptions : dollState.Race.FemaleOptions;
            for (int index = 0; index < customizationOptions.Heads.Length; index++)
            {
                EquipmentEntityLink head = customizationOptions.Heads[index];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.Face, index + 1),
                    Setter = delegate ()
                    {
                        dollState.SetHead(head);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (head.Load(false) == dollState.Head.Load())
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CD3 RID: 3283
        // (get) Token: 0x06004EB2 RID: 20146 RVA: 0x001B8474 File Offset: 0x001B6674
        public StringSequentialSelectorVM HairSelectorVM
        {
            get
            {
                return this.m_HairSelectorVM = (this.m_HairSelectorVM ?? this.GetHairSelectorVM(this.DollState));
            }
        }

        // Token: 0x17000CD4 RID: 3284
        // (get) Token: 0x06004EB3 RID: 20147 RVA: 0x001B84A0 File Offset: 0x001B66A0
        public SelectionGroupRadioVM<TextureSelectorItemVM> HairColorSelectorVM
        {
            get
            {
                return this.m_HairColorSelectorVM = (this.m_HairColorSelectorVM ?? this.CreateHairColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EB4 RID: 20148 RVA: 0x001B84CC File Offset: 0x001B66CC
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateHairColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateHairColorList(dollState, selector);
            return selector;
        }

        // Token: 0x06004EB5 RID: 20149 RVA: 0x001B84F0 File Offset: 0x001B66F0
        private void UpdateHairColorList(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> hairRamps = dollState.GetHairRamps();
            for (int index = 0; index < hairRamps.Count; index++)
            {
                int i1 = index;
                Texture2D texture2D = hairRamps[index];
                Action setter = delegate ()
                {
                    dollState.SetHairColor(i1);
                    this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index, texture2D, setter);
                if (dollState.HairRampIndex == index)
                {
                    selector.TrySelectEntity(entitiesCollection[index]);
                }
            }
            selector.ClearFromIndex(hairRamps.Count);
        }

        // Token: 0x06004EB6 RID: 20150 RVA: 0x001B8590 File Offset: 0x001B6790
        private StringSequentialSelectorVM GetHairSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHairList(dollState, out current), current, true);
        }

        // Token: 0x06004EB7 RID: 20151 RVA: 0x001B85B0 File Offset: 0x001B67B0
        private List<StringSequentialEntity> GetHairList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            CustomizationOptions customizationOptions = (dollState.Gender == Gender.Male) ? dollState.Race.MaleOptions : dollState.Race.FemaleOptions;
            for (int index = 0; index < customizationOptions.Hair.Length; index++)
            {
                EquipmentEntityLink hair = customizationOptions.Hair[index];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.HairStyle, index + 1),
                    Setter = delegate ()
                    {
                        dollState.SetHair(hair);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (hair.Load(false) == dollState.Hair.Load())
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CD5 RID: 3285
        // (get) Token: 0x06004EB8 RID: 20152 RVA: 0x00030DE7 File Offset: 0x0002EFE7
        public int WarpaintsNumber
        {
            get
            {
                return 5;
            }
        }

        // Token: 0x17000CD6 RID: 3286
        // (get) Token: 0x06004EB9 RID: 20153 RVA: 0x001B86B8 File Offset: 0x001B68B8
        public List<StringSequentialSelectorVM> WarpaintsSelectorVMList
        {
            get
            {
                return this.m_WarpaintSelectorVMList = (this.m_WarpaintSelectorVMList ?? this.GetWarpaintsSelectorVM(this.DollState));
            }
        }

        // Token: 0x17000CD7 RID: 3287
        // (get) Token: 0x06004EBA RID: 20154 RVA: 0x001B86E4 File Offset: 0x001B68E4
        public List<SelectionGroupRadioVM<TextureSelectorItemVM>> WarpaintsColorSelectorVMList
        {
            get
            {
                return this.m_WarpaintColorSelectorVMList = (this.m_WarpaintColorSelectorVMList ?? this.CreateWarpaintsColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EBB RID: 20155 RVA: 0x001B8710 File Offset: 0x001B6910
        private List<SelectionGroupRadioVM<TextureSelectorItemVM>> CreateWarpaintsColorSelectorVM(DollState dollState)
        {
            List<SelectionGroupRadioVM<TextureSelectorItemVM>> selectionGroupRadioVmList = new List<SelectionGroupRadioVM<TextureSelectorItemVM>>();
            for (int index = 0; index < 5; index++)
            {
                int index2 = index;
                SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
                base.AddDisposable(selector);
                this.UpdateWarpaintsColorList(dollState, selector, index2);
                selectionGroupRadioVmList.Add(selector);
            }
            return selectionGroupRadioVmList;
        }

        // Token: 0x06004EBC RID: 20156 RVA: 0x001B8754 File Offset: 0x001B6954
        private void UpdateWarpaintsColorList(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector, int index)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> warpaintRamps = dollState.GetWarpaintRamps();
            for (int index2 = 0; index2 < warpaintRamps.Count; index2++)
            {
                int i1 = index2;
                Texture2D texture2D = warpaintRamps[index2];
                Action setter = delegate ()
                {
                    dollState.SetWarpaintColor(i1, index);
                    this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index2, texture2D, setter);
                if (dollState.Warprints[index].PaintRampIndex == index2)
                {
                    selector.TrySelectEntity(entitiesCollection[index2]);
                }
            }
            selector.ClearFromIndex(warpaintRamps.Count);
        }

        // Token: 0x06004EBD RID: 20157 RVA: 0x001B8818 File Offset: 0x001B6A18
        private List<StringSequentialSelectorVM> GetWarpaintsSelectorVM(DollState dollState)
        {
            List<StringSequentialSelectorVM> sequentialSelectorVmList = new List<StringSequentialSelectorVM>();
            for (int index = 0; index < 5; index++)
            {
                int index2 = index;
                StringSequentialEntity current;
                List<StringSequentialEntity> warpaintList = this.GetWarpaintList(dollState, index2, out current);
                sequentialSelectorVmList.Add(new StringSequentialSelectorVM(warpaintList, current, true));
            }
            return sequentialSelectorVmList;
        }

        // Token: 0x06004EBE RID: 20158 RVA: 0x001B8854 File Offset: 0x001B6A54
        private List<StringSequentialEntity> GetWarpaintList(DollState dollState, int index, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            if (dollState.Warprints[index] != null)
            {
                List<EquipmentEntityLink> paints = dollState.Warprints[index].Paints;
                for (int index2 = 0; index2 < paints.Count; index2++)
                {
                    EquipmentEntityLink option = paints[index2];
                    StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                    {
                        Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.Warpaint, index2 + 1),
                        Setter = delegate ()
                        {
                            dollState.SetWarpaint(option, index);
                            this.LevelUpController.Unit.SaveDollState(dollState);
                        }
                    };
                    sequentialEntityList.Add(sequentialEntity2);
                    if (option.Load(false) == dollState.Warprints[index].PaintEE.Load())
                    {
                        current = sequentialEntity2;
                    }
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CD8 RID: 3288
        // (get) Token: 0x06004EBF RID: 20159 RVA: 0x00030DE7 File Offset: 0x0002EFE7
        public int TattoosNumber
        {
            get
            {
                return 5;
            }
        }

        // Token: 0x17000CD9 RID: 3289
        // (get) Token: 0x06004EC0 RID: 20160 RVA: 0x001B895C File Offset: 0x001B6B5C
        public List<StringSequentialSelectorVM> TattoosSelectorVMList
        {
            get
            {
                return this.m_TattooSelectorVMList = (this.m_TattooSelectorVMList ?? this.GetTattoosSelectorVM(this.DollState));
            }
        }

        // Token: 0x17000CDA RID: 3290
        // (get) Token: 0x06004EC1 RID: 20161 RVA: 0x001B8988 File Offset: 0x001B6B88
        public List<SelectionGroupRadioVM<TextureSelectorItemVM>> TattoosColorSelectorVMList
        {
            get
            {
                return this.m_TattooColorSelectorVMList = (this.m_TattooColorSelectorVMList ?? this.CreateTattoosColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EC2 RID: 20162 RVA: 0x001B89B4 File Offset: 0x001B6BB4
        private List<SelectionGroupRadioVM<TextureSelectorItemVM>> CreateTattoosColorSelectorVM(DollState dollState)
        {
            List<SelectionGroupRadioVM<TextureSelectorItemVM>> selectionGroupRadioVmList = new List<SelectionGroupRadioVM<TextureSelectorItemVM>>();
            for (int index = 0; index < 5; index++)
            {
                int index2 = index;
                SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
                base.AddDisposable(selector);
                this.UpdateTattoosColorList(dollState, selector, index2);
                selectionGroupRadioVmList.Add(selector);
            }
            return selectionGroupRadioVmList;
        }

        // Token: 0x06004EC3 RID: 20163 RVA: 0x001B89F8 File Offset: 0x001B6BF8
        private void UpdateTattoosColorList(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector, int index)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> tattooRamps = dollState.GetTattooRamps();
            for (int index2 = 0; index2 < tattooRamps.Count; index2++)
            {
                int i1 = index2;
                Texture2D texture2D = tattooRamps[index2];
                Action setter = delegate ()
                {
                    dollState.SetTattooColor(i1, index);
                    this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index2, texture2D, setter);
                if (dollState.Tattoos[index].PaintRampIndex == index2)
                {
                    selector.TrySelectEntity(entitiesCollection[index2]);
                }
            }
            selector.ClearFromIndex(tattooRamps.Count);
        }

        // Token: 0x06004EC4 RID: 20164 RVA: 0x001B8ABC File Offset: 0x001B6CBC
        private List<StringSequentialSelectorVM> GetTattoosSelectorVM(DollState dollState)
        {
            List<StringSequentialSelectorVM> sequentialSelectorVmList = new List<StringSequentialSelectorVM>();
            for (int index = 0; index < 5; index++)
            {
                int index2 = index;
                StringSequentialEntity current;
                List<StringSequentialEntity> tattooList = this.GetTattooList(dollState, index2, out current);
                sequentialSelectorVmList.Add(new StringSequentialSelectorVM(tattooList, current, true));
            }
            return sequentialSelectorVmList;
        }

        // Token: 0x06004EC5 RID: 20165 RVA: 0x001B8AF8 File Offset: 0x001B6CF8
        private List<StringSequentialEntity> GetTattooList(DollState dollState, int index, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> sequentialEntityList = new List<StringSequentialEntity>();
            current = null;
            List<EquipmentEntityLink> paints = dollState.Tattoos[index].Paints;
            for (int index2 = 0; index2 < paints.Count; index2++)
            {
                EquipmentEntityLink option = paints[index2];
                StringSequentialEntity sequentialEntity2 = new StringSequentialEntity
                {
                    Title = string.Format("{0} {1}", UIStrings.Instance.CharGen.Tattoo, index2 + 1),
                    Setter = delegate ()
                    {
                        dollState.SetTattoo(option, index);
                        this.LevelUpController.Unit.SaveDollState(dollState);
                    }
                };
                sequentialEntityList.Add(sequentialEntity2);
                if (option.Load(false) == dollState.Tattoos[index].PaintEE.Load())
                {
                    current = sequentialEntity2;
                }
            }
            return sequentialEntityList;
        }

        // Token: 0x17000CDB RID: 3291
        // (get) Token: 0x06004EC6 RID: 20166 RVA: 0x001B8C00 File Offset: 0x001B6E00
        public SelectionGroupRadioVM<TextureSelectorItemVM> PrimaryOutfitColorVM
        {
            get
            {
                return this.m_PrimaryOutfitColorVM = (this.m_PrimaryOutfitColorVM ?? this.CreatePrimaryOutfitColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x17000CDC RID: 3292
        // (get) Token: 0x06004EC7 RID: 20167 RVA: 0x001B8C2C File Offset: 0x001B6E2C
        public SelectionGroupRadioVM<TextureSelectorItemVM> SecondaryOutfitColorVM
        {
            get
            {
                return this.m_SecondaryOutfitColorVM = (this.m_SecondaryOutfitColorVM ?? this.CreateSecondaryOutfitColorSelectorVM(this.DollState));
            }
        }

        // Token: 0x06004EC8 RID: 20168 RVA: 0x001B8C58 File Offset: 0x001B6E58
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreatePrimaryOutfitColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdatePrimaryOutfitColors(dollState, selector);
            return selector;
        }

        // Token: 0x06004EC9 RID: 20169 RVA: 0x001B8C7C File Offset: 0x001B6E7C
        private void UpdatePrimaryOutfitColors(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> outfitRampsPrimary = dollState.GetOutfitRampsPrimary();
            for (int index = 0; index < outfitRampsPrimary.Count; index++)
            {
                int i1 = index;
                Texture2D texture2D = outfitRampsPrimary[index];
                Action setter = delegate ()
                {
                    dollState.SetPrimaryEquipColor(i1);
                    this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index, texture2D, setter);
                if (dollState.EquipmentRampIndex == index)
                {
                    selector.TrySelectEntity(entitiesCollection[index]);
                }
            }
            selector.ClearFromIndex(outfitRampsPrimary.Count);
        }

        // Token: 0x06004ECA RID: 20170 RVA: 0x001B8D1C File Offset: 0x001B6F1C
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateSecondaryOutfitColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateSecondaryOutfitColors(dollState, selector);
            return selector;
        }

        // Token: 0x06004ECB RID: 20171 RVA: 0x001B8D40 File Offset: 0x001B6F40
        private void UpdateSecondaryOutfitColors(DollState dollState, SelectionGroupRadioVM<TextureSelectorItemVM> selector)
        {
            ReactiveCollection<TextureSelectorItemVM> entitiesCollection = selector.EntitiesCollection;
            List<Texture2D> outfitRampsSecondary = dollState.GetOutfitRampsSecondary();
            for (int index = 0; index < outfitRampsSecondary.Count; index++)
            {
                int i1 = index;
                Texture2D texture2D = outfitRampsSecondary[index];
                Action setter = delegate ()
                {
                    dollState.SetSecondaryEquipColor(i1);
                    this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index, texture2D, setter);
                if (dollState.EquipmentRampIndexSecondary == index)
                {
                    selector.TrySelectEntity(entitiesCollection[index]);
                }
            }
            selector.ClearFromIndex(outfitRampsSecondary.Count);
        }

        // Token: 0x0400328D RID: 12941
        public readonly DollState DollState;

        // Token: 0x0400328E RID: 12942
        public readonly bool IsAlternative;

        // Token: 0x0400328F RID: 12943
        private bool m_Subscribed;

        // Token: 0x04003290 RID: 12944
        private SelectionGroupRadioVM<TextureSelectorItemVM> m_BodyColorSelectorVM;

        // Token: 0x04003291 RID: 12945
        private StringSequentialSelectorVM m_BodySelectorVM;

        // Token: 0x04003292 RID: 12946
        private SelectionGroupRadioVM<TextureSelectorItemVM> m_EyesColorSelectorVM;

        // Token: 0x04003293 RID: 12947
        private StringSequentialSelectorVM m_ScarsSelectorVM;

        // Token: 0x04003294 RID: 12948
        private StringSequentialSelectorVM m_HandSelectorVM;

        // Token: 0x04003295 RID: 12949
        private StringSequentialSelectorVM m_BeardSelectorVM;

        // Token: 0x04003296 RID: 12950
        private StringSequentialSelectorVM m_HornSelectorVM;

        // Token: 0x04003297 RID: 12951
        private SelectionGroupRadioVM<TextureSelectorItemVM> m_HornColorSelectorVM;

        // Token: 0x04003298 RID: 12952
        private StringSequentialSelectorVM m_HeadSelectorVM;

        // Token: 0x04003299 RID: 12953
        private StringSequentialSelectorVM m_HairSelectorVM;

        // Token: 0x0400329A RID: 12954
        private SelectionGroupRadioVM<TextureSelectorItemVM> m_HairColorSelectorVM;

        // Token: 0x0400329B RID: 12955
        private List<StringSequentialSelectorVM> m_WarpaintSelectorVMList;

        // Token: 0x0400329C RID: 12956
        private List<SelectionGroupRadioVM<TextureSelectorItemVM>> m_WarpaintColorSelectorVMList;

        // Token: 0x0400329D RID: 12957
        private List<StringSequentialSelectorVM> m_TattooSelectorVMList;

        // Token: 0x0400329E RID: 12958
        private List<SelectionGroupRadioVM<TextureSelectorItemVM>> m_TattooColorSelectorVMList;

        // Token: 0x0400329F RID: 12959
        private SelectionGroupRadioVM<TextureSelectorItemVM> m_PrimaryOutfitColorVM;

        // Token: 0x040032A0 RID: 12960
        private SelectionGroupRadioVM<TextureSelectorItemVM> m_SecondaryOutfitColorVM;
    }
    public class CharGenAppearancePhaseDetailedPCViewModified : CharGenPhaseDetailedBaseView<CharGenAppearancePhaseVMModified>
    {
        // Token: 0x060062CF RID: 25295 RVA: 0x001FBE98 File Offset: 0x001FA098
        public override void Initialize()
        {
            //base.Initialize();
            UICharGen charGen = UIStrings.Instance.CharGen;
            this.m_ChooseBodyLabel.text = UIUtility.GetSaberBookFormat(charGen.BodyConstitution, default(Color), 140, null, 0f);
            this.m_ChooseHairLabel.text = UIUtility.GetSaberBookFormat(charGen.HairStyle, default(Color), 140, null, 0f);
            this.m_ChooseHornsLabel.text = UIUtility.GetSaberBookFormat(charGen.Horns, default(Color), 140, null, 0f);
            this.m_ChooseWarpaintsLabel.text = UIUtility.GetSaberBookFormat(charGen.Warpaint, default(Color), 140, null, 0f);
            this.m_ChooseTattoosLabel.text = UIUtility.GetSaberBookFormat(charGen.Tattoo, default(Color), 140, null, 0f);
            this.m_ChoosePrimaryColorLabel.text = UIUtility.GetSaberBookFormat(charGen.ClothColor, default(Color), 140, null, 0f);
            this.m_BodySelectorPcView.SetTitleText(charGen.BodyConstitution);
            this.m_FaceSelectorPcView.SetTitleText(charGen.Face);
            this.m_ScarSelectorPcView.SetTitleText(charGen.Scar);
            this.m_BodyColorSelectorView.SetTitleText(charGen.SkinTone);
            this.m_HairSelectorPcView.SetTitleText(charGen.HairStyle);
            this.m_BeardSelectorPcView.SetTitleText(charGen.Beard);
            this.m_HairColorSelectorView.SetTitleText(charGen.HairColor);
            this.m_EyesColorSelectorView.SetTitleText(charGen.EyesColor);
            this.m_WarpaintSelectorPcView.SetTitleText(charGen.Warpaint);
            this.m_TatooSelectorPcView.SetTitleText(charGen.Tattoo);
            this.m_HornSelectorPcView.SetTitleText(charGen.Horns);
            this.m_HornColorSelectorView.SetTitleText(charGen.HornsColor);
            this.m_PrimaryOutfitColorSelectorView.SetTitleText(charGen.PrimaryClothColor);
            this.m_SecondaryOutfitColorSelectorView.SetTitleText(charGen.SecondaryClothColor);
            this.m_HairSelectorPcView.SetTitleText(charGen.HairStyle);
            this.m_BeardSelectorPcView.SetTitleText(charGen.Beard);
            this.m_HairColorSelectorView.SetTitleText(charGen.HairColor);
            this.m_CharacterController.Unbind();
            this.VisualSettings.Intialize();
        }

        // Token: 0x060062D0 RID: 25296 RVA: 0x001FC15C File Offset: 0x001FA35C
        public override void BindViewImplementation()
        {
            base.BindViewImplementation();
            this?.VisualSettings?.Bind(base.ViewModel?.DollState);
            this.m_CharacterController.Bind(base.ViewModel.DollState);
            this.m_CharacterController.SetTransform(this.m_TargetSizeInfoDollTransform);
            this.m_BodySelectorPcView.Bind(base.ViewModel.BodySelectorVM);
            this.m_FaceSelectorPcView.Bind(base.ViewModel.HeadSelectorVM);
            this.m_ScarSelectorPcView.Bind(base.ViewModel.ScarsSelectorVM);
            this.m_BodyColorSelectorView.Bind(base.ViewModel.BodyColorSelectorVM);
            this.m_EyesColorSelectorView.Bind(base.ViewModel.EyesColorSelectorVM);
            this.m_HairSelectorPcView.Bind(base.ViewModel.HairSelectorVM);
            this.m_BeardSelectorPcView.Bind(base.ViewModel.BeardSelectorVM);
            this.m_HairColorSelectorView.Bind(base.ViewModel.HairColorSelectorVM);
            bool flag = this.m_HairSelectorPcView.IsActive || this.m_BeardSelectorPcView.IsActive || this.m_HairColorSelectorView.IsActive;
            this.m_HairBlock.SetActive(flag);
            this.m_HairBlockPlaceholder.SetActive(!flag);
            this.m_HornSelectorPcView.Bind(base.ViewModel.HornSelectorVM);
            this.m_HornColorSelectorView.Bind(base.ViewModel.HornColorSelectorVM);
            this.m_HornBlock.SetActive(base.ViewModel.HornSelectorVM.IsValid());
            this.m_WarpaintPaginator.Initialize(base.ViewModel.WarpaintsNumber, new Action<int>(this.BindWarpaints));
            this.m_TatooPaginator.Initialize(base.ViewModel.TattoosNumber, new Action<int>(this.BindTattoos));
            this.m_TatooSelectorPcView.SetOnChangeCallback(delegate
            {
                this.VisualSettings.ShowIfNotSeenAndSwitchClothTo(false);
            });
            this.m_TatooColorSelectorView.SetOnChangeCallback(delegate
            {
                this.VisualSettings.ShowIfNotSeenAndSwitchClothTo(false);
            });
            this.m_PrimaryOutfitColorSelectorView.Bind(base.ViewModel.PrimaryOutfitColorVM);
            this.m_PrimaryOutfitColorSelectorView.SetOnChangeCallback(delegate
            {
                this.VisualSettings.ShowIfNotSeenAndSwitchClothTo(true);
            });
            this.m_SecondaryOutfitColorSelectorView.Bind(base.ViewModel.SecondaryOutfitColorVM);
            this.m_SecondaryOutfitColorSelectorView.SetOnChangeCallback(delegate
            {
                this.VisualSettings.ShowIfNotSeenAndSwitchClothTo(true);
            });
            //if (base.ViewModel.HornSelectorVM.IsValid())
            {
                this.m_PrimaryOutfitColorSelectorView.SetRowNumber(1);
                this.m_SecondaryOutfitColorSelectorView.SetRowNumber(1);
                //return;
            }
            this.m_PrimaryOutfitColorSelectorView.SetRowNumber(1);
            this.m_SecondaryOutfitColorSelectorView.SetRowNumber(1);

            {

                //My stuff
                {
                    var comp = m_RaceSelectorPCView;
                    var n = new List<StringSequentialEntity>();
                    foreach (BlueprintRace race in Kingmaker.Game.Instance.BlueprintRoot.Progression.CharacterRaces)
                    {

                        var newseq = new StringSequentialEntity();
                        newseq.Title = race.Name;
                        newseq.Setter = () =>
                        {
                            comp.SetTitleText(race.Name);
                            if (race.Name != this.ViewModel.DollState.Race.Name)
                            {
                                this.ViewModel?.DollState?.SetRace(race);
                                if (this.ViewModel != null)
                                {
                                    CharGenAppearancePhaseVMModified.pcview = this;
                                    var vmnew = new CharGenAppearancePhaseVMModified(this.ViewModel.LevelUpController, this.ViewModel.DollState, false);
                                    this.Bind(vmnew);
                                }
                            }
                        };
                        n.Add(newseq);
                    }
                    var newvm = new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(n, n.First(a => a.Title == this.ViewModel.DollState.Race.Name), false);
                    comp.Bind(newvm);
                    //comp.SetCurrentIndex(n.FindIndex(a => a.Title == this.ViewModel.DollState.Race.Name));



                    var visualsettings = this.transform.parent.Find("DollRoom(Clone)/CharacterVisualSettingsView").GetComponent<CharacterVisualSettingsView>();
                    visualsettings.Bind(this.ViewModel.DollState);

                    var DollRoomComp = this.transform.parent.Find("DollRoom(Clone)").GetComponent<DollCharacterController>();
                    DollRoomComp.Bind(this.ViewModel.DollState);

                    //this.m_RaceSelectorPCView.SetCurrentIndex(this.m_RaceSelectorPCView.ViewModel.ValueList.FindIndex(a => a.Title == this.ViewModel.DollState.Race.Name));// ?? 1);
                }
            }
        }

        // Token: 0x060062D1 RID: 25297 RVA: 0x001FC3F8 File Offset: 0x001FA5F8
        private void BindWarpaints(int index)
        {
            this.m_WarpaintSelectorPcView.Bind(base.ViewModel.WarpaintsSelectorVMList[index]);
            bool isActive = this.m_WarpaintSelectorPcView.IsActive;
            this.m_WarpaintBlock.SetActive(isActive);
            if (isActive)
            {
                this.m_WarpaintColorSelectorView.Bind(base.ViewModel.WarpaintsColorSelectorVMList[index]);
            }
        }

        // Token: 0x060062D2 RID: 25298 RVA: 0x000424B2 File Offset: 0x000406B2
        private void BindTattoos(int index)
        {
            this.m_TatooSelectorPcView.Bind(base.ViewModel.TattoosSelectorVMList[index]);
            this.m_TatooColorSelectorView.Bind(base.ViewModel.TattoosColorSelectorVMList[index]);
        }

        // Token: 0x060062D3 RID: 25299 RVA: 0x000424EC File Offset: 0x000406EC
        public override void DestroyViewImplementation()
        {
            base.DestroyViewImplementation();
            this.VisualSettings.Dispose();
            this.m_CharacterController.Unbind();
        }

        //My stuff

        public SlideSelectorPCView m_RaceSelectorPCView;



        //End my stuff
        // Token: 0x04004240 RID: 16960
        [SerializeField]
        public FadeAnimator m_LeftAnimator;

        // Token: 0x04004241 RID: 16961
        [SerializeField]
        public FadeAnimator m_RightAnimator;

        // Token: 0x04004242 RID: 16962
        [Header("Labels")]
        [SerializeField]
        public TextMeshProUGUI m_ChooseBodyLabel;

        // Token: 0x04004243 RID: 16963
        [SerializeField]
        public TextMeshProUGUI m_ChooseHairLabel;

        // Token: 0x04004244 RID: 16964
        [SerializeField]
        public TextMeshProUGUI m_ChooseHornsLabel;

        // Token: 0x04004245 RID: 16965
        [SerializeField]
        public TextMeshProUGUI m_ChooseWarpaintsLabel;

        // Token: 0x04004246 RID: 16966
        [SerializeField]
        public TextMeshProUGUI m_ChooseTattoosLabel;

        // Token: 0x04004247 RID: 16967
        [SerializeField]
        public TextMeshProUGUI m_ChoosePrimaryColorLabel;

        // Token: 0x04004248 RID: 16968
        [SerializeField]
        public TextMeshProUGUI m_ChooseSecondaryColorLabel;

        // Token: 0x04004249 RID: 16969
        [Header("Body")]
        [SerializeField]
        public SlideSelectorPCView m_BodySelectorPcView;

        // Token: 0x0400424A RID: 16970
        [SerializeField]
        public SlideSelectorPCView m_FaceSelectorPcView;

        // Token: 0x0400424B RID: 16971
        [SerializeField]
        public SlideSelectorPCView m_ScarSelectorPcView;

        // Token: 0x0400424C RID: 16972
        [SerializeField]
        public TextureSelectorPCView m_BodyColorSelectorView;

        // Token: 0x0400424D RID: 16973
        [SerializeField]
        public TextureSelectorPCView m_EyesColorSelectorView;

        // Token: 0x0400424E RID: 16974
        [Header("Hair")]
        [SerializeField]
        public GameObject m_HairBlock;

        // Token: 0x0400424F RID: 16975
        [SerializeField]
        public GameObject m_HairBlockPlaceholder;

        // Token: 0x04004250 RID: 16976
        [SerializeField]
        public SlideSelectorPCView m_HairSelectorPcView;

        // Token: 0x04004251 RID: 16977
        [SerializeField]
        public SlideSelectorPCView m_BeardSelectorPcView;

        // Token: 0x04004252 RID: 16978
        [SerializeField]
        public TextureSelectorPCView m_HairColorSelectorView;

        // Token: 0x04004253 RID: 16979
        [Header("Warpaint")]
        [SerializeField]
        public GameObject m_WarpaintBlock;

        // Token: 0x04004254 RID: 16980
        [SerializeField]
        public SlideSelectorPCView m_WarpaintSelectorPcView;

        // Token: 0x04004255 RID: 16981
        [SerializeField]
        public TextureSelectorPCView m_WarpaintColorSelectorView;

        // Token: 0x04004256 RID: 16982
        [SerializeField]
        public ClickablePageNavigation m_WarpaintPaginator;

        // Token: 0x04004257 RID: 16983
        [Header("Tatoo")]
        [SerializeField]
        public SlideSelectorPCView m_TatooSelectorPcView;

        // Token: 0x04004258 RID: 16984
        [SerializeField]
        public TextureSelectorPCView m_TatooColorSelectorView;

        // Token: 0x04004259 RID: 16985
        [SerializeField]
        public ClickablePageNavigation m_TatooPaginator;

        // Token: 0x0400425A RID: 16986
        [Header("Horns")]
        [SerializeField]
        public GameObject m_HornBlock;

        // Token: 0x0400425B RID: 16987
        [SerializeField]
        public SlideSelectorPCView m_HornSelectorPcView;

        // Token: 0x0400425C RID: 16988
        [SerializeField]
        public TextureSelectorPCView m_HornColorSelectorView;

        // Token: 0x0400425D RID: 16989
        [Header("Cloth")]
        [SerializeField]
        public TextureSelectorPCView m_PrimaryOutfitColorSelectorView;

        // Token: 0x0400425E RID: 16990
        [SerializeField]
        public TextureSelectorPCView m_SecondaryOutfitColorSelectorView;

        // Token: 0x0400425F RID: 16991
        [Header("Doll")]
        [SerializeField]
        public DollCharacterController m_CharacterController;

        // Token: 0x04004260 RID: 16992
        [SerializeField]
        public RectTransform m_TargetSizeInfoDollTransform;

        // Token: 0x04004261 RID: 16993
        [SerializeField]
        public CharacterVisualSettingsView VisualSettings;
    }
}
