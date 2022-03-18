using Kingmaker;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Parts;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Owlcat.Runtime.UniRx;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class DollVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public DollVM()
        {
            base.AddDisposable(this);
            base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe((UnitDescriptor _) => { OnCharacterChanged(); }));
            OnCharacterChanged(true);
        }
        public void AddUnitPart()
        {
            try
            {
                var dolldata = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.Parts.Add<UnitPartDollData>();
                var data = dolldata.SetupForStoryCompanion();
                //dolldata.Default = data;
                dolldata.SetDefault(data);
                Game.Instance.SelectionCharacter.SelectedUnit.Value.ForcceUseClassEquipment = true;
                OnCharacterChanged(true);
            }
            catch(Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public void RemoveUnitPart()
        {
            try
            {
                Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.Parts.Remove<UnitPartDollData>();
                Game.Instance.SelectionCharacter.SelectedUnit.Value.ForcceUseClassEquipment = false;
                OnCharacterChanged(true);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public void OnCharacterChanged(bool forcechange = false)
        {
            try
            {
                if (this.IsDisposed) return;
                //Main.Logger.Log("charch");
                var unit = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit;
                if (unit.Get<UnitPartDollData>() != null && (forcechange || this.createDollVM?.Value?.charname != unit.CharacterName))
                {
                    var doll = unit.GetDollState();
                    if (doll.Race != null)
                    {
                       // Main.Logger.Log("NotNullRace");
                        //var lvlcontroller = new LevelUpController(unit, false, LevelUpState.CharBuildMode.SetName);
                        //Main.Logger.Log("AfterLvlCtor");
                        //lvlcontroller.Doll = doll;
                        CharGenAppearancePhaseVMModified.pcview.gameObject.SetActive(true);
                        CharGenAppearancePhaseVMModified.pcview.transform.parent.gameObject.SetActive(true);
                        base.AddDisposable(this.m_DollAppearanceVM.Value = new CharGenAppearancePhaseVMModified(/*lvlcontroller,*/ doll, false));
                        ///Fails after here somewhere
                        //Main.Logger.Log("AfterBind");
                    }
                    else
                    {
                        //Main.Logger.Log("NullRace");
                        var newvm = new CreateDollVM();
                        base.AddDisposable(this.createDollVM.Value = newvm);
                        //  newvm.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe((UnitDescriptor dat) => { if (dat.CharacterName != newvm.charname) { newvm.Dispose(); this.m_DollAppearanceVM.Value?.Dispose(); this.ShowWindow(VisualWindowType.Doll); } }));
                    }
                    this.createDollVM.Value?.Dispose();
                    // this.createDollVM.Value = null;
                    return;
                }
                else if (this.createDollVM.Value == null || forcechange || this.createDollVM?.Value?.charname != unit.CharacterName)
                {
                    //Main.Logger.Log("notDollData");
                    var newvm = new CreateDollVM();
                    base.AddDisposable(this.createDollVM.Value = newvm);
                    // newvm.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe((UnitDescriptor _) => { newvm.Dispose(); this.m_DollAppearanceVM.Value?.Dispose(); this.ShowWindow(VisualWindowType.Doll); }));
                    // newvm.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe((UnitDescriptor _) => { newvm.Dispose(); this.ShowWindow(VisualWindowType.Doll); }));
                    // TODO: Create doll button if no doll present.
                }
            }
            catch(Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public override void DisposeImplementation()
        {
           // Main.Logger.Log("DisposedDOllVM");
            //this.Dispose();
        }
        public ReactiveProperty<CreateDollVM> createDollVM = new ReactiveProperty<CreateDollVM>();
        public ReactiveProperty<CharGenAppearancePhaseVMModified> m_DollAppearanceVM = new ReactiveProperty<CharGenAppearancePhaseVMModified>();
    }
}
