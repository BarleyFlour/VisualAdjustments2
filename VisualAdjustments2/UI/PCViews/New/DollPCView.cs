using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Owlcat.Runtime.UniRx;
using Kingmaker;
using Kingmaker.UnitLogic;

namespace VisualAdjustments2.UI
{
    public class DollPCView : ViewBase<DollVM>
    {
        public override void BindViewImplementation()
        {
            base.AddDisposable(base.ViewModel.m_DollAppearanceVM.Subscribe((CharGenAppearancePhaseVMModified val) => { this.m_CharGenAppearancePCView.Bind(val); }));
            base.AddDisposable(base.ViewModel.createDollVM.Subscribe(new Action<CreateDollVM>(this.m_CreateDollPCView.Bind)));
            //base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe((UnitDescriptor _) => { this.Unbind(); }));
        }
        public void DeleteDoll()
        {
            this.ViewModel.RemoveUnitPart();
            Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.RebuildCharacter();
            this.m_CharGenAppearancePCView.Unbind();
        }
        public override void DestroyViewImplementation()
        {
            try
            {
                // this.Unbind();
                this.gameObject.SetActive(false);
                m_CharGenAppearancePCView.Unbind();
                m_CreateDollPCView.Unbind();
            }
            catch (Exception ex)
            {
                Main.Logger.Error(ex.ToString());
            }
        }
        public CharGenAppearancePhaseDetailedPCViewModified m_CharGenAppearancePCView;
        public CreateDollPCView m_CreateDollPCView;

        internal void Initialize()
        {
        }
    }
}
