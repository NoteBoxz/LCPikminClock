using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LCPikminClock;

[HarmonyPatch(typeof(HUDManager))]
public class HUDManagerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(HUDManager __instance)
    {
        //Spawns Clock
        GameObject ClockInst = null!;
        if (LCPikminClock.LCPikminClock.ClockUI != null)
            ClockInst = UnityEngine.Object.Instantiate(LCPikminClock.LCPikminClock.ClockUI, __instance.Clock.canvasGroup.gameObject.transform.Find("Container"));
        
        //Repalces Refernces
        if(ClockInst == null){return;}
        __instance.clockIcon = ClockInst.transform.Find("Icon").GetComponent<Image>();
        if (LCPikminClock.LCPikminClock.ShowTime)
            __instance.clockNumber = ClockInst.transform.Find("Icon/Number").GetComponent<TextMeshProUGUI>();
        __instance.Clock.canvasGroup.gameObject.transform.Find("Container/Box").gameObject.SetActive(false);
        __instance.shipLeavingEarlyIcon.transform.localPosition = new Vector3(18f, 183f, 33);
    }
}