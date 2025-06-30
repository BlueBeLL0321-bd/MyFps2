using UnityEngine;

namespace Unity.FPS.Game
{
    // Time Self Destruct 컴포넌트가 있으면 생성 후 Life Time이 지나면 자동으로 킬
    public class TimeSelfDestruct : MonoBehaviour
    {
        #region Variables
        public float lifeTime = 5f;
        private float spawnTime;        // 생성 시간
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 생성되는 시간 저장
            spawnTime = Time.time;
        }

        private void Update()
        {
            // 생성 후 라이프 타임이 지나면
            if((spawnTime + lifeTime) <= Time.time)
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}

