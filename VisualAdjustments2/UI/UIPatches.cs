using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.ResourceLinks;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Appearance;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Common;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.FeatureSelector;
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
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Utility;
using Kingmaker.View.Animation;
using Owlcat.Runtime.Core.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.SelectionGroup;
using Owlcat.Runtime.UI.SelectionGroup.View;
using Owlcat.Runtime.UI.VirtualListSystem.Vertical;
using Owlcat.Runtime.UniRx;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VisualAdjustments2.Infrastructure;
using VisualAdjustments2.UI;

namespace VisualAdjustments2
{
    public enum HideButtonType
    {
        Armor = 2,
        Feet = 5,
        Glasses = 14,
        Gloves = 6,
        Head = 7,
        Shirt = 15,
        Shoulders = 9,
        Wrist = 10,
        Usable = 11, //Belt items
        Weapon = 0,
        Enchantments
    };
    public enum Extended
    {
        Visual = 50
    };
    [HarmonyPatch(typeof(ServiceWindowsPCView), nameof(ServiceWindowsPCView.Initialize))]
    public static class ServiceWindowsPCView_Initialize_Patch
    {
        public static List<(string,string)> m_Classes;
        public static List<(string,string)> Classes
        {
            get
            {
#if DEBUG
                var sw = Stopwatch.StartNew();
#endif
                if (m_Classes == null)
                {
                    m_Classes = new List<(string,string)>();
                    m_Classes.Add(("Default",""));
                    var bps = Kingmaker.Cheats.Utilities.GetAllBlueprints();
                    Main.Logger.Log(bps.Entries.Where(a => a.Type == typeof(BlueprintCharacterClass)).Count() + " asda");
                    foreach (BlueprintCharacterClass c in bps.Entries.Where(a => a.m_Type == typeof(BlueprintCharacterClass)).Select(b => ResourcesLibrary.TryGetBlueprint<BlueprintCharacterClass>(b.Guid)))
                    {
                        Main.Logger.Log(c.NameForAcronym);
                        if (c.StartingItems.Any() && !c.PrestigeClass && !c.ToString().Contains("Scion"))
                        {
                            m_Classes.Add((c.LocalizedName,c.AssetGuidThreadSafe));
                        }
                    }
                }
#if DEBUG
                sw.Stop();
                if(sw.ElapsedMilliseconds > 0)Main.Logger.Log($"Classes gotten in {sw.ElapsedMilliseconds}ms");
#endif
                return m_Classes;
            }
        }
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
                    //newcomp.m_PageAnimator = appearancecomponent.m_PageAnimator;
                    newcomp.m_PrimaryOutfitColorSelectorView = appearancecomponent.m_PrimaryOutfitColorSelectorView;
                    // newcomp.m_RectTransform = appearancecomponent.m_RectTransform;
                    newcomp.m_RightAnimator = appearancecomponent.m_RightAnimator;
                    newcomp.m_ScarSelectorPcView = appearancecomponent.m_ScarSelectorPcView;
                    newcomp.m_SecondaryOutfitColorSelectorView = appearancecomponent.m_SecondaryOutfitColorSelectorView;
                    //newcomp.m_ShowRequest = appearancecomponent.m_ShowRequest;
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

                    //Apply button
                    var ApplyButtonGameObject = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage"), newcomp.transform);
                    ApplyButtonGameObject.localPosition = new Vector3(-195, -398, 0);
                    ApplyButtonGameObject.Find("Button/FinneanLabel").gameObject.SetActive(false);
                    ApplyButtonGameObject.Find("Button/StashLabel").GetComponent<TextMeshProUGUI>().text = "Apply";
                    var owlbutt = ApplyButtonGameObject.Find("Button").GetComponent<OwlcatButton>();
                    owlbutt.OnLeftClick.AddListener(() =>
                    {
                        Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit?.SaveDollState(newcomp.ViewModel.DollState);
                    });
                    newcomp.m_ApplyButton = owlbutt;
                    // comp.BindViewImplementation();
                }

                // Main.Logger.Log(currentchar.CharacterName);

                //var lvl = new LevelUpController(currentchar, false, LevelUpState.CharBuildMode.SetName);
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

                    var dollroomcomp = newdollroom.GetComponent<DollCharacterController>();
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

                    var FXViewerButton = CreateButton(inventory, top, "Buff Viewer");
                    var DollButton = CreateButton(inventory, top, "Doll");
                    var EquipmentButton = CreateButton(inventory, top, "Equipment");
                    var EEPickerButton = CreateButton(inventory, top, "EE Picker");


                    UnityEngine.Object.Destroy(inventory);

                    var windowcontainer = newgameobject.transform.Find("DollRoom(Clone)/CharacterVisualSettingsView/WindowContainer");
                    windowcontainer.localPosition = new Vector3(-267, 169, 0);

                    var oldpcview = newselectionbar.gameObject.GetComponent<ServiceWindowMenuPCView>();

                    var comp = newselectionbar.gameObject.AddComponent<ServiceWindowMenuPCViewModified>();

                    var NewButton = GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage"), newgameobject.transform);
                    NewButton.localScale = new Vector3((float)1.5, (float)1.5, 1);

                    var cmp = NewButton.gameObject.AddComponent<CreateDollPCView>();
                    cmp.Button = NewButton.Find("Button").GetComponent<OwlcatButton>();
                    cmp.Button.OnLeftClick.AddListener(() => { });
                    cmp.Label = NewButton.Find("Button/StashLabel").GetComponent<TextMeshProUGUI>();
                    Component.Destroy(NewButton.GetComponent<Image>());
                    NewButton.Find("Button/FinneanLabel").gameObject.SetActive(false);
                    NewButton.gameObject.SetActive(false);

