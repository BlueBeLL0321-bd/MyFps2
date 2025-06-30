using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    // UI 게이지 Bar 이미지 컬러 변경 효과
    public class ForeBackColorChange : MonoBehaviour
    {
        #region Variables
        public Image foregroundImage;
        public Color defaultForegroundColor;                        // 게이지 바 기본 컬러
        public Color fullFlashForegroundColor;                      // 100% 충전 시 플래시 효과 컬러

        public Image backgroundImage;
        public Color defaultBackgroundColor;                        // 게이지 바 백그라운드 이미지 기본 컬러
        public Color emptyFlashBackgroundColor;                     // 0% 시 플래시 효과 컬러

        [SerializeField] private float fullValue = 1f;              // 게이지 바 Rate Max Value
        [SerializeField] private float emptyValue = 0f;             // 게이지 바 Rate Min Value

        [SerializeField] private float colorChangeSharpness = 5f;   // 컬러 변경 속도 (Lerp 계수)

        private float m_PreviousValue;                              // 연출을 위한 was 변수
        #endregion

        #region Custom Method
        // UI 게이지 바 초기화
        public void Initialize(float fullValueRatio, float emptyValueRatio)
        {
            fullValue = fullValueRatio;
            emptyValue = emptyValueRatio;

            m_PreviousValue = fullValue;
        }

        // 게이지 바 효과 Update
        public void UpdateVisual(float currentRatio)
        {
            // 100% 충전되는 순간 체크
            if (currentRatio == fullValue && currentRatio != m_PreviousValue)
            {
                foregroundImage.color = fullFlashForegroundColor;
            }
            else if (currentRatio < emptyValue)
            {
                backgroundImage.color = emptyFlashBackgroundColor;
            }
            else
            {
                foregroundImage.color = Color.Lerp(foregroundImage.color, defaultForegroundColor,
                    Time.deltaTime * colorChangeSharpness);
                backgroundImage.color = Color.Lerp(backgroundImage.color, defaultBackgroundColor,
                    Time.deltaTime * colorChangeSharpness);
            }

            //
            m_PreviousValue = currentRatio;
        }
        #endregion
    }
}

