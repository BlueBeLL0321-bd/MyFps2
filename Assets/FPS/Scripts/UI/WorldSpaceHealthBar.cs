using UnityEngine;
using Unity.FPS.Game;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    // 캐릭터의 머리 위에 있는 Health Bar를 관리하는 클래스
    public class WorldSpaceHealthBar : MonoBehaviour
    {
        #region Variables
        public Health health;

        public Image healthBarImage;            // 게이지 바 이미지
        public Transform healthBarPivot;        // healthBar UI를 관리하는 오브젝트

        // HP가 Full이면 게이지 바가 보이지 않도록 한다
        [SerializeField]
        private bool hideFullHealthBar = true;
        #endregion

        #region Unity Event Method
        private void Update()
        {
            // 게이지 구현
            healthBarImage.fillAmount = health.GetRatio();

            // 게이지 바가 항상 플레이어를 바라보도록 한다
            healthBarPivot.LookAt(Camera.main.transform.position);

            // HP가 Full이면 게이지 바가 보이지 않도록 한다 Full이 아니면 보인다
            if(hideFullHealthBar)
            {
                healthBarPivot.gameObject.SetActive(healthBarImage.fillAmount != 1f);
            }
        }
        #endregion
    }
}

