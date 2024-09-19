using System;
using HarmonyLib;
using LethalMin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(HUDManager))]
public class HUDManagerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(HUDManager __instance)
    {
    }
}