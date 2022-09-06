using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScreenResolutionType
{
    SD, HD, FHD, QHD, UHD
}

public static class ScreenResolutionExtenstion
{
    public static float GetScreenIncreaseRatio(ScreenResolutionType from, ScreenResolutionType to)
    {
        float f = GetResolution(from);
        float t = GetResolution(to);

        return t / f;
    }

    public static float GetScreenIncreaseRatioAccordingFHD(ScreenResolutionType to)
    {
        return GetScreenIncreaseRatio(ScreenResolutionType.FHD, to);
    }

    public static float GetScreenIncreaseRatioAuto()
    {
        return GetScreenIncreaseRatioAccordingFHD(GetResolutionType(Screen.width));
    }

    private static float GetResolution(ScreenResolutionType resolution)
    {
        switch (resolution)
        {
            case ScreenResolutionType.SD: return 720;
            case ScreenResolutionType.HD: return 1280;
            case ScreenResolutionType.FHD: return 1920;
            case ScreenResolutionType.QHD: return 2560;
            case ScreenResolutionType.UHD: return 3849;
        }

        return 0;
    }

    private static ScreenResolutionType GetResolutionType(int width)
    {
        switch (width)
        {
            case 720: return ScreenResolutionType.SD;
            case 1280: return ScreenResolutionType.HD;
            case 1920: return ScreenResolutionType.FHD;
            case 2560: return ScreenResolutionType.QHD;
            case 3840: return ScreenResolutionType.UHD;
        }

        return ScreenResolutionType.FHD;
    }
}
