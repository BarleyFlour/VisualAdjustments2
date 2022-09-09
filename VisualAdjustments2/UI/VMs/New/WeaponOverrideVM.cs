using Kingmaker;
using Kingmaker.Items;
using Kingmaker.Utility;
using Kingmaker.View.Animation;
using Kingmaker.View.Equipment;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class WeaponOverrideVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {

        public ReactiveProperty<int> slot = new ReactiveProperty<int>(0);
        public ReactiveProperty<bool> hand = new ReactiveProperty<bool>(true);
        public ReactiveProperty<WeaponAnimationStyle> animStyle = new ReactiveProperty<WeaponAnimationStyle>();
        public ReactiveProperty<ListViewVM> m_ListViewVM = new ReactiveProperty<ListViewVM>();
        public static Dictionary<int, WeaponAnimationStyle> m_IntToAnim;
        public static Dictionary<WeaponAnimationStyle, int> m_AnimToInt;
        //For FX
        public void AddOverride(string guid)
        {
            try
            {

                var settings = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings();
                var first = settings.EnchantOverrides.FirstOrDefault(b => b.Slot == slot.Value && b.MainOrOffHand == hand.Value);
                if (first == null)
                {

                    if (!guid.IsNullOrEmpty())
                    {
                        settings.EnchantOverrides.Add(new EnchantOverride(hand.Value, slot.Value, guid));
                    }
                    Refresh();
                }
                else
                {
                    if (guid.IsNullOrEmpty())
                    {
                        settings.EnchantOverrides.Remove(first);
                    }
                    first.GUID = guid;
                    Refresh();
                }
                //Refresh magic
                void Refresh()
                {
                    Game.Instance.UI.Common.DollRoom.m_AvatarHands.UpdateAll();
                    Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View?.HandsEquipment?.UpdateAll();
                    if (Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.HandsEquipment.m_ActiveSet.MainHand.VisibleItem != null) Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.HandsEquipment.m_ActiveSet?.MainHand?.UpdateWeaponEnchantmentFx(true);
                    if (Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.HandsEquipment.m_ActiveSet.OffHand.VisibleItem != null) Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.HandsEquipment.m_ActiveSet?.OffHand?.UpdateWeaponEnchantmentFx(true);
                    if (Game.Instance.UI.Common.DollRoom.m_AvatarHands.m_ActiveSet?.MainHand?.VisibleItem != null) Game.Instance.UI.Common.DollRoom.m_AvatarHands.m_ActiveSet?.MainHand?.UpdateWeaponEnchantmentFx(true);
                    if (Game.Instance.UI.Common.DollRoom.m_AvatarHands.m_ActiveSet?.OffHand?.VisibleItem != null) Game.Instance.UI.Common.DollRoom.m_AvatarHands.m_ActiveSet?.OffHand?.UpdateWeaponEnchantmentFx(true);
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public void AddOverride(string guid, WeaponAnimationStyle style)
        {
            try
            {
                var settings = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings();
                var first = settings.WeaponOverrides.FirstOrDefault(b => b.AnimStyle == style.ToString() && b.Slot == slot.Value && b.MainOrOffHand == hand.Value);
                if (first == null)
                {
                    if (!guid.IsNullOrEmpty())
                    {
                        settings.WeaponOverrides.Add(new WeaponOverride(hand.Value, slot.Value, guid, style.ToString()));
                    }
                    Refresh();
                }
                else
                {
                    if (guid.IsNullOrEmpty())
                    {
                        settings.WeaponOverrides.Remove(first);
                    }
                    else
                        first.GUID = guid;
                    Refresh();
                }
                //Refresh magic
                void Refresh()
                {
                    Game.Instance?.UI?.Common?.DollRoom?.m_AvatarHands?.UpdateAll();
                    Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value.View?.HandsEquipment?.UpdateAll();
                    if (Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value.View?.HandsEquipment?.m_ActiveSet?.MainHand?.VisibleItem != null) Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.HandsEquipment.m_ActiveSet?.MainHand?.UpdateWeaponEnchantmentFx(true);
                    if (Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value.View?.HandsEquipment?.m_ActiveSet?.OffHand?.VisibleItem != null) Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.HandsEquipment.m_ActiveSet?.OffHand?.UpdateWeaponEnchantmentFx(true);
                    if (Game.Instance?.UI?.Common?.DollRoom?.m_AvatarHands?.m_ActiveSet?.MainHand?.VisibleItem != null) Game.Instance.UI.Common.DollRoom.m_AvatarHands.m_ActiveSet?.MainHand?.UpdateWeaponEnchantmentFx(true);
                    if (Game.Instance?.UI?.Common?.DollRoom?.m_AvatarHands?.m_ActiveSet?.OffHand?.VisibleItem != null) Game.Instance.UI.Common.DollRoom.m_AvatarHands.m_ActiveSet?.OffHand?.UpdateWeaponEnchantmentFx(true);
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
        public ListViewItemVM SelectFromSettings(int dropDownIndex = 0)
        {
            try
            {
                // if (((int)this.animStyle.Value) == 14)
#if DEBUG
                Main.Logger.Log(((int)this.animStyle.Value).ToString());
#endif
                if((dropDownIndex == 14))
              //  if (m_AnimToInt[this.animStyle.Value] == 99)
                {
                    // var selected = this.m_ListViewVM.FirstOrDefault(a => null != Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings().EnchantOverrides.FirstOrDefault(x => (x.GUID == a.Guid) && x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value));
                    // selected = (selected != null ? selected : CurrentReactive.FirstOrDefault());
                    //  Main.Logger.Log(selected.Guid + " " + selected.DisplayName);
                    //  base.AddDisposable(m_ListViewVM.Value = new ListViewVM(CurrentReactive, new ReactiveProperty<ListViewItemVM>(selected)));

                    //var s = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings().EnchantOverrides.FirstOrDefault(x => x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value);
                    // Main.Logger.Log(s.ToString());
                    //return this.m_ListViewVM.Value.EntitiesCollection.FirstOrDefault(a => a.Guid == s.GUID);
                    var selected = this.m_ListViewVM.Value.EntitiesCollection.FirstOrDefault(a => null != Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().EnchantOverrides.FirstOrDefault(x => (x.GUID == a.Guid) && x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value));
                    selected = (selected != null ? selected : this.m_ListViewVM.Value.EntitiesCollection.FirstOrDefault());
                    return selected;
                    /*var enchant = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings().EnchantOverrides.FirstOrDefault(x => x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value);
                    if (enchant != null)
                    {
                        return this.m_ListViewVM?.Value?.EntitiesCollection?.FirstOrDefault(c => c.Guid == enchant.GUID);
                    }
                    else return this.m_ListViewVM?.Value?.EntitiesCollection?.FirstOrDefault(c => c.DisplayName == "None");*/
                }
                else
                {
                    var animstyleasstring = this.animStyle.Value.ToString();
                    //Main.Logger.Log(animstyleasstring);
                    var s = Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().WeaponOverrides.FirstOrDefault(x => x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value && x.AnimStyle == animstyleasstring);
                    if (s != null)
                    {
                        return this.m_ListViewVM.Value.EntitiesCollection.FirstOrDefault(c => c.Guid == s.GUID);
                    }
                    else return this.m_ListViewVM?.Value?.EntitiesCollection?.FirstOrDefault(c => c.Guid == "");
                }
                // var settings = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.GetSettings();
                // var first = settings.WeaponOverrides.FirstOrDefault(b => b.AnimStyle == style.ToString() && b.Slot == slot.Value && b.MainOrOffHand == hand.Value);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
                throw new Exception(e.ToString());
            }
        }
        public WeaponOverrideVM()
        {
            base.AddDisposable(this.slot.Subscribe((int i) =>
            {
                var viewVM = SelectFromSettings();
                if (viewVM != null)
                {
                    this.m_ListViewVM?.Value?.TrySelectEntity(viewVM);
                }
                else this.m_ListViewVM?.Value?.TryUnselectEntity(this.m_ListViewVM.Value.SelectedEntity.Value);
            }));
            //base.AddDisposable(this.animStyle.Subscribe((WeaponAnimationStyle i) => { this.m_ListViewVM.Value.TrySelectEntity(SelectFromSettings()); }));
            base.AddDisposable(this.hand.Subscribe((bool i) =>
            {
                var viewVM = SelectFromSettings();
                if (viewVM != null)
                    this.m_ListViewVM?.Value?.TrySelectEntity(viewVM);
                else this.m_ListViewVM?.Value?.TryUnselectEntity(this.m_ListViewVM.Value.SelectedEntity.Value);
            }));
            OnWeaponTypeChanged(0);
        }
        public void OnWeaponTypeChanged(int i)
        {
            var CurrentReactive = new ReactiveCollection<ListViewItemVM>();
            if (i == 14)
            {
                CurrentReactive.Add(new ListViewItemVM("None", false, (ListViewItemVM v) => { AddOverride(v.Guid); }, false));
                CurrentReactive.Add(new ListViewItemVM("Hide", "Hide", false, (ListViewItemVM v) => { AddOverride(v.Guid); }, false));
                foreach (var ee in ResourceLoader.AllEnchants)
                {
                    //Main.Logger.Log(ee.name);
                    // var inf = ee.ToEEInfo();
                    if (!CurrentReactive.Any(a => a.Guid == ee.GUID))
                    {
                        CurrentReactive.Add(new ListViewItemVM(ee, false, (ListViewItemVM v) => { AddOverride(v.Guid); }, false));
                    }
                }
            }
            else
            {
                this.animStyle.Value = m_IntToAnim[i];
                CurrentReactive.Add(new ListViewItemVM("None", false, (ListViewItemVM v) => { AddOverride(v.Guid, m_IntToAnim[i]); }, false));
                foreach (var ee in ResourceLoader.AllWeapons[m_IntToAnim[i]])
                {
                    //Main.Logger.Log(ee.name);
                    // var inf = ee.ToEEInfo();
                    if (!CurrentReactive.Any(a => a.Guid == ee.GUID))
                    {
                        CurrentReactive.Add(new ListViewItemVM(ee, false, (ListViewItemVM v) => { AddOverride(v.Guid, m_IntToAnim[i]); }, false));
                    }
                }
            }
            m_ListViewVM.Value?.Dispose();
            if (i == 14)
            {
                var selected = CurrentReactive.FirstOrDefault(a => null != Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().EnchantOverrides.FirstOrDefault(x => (x.GUID == a.Guid) && x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value));
                selected = (selected != null ? selected : CurrentReactive.FirstOrDefault());
#if DEBUG
                Main.Logger.Log(selected.Guid + " " + selected.DisplayName);
#endif
                base.AddDisposable(m_ListViewVM.Value = new ListViewVM(CurrentReactive, new ReactiveProperty<ListViewItemVM>(selected)));
            }
            else
            {
                var selected = CurrentReactive.FirstOrDefault(a => null != Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.GetSettings().WeaponOverrides.FirstOrDefault(x => (x.GUID == a.Guid) && x.MainOrOffHand == hand.Value && x.Slot == this.slot.Value && x.AnimStyle == animStyle.Value.ToString()));
                selected = (selected != null ? selected : CurrentReactive.FirstOrDefault());
                base.AddDisposable(m_ListViewVM.Value = new ListViewVM(CurrentReactive, new ReactiveProperty<ListViewItemVM>(selected)));
            }
        }
        public override void DisposeImplementation()
        {

        }
    }
}
