using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

namespace Unity.FPS.AI
{
    // 렌더러 데이터를 관리하는 구조체
    public struct RendererIndexData
    {
        public Renderer renderer;
        public int materialIndex;

        // 생성자 - 매개 변수로 입력받은 데이터로 초기화
        public RendererIndexData(Renderer _renderer, int index)
        {
            renderer = _renderer;
            materialIndex = index;
        }
    }

    // Enemy Health, Patrol 데이터를 관리하는 클래스
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        #region Variables
        // 참조
        private Health health;

        private Actor actor;
        private Collider[] selfColliders;

        public GameObject deathVfxPrefab;
        public Transform deathVfxSpawnPosition;

        // 대미지 처리
        public UnityAction onDamaged;               // 대미지 입을 때 호출되는 이벤트 함수

        public Material bodyMaterial;               // 대미지 효과를 구현할 매터리얼
        [GradientUsage(true)]
        public Gradient onHitBodyGradient;          // 대미지 효과 그라디언트 색 변환 효과

        private float flashOnHitDuration = 0.5f;    // 색 변환 플래시 효과 시간

        public AudioClip damageSfx;                 // 대미지 사운드 효과

        // body Material을 가진 렌더러 리스트
        private List<RendererIndexData> bodyRenderers = new List<RendererIndexData>();
        // Material 속성 변경
        private MaterialPropertyBlock bodyFlashMaterialPropertyBlock;

        private float lastTimeDamaged = float.NegativeInfinity;
        private bool wasDamagedThisFrame = false;

        // Patrol
        private int pathDestinationNodeIndex;               // 이동할 목표 노드 인덱스
        [SerializeField]
        private float pathReachingRadius = 1f;              // 도착 체크 범위

        // Detection
        public Material eyeColorMaterial;
        [ColorUsage(true, true)]
        public Color defaultEyeColor;
        [ColorUsage(true, true)]
        public Color attackEyeColor;

        private RendererIndexData eyeRendererData;
        private MaterialPropertyBlock eyeColorMaterialPropertyBlock;

        // 디텍팅 하는 순간 등록된 함수 호출하는 이벤트 함수
        public UnityAction onDetectedTarget;
        // 적을 잃어버리는 순간 등록된 함수 호출하는 이벤트 함수
        public UnityAction onLostTarget;
        #endregion

        #region Property
        // Patrol
        public NavMeshAgent Agent { get; private set; }
        // 패트롤 할 path - enemy가 등록되어 있는 path
        public PatrolPath patrolPath { get; set; }

        // detection
        public DetectionModule detectionModule { get; private set; }
        public GameObject KnownDetectedTarget => detectionModule.KnownDetectedTarget;
        public bool HadKnownTarget => detectionModule.HadKnownTarget;
        public bool IsSeeingTarget => detectionModule.IsSeeingTarget;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            health = this.GetComponent<Health>();
            Agent = this.GetComponent<NavMeshAgent>();

            actor = this.GetComponent<Actor>();
            selfColliders = GetComponentsInChildren<Collider>();

