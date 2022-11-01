using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.DLC;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Enums;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Portrait;
using Kingmaker.UI.MVVM._VM.CharGen.Portrait;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

namespace VisualAdjustments2.UI
{
    public class PickerVMTst : CharGenPortraitPhaseVM
    {
        public PickerVMTst(LevelUpController levelUpController) : base(levelUpController)
        {
            base.AddDisposable(this);
        }

        public override void CollectCharacterPortraits(LevelUpController levelUpController)
        {
            foreach (BlueprintPortrait portrait in Game.Instance.BlueprintRoot.CharGen.Portraits)
            {
                if (true/*(!levelUpController.State.IsEmployee || (portrait.BackupPortrait == null && Game.Instance.Player.MainCharacter.Value.UISettings.PortraitBlueprint != portrait))*/)
                {
                    CharGenPortraitSelectorItemVM charGenPortraitSelectorItemVM = new CharGenPortraitSelectorItemVM(portrait);
                    AllPortraitsCollection.Add(charGenPortraitSelectorItemVM);
                    PortraitCategory portraitCategory = portrait.Data.PortraitCategory;
                    if (!PortraitGroupVms.ContainsKey(portraitCategory))
                    {
                        PortraitGroupVms.Add(portraitCategory, new CharGenPortraitGroupVM(portraitCategory));
                        PortraitGroupVms[portraitCategory].Expanded.Value = (portraitCategory != PortraitCategory.KingmakerNPC);
                    }

                    PortraitGroupVms[portraitCategory].Add(charGenPortraitSelectorItemVM);
                }
            }
            foreach (var bp in Kingmaker.Cheats.Utilities.GetAllBlueprints().Entries.Where(a => a.Type == typeof(BlueprintPortrait)).Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintPortrait>(b.Guid)))
            {
                {
                    if (bp == null) return;
                    if (!bp.InitiativePortrait && bp.Data.SmallPortrait != null && bp.Data.HalfLengthPortrait != null && bp.Data.FullLengthPortrait != null)
                    {
                        if (bp.AssetGuid.ToString() == null) return;
                        CharGenPortraitSelectorItemVM charGenPortraitSelectorItemVM = new CharGenPortraitSelectorItemVM(bp, false);
                        AllPortraitsCollection.Add(charGenPortraitSelectorItemVM);
                        PortraitCategory portraitCategory = bp.Data.PortraitCategory;
                        if (!PortraitGroupVms.ContainsKey(portraitCategory))
                        {
                            PortraitGroupVms.Add(portraitCategory, new CharGenPortraitGroupVM(portraitCategory));
                            PortraitGroupVms[portraitCategory].Expanded.Value = (portraitCategory != PortraitCategory.KingmakerNPC);
                        }
                        if (!PortraitGroupVms[portraitCategory].PortraitCollection.Any(a => a.m_BlueprintPortrait == bp))
                        {
                            PortraitGroupVms[0].PortraitCollection.Add(charGenPortraitSelectorItemVM);
                        }
                    }
                }
            }
        }
    }
    public class PortraitPickerVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public ReactiveProperty<CharGenPortraitVM> PreviewVM = new();
        public ReactiveProperty<CharGenPortraitPhaseVM> PickerVM = new();
        public VoicePickerVM VoiceVM;

        public void OnCharacterChanged(UnitReference data)
        {

                this.PickerVM.Value.SelectedPortrait.Value =
                    this.PickerVM.Value.AllPortraitsCollection.FirstOrDefault(
                        a => a.PortraitData == data.Value.Portrait);
                this.VoiceVM.supressVoiceLine = true;
                this.VoiceVM.SetupSelected();
        }
        //Onchar changed stuff
        public PortraitPickerVM()
        {
            base.AddDisposable(this.VoiceVM = new VoicePickerVM());
            base.AddDisposable(this.PreviewVM.Value = new(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Portrait));
            base.AddDisposable(this.PickerVM.Value = new PickerVMTst(new Kingmaker.UnitLogic.Class.LevelUp.LevelUpController(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Value, true, Kingmaker.UnitLogic.Class.LevelUp.LevelUpState.CharBuildMode.CharGen)));
            base.AddDisposable(this.PickerVM.Value.SelectedPortrait.Value = this.PickerVM.Value.AllPortraitsCollection.FirstOrDefault(a => a.PortraitData == Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Portrait));
            base.AddDisposable(this.PickerVM?.Value?.SelectedPortrait?.Subscribe(b => 
            { 
                if(this.PreviewVM?.Value?.IsDisposed is not false or null) this.PreviewVM?.Value?.Dispose();
                if (b?.PortraitData != null)
                {
                    this.PreviewVM.Value = new(b.PortraitData);
                    Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.UISettings.SetPortrait(b.GetBlueprintPortrait());
                }
            }));
            
            base.AddDisposable(Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Subscribe(OnCharacterChanged));

        }
        public override void DisposeImplementation()
        {

        }
    }
}
