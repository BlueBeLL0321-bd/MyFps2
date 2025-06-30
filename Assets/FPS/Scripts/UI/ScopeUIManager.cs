using UnityEngine;
using Unity.FPS.Gameplay;

namespace Unity.FPS.UI
{
    // Scope UI On/Off
    public class ScopeUIManager : MonoBehaviour
    {
        #region Variables
        // 참조
        private PlayerWeaponManager weaponManager;

        public GameObject scopeUI;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            weaponManager = FindFirstObjectByType<PlayerWeaponManager>();
        }

        private void OnEnable()
        {
            // 이벤트 함수 등록
            weaponManager.OnScopedWeapon += OnScope;
            weaponManager.OffScopedWeapon += OffScope;
        }

        private void OnDisable()
        {
            // 이벤트 함수 해제
            weaponManager.OnScopedWeapon -= OnScope;
            weaponManager.OffScopedWeapon -= OffScope;
        }
        #endregion

        #region Custom Method
        private void OnScope()
        {
            scopeUI.SetActive(true);
        }

        private void OffScope()
        {
            scopeUI.SetActive(false);
        }
        #endregion
    }
}

