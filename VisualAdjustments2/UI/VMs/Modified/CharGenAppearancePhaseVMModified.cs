using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.ResourceLinks;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.Party;
using Kingmaker.UI.MVVM._VM.CharGen.Phases;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Appearance;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._VM.Party;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UniRx;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using VisualAdjustments2.UI;

namespace VisualAdjustments2.UI
{
    public class CharGenAppearancePhaseVMModified : BaseDisposable, IViewModel, IBaseDisposable, IDisposable
    {
        public static CharGenAppearancePhaseDetailedPCViewModified pcview;
        public static DollCharacterController charController;
        public CharGenAppearancePhaseVMModified(DollState dollState, bool isAlternative)
        {
            try
            {
                this.unit_GUID = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.UniqueId;
                this.DollState = dollState;
                //base.AddDisposable(dollState.m_OnUpdateAction += () => { levelUpController.Unit.SaveDollState(dollState); });
                this.IsAlternative = isAlternative;

                base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(this.OnUnitChanged));
                charController.Bind(dollState);
                //base.AddDisposable(this.RefreshView?.Subscribe(() => { this.Change(); }));
            }
            catch(Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw;
            }
        }
        private string unit_GUID;
        public void OnUnitChanged(UnitDescriptor descriptor)
        {
            try
            {
                this.Change();
                
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
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
       /* public override bool CheckIsCompleted()
        {
            return false;
            //This is just levelupstuff so we dont need to bother with any logic.
        }*/
        private bool SelectionStateIsCompleted(LevelUpController controller)
        {
            return true;
            //This is just levelupstuff so we dont need to bother with any logic.
        }

        // Token: 0x06004E94 RID: 20116 RVA: 0x001B7780 File Offset: 0x001B5980
        /*public override void OnBeginDetailedView()
        {
            if (!this.m_Subscribed)
            {
                //Check whatever this does
                /*base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Gender, true).Subscribe(delegate (Gender _)
                {
                    this.Change();
                }));
                base.AddDisposable(base.LevelUpController.GetReactiveProperty((LevelUpController controller) => this.DollState.Race, true).Subscribe(delegate (BlueprintRace _)
                {
                    this.Change();
                }));*//*
                this.m_Subscribed = true;
            }
            this.Change();
        }*/
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
            //this.LevelUpController.Unit.SaveDollState(this.DollState);
            //Main.Logger.Log("changed");
        }
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
        public SelectionGroupRadioVM<TextureSelectorItemVM> BodyColorSelectorVM
        {
            get
            {
                return this.m_BodyColorSelectorVM = (this.m_BodyColorSelectorVM ?? this.CreateBodyColorSelectorVM(this.DollState));
            }
        }
        public StringSequentialSelectorVM BodySelectorVM
        {
            get
            {
                return this.m_BodySelectorVM = (this.m_BodySelectorVM ?? this.GetBodySelectorVM(this.DollState));
            }
        }
        private StringSequentialSelectorVM GetBodySelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetBodyList(dollState, out current), current, true);
        }
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateBodyColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateBodyColorList(dollState, selector);
            return selector;
        }
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
        public SelectionGroupRadioVM<TextureSelectorItemVM> EyesColorSelectorVM
        {
            get
            {
                return this.m_EyesColorSelectorVM = (this.m_EyesColorSelectorVM ?? this.CreateEyesColorSelectorVM(this.DollState));
            }
        }
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateEyesColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateEyesColorList(dollState, selector);
            return selector;
        }
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
                       // this.LevelUpController.Unit.SaveDollState(dollState);
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
        public StringSequentialSelectorVM ScarsSelectorVM
        {
            get
            {
                return this.m_ScarsSelectorVM = (this.m_ScarsSelectorVM ?? this.GetScarsSelectorVM(this.DollState));
            }
        }
        private StringSequentialSelectorVM GetScarsSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetScarsList(dollState, out current), current, true);
        }
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
        public StringSequentialSelectorVM HandSelectorVM
        {
            get
            {
                return this.m_HandSelectorVM = (this.m_HandSelectorVM ?? this.GetHandSelectorVM(this.DollState));
            }
        }
        private StringSequentialSelectorVM GetHandSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHandList(dollState, out current), current, true);
        }
        private List<StringSequentialEntity> GetHandList(DollState dollState, out StringSequentialEntity current)
        {
            List<StringSequentialEntity> result = new List<StringSequentialEntity>();
            current = null;
            return result;
        }
        public StringSequentialSelectorVM BeardSelectorVM
        {
            get
            {
                return this.m_BeardSelectorVM = (this.m_BeardSelectorVM ?? this.GetBeardSelectorVM(this.DollState));
            }
        }
        private StringSequentialSelectorVM GetBeardSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetBeardList(dollState, out current), current, true);
        }
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
        public StringSequentialSelectorVM HornSelectorVM
        {
            get
            {
                return this.m_HornSelectorVM = (this.m_HornSelectorVM ?? this.GetHornSelectorVM(this.DollState));
            }
        }
        public SelectionGroupRadioVM<TextureSelectorItemVM> HornColorSelectorVM
        {
            get
            {
                return this.m_HornColorSelectorVM = (this.m_HornColorSelectorVM ?? this.CreateHornColorSelectorVM(this.DollState));
            }
        }
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateHornColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateHornColorList(dollState, selector);
            return selector;
        }
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
                    //this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index, texture2D, setter);
                if (dollState.HairRampIndex == index)
                {
                    selector.TrySelectEntity(entitiesCollection[index]);
                }
            }
            selector.ClearFromIndex(hornsRamps.Count);
        }
        private StringSequentialSelectorVM GetHornSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHornList(dollState, out current), current, true);
        }
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
        public StringSequentialSelectorVM HeadSelectorVM
        {
            get
            {
                return this.m_HeadSelectorVM = (this.m_HeadSelectorVM ?? this.GetHeadSelectorVM(this.DollState));
            }
        }
        private StringSequentialSelectorVM GetHeadSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHeadsList(dollState, out current), current, true);
        }
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
        public StringSequentialSelectorVM HairSelectorVM
        {
            get
            {
                return this.m_HairSelectorVM = (this.m_HairSelectorVM ?? this.GetHairSelectorVM(this.DollState));
            }
        }
        public SelectionGroupRadioVM<TextureSelectorItemVM> HairColorSelectorVM
        {
            get
            {
                return this.m_HairColorSelectorVM = (this.m_HairColorSelectorVM ?? this.CreateHairColorSelectorVM(this.DollState));
            }
        }
        private SelectionGroupRadioVM<TextureSelectorItemVM> CreateHairColorSelectorVM(DollState dollState)
        {
            SelectionGroupRadioVM<TextureSelectorItemVM> selector = new SelectionGroupRadioVM<TextureSelectorItemVM>(new ReactiveCollection<TextureSelectorItemVM>());
            this.UpdateHairColorList(dollState, selector);
            return selector;
        }
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
                    //this.LevelUpController.Unit.SaveDollState(dollState);
                };
                CharGenAppearancePhaseVM.GetTextureSelectorItemVM(entitiesCollection, index, texture2D, setter);
                if (dollState.HairRampIndex == index)
                {
                    selector.TrySelectEntity(entitiesCollection[index]);
                }
            }
            selector.ClearFromIndex(hairRamps.Count);
        }
        private StringSequentialSelectorVM GetHairSelectorVM(DollState dollState)
        {
            StringSequentialEntity current;
            return new StringSequentialSelectorVM(this.GetHairList(dollState, out current), current, true);
        }
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
        public int WarpaintsNumber
        {
            get
            {
                return 5;
            }
        }
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
                    //this.LevelUpController.Unit.SaveDollState(dollState);
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
            if (index < dollState.Warprints.Capacity && index >= 0 && dollState.Warprints[index] != null)
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
                            //this.LevelUpController.Unit.SaveDollState(dollState);
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
                    //this.LevelUpController.Unit.SaveDollState(dollState);
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
                        //this.LevelUpController.Unit.SaveDollState(dollState);
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
                    //this.LevelUpController.Unit.SaveDollState(dollState);
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
                    //this.LevelUpController.Unit.SaveDollState(dollState);
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
}
