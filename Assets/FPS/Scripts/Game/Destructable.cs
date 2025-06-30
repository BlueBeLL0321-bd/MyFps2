using UnityEngine;

namespace Unity.FPS.Game
{
    // 죽었을 때 (health를 가지고 있는) 오브젝트를 킬 하는 클래스
    public class Destructable : MonoBehaviour
    {
        #region Variables
        // 참조
        private Health health;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            health = this.GetComponent<Health>();
        }

        private void Start()
        {
            // 초기화
            // Health의 UnityAction 함수 등록
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;
        }
        #endregion

        #region Custom Method
        // Health의 UnityAction 함수 OnDamaged에 등록될 함수
        private void OnDamaged(float damage, GameObject damageSource)
        {
            // TODO : 대미지 구현
        }

        // Health의 UnityAction 함수 OnDie에 등록될 함수
        private void OnDie()
        {
            // 죽음 처리...

            // 오브젝트 킬
            Destroy(gameObject);
        }
        #endregion
    }
}

