using System;
using System.Linq;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Utility;
using Kingmaker.Visual.Sound;
using Owlcat.Runtime.UI.MVVM;
using UniRx;
using VisualAdjustments2.Infrastructure;

namespace VisualAdjustments2.UI
{
    public class VoicePickerVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public bool supressVoiceLine = false;
        public void SetupSelected()
        {
            var unitBarks =  Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value.Descriptor.CustomAsks != null ? Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value.Descriptor.CustomAsks
                .AssetGuidThreadSafe : Game.Instance?.SelectionCharacter?.SelectedUnit?.Value.Value.Blueprint.Visual.Barks.AssetGuidThreadSafe;
            m_VoiceList.TrySelectEntity(m_VoiceList?.EntitiesCollection.First(a =>
                a.Guid == unitBarks));
            supressVoiceLine = false;
        }
        public VoicePickerVM()
        {
            
            var CurrentReactive = new ReactiveCollection<ListViewItemVM>();
            foreach (var VoiceBP in ResourceLoader.AllVoices)
            {
                //Main.Logger.Log(ee.name);
                if (!CurrentReactive.Any(a => a.Guid == VoiceBP.Key.AssetGuidThreadSafe))
                {
                    CurrentReactive.Add(new ListViewItemVM(VoiceBP.Value,VoiceBP.Key.AssetGuidThreadSafe,false, (a) =>
                    {
                        Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.Descriptor.CustomAsks = VoiceBP.Key;
                        Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.UpdateAsks();
                        if (!supressVoiceLine)
                        {
                            var component = VoiceBP.Key.GetComponent<UnitAsksComponent>();
                            if (component != null && component.PreviewSound != "")
                            {
                                component.PlayPreview();
                            }

                            if (component != null && component.Selected.HasBarks)
                            {
                                var bark = component.Selected.Entries.Random();
                                AkSoundEngine.PostEvent(bark.AkEvent,
                                    Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.View.gameObject);
                            }
                        }
                    },false));
                }
            }
            m_VoiceList = new ListViewVM(CurrentReactive,SelectedItem);
            //SetupSelected();
        }
        public override void DisposeImplementation()
        {
            
        }
        public ReactiveProperty<ListViewItemVM> SelectedItem = new(null);
        public ListViewVM m_VoiceList;
    }
}