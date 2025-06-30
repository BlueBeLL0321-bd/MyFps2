using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    // Enemy 상태 Enum
    public enum AIState
    {
        Patrol,
        Follow,
        Attack
    }

    // Enemy 상태를 구현하는 클래스
    [RequireComponent(typeof(EnemyController))]
    public class EnemyMobile : MonoBehaviour
    {
        #region Variables
        // 참조
        private EnemyController enemyController;
        private AudioSource audioSource;
        public Animator animator;

        // 대미지 입을 때 효과
        public ParticleSystem[] randomHitSparks;

        // 이동 사운드 효과
        public AudioClip moveSound;
        public MinMaxFloat pitchMovementSpeed;          // 이동 속도에 따른 재생 속도 min, max 설정값

        // 애니메이션 파라미터 값
        const string k_AnimAttackParameter = "Attack";
        const string k_AnimMoveSpeedParameter = "MoveSpeed";
        const string k_AnimAlertParameter = "Alerted";
        const string k_AnimOnDamagedParameter = "OnDamaged";
        const string k_AnimOnDeathParameter = "Death";
        #endregion

        #region Property
        // enemy 상태
        public AIState aiState { get; private set; }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            enemyController = this.GetComponent<EnemyController>();
            audioSource = this.GetComponent<AudioSource>();
        }

        private void Start()
        {
            // 초기화
            aiState = AIState.Patrol;

            // onDamaged 이벤트 함수에 등록
            enemyController.onDamaged += OnDamaged;
        }

        private void Update()
        {
            // 적 상태 구현
            UpdateCurrentAiState();
        }
        #endregion

        #region Custom Method
        private void UpdateCurrentAiState()
        {
            switch(aiState)
            {
                case AIState.Patrol:
                    break;

                case AIState.Follow:
                    break;

                case AIState.Attack:
                    break;
            }
        }

        // 대미지를 입으면 Hit Spark를 랜덤하게 하나 플레이
        private void OnDamaged()
        {
            // 파티클 플레이
            if(randomHitSparks.Length > 0)
            {
                int randNum = Random.Range(0, randomHitSparks.Length);
                randomHitSparks[randNum].Play();
            }

            // 애니메이션
            animator.SetTrigger(k_AnimOnDamagedParameter);
        }
        #endregion
    }
}

