using System;
using HarmonyLib;
using LethalMin;
using UnityEngine;

[HarmonyPatch(typeof(TimeOfDay))]
public class TimeOfDayPatchPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    public static void MoveDial(TimeOfDay __instance)
    {
        if (!LethalMin.LethalMin.ReplaceClock) { return; }
        
        if (__instance.currentDayTimeStarted)
        {
            float startX = 371.3f;
            float endX = -387f;
            float normalizedTime = __instance.normalizedTimeOfDay;
            
            // Linearly interpolate between startX and endX based on normalizedTime
            float newX = Mathf.Lerp(startX, endX, normalizedTime);
            
            // Get the current position of the clock icon
            Vector3 currentPosition = HUDManager.Instance.clockIcon.transform.localPosition;
            
            // Update only the x component of the position
            currentPosition.x = newX;
            
            // Apply the new position
            HUDManager.Instance.clockIcon.transform.localPosition = currentPosition;
        }
    }
}