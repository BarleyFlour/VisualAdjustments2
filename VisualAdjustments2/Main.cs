﻿using HarmonyLib;
using Kingmaker;
using Kingmaker.Modding;
using Kingmaker.PubSubSystem;
using Owlcat.Runtime.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualAdjustments2.Infrastructure;
using UnityEngine;
using System.Collections;
using Kingmaker.BundlesLoading;
using System.Diagnostics;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.Blueprints.Root;
#if true
using UnityModManagerNet;
#endif
using Kingmaker.Visual.Particles;

namespace VisualAdjustments2
{
    [HarmonyPatch(typeof(MainMenu),nameof(MainMenu.Awake))]
    public static class GameStarter_Patch
    {
        [HarmonyPostfix]
        public static void Patch()
        {
#if DEBUG
            Main.Logger.Log("Started Loading VA2 Resources");
#endif
            LoaderGameObject.CreateLoaderAndLoad();
        }
    }
#if DEBUG
    [EnableReloading]
#endif
    public static class Main
    {
        public static bool IsEnabled { get; private set; } = true;
#if true
        public static UnityModManager.ModEntry ModEntry;
        public static UnityModManager.ModEntry.ModLogger Logger;
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                ModEntry = modEntry;
                Logger = modEntry.Logger;
                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
#if DEBUG
                modEntry.OnGUI = OnGUI;
                modEntry.OnUnload = Unload;
#endif
                //LoaderGameObject.CreateLoaderAndLoad();
                //ResourceLoader.GetEEs();
                //ResourceLoader.StartEEGetting();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString() + "\n" + e.StackTrace);
                throw e;
            }
            return true;
        }
#else
        public static Kingmaker.Modding.OwlcatModification Modification { get; private set; }
		public static LogChannel Logger
			=> Modification.Logger;
		// ReSharper disable once UnusedMember.Global
		[OwlcatModificationEnterPoint]
		public static void Initialize(Kingmaker.Modding.OwlcatModification modification)
		{
			Modification = modification;

			var harmony = new Harmony(modification.Manifest.UniqueName);
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			modification.OnDrawGUI += OnGUI; //Make this tell you to go to the ingame menu
			modification.IsEnabled += () => IsEnabled;
			modification.OnSetEnabled += enabled => IsEnabled = enabled;
			//modification.OnShowGUI += () => Logger.Log("OnShowGUI");
			//modification.OnHideGUI += () => Logger.Log("OnHideGUI");
		}
#endif
#if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            new Harmony(modEntry.Info.Id).UnpatchAll();

            return true;
        }
        private static void OnGUI(UnityModManager.ModEntry modentry)
#else
        private static void OnGUI()
#endif
        {
#if DEBUG
            if(GUILayout.Button("TestAddEEComp"))
            {
            }
            if(GUILayout.Button("Loady Stuff"))
            {
                LoaderGameObject.CreateLoaderAndLoad();
            }
            if (GUILayout.Button("EEGetOld"))
            {
                ResourceLoader.StartEEGetting();
            }
            if (GUILayout.Button("FXGet"))
            {
                var a = ResourceLoader.GetFXs();
              //  FxHelper.SpawnFxOnUnit(ResourcesLibrary.TryGetResource<GameObject>(a.First().GUID),Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Unit.View);
            }
            if(GUILayout.Button(""))
            {
                  
            }
            if(GUILayout.Button("EEGuidCompare"))
            {
                try
                {
                /*    var guaranteedarray = ResourceLoader.GuaranteedCorrect.ToArray();
                   // var arraytocheck = ResourceLoader.m_AllEEs.ToArray();
                    int x = 0;
                    /*foreach (var stuff in guaranteedarray)
                    {
                        Main.Logger.Log($"Stuff:{(guaranteedarray[x].ToString() == arraytocheck[x].ToString()).ToString()}");

                        x++;
                    }*//*
                    for (int i = 0; i < guaranteedarray.Length; i++)
                    {
                        Main.Logger.Log("a");
                        Main.Logger.Log($"Stuff:{(guaranteedarray[i].ToString() == arraytocheck[i].ToString()).ToString()}");
                    }*/
                }
                catch(Exception e)
                {
                    Main.Logger.Error(e.ToString());
                }
            }
            if (GUILayout.Button("DollSaveTest"))
            {
                var asd = Kingmaker.Game.Instance.Player.AllCharacters.First().GetDollState();
                Main.Logger.Log(asd.ToString());
            }
            if (GUILayout.Button("DollRecoverTest"))
            {
                var doll = Kingmaker.Game.Instance.Player.AllCharacters.First().GetDollState();
                Kingmaker.Game.Instance.Player.AllCharacters.First().SaveDollState(doll);
                Main.Logger.Log(doll.ToString());
            }
#endif
        }
    }
}
