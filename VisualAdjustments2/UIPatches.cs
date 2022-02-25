using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.ResourceLinks;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Appearance;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.ServiceWindows;
using Kingmaker.UI.MVVM._PCView.ServiceWindows.Menu;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Appearance;
using Kingmaker.UI.MVVM._VM.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._VM.ServiceWindows;
using Kingmaker.UI.MVVM._VM.ServiceWindows.Menu;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Components;
using Kingmaker.Utility;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UI.SelectionGroup.View;
using Owlcat.Runtime.UniRx;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VisualAdjustments2.UI;

namespace VisualAdjustments2
{

    public enum Extended
    {
        Visual = 50
    };
    [HarmonyPatch(typeof(ServiceWindowsPCView), nameof(ServiceWindowsPCView.Initialize))]
    public static class ServiceWindowsPCView_Initialize_Patch
    {
        public static VisualWindowsMenuEntityPCView CreateButton(GameObject template, Transform parent, string label)
        {
            var newgameobject = UnityEngine.GameObject.Instantiate(template, parent);
            newgameobject.transform.name = label;
            var newcomp = newgameobject.AddComponent<VisualWindowsMenuEntityPCView>();
            var oldcomp = newgameobject.GetComponent<ServiceWindowsMenuEntityPCView>();
            newcomp.SetupFromServicePCView(oldcomp);
            UnityEngine.Component.Destroy(oldcomp);
            return newcomp;
        }
        public static void Prefix(ServiceWindowsPCView __instance)
        {
            try
            {


                //var currentchar = Kingmaker.UI.Common.UIUtility.GetCurrentCharacter();
                var currentchar = Kingmaker.Game.Instance.Player.AllCharacters.First();
                var doll = currentchar.GetDollState();
                // doll.SetupFromUnitLocal(currentchar);



                var detailedviewzone = Kingmaker.Game.Instance.UI.Canvas.transform.Find("ChargenPCView/ContentWrapper/DetailedViewZone");
                var newdollroom = UnityEngine.Object.Instantiate(detailedviewzone.Find("DollRoom"));
                var newappearance = UnityEngine.Object.Instantiate(detailedviewzone.Find("ChargenAppearanceDetailedPCView"));
                var newgameobject = new UnityEngine.GameObject("ParentThing");
                newgameobject.transform.SetParent(__instance.transform);
                newgameobject.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
                newgameobject.transform.localPosition = new UnityEngine.Vector3(0, 0, 0);

                // TODO: instantiate the FXViewer, EEPicker & Equipment PCViews




                newdollroom.SetParent(newgameobject.transform);
                newappearance.SetParent(newgameobject.transform);

                newappearance.localPosition = new UnityEngine.Vector3(0, 0, 0);
                newappearance.localScale = new UnityEngine.Vector3(1, 1, 1);

                newdollroom.localPosition = new UnityEngine.Vector3(0, 0, 0);
                newdollroom.localScale = new UnityEngine.Vector3(1, 1, 1);

                var appearancecomponent = newappearance.GetComponent<Kingmaker.UI.MVVM._PCView.CharGen.Phases.Appearance.CharGenAppearancePhaseDetailedPCView>();

                var newcomp = newappearance.gameObject.AddComponent<CharGenAppearancePhaseDetailedPCViewModified>();

                {
                    newcomp.m_BeardSelectorPcView = appearancecomponent.m_BeardSelectorPcView;
                    newcomp.m_BodyColorSelectorView = appearancecomponent.m_BodyColorSelectorView;
                    newcomp.m_BodySelectorPcView = appearancecomponent.m_BodySelectorPcView;
                    newcomp.m_CharacterController = appearancecomponent.m_CharacterController;
                    newcomp.m_ChooseBodyLabel = appearancecomponent.m_ChooseBodyLabel;
                    newcomp.m_ChooseHairLabel = appearancecomponent.m_ChooseHairLabel;
                    newcomp.m_ChooseHornsLabel = appearancecomponent.m_ChooseHornsLabel;
                    newcomp.m_ChoosePrimaryColorLabel = appearancecomponent.m_ChoosePrimaryColorLabel;
                    newcomp.m_ChooseSecondaryColorLabel = appearancecomponent.m_ChooseSecondaryColorLabel;
                    newcomp.m_ChooseTattoosLabel = appearancecomponent.m_ChooseTattoosLabel;
                    newcomp.m_ChooseWarpaintsLabel = appearancecomponent.m_ChooseWarpaintsLabel;
                    newcomp.m_EyesColorSelectorView = appearancecomponent.m_EyesColorSelectorView;
                    newcomp.m_FaceSelectorPcView = appearancecomponent.m_FaceSelectorPcView;
                    newcomp.m_HairBlock = appearancecomponent.m_HairBlock;
                    newcomp.m_HairBlockPlaceholder = appearancecomponent.m_HairBlockPlaceholder;
                    newcomp.m_HairColorSelectorView = appearancecomponent.m_HairColorSelectorView;
                    newcomp.m_HairSelectorPcView = appearancecomponent.m_HairSelectorPcView;
                    newcomp.m_HornBlock = appearancecomponent.m_HornBlock;
                    newcomp.m_HornColorSelectorView = appearancecomponent.m_HornColorSelectorView;
                    newcomp.m_HornSelectorPcView = appearancecomponent.m_HornSelectorPcView;
                    newcomp.m_LeftAnimator = appearancecomponent.m_LeftAnimator;
                    newcomp.m_PageAnimator = appearancecomponent.m_PageAnimator;
                    newcomp.m_PrimaryOutfitColorSelectorView = appearancecomponent.m_PrimaryOutfitColorSelectorView;
                    newcomp.m_RectTransform = appearancecomponent.m_RectTransform;
                    newcomp.m_RightAnimator = appearancecomponent.m_RightAnimator;
                    newcomp.m_ScarSelectorPcView = appearancecomponent.m_ScarSelectorPcView;
                    newcomp.m_SecondaryOutfitColorSelectorView = appearancecomponent.m_SecondaryOutfitColorSelectorView;
                    newcomp.m_ShowRequest = appearancecomponent.m_ShowRequest;
                    newcomp.m_TargetSizeInfoDollTransform = appearancecomponent.m_TargetSizeInfoDollTransform;
                    newcomp.m_TatooColorSelectorView = appearancecomponent.m_TatooColorSelectorView;
                    newcomp.m_TatooPaginator = appearancecomponent.m_TatooPaginator;
                    newcomp.m_TatooSelectorPcView = appearancecomponent.m_TatooSelectorPcView;
                    newcomp.m_WarpaintBlock = appearancecomponent.m_WarpaintBlock;
                    newcomp.m_WarpaintColorSelectorView = appearancecomponent.m_WarpaintColorSelectorView;
                    newcomp.m_WarpaintPaginator = appearancecomponent.m_WarpaintPaginator;
                    newcomp.m_WarpaintSelectorPcView = appearancecomponent.m_WarpaintSelectorPcView;
                    newcomp.VisualSettings = appearancecomponent.VisualSettings;
                }
                //Instantiate new things
                {
                    var RaceSelector = UnityEngine.Object.Instantiate(newappearance.Find("AppearanceBlock/LeftBlock/Body/SelectorsPlace/PC_Body_SlideSequentionalSelector (1)"));
                    RaceSelector.SetParent(newappearance.Find("AppearanceBlock/LeftBlock/Body/SelectorsPlace"));
                    RaceSelector.SetSiblingIndex(2);
                    RaceSelector.localScale = new Vector3(1, 1, 1);
                    var comp = RaceSelector.GetComponent<SlideSelectorPCView>();
                    newcomp.m_RaceSelectorPCView = comp;


                    // comp.BindViewImplementation();
                }
                // Main.Logger.Log(currentchar.CharacterName);

                var lvl = new LevelUpController(currentchar, false, LevelUpState.CharBuildMode.SetName);
                //var viewmodel = new CharGenAppearancePhaseVMModified(lvl, doll, false);
                //newcomp.Bind(viewmodel);
                //newcomp.BindViewImplementation();
                UnityEngine.Component.Destroy(appearancecomponent);
                UnityEngine.Object.Destroy(newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/ArtDollRoom").gameObject);

                newcomp.Initialize();
                newgameobject.SetActive(false);

                //Dollroom
                {
                    CharGenAppearancePhaseVMModified.charController = newgameobject.transform.Find("DollRoom(Clone)").GetComponent<DollCharacterController>();
                    CharGenAppearancePhaseVMModified.pcview = __instance.transform.Find("ParentThing/ChargenAppearanceDetailedPCView(Clone)").GetComponent<CharGenAppearancePhaseDetailedPCViewModified>();
                }
                //New Service Window Button
                {
                    var a = __instance.transform.Find("ServiceWindowMenuPCView/Top/Map");

                    var newbutton = UnityEngine.Object.Instantiate(a);
                    newbutton.SetParent(__instance.transform.Find("ServiceWindowMenuPCView/Top").transform);
                    newbutton.transform.localScale = new UnityEngine.Vector3(1, 1, 1);

                    __instance.transform.Find("ServiceWindowMenuPCView/Top/Close").SetAsLastSibling();
                    var label = newbutton.Find("Label");
                    var labelcomp = label.GetComponent<TextMeshProUGUI>();
                    labelcomp.text = "Visual";
                    var component = newbutton.gameObject.GetComponent<ServiceWindowsMenuEntityPCView>();

                    //Button stuff
                    {

                    }
                    var thing = __instance.transform.Find("ServiceWindowMenuPCView/Top").gameObject.GetComponent<HorizontalLayoutGroupWorkaround>();
                    Traverse.Create(thing).Field<List<RectTransform>>("m_RectChildren").Value.Add(newbutton.GetComponent<RectTransform>());
                    newbutton.gameObject.SetActive(true);
                    a.GetComponent<LayoutElement>().minWidth = 125;
                    a.Find("Separator").gameObject.SetActive(true);
                    newbutton.GetComponent<LayoutElement>().minWidth = 200;
                    //New selection bar thingie
                    //Main.Logger.Log((string)(__instance.transform.Find("ServiceWindowMenuPCView").ToString()));
                    var oldbar = __instance.transform.Find("ServiceWindowMenuPCView");
                    var newselectionbar = UnityEngine.Object.Instantiate(oldbar);
                    newselectionbar.SetParent(__instance.transform);
                    var top = newselectionbar.Find("Top");
                    UnityEngine.Object.Destroy(top.Find("Mythic").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Spellbook").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Journal").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Encyclopedia").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Map").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Map(Clone)").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Close").gameObject);
                    UnityEngine.Object.Destroy(top.Find("Character").gameObject);
                    var inventory = top.Find("Inventory").gameObject;

                    var FXViewerButton = CreateButton(inventory, top, "FX Viewer");
                    var DollButton = CreateButton(inventory, top, "Doll");
                    var EquipmentButton = CreateButton(inventory, top, "Equipment");
                    var EEPickerButton = CreateButton(inventory, top, "EE Picker");


                    UnityEngine.Object.Destroy(inventory);

                    var oldpcview = newselectionbar.gameObject.GetComponent<ServiceWindowMenuPCView>();

                    var comp = newselectionbar.gameObject.AddComponent<ServiceWindowMenuPCViewModified>();

                    comp.m_Animator = oldpcview.m_Animator;
                    comp.m_BindDisposable = oldpcview.m_BindDisposable;
                    UnityEngine.Component.Destroy(oldpcview);
                    var newmenuselector = top.gameObject.AddComponent<ServiceWindowMenuSelectorPCViewModified>();
                    UnityEngine.Component.Destroy(top.gameObject.GetComponent<ServiceWindowMenuSelectorPCView>());
                    newmenuselector.m_MenuEntities = new List<VisualWindowsMenuEntityPCView>();
                    newmenuselector.m_BindDisposable = new List<IDisposable>();
                    comp.m_MenuSelector = newmenuselector;

                    //comp.m_MenuSelector?.m_BindDisposable?.Clear();
                    //comp.m_MenuSelector?.m_MenuEntities?.Clear();

                    comp.m_MenuSelector.m_MenuEntities.Add(FXViewerButton);
                    comp.m_MenuSelector.m_MenuEntities.Add(EEPickerButton);
                    comp.m_MenuSelector.m_MenuEntities.Add(EquipmentButton);
                    comp.m_MenuSelector.m_MenuEntities.Add(DollButton);
                    ServiceWindowsVM_ShowWindow_Patch.pcview = comp;


                    newselectionbar.transform.localPosition = (oldbar.transform.localPosition) + (new Vector3(0, -50, 0));

                    var gameobject = new GameObject("FXViewer");
                    gameobject.transform.SetParent(newgameobject.transform);
                    var FXViewerPCView = gameobject.AddComponent<FXViewerPCView>();

                    var gameobject2 = new GameObject("EEPicker");
                    gameobject.transform.SetParent(newgameobject.transform);
                    var EEPickerPCView = gameobject.AddComponent<EEPickerPCView>();

                    var gameobject3 = new GameObject("Equipment");
                    gameobject.transform.SetParent(newgameobject.transform);
                    var EquipmentPCView = gameobject.AddComponent<EquipmentPCView>();
                    newselectionbar.transform.localScale = oldbar.transform.localScale;
                    //Add visual adjustments Window PCView
                    {
                        var oldcomp = newgameobject.transform.parent.GetComponent<ServiceWindowsPCView>();
                        var compPCView = newgameobject.AddComponent<ServiceWindowsPCViewModified>();
                        compPCView.m_Background = oldcomp.m_Background;
                        compPCView.m_ServiceWindowMenuPcView = comp;
                        compPCView.m_DollPCView = newcomp;
                        compPCView.m_EEPickerPCView = EEPickerPCView;
                        compPCView.m_EquipmentPCView = EquipmentPCView;
                        compPCView.m_FXViewerPCView = FXViewerPCView;
                        ServiceWindowsVM_ShowWindow_Patch.swPCView = compPCView;
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }

    [HarmonyPatch(typeof(ServiceWindowsMenuVM), nameof(ServiceWindowsMenuVM.CreateEntities))]
    ///<summary>
    ///Makes the VM.
    /// </summary>
    public static class ServiceWindowsMenuVM_CreateEntities_Patch
    {
        public static void Postfix(ServiceWindowsMenuVM __instance)
        {
            try
            {
                ServiceWindowsMenuEntityVM windowsMenuEntityVm = new ServiceWindowsMenuEntityVM((ServiceWindowsType)Extended.Visual);
                __instance.AddDisposable(windowsMenuEntityVm);
                if (!__instance?.m_EntitiesList?.Contains(windowsMenuEntityVm) == true) __instance?.m_EntitiesList?.Add(windowsMenuEntityVm);
                //if(__instance.SelectionGroup.EntitiesCollection.Contains(windowsMenuEntityVm) == false) __instance.SelectionGroup.EntitiesCollection.Add(windowsMenuEntityVm);

                //var component = newbutton.gameObject.GetComponent<ServiceWindowsMenuEntityPCView>();
                //var newVM = new ServiceWindowsMenuEntityVM((ServiceWindowsType)Extended.Visual);
                //var newVM = __instance.ViewModel?.
                //if (!__instance.m_MenuEntities.Contains(component)) __instance.m_MenuEntities.Add(component);
                // __instance.m_MenuEntities.FirstOrDefault(a => a.name == component.name)?.Bind(newVM);
                // __instance.transform.parent.GetComponent<ServiceWindowMenuPCView>().ViewModel.SelectionGroup.EntitiesCollection.Add(newVM);
                //newVM.RefreshView.Subscribe(new Action(() =>
                //{
                //component.OnChangeSelectedState(!component.ViewModel.IsSelected.Value);
                //}));
                //component.ViewModel.HasView = true;
                //component.BindViewImplementation();*/
                //__instance.SelectionGroup.SubscribeNewItem(windowsMenuEntityVm);
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
    [HarmonyPatch(typeof(ServiceWindowMenuSelectorPCView), nameof(ServiceWindowMenuSelectorPCView.BindViewImplementation))]
    public static class ServiceWindowMenuSelectorPCView_BindViewImplementation_Patch
    {
        // public static void Prefix(ServiceWindowMenuSelectorPCView __instance)
        //{
        //   var vm = __instance.
        //  __instance.ViewModel.EntitiesCollection.Add(__instance.)
        //}
        public static void Postfix(ServiceWindowMenuSelectorPCView __instance)
        {
            var newbutton = __instance.transform.Find("Map(Clone)");
            newbutton.gameObject.SetActive(true);
            try
            {
                var component = newbutton.gameObject.GetComponent<ServiceWindowsMenuEntityPCView>();
                //var newVM = new ServiceWindowsMenuEntityVM((ServiceWindowsType)Extended.Visual);
                var newVM = __instance.transform.parent.GetComponent<ServiceWindowMenuPCView>().ViewModel.m_EntitiesList.Last();
                if (!__instance.m_MenuEntities.Contains(component)) __instance.m_MenuEntities.Add(component);
                __instance.m_MenuEntities.FirstOrDefault(a => a.name == component.name)?.Bind(newVM);
                __instance.transform.parent.GetComponent<ServiceWindowMenuPCView>().ViewModel.SelectionGroup.EntitiesCollection.Add(newVM);
                //newVM.RefreshView.Subscribe(new Action(() =>
                //{
                //component.OnChangeSelectedState(!component.ViewModel.IsSelected.Value);
                //}));
                component.ViewModel.HasView = true;
                //component.BindViewImplementation();

            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
    //Gives it its proper Label instead of blank.
    [HarmonyPatch(typeof(UIUtility), nameof(UIUtility.GetServiceWindowsLabel))]
    public static class UIUtility_GetServiceWindowsLabel_Patch
    {
        public static void Postfix(ServiceWindowsType type, ref string __result)
        {
            try
            {
                if ((int)type == 50) __result = "Visual";
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
    [HarmonyPatch(typeof(ServiceWindowsVM), nameof(ServiceWindowsVM.OnSelectWindow))]
    public static class ServiceWindowsVM_ShowWindow_Patch
    {
        public static ServiceWindowsPCViewModified swPCView;
        public static ServiceWindowMenuPCViewModified pcview;
        public static void Postfix(ServiceWindowsVM __instance, ServiceWindowsType type)
        {
            try
            {
                //Main.Logger.Log("stufforsomething");
                if ((int)type == 50)
                {

                    // TODO: bind the new selection bar
                    {


                        // if (pcview == null || swPCView == null) throw new NullReferenceException("[HarmonyPatch(typeof(ServiceWindowsVM), nameof(ServiceWindowsVM.OnSelectWindow))]");
                        
                        var swVM = new ServiceWindowsVMModified();
                        if (swPCView != null) swPCView.Bind(swVM);

                        var vm = new ServiceWindowsMenuVMModified(swVM.OnSelectWindow);
                        pcview.Bind(vm);
                        swPCView.Bind(swVM);

                        __instance.OnDispose += () => { vm.Dispose(); swVM.Dispose(); };


                        swVM.AddDisposable(swVM.ServiceWindowsMenuVM.Value = vm);
                    }
                    //Old stuff for direct to doll page
                    /*
                    var unit = UIUtility.GetCurrentCharacter();
                    var doll = unit.GetDollState();
                    if (doll.Race != null)
                    {
                        var lvlcontroller = new LevelUpController(unit, false, LevelUpState.CharBuildMode.SetName);
                        lvlcontroller.Doll = doll;
                        //doll.SetupFromUnitLocal(unit);

                        CharGenAppearancePhaseVMModified.pcview.Unbind();
                        CharGenAppearancePhaseVMModified.pcview.Bind(new CharGenAppearancePhaseVMModified(lvlcontroller, doll, false));
                        CharGenAppearancePhaseVMModified.pcview.ViewModel.RefreshView.Subscribe(() => { CharGenAppearancePhaseVMModified.pcview?.ViewModel?.Change(); });
                        //viewModified.BindViewImplementation();
                        CharGenAppearancePhaseVMModified.pcview.gameObject.SetActive(true);
                        CharGenAppearancePhaseVMModified.pcview.transform.parent.gameObject.SetActive(true);
                        //viewModified.transform.parent.Find("DollRoom(Clone)").gameObject.SetActive(true);
                        Main.Logger.Log("didstuff");
                        //   __instance.AddDisposable(__instance.InventoryVM.Value = new InventoryVM(InventoryType.Stash));
                    }*/
                    return;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
    [HarmonyPatch(typeof(ServiceWindowsVM), nameof(ServiceWindowsVM.HideWindow))]
    public static class ServiceWindowsVM_HideWindow_Patch
    {
        public static void Postfix(ServiceWindowsVM __instance, ServiceWindowsType type)
        {
            try
            {
                if ((int)type == 50)
                {
                    ServiceWindowsVM_ShowWindow_Patch.pcview?.ViewModel?.Dispose();
                    ServiceWindowsVM_ShowWindow_Patch.swPCView?.ViewModel?.Dispose();
                    CharGenAppearancePhaseVMModified.charController?.Unbind();
                    return;
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }
    }
}