                    var NewButton2 = GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage"), newcomp.transform);
                    //NewButton2.localScale = new Vector3((float)1.5, (float)1.5, 1);
                    NewButton2.localPosition = new Vector3(58, -398, 0);
                    newcomp.m_DeleteDollButton = NewButton2.Find("Button").GetComponent<OwlcatButton>();
                    NewButton2.Find("Button/FinneanLabel").gameObject.SetActive(false);
                    NewButton2.gameObject.SetActive(false);
                    NewButton2.Find("Button/StashLabel").GetComponent<TextMeshProUGUI>().text = "Delete Doll";

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
                    gameobject.transform.localPosition = new Vector3(0, 0, 0);
                    gameobject.transform.localScale = new Vector3(1, 1, 1);
                    gameobject.SetActive(false);
                    {
                        FXViewerPCView.m_VisualSettings = newgameobject.transform.Find("DollRoom(Clone)/CharacterVisualSettingsView").GetComponent<CharacterVisualSettingsView>();
                        FXViewerPCView.m_dollCharacterController = dollroomcomp;
                        //Current FXs List
                        {
                            var alleelistview = GameObject.Instantiate(gameobject.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, gameobject.transform);
                            alleelistview.transform.localPosition = new Vector3(-650, -50, 0);
                            var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                            var newcompl = alleelistview.AddComponent<BuffListPCView>();
                            newcompl.SetupFromChargenList(oldcomp, false, "Current Buffs");
                            UnityEngine.Component.Destroy(oldcomp);
                            {
                                // var SelectedTransform = newgameobject.transform.parent.Find("InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/All/Selected");

                                var PrimSec = new GameObject("ModeHandler");
                                var togglegroup = PrimSec.AddComponent<ToggleWithTextHandler>();
                                PrimSec.transform.SetParent(alleelistview.transform);
                                PrimSec.transform.localScale = new Vector3(1, 1, 1);
                                // var layoutelement = PrimSec.AddComponent<LayoutElement>();
                                //layoutelement.m_FlexibleHeight = 5;
                                // layoutelement.m_MinHeight = 5;
                                //layoutelement.m_PreferredHeight = 5;



                                var horizontalLayoutGroup = PrimSec.AddComponent<HorizontalLayoutGroup>();
                                horizontalLayoutGroup.childForceExpandHeight = false;
                                horizontalLayoutGroup.childScaleHeight = false;
                                horizontalLayoutGroup.padding.left = 5;
                                horizontalLayoutGroup.padding.right = 12;
                                horizontalLayoutGroup.spacing = 5;
                                horizontalLayoutGroup.childForceExpandWidth = false;
                                horizontalLayoutGroup.childAlignment = TextAnchor.MiddleLeft;




                                var PrimButton = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), PrimSec.transform);

                                var LabelGameObject = UnityEngine.GameObject.Instantiate(newgameobject.transform.Find("FXViewer/FeatureSelectorView(Clone)/HeaderH2/Label").gameObject);
                                LabelGameObject.transform.SetParent(PrimSec.transform);
                                LabelGameObject.transform.localScale = new Vector3(1, 1, 1);

                                //togglegroup.m_PrimarySelected = UnityEngine.Object.Instantiate(SelectedTransform, PrimButton.transform).gameObject;
                                //UnityEngine.Component.Destroy(togglegroup.m_PrimarySelected.transform.GetComponent<CanvasGroup>());
                                //UnityEngine.Component.Destroy(togglegroup.m_PrimarySelected.transform.GetComponent<Image>());
                                var LeftButtonElement = PrimButton.EnsureComponent<LayoutElement>();
                                LeftButtonElement.minHeight = 30;
                                LeftButtonElement.preferredWidth = 155;

                                var buttonTextComponent = PrimButton.Find("StashLabel").GetComponent<TextMeshProUGUI>();

                                LabelGameObject.AddComponent<LayoutElement>();
                                PrimButton.Find("FinneanLabel").gameObject.SetActive(false);

                                // var txt = LabelGameObject.AddComponent<TextMeshProUGUI>();
                                //txt.text = "$";

                                LabelGameObject.transform.SetAsLastSibling();
                                //PrimButton.gameObject.AddComponent<LayoutElement>();
                                buttonTextComponent.text = "Toggle Mode";
                                togglegroup.Setup(PrimButton.GetComponent<OwlcatButton>(), LabelGameObject.GetComponent<TextMeshProUGUI>());
                                FXViewerPCView.m_WhiteOrBlacklist = togglegroup;
                                togglegroup.transform.SetSiblingIndex(1);
                            }
                            FXViewerPCView.m_CurrentFX = newcompl;
                        }
                        //All FXs list
                        {
                            var alleelistview = GameObject.Instantiate(gameobject.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, gameobject.transform);
                            alleelistview.transform.localPosition = new Vector3(650, -50, 0);
                            var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                            var newcompl = alleelistview.AddComponent<BuffListPCView>();
                            newcompl.SetupFromChargenList(oldcomp, true, "All Buffs");
                            UnityEngine.Component.Destroy(oldcomp);
                            FXViewerPCView.m_AllFX = newcompl;
                            newcompl.VirtualList.m_ScrollSettings.ScrollWheelSpeed = 333;
                        }
                    }

                    EEPickerPCView EEPickerPCView = CreateEEPicker(newgameobject, dollroomcomp);
                    EEPickerPCView.m_VisualSettings = newgameobject.transform.Find("DollRoom(Clone)/CharacterVisualSettingsView").GetComponent<CharacterVisualSettingsView>();
                    //EEPickerPCView.m_VisualSettings = newcomp.VisualSettings;

                    var gameobject3 = new GameObject("Equipment");
                    gameobject3.transform.SetParent(newgameobject.transform);
                    var EquipmentPCView = gameobject3.AddComponent<EquipmentPCView>();
                    gameobject3.transform.localPosition = new Vector3(0, 0, 0);
                    gameobject3.transform.localScale = new Vector3(1, 1, 1);
                    gameobject3.SetActive(false);


                    //Equipment
                    {
                        {
                            EquipmentPCView.m_VisualSettings = newgameobject.transform.Find("DollRoom(Clone)/CharacterVisualSettingsView").GetComponent<CharacterVisualSettingsView>();
                            EquipmentPCView.m_dollCharacterController = dollroomcomp;
                        }

                        var alleelistview = GameObject.Instantiate(gameobject3.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, gameobject3.transform);

                        // alleelistview.transform.Find("StandardScrollView/Viewport").GetComponent<VerticalLayoutGroup>().childScaleWidth = false;
                        alleelistview.transform.localPosition = new Vector3(650, -50, 0);
                        alleelistview.transform.localScale = new Vector3(1, 1, 1);
                        var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                        var newcompl = alleelistview.AddComponent<EquipmentListPCView>();

                        newcompl.SetupFromChargenList(oldcomp, false, "Equipment");
                        ((VirtualListLayoutSettingsVertical)newcompl.VirtualList.LayoutSettings).Width = 530;
                        //newcompl.VirtualList.m_ScrollSettings.ScrollWheelSpeed = 500;
                        UnityEngine.Component.Destroy(oldcomp);
                        var windowLayout = newcompl.transform.Find("StandardScrollView").gameObject.AddComponent<LayoutElement>();
                        windowLayout.minHeight = 585;
                        windowLayout.minWidth = 555;
                        newcompl.m_CharGenFeatureSearchView.transform.Find("FeatureSearchView").GetComponent<LayoutElement>().minHeight = 56;

                        var header = alleelistview.transform.Find("HeaderH2");
                        header.GetComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
                        var newDropDown = GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/Sorting/Dropdown"), header);
                        var layout = newDropDown.gameObject.AddComponent<LayoutElement>();
                        layout.minWidth = 400;
                        layout.minHeight = 35;
                        header.Find("Label").gameObject.SetActive(false);
                        var dropdown = newDropDown.GetComponent<TMP_DropdownWorkaround>();
                        //Manual list of appropriate Animstyles and Indices so we dont have empety/unused ones.
                        Dictionary<int, WeaponAnimationStyle> m_IntToAnim = new Dictionary<int, WeaponAnimationStyle>();
                        Dictionary<WeaponAnimationStyle, int> m_AnimToInt = new Dictionary<WeaponAnimationStyle, int>();
                        var enumList = new Dictionary<WeaponAnimationStyle, string>()
                        {
                            [WeaponAnimationStyle.Bow] = "Bows",
                            [WeaponAnimationStyle.Crossbow] = "Crossbows",
                            [WeaponAnimationStyle.Dagger] = "Daggers",
                            [WeaponAnimationStyle.ThrownStraight] = "Dart/Javelin",
                            [WeaponAnimationStyle.Double] = "Double Bladed/Quarterstaves",
                            [WeaponAnimationStyle.Fencing] = "Fencing Blades",
                            [WeaponAnimationStyle.PiercingOneHanded] = "One-Handed Polearms",
                            [WeaponAnimationStyle.SlashingOneHanded] = "One-Handed Slashing/Blunt Weapons",
                            [WeaponAnimationStyle.Fist] = "Punching Daggers",
                            [WeaponAnimationStyle.Shield] = "Shields",
                            [WeaponAnimationStyle.ThrownArc] = "Throwing Axes",
                            [WeaponAnimationStyle.AxeTwoHanded] = "Two Handed Axes",
                            [WeaponAnimationStyle.PiercingTwoHanded] = "Two-Handed Polearms",
                            [WeaponAnimationStyle.SlashingTwoHanded] = "Two-Handed Slashing/Blunt Weapons"
                        };
                        for (int i = 0; i < enumList.Count(); i++)
                        {
                            var anim = enumList.ElementAt(i);
                            m_IntToAnim[i] = anim.Key;
                            m_AnimToInt[anim.Key] = i;
                            dropdown.options.Add(new TMP_Dropdown.OptionData(anim.Value));
                        }
                        dropdown.options.Add(new TMP_Dropdown.OptionData("FX"));
                        /*foreach (var animstyle in enumList)
                        {
                            m_IntToAnim[]
                            dropdown.options.Add(new TMP_Dropdown.OptionData(animstyle.Value));
                        }*/

                        //dropdown

                        EquipmentPCView.m_ListPCView = newcompl;
                        //Right block
                        {
                            var dollRightBlock = newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock");
                            var rightBlock = new GameObject("RightBlock");
                            var weaponOverridePCView = rightBlock.AddComponent<WeaponOverridePCView>();
                            //rightBlock.transform.localScale = new Vector3(1, 1, 1);
                            weaponOverridePCView.m_DropDown = dropdown;
                            weaponOverridePCView.m_ListPCView = newcompl;
                            WeaponOverrideVM.m_IntToAnim = m_IntToAnim;
                            WeaponOverrideVM.m_AnimToInt = m_AnimToInt;


                            EquipmentPCView.m_weaponOverridePCView = weaponOverridePCView;

                            rightBlock.transform.SetParent(EquipmentPCView.transform);
                            rightBlock.transform.localScale = new Vector3(1, 1, 1);
                            var vertlayout = rightBlock.AddComponent<VerticalLayoutGroupWorkaround>(dollRightBlock.GetComponent<VerticalLayoutGroupWorkaround>());
                            vertlayout.m_TotalMinSize = new Vector2(570, 570);
                            //var layoutElement = rightBlock.AddComponent<LayoutElement>();
                            //layoutElement.minWidth = 150;
                            var rightBlockRectTransform = rightBlock.GetComponent<RectTransform>();
                            var oldRightBlockRectTransform = dollRightBlock.GetComponent<RectTransform>();
                            rightBlockRectTransform.anchoredPosition = oldRightBlockRectTransform.anchoredPosition;
                            rightBlockRectTransform.anchoredPosition3D = oldRightBlockRectTransform.anchoredPosition3D;
                            rightBlockRectTransform.anchorMax = oldRightBlockRectTransform.anchorMax;
                            rightBlockRectTransform.anchorMin = oldRightBlockRectTransform.anchorMin;
                            //rightBlockRectTransform.localPosition = new Vector3(655,0,0);
                            rightBlock.transform.localPosition = new Vector3(622, 360, rightBlock.transform.localPosition.z);
                            var dollPageTemplate = newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock/FaceWarpaint");
                            var SlotSelector = GameObject.Instantiate(dollPageTemplate.gameObject, rightBlock.transform);
                            SlotSelector.GetComponent<VerticalLayoutGroupWorkaround>().padding.right = 3;

                            UnityEngine.Object.Destroy(SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_SlideSequentionalSelector (1)").gameObject);
                            var page = SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_TextureSelector");
                            var clickablePageNav = SlotSelector.transform.Find("HeaderH2");
                            SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_TextureSelector/PalettePlace").gameObject.SetActive(false);
                            clickablePageNav.transform.SetParent(page);
                            var lbl = clickablePageNav.Find("Label").GetComponent<TextMeshProUGUI>();
                            lbl.text = "Weapon Set";
                            lbl.verticalAlignment = VerticalAlignmentOptions.Geometry;
                            clickablePageNav.localScale = new Vector3(1, 1, 1);
                            clickablePageNav.GetComponent<LayoutElement>().minWidth = 545;
                            var clickableComponent = clickablePageNav.Find("ClickablePageNavigation").GetComponent<ClickablePageNavigation>();
                            clickableComponent.FillPoints(4);

                            clickableComponent.m_ChooseCallback = (i) =>
                            {
                                if (weaponOverridePCView.ViewModel != null)
                                {
                                    weaponOverridePCView.ViewModel.slot.Value = i;
                                    Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.Body.CurrentHandEquipmentSetIndex = i;
                                    var animstyle = Game.Instance.SelectionCharacter.SelectedUnit.Value?.Unit?.View?.HandsEquipment?.m_ActiveSet?.MainHand?.VisibleItemBlueprint?.VisualParameters?.AnimStyle;
                                    if (animstyle != null) weaponOverridePCView.m_DropDown.value = WeaponOverrideVM.m_AnimToInt[(WeaponAnimationStyle)animstyle];
                                }

                                //EquipmentPCView.ViewModel;
                            };
                            clickableComponent.m_Points?.FirstOrDefault()?.OnClick();

                            var vertlayoutclickable = clickablePageNav.GetComponent<HorizontalLayoutGroup>();



                            //vertlayoutclickable.padding.top = 8;

                            // UnityEngine.Object.Destroy(SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_TextureSelector").gameObject);


                            alleelistview.transform.SetParent(rightBlock.transform);
                            //prim/sec buttons
                            {
                                var vertlayout2 = page.GetComponent<VerticalLayoutGroup>();
                                vertlayout2.padding.bottom = 2;

                                // var topbar = new GameObject("TopBar");

                                var PrimSec = new GameObject("PrimSec");
                                PrimSec.transform.SetParent(page.transform);
                                PrimSec.transform.localScale = new Vector3(1, 1, 1);
                                var le = PrimSec.AddComponent<LayoutElement>();
                                var HLG = PrimSec.AddComponent<HorizontalLayoutGroup>();

                                var togglegroup = PrimSec.AddComponent<ToggleGroupHandler>();
                                // topbar.transform.SetParent(window);
                                //var le = topbar.AddComponent<LayoutElement>();
                                le.minHeight = 30;
                                HLG.padding.left = 10;
                                HLG.padding.right = 15;
                                HLG.spacing = 5;
                                // var windowVertLayout = window.GetComponent<VerticalLayoutGroup>();
                                // windowVertLayout.padding.top = 5;
                                var SelectedTransform = newgameobject.transform.parent.Find("InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/All/Selected");

                                var PrimButton = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), PrimSec.transform);
                                togglegroup.m_PrimarySelected = UnityEngine.Object.Instantiate(SelectedTransform, PrimButton.transform).gameObject;
                                UnityEngine.Component.Destroy(togglegroup.m_PrimarySelected.transform.GetComponent<CanvasGroup>());
                                UnityEngine.Component.Destroy(togglegroup.m_PrimarySelected.transform.GetComponent<Image>());

                                PrimButton.Find("FinneanLabel").gameObject.SetActive(false);
                                PrimButton.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Main Hand";
                                PrimButton.gameObject.AddComponent<LayoutElement>();

                                var SecButton = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), PrimSec.transform);
                                togglegroup.m_SecondarySelected = UnityEngine.Object.Instantiate(SelectedTransform, SecButton.transform).gameObject;
                                UnityEngine.Component.Destroy(togglegroup.m_SecondarySelected.transform.GetComponent<CanvasGroup>());
                                UnityEngine.Component.Destroy(togglegroup.m_SecondarySelected.transform.GetComponent<Image>());


                                SecButton.Find("FinneanLabel").gameObject.SetActive(false);
                                SecButton.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Off Hand";
                                SecButton.gameObject.AddComponent<LayoutElement>();

                                togglegroup.Setup(PrimButton.GetComponent<OwlcatButton>(), SecButton.GetComponent<OwlcatButton>());
                                weaponOverridePCView.m_ToggleGroup = togglegroup;
                                weaponOverridePCView.m_SlotButtons = clickableComponent;
                            }
                            weaponOverridePCView.Initialize();
                        }
                        //Left block
                        {
                            var dollRightBlock = newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock");
                            var rightBlock = new GameObject("LeftBlock");
                            //  var weaponOverridePCView = rightBlock.AddComponent<WeaponOverridePCView>();
                            //rightBlock.transform.localScale = new Vector3(1, 1, 1);
                            // weaponOverridePCView.m_DropDown = dropdown;
                            // weaponOverridePCView.m_ListPCView = newcompl;
                            // WeaponOverrideVM.m_IntToAnim = m_IntToAnim;
                            // WeaponOverrideVM.m_AnimToInt = m_AnimToInt;


                            // EquipmentPCView.m_weaponOverridePCView = weaponOverridePCView;

                            rightBlock.transform.SetParent(EquipmentPCView.transform);
                            rightBlock.transform.localScale = new Vector3(1, 1, 1);
                            var vertlayout = rightBlock.AddComponent<VerticalLayoutGroupWorkaround>(dollRightBlock.GetComponent<VerticalLayoutGroupWorkaround>());
                            vertlayout.m_TotalMinSize = new Vector2(570, 570);
                            //var layoutElement = rightBlock.AddComponent<LayoutElement>();
                            //layoutElement.minWidth = 150;
                            var rightBlockRectTransform = rightBlock.GetComponent<RectTransform>();
                            var oldRightBlockRectTransform = dollRightBlock.GetComponent<RectTransform>();
                            rightBlockRectTransform.anchoredPosition = oldRightBlockRectTransform.anchoredPosition;
                            rightBlockRectTransform.anchoredPosition3D = oldRightBlockRectTransform.anchoredPosition3D;
                            rightBlockRectTransform.anchorMax = oldRightBlockRectTransform.anchorMax;
                            rightBlockRectTransform.anchorMin = oldRightBlockRectTransform.anchorMin;
                            //rightBlockRectTransform.localPosition = new Vector3(655,0,0);
                            rightBlock.transform.localPosition = new Vector3(-622, 360, rightBlock.transform.localPosition.z);
                            var dollPageTemplate = newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock/FaceWarpaint");
                            var SlotSelector = GameObject.Instantiate(dollPageTemplate.gameObject, rightBlock.transform);
                            SlotSelector.GetComponent<VerticalLayoutGroupWorkaround>().padding.right = 3;

                            UnityEngine.Object.Destroy(SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_SlideSequentionalSelector (1)").gameObject);
                            var page = SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_TextureSelector");
                            var clickablePageNav = SlotSelector.transform.Find("HeaderH2");
                            SlotSelector.transform.Find("SelectorsPlace/PC_Warpaint_TextureSelector/PalettePlace").gameObject.SetActive(false);
                            clickablePageNav.transform.SetParent(page);
                            var lblgm = clickablePageNav.Find("Label");
                            var thingg = lblgm.gameObject.AddComponent<LayoutElement>();
                            thingg.minWidth = 500;
                            var lbl = lblgm.GetComponent<TextMeshProUGUI>();
                            lbl.text = "Hide Equipment";
                            lbl.verticalAlignment = VerticalAlignmentOptions.Geometry;
                            var buttontemplate = newgameobject.transform.Find("DollRoom(Clone)/CharacterVisualSettingsView/WindowContainer/ShowClothContainer");

                            var parent = rightBlock.transform.Find("FaceWarpaint(Clone)/SelectorsPlace");
                            var parentVerticalLayoutGroup = parent.GetComponent<VerticalLayoutGroup>();
                            parentVerticalLayoutGroup.childForceExpandWidth = false;
                            parentVerticalLayoutGroup.childAlignment = TextAnchor.UpperCenter;
                            {
                                var superparent = new GameObject("superparent");
                                superparent.transform.SetParent(parent);
                                superparent.transform.localScale = new Vector3(1, 1, 1);
                                var superparentHLG = superparent.AddComponent<HorizontalLayoutGroup>();
                                superparentHLG.padding.left = 25;
                                superparentHLG.padding.bottom = 5;
                                superparentHLG.padding.top = 10;


                                //var enumValues = Enum.GetValues(typeof(ItemsFilter.ItemType));


                                //Do Quivers and whatnot
                                //Barding will be in separate animal comp page


                                HideButtonType[] enumValues = new HideButtonType[]
                                {
                                    HideButtonType.Armor,
                                    HideButtonType.Feet,
                                    HideButtonType.Glasses,
                                    HideButtonType.Gloves,
                                    HideButtonType.Head,
                                    HideButtonType.Shirt,
                                    HideButtonType.Shoulders,
                                    HideButtonType.Wrist,
                                    HideButtonType.Usable,
                                    HideButtonType.Weapon
                                };
                                Dictionary<HideButtonType, string> enumValuesNames = new Dictionary<HideButtonType, string>()
                                {
                                    [HideButtonType.Armor] = "Armour",
                                    [HideButtonType.Feet] = "Boots",
                                    [HideButtonType.Glasses] = "Glasses", //Maybe mask or smthn
                                    [HideButtonType.Gloves] = "Gloves",
                                    [HideButtonType.Head] = "Head",
                                    [HideButtonType.Shirt] = "Shirt",
                                    [HideButtonType.Shoulders] = "Cape",
                                    [HideButtonType.Wrist] = "Bracers",
                                    [HideButtonType.Usable] = "Belt items",
                                    [HideButtonType.Weapon] = "Sheaths",
                                    [HideButtonType.Enchantments] = "Enchantments"
                                };

                                Transform toAttachTo = null;
                                var vertRows = 3;
                                int Offset = 0;
                                int OverFlow = 0;
                                Main.Logger.Log((enumValues.Length % vertRows).ToString());
                                if ((enumValues.Length % vertRows) > 0) OverFlow = (enumValues.Length % vertRows);
                                for (int i = 0; i < ((vertRows)); i++)
                                {
                                    var parentV = new GameObject("Parent" + i);
                                    parentV.transform.SetParent(superparent.transform);
                                    parentV.transform.localScale = new Vector3(1, 1, 1);
                                    parentV.AddComponent<LayoutElement>();
                                    var vlgP = parentV.AddComponent<VerticalLayoutGroup>(parent.GetComponent<VerticalLayoutGroup>());
                                    parentV.name = "Parent" + (i + 1);
                                    vlgP.spacing = 3;
                                    toAttachTo = parentV.transform;
                                    var å = 0;
                                    for (int y = 0; y < ((enumValues.Length / vertRows) + OverFlow); y++)
                                    {
                                        if (y + Offset >= enumValues.Length) continue;
                                        var ab = enumValues.GetValue(y + Offset);
                                        var newbuttonn = GameObject.Instantiate(buttontemplate, toAttachTo);
                                        // var newbuttonn = (i >= (enumValues.Length/2)) ? GameObject.Instantiate(buttontemplate,parent1.transform) : GameObject.Instantiate(buttontemplate, parent2.transform);
                                        var pcview = newbuttonn.GetComponent<CharacterVisualSettingsEntityView>();
                                        var newPCView = newbuttonn.gameObject.AddComponent<HideEquipmentButtonPCView>();
                                        newPCView.SetupFromVisualSettingsView(pcview);
                                        newPCView.Bind(enumValuesNames[(HideButtonType)ab], (HideButtonType)ab);
                                        //pcview.Bind(nameof(ab), () => { Kingmaker.Game.Instance.SelectionCharacter.CurrentSelectedCharacter.GetSettings().HideEquipmentDict[(ItemsFilter.ItemType)ab] = !Kingmaker.Game.Instance.SelectionCharacter.CurrentSelectedCharacter.GetSettings().HideEquipmentDict[(ItemsFilter.ItemType)ab]; Kingmaker.Game.Instance?.SelectionCharacter?.CurrentSelectedCharacter?.View?.UpdateBodyEquipmentVisibility(); }, () => { return false; });
                                        //newbuttonn.Find("ToggleOwlcatMultiSelectable/Label").GetComponent<TextMeshProUGUI>().text = enumValuesNames[(HideButtonType)ab];
                                        å++;
                                        EquipmentPCView.m_EquipmentHideButtons.Add(newPCView);
                                    }
                                    if (OverFlow > 0) OverFlow--;
                                    Offset += å;
                                }
                            }
                            //Class outfit
                            {
                                var newheader = GameObject.Instantiate(lblgm.parent, parent);
                                var lbl2 = newheader.Find("Label").GetComponent<TextMeshProUGUI>();
                                lbl2.text = "Class Outfit";


                                var superparent = new GameObject("superparent2");
                                superparent.transform.SetParent(parent);
                                
                                superparent.transform.localScale = new Vector3(1, 1, 1);
                                var superparentHLG = superparent.AddComponent<HorizontalLayoutGroup>();
                                superparentHLG.padding.left = 12;
                                superparentHLG.padding.bottom = -10;
                                superparentHLG.padding.top = 25;
                                superparentHLG.padding.right = 25;

                                var ClassEquipmentSelectorPCView = parent.gameObject.AddComponent<ClassOutfitSelectorPCView>();


                                Transform toAttachTo = null;
                                var vertRows = 3;
                                int Offset = 0;
                                int OverFlow = 0;
                                Main.Logger.Log((Classes.Count % vertRows).ToString());
                                if ((Classes.Count % vertRows) > 0) OverFlow = (Classes.Count % vertRows);
                                var classButtonTemplate = GameObject.Instantiate(oldcomp.SlotPrefabs.First());
                                var newButtonPCView = classButtonTemplate.ConvertToClassButtonPCView();
                                for (int i = 0; i < ((vertRows)); i++)
                                {
                                    var parentV = new GameObject();
                                    parentV.transform.SetParent(superparent.transform);
                                    parentV.transform.localScale = new Vector3(1, 1, 1);
                                    parentV.AddComponent<LayoutElement>();
                                    var vlgP = parentV.AddComponent<VerticalLayoutGroup>(parent.GetComponent<VerticalLayoutGroup>());
                                    parentV.name = "Parent" + (i + 1);
                                    vlgP.spacing = 3;
                                    toAttachTo = parentV.transform;
                                    var å = 0;
                                    int z = 0;
                                    if (OverFlow > 0) z = 1;
                                    for (int y = 0; y < ((Classes.Count / vertRows) + (z)); y++)
                                    {
                                        if (y + Offset >= Classes.Count) continue;
                                        var ab = Classes[y + Offset];
                                        Main.Logger.Log(y.ToString() + ab.Item1);
                                        var newbuttonn = GameObject.Instantiate(newButtonPCView.transform, toAttachTo);
                                        var PCView = newbuttonn.GetComponent<ClassOutfitSelectorButtonPCView>();
                                        PCView.ClassName = ab.Item1;
                                        PCView.GUID = ab.Item2;
                                        PCView.Label.text = ab.Item1;
                                        ClassEquipmentSelectorPCView.Buttons.Add(ab.Item2,PCView);
                                        // var newbuttonn = (i >= (enumValues.Length/2)) ? GameObject.Instantiate(buttontemplate,parent1.transform) : GameObject.Instantiate(buttontemplate, parent2.transform);
                                        // var pcview = newbuttonn.GetComponent<CharacterVisualSettingsEntityView>();
                                        //pcview.Bind(nameof(ab), () => { Kingmaker.Game.Instance.SelectionCharacter.CurrentSelectedCharacter.GetSettings().HideEquipmentDict[(ItemsFilter.ItemType)ab] = !Kingmaker.Game.Instance.SelectionCharacter.CurrentSelectedCharacter.GetSettings().HideEquipmentDict[(ItemsFilter.ItemType)ab]; Kingmaker.Game.Instance?.SelectionCharacter?.CurrentSelectedCharacter?.View?.UpdateBodyEquipmentVisibility(); }, () => { return false; });
                                        // newbuttonn.Find("ToggleOwlcatMultiSelectable/Label").GetComponent<TextMeshProUGUI>().text = $"{ab.NameForAcronym} || {ab.Name} || {ab.name}";
                                        å++;
                                    }
                                    if (OverFlow > 0) OverFlow--;
                                    Offset += å;
                                }
#if false // Overflow doing an extra row at the bottom instead (also gotta remove the overflow stuff above ^^
                                var superparent3 = new GameObject("superparent3");
                                superparent3.transform.SetParent(parent);
                                superparent3.transform.localScale = new Vector3(1, 1, 1);
                                var superparent3HLG = superparent3.AddComponent<HorizontalLayoutGroup>();
                                superparent3HLG.padding.left = 12;
                                superparent3HLG.padding.bottom = 4;
                                superparent3HLG.padding.top = 17;
                                superparent3HLG.padding.right = 25;
                                for (int y = 0; y < OverFlow; y++)
                                {
                                    if (y + Offset >= Classes.Count) continue;
                                    var ab = Classes[y + Offset];
                                    Main.Logger.Log(y.ToString() + ab.Item1);
                                    var newbuttonn = GameObject.Instantiate(newButtonPCView.transform, superparent3.transform);
                                    var PCView = newbuttonn.GetComponent<ClassOutfitSelectorButtonPCView>();
                                    PCView.ClassName = ab.Item1;
                                    PCView.GUID = ab.Item2;
                                    PCView.Label.text = ab.Item1;
                                    ClassEquipmentSelectorPCView.Buttons.Add(ab.Item2, PCView);
                                }
#endif
                                foreach(var button in ClassEquipmentSelectorPCView.Buttons)
                                {
                                    button.Value.Button.OnLeftClick.AddListener(new UnityAction(() => { ClassEquipmentSelectorPCView.Selected.Value = button.Value; }));
                                }                      
                                EquipmentPCView.m_classOutfitSelectorPCView = ClassEquipmentSelectorPCView;
                            }
                        }
                    }


                    newselectionbar.transform.localScale = oldbar.transform.localScale;
                    //Add visual adjustments Window PCView
                    {
                        var oldcomp = newgameobject.transform.parent.GetComponent<ServiceWindowsPCView>();
                        var compPCView = newgameobject.AddComponent<ServiceWindowsPCViewModified>();

                        var Doll = new GameObject("Doll");
                        Doll.transform.SetParent(newgameobject.transform);
                        var DollPCView = Doll.AddComponent<DollPCView>();


                        compPCView.m_DollPCView = DollPCView;
                        DollPCView.m_CharGenAppearancePCView = newcomp;
                        DollPCView.m_CreateDollPCView = cmp;
                        DollPCView.m_CreateDollPCView.Button.OnLeftClick.AddListener(() => { DollPCView.ViewModel?.AddUnitPart(); });

                        compPCView.m_Background = oldcomp.m_Background;
                        compPCView.m_ServiceWindowMenuPcView = comp;
                        compPCView.m_EEPickerPCView = EEPickerPCView;
                        compPCView.m_EquipmentPCView = EquipmentPCView;
                        compPCView.m_FXViewerPCView = FXViewerPCView;
                        compPCView.m_DollRoom = dollroomcomp;

                        newcomp.m_DeleteDollButton.OnLeftClick.AddListener(() => { DollPCView.DeleteDoll(); });
                        ServiceWindowsVM_ShowWindow_Patch.swPCView = compPCView;
                    }
                }
            }
            catch (Exception e)
            {
                Main.Logger.Error(e.ToString());
            }
        }

        public static EEPickerPCView CreateEEPicker(GameObject newgameobject, DollCharacterController dollroomcomp)
        {
            var gameobject2 = new GameObject("EEPicker");
            gameobject2.transform.SetParent(newgameobject.transform);
            var EEPickerPCView = gameobject2.AddComponent<EEPickerPCView>();
            //All EEs List
            {
                EEPickerPCView.m_dollCharacterController = dollroomcomp;
                var alleelistview = GameObject.Instantiate(gameobject2.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, gameobject2.transform);
                alleelistview.name = "CurrentEEs";
                alleelistview.transform.localPosition = new Vector3(650, -50, 0);
                var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                var newcompl = alleelistview.AddComponent<ListPCView>();
                newcompl.SetupFromChargenList(oldcomp, true, "All EEs");
                newcompl.VirtualList.m_ScrollSettings.ScrollWheelSpeed = 666;
                UnityEngine.Component.Destroy(oldcomp);
                EEPickerPCView.m_AllEEs = newcompl;
            }
            //Current EEs list
            {
                var alleelistview = GameObject.Instantiate(gameobject2.transform.parent.parent.parent.Find("ChargenPCView/ContentWrapper/DetailedViewZone/ChargenFeaturesDetailedPCView/FeatureSelectorPlace/FeatureSelectorView").gameObject, gameobject2.transform);
                alleelistview.name = "AllEEs";
                alleelistview.transform.localPosition = new Vector3(-650, -50, 0);
                var oldcomp = alleelistview.GetComponent<CharGenFeatureSelectorPCView>();
                var newcompl = alleelistview.AddComponent<ListPCView>();
                newcompl.SetupFromChargenList(oldcomp, false, "Current EEs");
                UnityEngine.Component.Destroy(oldcomp);
                EEPickerPCView.m_CurrentEEs = newcompl;
                //Reset button
                {
                    var ResetButtonGameObject = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), alleelistview.transform.Find("HeaderH2"));
                    // ResetButtonGameObject.localPosition = new Vector3(-195, -398, 0);
                    ResetButtonGameObject.Find("FinneanLabel").gameObject.SetActive(false);
                    ResetButtonGameObject.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Reset";
                    var owlbutt = ResetButtonGameObject.GetComponent<OwlcatButton>();
                    var element = ResetButtonGameObject.gameObject.AddComponent<LayoutElement>();
                    element.minWidth = 100;
                    element.minHeight = 30;
                    var horizontal = alleelistview.transform.Find("HeaderH2").GetComponent<HorizontalLayoutGroup>();
                    horizontal.spacing = 10;
                    owlbutt.OnLeftClick.AddListener(() =>
                    {
                        EEPickerPCView.ResetChanges();
                    });
                    EEPickerPCView.m_ResetButton = owlbutt;
                }
                //Reset Changes
                {
                    var ResetButtonGameObject = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), alleelistview.transform.Find("HeaderH2"));
                    // ResetButtonGameObject.localPosition = new Vector3(-195, -398, 0);
                    ResetButtonGameObject.Find("FinneanLabel").gameObject.SetActive(false);
                    ResetButtonGameObject.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Reset To Default";
                    var owlbutt = ResetButtonGameObject.GetComponent<OwlcatButton>();
                    var element = ResetButtonGameObject.gameObject.AddComponent<LayoutElement>();
                    element.minWidth = 175;
                    element.minHeight = 30;
                    var horizontal = alleelistview.transform.Find("HeaderH2").GetComponent<HorizontalLayoutGroup>();
                    horizontal.spacing = 10;
                    owlbutt.OnLeftClick.AddListener(() =>
                    {
                        var unit = Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit;
                        var settings = unit.GetSettings();
                        settings.EeSettings.EEs.Clear();
                        unit.RebuildCharacter();
                        EEPickerPCView.ResetChanges();
                    });
                    EEPickerPCView.m_ResetAllButton = owlbutt;
                }
            }
            //Apply button
            {
                var ApplyButtonGameObject = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage"), EEPickerPCView.transform);
                ApplyButtonGameObject.localPosition = new Vector3(-195, -398, 0);
                ApplyButtonGameObject.Find("Button/FinneanLabel").gameObject.SetActive(false);
                ApplyButtonGameObject.Find("Button/StashLabel").GetComponent<TextMeshProUGUI>().text = "Apply";
                var owlbutt = ApplyButtonGameObject.Find("Button").GetComponent<OwlcatButton>();
                owlbutt.OnLeftClick.AddListener(() =>
                {
                    var doll = EEPickerPCView.ViewModel.UnitDescriptor.Value.Unit.Get<UnitPartDollData>();
                    var settings = EEPickerPCView.ViewModel.UnitDescriptor.Value.Unit.GetSettings();
                    foreach (var action in EEPickerPCView.ViewModel?.applyActions)
                    {
                        action.Value.Apply(EEPickerPCView.ViewModel.UnitDescriptor.Value.Unit, settings);
                    }
                });
                EEPickerPCView.m_ApplyButton = owlbutt;
            }
            //Colour picker
            {
                var ColPicker = UnityEngine.Object.Instantiate(newgameobject.transform.Find("DollRoom(Clone)/CharacterVisualSettingsView"), EEPickerPCView.transform);
                ColPicker.localPosition = new Vector3(398, -419, 0);
                var window = ColPicker.Find("WindowContainer");
                window.localPosition = new Vector3(-262, 205, 0);
                var oldcomp = ColPicker.GetComponent<CharacterVisualSettingsView>();
                var newcomp = ColPicker.gameObject.AddComponent<EEColorPickerPCView>();
                newcomp.SetupFromVisualSettings(oldcomp);
                Component.Destroy(oldcomp);
                //Apply and Secondary/Primary buttons
                {
                    var topbar = new GameObject("TopBar");
                    var togglegroup = topbar.AddComponent<ToggleGroupHandler>();
                    topbar.transform.SetParent(window);
                    var le = topbar.AddComponent<LayoutElement>();
                    le.minHeight = 30;
                    var HLG = topbar.AddComponent<HorizontalLayoutGroup>();
                    HLG.padding.left = 10;
                    HLG.padding.right = 10;

                    var windowVertLayout = window.GetComponent<VerticalLayoutGroup>();
                    windowVertLayout.padding.top = 5;

                    var TopLayout = new GameObject("TopLayout");
                    var TopLE = TopLayout.AddComponent<LayoutElement>();
                    TopLE.minHeight = 25;
                    TopLayout.transform.SetParent(window);
                    TopLayout.transform.SetAsFirstSibling();
                    window.Find("Background").SetAsFirstSibling();
                    var TopHor = TopLayout.AddComponent<HorizontalLayoutGroup>();

                    TopHor.padding.top = -22;
                    TopHor.padding.left = 10;
                    TopHor.padding.right = 10;
                    var Image = window.Find("Title");
                    Image.SetParent(Image);
                    var ImgLE = Image.gameObject.AddComponent<LayoutElement>();
                    ImgLE.minHeight = 37;

                    /*{
                        var left = new GameObject("LeftColPreview");
                        var lImg = left.AddComponent<Image>();
                        left.transform.SetParent(TopLayout.transform);
                        var CPV = left.AddComponent<ColorPreviewView>();
                        CPV.m_ToColor = lImg;
                        var LE = CPV.gameObject.AddComponent<LayoutElement>();
                        LE.minHeight = 15;
                        newcomp.m_LeftColor = CPV;
                    }*/
                    {
                        var right = new GameObject("RightColPreview");
                        var rImg = right.AddComponent<Image>();
                        right.transform.SetParent(TopLayout.transform);
                        right.transform.SetAsFirstSibling();
                        var CPV = right.AddComponent<ColorPreviewView>();
                        CPV.m_ToColor = rImg;
                        var LE = CPV.gameObject.AddComponent<LayoutElement>();
                        LE.minHeight = 15;
                        newcomp.m_Color = CPV;
                    }

                    var SelectedTransform = newgameobject.transform.parent.Find("InventoryPCView/Inventory/Stash/StashContainer/PC_FilterBlock/FilterPCView/SwitchBar/All/Selected");

                    var PrimSec = new GameObject("PrimSec");
                    PrimSec.transform.SetParent(topbar.transform);
                    PrimSec.AddComponent<LayoutElement>();
                    PrimSec.AddComponent<HorizontalLayoutGroup>();

                    var PrimButton = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), PrimSec.transform);
                    togglegroup.m_PrimarySelected = UnityEngine.Object.Instantiate(SelectedTransform, PrimButton.transform).gameObject;
                    UnityEngine.Component.Destroy(togglegroup.m_PrimarySelected.transform.GetComponent<CanvasGroup>());
                    UnityEngine.Component.Destroy(togglegroup.m_PrimarySelected.transform.GetComponent<Image>());

                    PrimButton.Find("FinneanLabel").gameObject.SetActive(false);
                    PrimButton.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Primary";
                    PrimButton.gameObject.AddComponent<LayoutElement>();

                    var SecButton = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), PrimSec.transform);
                    togglegroup.m_SecondarySelected = UnityEngine.Object.Instantiate(SelectedTransform, SecButton.transform).gameObject;
                    UnityEngine.Component.Destroy(togglegroup.m_SecondarySelected.transform.GetComponent<CanvasGroup>());
                    UnityEngine.Component.Destroy(togglegroup.m_SecondarySelected.transform.GetComponent<Image>());


                    SecButton.Find("FinneanLabel").gameObject.SetActive(false);
                    SecButton.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Secondary";
                    SecButton.gameObject.AddComponent<LayoutElement>();

                    togglegroup.Setup(PrimButton.GetComponent<OwlcatButton>(), SecButton.GetComponent<OwlcatButton>());
                    newcomp.m_ToggleGroupHandler = togglegroup;
                }
                //RGB Sliders
                {
                    var newsliderR = UnityEngine.Object.Instantiate(newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock/Tatoo/SelectorsPlace/PC_Warpaint_SlideSequentionalSelector (1)"), newcomp.transform.Find("WindowContainer"));
                    var oldcomp_R = newsliderR.GetComponent<SlideSelectorPCView>();
                    var newcomp_R = newsliderR.gameObject.AddComponent<BarleySlideSelectorPCView>();
                    newcomp_R.SetupFromSlideSelector(oldcomp_R);
                    newcomp_R.m_Prefix = "R";
                    newcomp.m_R_Slider = newcomp_R;

                    var newsliderG = UnityEngine.Object.Instantiate(newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock/Tatoo/SelectorsPlace/PC_Warpaint_SlideSequentionalSelector (1)"), newcomp.transform.Find("WindowContainer"));
                    var oldcomp_G = newsliderG.GetComponent<SlideSelectorPCView>();
                    var newcomp_G = newsliderG.gameObject.AddComponent<BarleySlideSelectorPCView>();
                    newcomp_G.SetupFromSlideSelector(oldcomp_G);
                    newcomp_G.m_Prefix = "G";
                    newcomp.m_G_Slider = newcomp_G;

                    var newsliderB = UnityEngine.Object.Instantiate(newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock/Tatoo/SelectorsPlace/PC_Warpaint_SlideSequentionalSelector (1)"), newcomp.transform.Find("WindowContainer"));
                    var oldcomp_B = newsliderB.GetComponent<SlideSelectorPCView>();
                    var newcomp_B = newsliderB.gameObject.AddComponent<BarleySlideSelectorPCView>();
                    newcomp_B.SetupFromSlideSelector(oldcomp_B);
                    newcomp_B.m_Prefix = "B";
                    newcomp.m_B_Slider = newcomp_B;
                }
                //Apply Button
                {
                    var bottombar = new GameObject("BottomBar");
                    bottombar.transform.SetParent(window);
                    var le = bottombar.AddComponent<LayoutElement>();
                    le.minHeight = 60;
                    var HLG = bottombar.AddComponent<HorizontalLayoutGroup>();
                    HLG.padding.left = 10;
                    HLG.padding.right = 10;
                    HLG.padding.top = 15;
                    HLG.padding.bottom = 15;

                    var ApplyButton = UnityEngine.GameObject.Instantiate(newgameobject.transform.parent.Find("InventoryPCView/Inventory/SmartItemButton/FrameImage/Button"), bottombar.transform);
                    ApplyButton.Find("FinneanLabel").gameObject.SetActive(false);
                    ApplyButton.Find("StashLabel").GetComponent<TextMeshProUGUI>().text = "Apply";
                    ApplyButton.gameObject.AddComponent<LayoutElement>();

                    newcomp.m_ConfirmButton = ApplyButton.GetComponent<OwlcatButton>();
                }
                EEPickerPCView.m_EEColorPicker = newcomp;
                //Ramp Sliders
                {
                    //
                    {
                        // var Container = new GameObject("ColorRampSlider");
                        //Container.transform.SetParent(window);
                        //Container.transform.localScale = new Vector3(1, 1, 1);

                        var newslider = UnityEngine.Object.Instantiate(newgameobject.transform.Find("ChargenAppearanceDetailedPCView(Clone)/AppearanceBlock/RightBlock/Tatoo/SelectorsPlace/PC_Warpaint_SlideSequentionalSelector (1)"), newcomp.transform.Find("WindowContainer"));
                        var oldcomp_ = newslider.GetComponent<SlideSelectorPCView>();
                        var newcomp_slider = newslider.gameObject.AddComponent<BarleySlideSelectorPCView>();
                        newcomp_slider.SetupFromSlideSelector(oldcomp_);
                        newcomp_slider.m_Prefix = "";
                        var lblplace = newslider.Find("LabelPlace");
                        lblplace.gameObject.SetActive(true);
                        lblplace.Find("Title-H4/Title-H4").GetComponent<TextMeshProUGUI>().text = "Index";
                        oldcomp_.gameObject.GetComponent<VerticalLayoutGroup>().padding.top = 25;
                        newslider.SetSiblingIndex(newslider.GetSiblingIndex() - 1);
                        //Add to pcview here
                    }

                    //throw new Exception("brunrbughufdhdfhg");
                }
            }
            gameobject2.transform.localPosition = new Vector3(0, 0, 0);
            gameobject2.transform.localScale = new Vector3(1, 1, 1);
            gameobject2.SetActive(false);
            return EEPickerPCView;
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
                component.ViewModel.HasView = true;
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
                if ((int)type == 50)
                {
                    {
                        if (!Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.IsPet)
                        {
                            var swVM = new ServiceWindowsVMModified();
                            if (swPCView != null) swPCView.Bind(swVM);

                            var vm = new ServiceWindowsMenuVMModified(swVM.OnSelectWindow);
                            pcview.Bind(vm);
                            swPCView.Bind(swVM);

                            __instance.OnDispose += () => { vm.Dispose(); swVM.Dispose(); };


                            swVM.AddDisposable(swVM.ServiceWindowsMenuVM.Value = vm);
                        }
                        else
                        {

                        }
                    }
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
