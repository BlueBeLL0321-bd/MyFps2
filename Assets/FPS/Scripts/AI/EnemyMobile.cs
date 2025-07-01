using UnityEngine;
using Unity.FPS.Game;

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

            // 사운드
            audioSource.clip = moveSound;
            audioSource.Play();

            // enemyController 이벤트 함수에 등록
            enemyController.onDamaged += OnDamaged;
            enemyController.onDetectedTarget += OnDetectedTarget;
            enemyController.onLostTarget += OnLostTarget;
        }

        private void Update()
        {
            // 적 상태 구현
            UpdateCurrentAiState();

            // 이동 속도에 따른 애니, 사운드 재생 속도 설정
            float moveSpeed = enemyController.Agent.velocity.magnitude;
            animator.SetFloat(k_AnimMoveSpeedParameter, moveSpeed);
            audioSource.pitch = pitchMovementSpeed.GetValueRatio(moveSpeed / enemyController.Agent.speed);
        }
        #endregion

        #region Custom Method
        private void UpdateCurrentAiState()
        {
            switch(aiState)
            {
                case AIState.Patrol:
                    enemyController.UpdatePathDestination(true);                                    // 노드 도착 체크 및 다음 목표 노드 설정
                    enemyController.SetNavDestination(enemyController.GetDestinationOnPath());      // 이동할 목표 위치를 구해 Agent 이동 위치 설정
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
        
        // 적을 찾으면 호출되는 함수
        private void OnDetectedTarget()
        {
            Debug.Log("적을 찾았다 - 추격 시작");
        }

        private void OnLostTarget()
        {
            Debug.Log("적을 잃어버렸다 - 패트롤로 다시 가기");
        }
        #endregion
    }
}

