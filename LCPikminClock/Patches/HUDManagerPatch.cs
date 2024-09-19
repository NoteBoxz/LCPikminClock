using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LCPikminClock;

[HarmonyPatch(typeof(HUDManager))]
public class HUDManagerPatch
{
    public static GameObject ClockInstance;
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPostfix(HUDManager __instance)
    {
        // Spawns Clock
        GameObject ClockInst = null!;
        if (LCPikminClock.LCPikminClock.ClockUI != null)
            ClockInst = UnityEngine.Object.Instantiate(LCPikminClock.LCPikminClock.ClockUI, __instance.Clock.canvasGroup.gameObject.transform.Find("Container"));

        // Replaces References
        if (ClockInst == null) { return; }
        __instance.clockIcon = ClockInst.transform.Find("Icon").GetComponent<Image>();
        if (LCPikminClock.LCPikminClock.ShowTime)
            __instance.clockNumber = ClockInst.transform.Find("Icon/Number").GetComponent<TextMeshProUGUI>();
        __instance.Clock.canvasGroup.gameObject.transform.Find("Container/Box").gameObject.SetActive(false);
        __instance.shipLeavingEarlyIcon.transform.localPosition = new Vector3(18f, 183f, 33);
        ClockInstance = ClockInst;
        SetColors();
    }
    public static void SetColors()
    {
        if (ClockInstance == null) { return; }
        // Set colors for Lines and Dots
        foreach (Transform line in ClockInstance.transform.Find("Lines"))
        {
            var lineImage = line.GetComponent<Image>();
            if (lineImage != null)
            {
                lineImage.color = ParseColor(LCPikminClock.LCPikminClock.LinesColor);
            }
        }

        foreach (Transform dot in ClockInstance.transform.Find("Dots"))
        {
            var dotImage = dot.GetComponent<Image>();
            if (dotImage != null)
            {
                dotImage.color = ParseColor(LCPikminClock.LCPikminClock.DotsColor);
            }
        }

        // Set colors for Icon and Icon/Circle
        var iconImage = ClockInstance.transform.Find("Icon").GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.color = ParseColor(LCPikminClock.LCPikminClock.IconColor);
        }

        var circleImage = ClockInstance.transform.Find("Icon/Circle").GetComponent<Image>();
        if (circleImage != null)
        {
            circleImage.color = ParseColor(LCPikminClock.LCPikminClock.IconColor);
        }

        // Set color for Icon/Number (TMP_Text)
        var numberText = ClockInstance.transform.Find("Icon/Number").GetComponent<TextMeshProUGUI>();
        if (numberText != null)
        {
            numberText.color = ParseColor(LCPikminClock.LCPikminClock.TimeColor);
        }
    }

    private static Color ParseColor(string colorString)
    {
        // Remove parentheses and split the string
        var values = colorString.Trim('(', ')').Split(',');
        if (values.Length == 4 &&
            int.TryParse(values[0], out int r) &&
            int.TryParse(values[1], out int g) &&
            int.TryParse(values[2], out int b) &&
            int.TryParse(values[3], out int a))
        {
            // Convert to Color with normalized values
            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }
        LCPikminClock.LCPikminClock.Logger.LogWarning($"Unable to parse color config!!!: Input {colorString} The input should be somthing like (000,000,000,000) for (R,G,B,A)");
        return Color.white; // Default color if parsing fails
    }
}