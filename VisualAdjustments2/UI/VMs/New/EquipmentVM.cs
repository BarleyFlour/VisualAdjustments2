using Kingmaker.EntitySystem.Entities;
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
