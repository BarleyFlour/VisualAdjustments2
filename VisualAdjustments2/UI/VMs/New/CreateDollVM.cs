using Kingmaker;
using Kingmaker.UnitLogic;
using Owlcat.Runtime.UI.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owlcat.Runtime.UniRx;
using UniRx;
using Kingmaker.UnitLogic.Parts;

namespace VisualAdjustments2.UI
{
    public class CreateDollVM : BaseDisposable, IDisposable, IViewModel, IBaseDisposable
    {
        public ReactiveProperty<string> ToDisplay = new ReactiveProperty<string>();
        public string charname;
        public CreateDollVM()
        {
            base.AddDisposable(this);
            
            charname = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.CharacterName;
            ToDisplay.Value = $"{charname} has no doll, create?";

           /* base.AddDisposable(Game.Instance.SelectionCharacter.SelectedUnit.Subscribe((UnitDescriptor val) =>
            {
                if(val.CharacterName != charname)
                {
                    charname = val.CharacterName;
                    if(val.Unit.Get<UnitPartDollData>() != null)
                    {
                        this.Dispose(); onUnitChanged(); return;
                    }
                    ToDisplay.Value = $"{charname} has no doll, create?";
                }
            }));*/
        }
        public override void DisposeImplementation()
        {
        }
    }
}