            var detectionModules = GetComponentsInChildren<DetectionModule>();
            detectionModule = detectionModules[0];
        }

        private void Start()
        {
            // 초기화
            pathDestinationNodeIndex = 0;

            // health 이벤트 함수 등록
            health.OnDamaged += OnDamaged;
            health.OnDie += OnDie;

            // detectionModule 이벤트 함수 등록
            detectionModule.onDetectedTarget += OnDetectedTarget;
            detectionModule.onLostTarget += OnLostTarget;

            // body Material 가져오기
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    // body 매터리얼 가져오기
                    if (renderer.sharedMaterials[i] == bodyMaterial)
                    {
                        // 리스트에 렌더러와 매터리얼 인덱스를 구조체 형식으로 저장
                        bodyRenderers.Add(new RendererIndexData(renderer, i));
                    }

                    // eye 매터리얼 가져오기
                    if (renderer.sharedMaterials[i] == eyeColorMaterial)
                    {
                        eyeRendererData = new RendererIndexData(renderer, i);
                    }
                }
            }

            // 매터리얼 속성 변경을 위한 MaterialPropertyBlock 객체 만들기
            bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();

            // eye 렌더러 데이터가 생성되어 있으면
            if(eyeRendererData.renderer != null)
            {
                // eye 매터리얼 속성 변경을 위한 MaterialPropertyBlock 객체 만들기
                eyeColorMaterialPropertyBlock = new MaterialPropertyBlock();
                // eye 컬러를 기본값으로 변경
                eyeColorMaterialPropertyBlock.SetColor("_EmissionColor", defaultEyeColor);
                eyeRendererData.renderer.SetPropertyBlock(eyeColorMaterialPropertyBlock, eyeRendererData.materialIndex);
            }
        }

        private void Update()
        {
            // 적 디텍팅
            detectionModule.HandleTargetDetection(actor, selfColliders);

            // 대미지 컬러 효과
            Color currentColor = onHitBodyGradient.Evaluate((Time.time - lastTimeDamaged) / flashOnHitDuration);
            // 매터리얼 속성 변경 내용
            bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);
            // 변경 내용 매터리얼 적용
            foreach (var data in bodyRenderers)
            {
                data.renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.materialIndex);
            }

            // 한 프레임에 대미지 사운드 한 번 플레이
            wasDamagedThisFrame = false;
        }
        #endregion

        #region Custom Method
        // health OnDamaged 실행 시 호출되는 함수
        private void OnDamaged(float damage, GameObject damageSource)
        {
            if (damageSource && !damageSource.GetComponent<EnemyController>())
            {
                // onDamage에 등록되어 있는 함수 호출
                onDamaged?.Invoke();

                // 대미지 효과 - 대미지 입은 시간 저장
                lastTimeDamaged = Time.time;

                // 대미지 효과 - Sfx
                if (damageSfx && wasDamagedThisFrame == false)
                {
                    AudioUtility.CreateSFX(damageSfx, transform.position, 0f);
                }
                wasDamagedThisFrame = true;
            }
        }

        // health OnDie 실행 시 호출되는 함수
        private void OnDie()
        {
            // 죽었을 때 효과 - Vfx
            GameObject effectGo = Instantiate(deathVfxPrefab, deathVfxSpawnPosition.position, Quaternion.identity);
            Destroy(effectGo, 5f);

            // ...
        }

        // 패트롤 path 유효 여부 체크
        private bool IsPathValid()
        {
            return patrolPath && patrolPath.pathNodes.Count > 0;
        }

        // 패트롤 path 목표 노드 리셋
        private void ResetPathDestination()
        {
            pathDestinationNodeIndex = 0;
        }

        // 현재 위치에서 가장 가까운 노드 구해서 목표 노드 설정
        private void SetPathDestinationToClosestNode()
        {
            // 패스 체크
            if(IsPathValid() == false)
            {
                ResetPathDestination();
                return;
            }

            int closestPathNodeIndex = 0;
            float closerDistance = float.PositiveInfinity;
            for (int i = 0; i < patrolPath.pathNodes.Count; i++)
            {
                float distance = patrolPath.GetDistanceToNode(transform.position, i);
                if(distance < closerDistance)
                {
                    closerDistance = distance;
                    closestPathNodeIndex = i;
                }
            }

            pathDestinationNodeIndex = closestPathNodeIndex;
        }

        // 이동할 목표 위치 구하기
        public Vector3 GetDestinationOnPath()
        {
            // 패스 체크
            if(IsPathValid() == false)
            {
                return this.transform.position;
            }

            return patrolPath.GetPositionOfPathNode(pathDestinationNodeIndex);
        }

        // Agent 이동 목표 설정하기
        public void SetNavDestination(Vector3 destination)
        {
            // Agent 체크
            if (Agent == null)
                return;

            Agent.SetDestination(destination);
        }

        // 도착 체크 및 다음 목표 지점 설정하기 - 매개 변수로 패트롤 방향(오름차순, 내림차순) 설정
        public void UpdatePathDestination(bool inverseOrder = false)
        {
            // 패스 체크
            if (IsPathValid() == false)
                return;

            // 도착 판정
            float distance = (transform.position - GetDestinationOnPath()).magnitude;
            if(distance <= pathReachingRadius)
            {
                // 도착했으면 다음 목표 인덱스 설정
                pathDestinationNodeIndex = inverseOrder ? pathDestinationNodeIndex - 1 : pathDestinationNodeIndex + 1;
                // pathDestinationNodeIndex가 범위를 벗어났을 때 처리
                if(pathDestinationNodeIndex < 0)
                {
                    pathDestinationNodeIndex += patrolPath.pathNodes.Count;
                }
                if(pathDestinationNodeIndex >= patrolPath.pathNodes.Count)
                {
                    pathDestinationNodeIndex -= patrolPath.pathNodes.Count;
                }
            }
        }

        // 적을 찾으면 호출되는 함수
        private void OnDetectedTarget()
        {
            onDetectedTarget?.Invoke();

            // eye 매터리얼 변경
            if (eyeRendererData.renderer != null)
            {
                eyeColorMaterialPropertyBlock.SetColor("_EmissionColor", attackEyeColor);
                eyeRendererData.renderer.SetPropertyBlock(eyeColorMaterialPropertyBlock, eyeRendererData.materialIndex);
            }
        }

        // 적을 잃어버리면 호출되는 함수
        private void OnLostTarget()
        {
            onLostTarget?.Invoke();

            // eye 매터리얼 디폴트로 변경
            if (eyeRendererData.renderer != null)
            {
                eyeColorMaterialPropertyBlock.SetColor("_EmissionColor", defaultEyeColor);
                eyeRendererData.renderer.SetPropertyBlock(eyeColorMaterialPropertyBlock, eyeRendererData.materialIndex);
            }
        }
        #endregion
    }
}

