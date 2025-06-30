using System;
using UnityEngine;

namespace Unity.FPS.Game
{
    // Lerp 함수 구현 구조체

    // Float의 Lerp값 구하기
    [Serializable]
    public struct MinMaxFloat
    {
        public float Min;
        public float Max;

        // 매개 변수로 받은 ratio(0 ~ 1)에 해당하는 Lerp값 가져오기
        public float GetValueRatio(float ratio)
        {
            return Mathf.Lerp(Min, Max, ratio);
        }
    }

    // Color의 Lerp값 구하기
    [Serializable]
    public struct MinMaxColor
    {
        [ColorUsage(true, true)] public Color Min;
        [ColorUsage(true, true)] public Color Max;

        // 매개 변수로 받은 ratio(0 ~ 1)에 해당하는 Color Lerp값 가져오기
        public Color GetValueRatio(float ratio)
        {
            return Color.Lerp(Min, Max, ratio);
        }
    }

    // Vector의 Lerp값 구하기
    [Serializable]
    public struct MinMaxVector
    {
        public Vector3 Min;
        public Vector3 Max;

        // 매개 변수로 받은 ratio(0 ~ 1)에 해당하는 Vector3 Lerp값 가져오기
        public Vector3 GetValueRatio(float ratio)
        {
            return Vector3.Lerp(Min, Max, ratio);
        }
    }
}

