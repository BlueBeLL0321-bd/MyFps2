using UnityEngine;
using UnityEngine.UI;
using Unity.FPS.Game;
using Unity.FPS.Gameplay;

namespace Unity.FPS.UI
{
    public class PlayerHealthBar : MonoBehaviour
    {
        #region Variables
        private Health playerHealth;

        // Health Bar 게이지 이미지
        public Image healthFillImage;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            PlayerCharacterController playerCharacterController 
                = FindFirstObjectByType<PlayerCharacterController>();

            playerHealth = playerCharacterController.GetComponent<Health>();
        }

        private void Update()
        {
            healthFillImage.fillAmount = playerHealth.GetRatio();
        }
        #endregion
    }
}

