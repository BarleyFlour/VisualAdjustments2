using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Root.Strings;
using Kingmaker.UI.Common;
using Kingmaker.UI.Common.Animations;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._VM.Utility;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;



//Add apply button
//Add a list of actions to take when applying
//Remove the automatic saving

namespace VisualAdjustments2.UI
{
    public class CharGenAppearancePhaseDetailedPCViewModified : ViewBase<CharGenAppearancePhaseVMModified>, IInitializable
    {
        // Token: 0x060062CF RID: 25295 RVA: 0x001FBE98 File Offset: 0x001FA098
        public void Initialize()
        {
            //base.Initialize();
            base.gameObject.SetActive(false);
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
            try
            {
                if (base.ViewModel == null)
                {
                    this.DestroyViewImplementation();
                    return;
                }
                base.gameObject.SetActive(true);
                //base.BindViewImplementation();
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
                                        var vmnew = new CharGenAppearancePhaseVMModified(/*this.ViewModel.LevelUpController,*/ this.ViewModel.DollState, false);
                                        this.Bind(vmnew);
                                    }
                                }
                            };
                            n.Add(newseq);
                        }
                        var newvm = new Kingmaker.UI.MVVM._VM.CharGen.Phases.Common.StringSequentialSelectorVM(n, n.FirstOrDefault(a => a.Title == this.ViewModel.DollState.Race.Name), false);
                        comp.Bind(newvm);
                        //comp.SetCurrentIndex(n.FindIndex(a => a.Title == this.ViewModel.DollState.Race.Name));



                        var visualsettings = this.transform.parent.Find("DollRoom(Clone)/CharacterVisualSettingsView").GetComponent<CharacterVisualSettingsView>();
                        visualsettings.Bind(this.ViewModel.DollState);

                        var DollRoomComp = this.transform.parent.Find("DollRoom(Clone)").GetComponent<DollCharacterController>();
                        DollRoomComp.Bind(this.ViewModel.DollState);

                        if (Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.IsStoryCompanion())
                        {
                            m_DeleteDollButton.gameObject.transform.parent.gameObject.SetActive(true);
                        }
                        else
                        {
                            m_DeleteDollButton.gameObject.transform.parent.gameObject.SetActive(false);
                        }
                        //this.m_RaceSelectorPCView.SetCurrentIndex(this.m_RaceSelectorPCView.ViewModel.ValueList.FindIndex(a => a.Title == this.ViewModel.DollState.Race.Name));// ?? 1);
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
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
            base.gameObject.SetActive(false);
            // base.DestroyViewImplementation();
            this.VisualSettings.Dispose();
            this.m_CharacterController.Unbind();
            //this.Unbind();
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

        public OwlcatButton m_ApplyButton;

        public OwlcatButton m_DeleteDollButton;
    }
}
