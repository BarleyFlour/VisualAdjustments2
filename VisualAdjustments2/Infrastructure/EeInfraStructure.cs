using Kingmaker.Blueprints;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.View;
using Kingmaker.Visual.CharacterSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualAdjustments2.Infrastructure
{
    // TODO: proper settings system with caching so i dont have to de/serialize all the time



    [HarmonyLib.HarmonyPatch(typeof(UnitEntityData),nameof(UnitEntityData.OnViewDidAttach))]
    public static class UnitEntityData_CreateView_Patch
    {
        public static void Postfix(UnitEntityData __instance)
        {
            try
            {
                if (__instance.View?.CharacterAvatar != null && __instance.IsPlayerFaction)
                {
                    foreach (var action in __instance.GetSettings()?.EeSettings?.EEs)
                    {
                        //Main.Logger.Log($"Action of type {action.actionType}, Guid:{action.GUID}");
                        action.Apply(__instance.View.CharacterAvatar);
                    }
                }
            }
            catch (Exception e)
            { Main.Logger.Error(e.ToString());}
        }
    }
    public abstract class EEApplyAction
    {
        public EEApplyAction(string guid)
        {
            GUID = guid;
        }
        public string GUID;
        public abstract void Apply(UnitEntityData unitData,CharacterSettings settings);
    }
    public class AddEE : EEApplyAction
    {
        public EE_Applier.ColorInfo PrimaryCol;
        public EE_Applier.ColorInfo SecondaryCol;
        public AddEE(string guid) : base(guid)
        {
        }

        public override void Apply(UnitEntityData unitData, CharacterSettings settings)
        {
            var character = unitData.View.CharacterAvatar;
            var loadedEE = ResourcesLibrary.TryGetResource<EquipmentEntity>(GUID);
            if (!character.EquipmentEntities.Any(a => a.name == loadedEE.name)) character.AddEquipmentEntity(loadedEE);
            var applier = new EE_Applier(this.GUID, EE_Applier.ActionType.Add);
            applier.Primary = this.PrimaryCol;
            applier.Secondary = this.SecondaryCol;
            if (!settings.EeSettings.EEs.Any(a => a.GUID == this.GUID && a.actionType == EE_Applier.ActionType.Add)) settings.EeSettings.EEs.Add(applier);
            this.PrimaryCol.Apply(loadedEE,character);
            this.SecondaryCol.Apply(loadedEE, character);
        }
    }
    public class RemoveEE : EEApplyAction
    {
        public RemoveEE(string guid) : base(guid)
        {
        }

        public override void Apply(UnitEntityData unitData, CharacterSettings settings)
        {
            var character = unitData.View.CharacterAvatar;
            var loadedEE = ResourcesLibrary.TryGetResource<EquipmentEntity>(GUID);
            if (character.EquipmentEntities.Any(a => a.name == loadedEE.name)) character.RemoveEquipmentEntity(loadedEE);
            if(!settings.EeSettings.EEs.Any(a => a.GUID == this.GUID && a.actionType == EE_Applier.ActionType.Remove)) settings.EeSettings.EEs.Add(new EE_Applier(this.GUID, EE_Applier.ActionType.Remove));
        }
    }
    public class EeInfraStructure
    {
        
    }
}
