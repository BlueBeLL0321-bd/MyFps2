using UnityEngine;
using Unity.FPS.Game;
using UnityEngine.Events;
using System.Linq;

namespace Unity.FPS.AI
{
    public class DetectionModule : MonoBehaviour
    {
        #region Variables
        // 참조
        private ActorManager actorManager;

        public Transform detectionSourcePoint;                          // 디텍션 포인트
        public float detectionRange = 20f;                              // 디텍션 거리

        public float knownTargetTimeout = 4f;                           // 디텍팅 해제 딜레이 시간
        protected float timeLastSeenTarget = Mathf.NegativeInfinity;    // 타깃을 마지막 본 시간

        // 디텍팅 시 등록된 함수 호출하는 이벤트 함수
        public UnityAction onDetectedTarget;
        // 적을 잃어버리면 등록된 함수 호출하는 이벤트 함수
        public UnityAction onLostTarget;

        // 공격
        public float attackRange = 10f;                                 // 공격 가능 범위
        #endregion

        #region Property
        // 발견된 타깃
        public GameObject KnownDetectedTarget { get; private set; }
        // 타깃 발견 여부
        public bool HadKnownTarget { get; private set; }
        // 타깃이 시야에 있는지 여부
        public bool IsSeeingTarget { get; private set; }
        // 타깃이 공격 범위에 있는지 여부
        public bool IsTargetInAttackRange { get; private set; }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            actorManager = FindFirstObjectByType<ActorManager>();
        }
        #endregion

        #region Custom Method
        // actor : 찾는 자신, selfColliders : 자신 오브젝트 충돌체
        public void HandleTargetDetection(Actor actor, Collider[] selfColliders)
        {
            // 타깃팅 지속 여부
            if(KnownDetectedTarget && IsSeeingTarget == false
                && (timeLastSeenTarget + knownTargetTimeout) < Time.time)
            {
                KnownDetectedTarget = null;
            }

            // 디텍팅 거리
            float sqrDetectionRange = detectionRange * detectionRange;
            IsSeeingTarget = false;
            float closestSqrDistance = Mathf.Infinity;
            foreach (var otherActor in actorManager.Actors)
            {
                // 아군, 적군 구분
                if(otherActor.affiliation != actor.affiliation)
                {
                    // 적과의 거리 구하기
                    float sqrDistance = (otherActor.aimPoint.position - detectionSourcePoint.position).sqrMagnitude;
                    if(sqrDistance < sqrDetectionRange && sqrDistance < closestSqrDistance)
                    {
                        RaycastHit[] hits = Physics.RaycastAll(detectionSourcePoint.position,
                            (otherActor.aimPoint.position - detectionSourcePoint.position).normalized, detectionRange,
                            -1, QueryTriggerInteraction.Ignore);

                        // 가장 가까운 hit 찾기
                        RaycastHit closestHit = new RaycastHit();
                        closestHit.distance = Mathf.Infinity;
                        bool foundValidHit = false;

                        foreach (var hit in hits)
                        {
                            // 거리 체크, 자신 콜라이더 체크
                            if(hit.distance < closestHit.distance 
                                && selfColliders.Contains(hit.collider) == false)
                            {
                                closestHit = hit;
                                foundValidHit = true;
                            }
                        }

                        // 가장 가까운 적을 찾으면
                        if(foundValidHit)
                        {
                            Actor hitActor = closestHit.collider.GetComponentInParent<Actor>();
                            if(hitActor == otherActor)
                            {
                                closestSqrDistance = sqrDistance;

                                IsSeeingTarget = true;
                                timeLastSeenTarget = Time.time;
                                KnownDetectedTarget = otherActor.aimPoint.gameObject;
                            }
                        }
                    }
                }
            }

            // 찾은 적(타깃)이 공격 범위에 있는지 여부 체크
            IsTargetInAttackRange = KnownDetectedTarget != null &&
                Vector3.Distance(KnownDetectedTarget.transform.position, transform.position) <= attackRange;

            // 적을 모르고 있다가 적을 발견한 순간
            if(HadKnownTarget == false && KnownDetectedTarget != null)
            {
                OnDetected();
            }

            // 적을 계속 주시하고 있다가 적을 놓치는 순간
            if(HadKnownTarget == true && KnownDetectedTarget == null)
            {
                OnLostTarget();
            }

            // 타깃 설정 여부
            HadKnownTarget = KnownDetectedTarget != null;
        }

        // 적을 찾았다
        private void OnDetected()
        {
            onDetectedTarget?.Invoke();
        }

        // 적을 놓쳤다
        private void OnLostTarget()
        {
            onLostTarget?.Invoke();
        }

        // 대미지 입었을 때 호출
        public void OnDamaged(GameObject damageSource)
        {
            // 나를 공격한 대상을 공격 목표로 설정
            KnownDetectedTarget = damageSource;
            timeLastSeenTarget = Time.time;
        }

        // 공격받았을 때 호출
        public void OnAttack()
        {
            //
        }
        #endregion
    }
}

