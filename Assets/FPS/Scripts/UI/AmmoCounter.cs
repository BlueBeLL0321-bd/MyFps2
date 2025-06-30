using UnityEngine;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;
using UnityEngine.UI;
using TMPro;

namespace Unity.FPS.UI
{
    // 무기의 Ammo Counter UI를 관리하는 클래스
    public class AmmoCounter : MonoBehaviour
    {
        #region Variables
        // 참조
        private PlayerWeaponManager weaponManager;
        private WeaponController weaponController;

        private int weaponCounterIndex;                 // Ammo Counter UI 인덱스 번호

        // UI
        public TextMeshProUGUI weaponIndexText;
        public Image ammoFillImage;

        public CanvasGroup canvasGroup;
        [SerializeField]
        [Range(0, 1)]
        private float unselectedOpacity = 0.5f;                     // 선택되지 않은 UI 투명값
        private Vector3 unselectedScale = Vector3.one * 0.8f;       // 선택되지 않은 UI 크기 (80%)

        [SerializeField]
        private float ammoFillSharpness = 10f;                      // Ammo UI 게이지 바 충전 속도 (Lerp 계수)
        [SerializeField]
        private float weaponSwitchSharpness = 10f;                  // 무기 변경 시 UI 투명도, 크기 변경 속도 (Lerp 계수)

        // 게이지 바 컬러 효과
        public ForeBackColorChange foreBackColorChange;
        #endregion

        #region Property
        public int WeaponCounterIndex => weaponCounterIndex;
        #endregion

        #region Unity Event Method
        private void Update()
        {
            float currentFillRate = weaponController.CurrentAmmoRate;

            // 게이지 바
            ammoFillImage.fillAmount = Mathf.Lerp(ammoFillImage.fillAmount, currentFillRate,
                Time.deltaTime * ammoFillSharpness);

            // 액티브 무기와 아닌 무기 구분
            bool isActiveWeapon = (weaponController == weaponManager.GetActiveWeapon());
            // UI 투명도 - 무기 교체 시 연출 구현
            float currentOpacity = isActiveWeapon ? 1f : unselectedOpacity;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, currentOpacity,
                Time.deltaTime * weaponSwitchSharpness);
            // UI 크기
            Vector3 currentScale = isActiveWeapon ? Vector3.one : unselectedScale;
            transform.localScale = Vector3.Lerp(transform.localScale, currentScale,
                Time.deltaTime * weaponSwitchSharpness);

            // 게이지 바 컬러 효과
            foreBackColorChange.UpdateVisual(currentFillRate);
        }
        #endregion

        #region Custom Method
        // Ammo Counter UI 초기화
        public void Initialize(WeaponController weapon, int weaponIndex)
        {
            weaponController = weapon;
            weaponCounterIndex = weaponIndex;

            // Weapon Manager 가져오기
            weaponManager = FindFirstObjectByType<PlayerWeaponManager>();

            // UI 초기화
            weaponIndexText.text = (weaponIndex + 1).ToString();

            // 컬러 효과 초기화
            foreBackColorChange.Initialize(1f, 0.1f);
        }
        #endregion
    }
}

