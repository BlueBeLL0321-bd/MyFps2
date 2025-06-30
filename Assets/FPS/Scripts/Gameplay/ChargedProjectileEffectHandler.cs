using UnityEngine;
using Unity.FPS.Game;

namespace Unity.FPS.Gameplay
{
    // 충전용 발사체 이펙트 관리 클래스
    public class ChargedProjectileEffectHandler : MonoBehaviour
    {
        #region Variables
        public GameObject chargingObject;
        public MinMaxVector scale;

        private ProjectileBase projectileBase;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            projectileBase = this.GetComponent<ProjectileBase>();
            projectileBase.OnShoot += OnShoot;
        }
        #endregion

        #region Custom Method
        private void OnShoot()
        {
            chargingObject.transform.localScale = scale.GetValueRatio(projectileBase.InitialCharge);
        }
        #endregion
    }
}

