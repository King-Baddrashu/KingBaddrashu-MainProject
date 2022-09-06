using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationExtenstion
{
    public static float EaseInOutCubic(float x)
    {
        return x < 0.5 ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    public static float EaseInOutExpo(float x)
    {
        return x == 0 ? 0 : x == 1 ? 1
            : x < 0.5 ? Mathf.Pow(2, 20 * x - 10) / 2
            : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
    }

    public static float EaseInOutElastic(float x)
    {
        const float c5 = (2 * Mathf.PI) / 4.5f;

        return x == 0 ? 0 : x == 1 ? 1
          : x < 0.5
          ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
          : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
    }

    public static float easeInOutCirc(float x)
    {
        return x < 0.5
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * x, 2))) / 2
            : (Mathf.Sqrt(1 - Mathf.Pow(-2 * x + 2, 2)) + 1) / 2;
    }
}
