using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.ResourceLinks;
using UnityEngine;
using UnityEngine.UI;

namespace VisualAdjustments2.Infrastructure
{
    public class CustomEyePictures
    {
        /*[HarmonyPatch(typeof(CustomPortraitsManager),
            methodName: nameof(CustomPortraitsManager.CreatePortraitData))]
        public class CustomPortraitPetEyeImg
        {
            public static bool Prefix(string id,ref PortraitData __result)
            {
                try
                {
                    PortraitData portraitData = new PortraitData(id);
                    portraitData.SmallPortraitHandle.Load();
                    portraitData.HalfPortraitHandle.Load();
                    portraitData.FullPortraitHandle.Load();
                    //

                    var eyePortrait = string.Format(CustomPortraitsManager.Instance.GetPortraitFolderPath(id),"Eye.png");
                    portraitData.m_PetEyeImage = ResourceLoader.Image2Sprite.Create(eyePortrait, size);
                    
                    
                    portraitData.CheckIfDefaultPortraitData(); 
                    
                    //
                    __result = portraitData;
                    return false;
                }
                catch (Exception e)
                {
                    Main.Logger.Error(e.ToString());
                    return true; 
                }
            }
        }*/


        [HarmonyPatch(typeof(PortraitData),methodType:MethodType.Getter, methodName:nameof(PortraitData.PetEyePortrait))]
        public class CustomPortraitPetEyeFixer
        {
            public static Dictionary<string, Sprite> loadedSprites = new();
            public static void Postfix(PortraitData __instance, ref Sprite __result)
            {
                #if DEBUG
                Main.Logger.Log($"Trying to get eye portrait for portrait \"{__instance.ToString()}\"");
                #endif
                if (__result == null && __instance.IsCustom)
                {
                    if (loadedSprites.TryGetValue(__instance.CustomId, out var thing))
                    {
                        __result = thing; 
                    }
                    else
                    {
                        var eyePortrait = CustomPortraitsManager.Instance.GetPortraitFolderPath(__instance.CustomId)+
                            "\\Eye.png";
                        if (File.Exists(eyePortrait))
                        {
                            byte[] data = File.ReadAllBytes(eyePortrait);
                            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                            texture2D.LoadImage(data);
                            __result = Sprite.Create(texture2D,
                                new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f),
                                200f);
                            loadedSprites[__instance.CustomId] = __result;
                        }
#if DEBUG
                        else
                        {
                            Main.Logger.Log($"Could not find \"{eyePortrait}\"");
                        }
#endif
                    }
                }
            }
        }
    }
}