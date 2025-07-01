using UnityEngine;

namespace Unity.FPS.Game
{
    // 게임에 등장하는 모든 캐릭터의 소속을 관리하는 클래스
    public class Actor : MonoBehaviour
    {
        #region Variables
        // 소속 - 0 : enemy, 1 : player
        public int affiliation;

        public Transform aimPoint;

        private ActorManager actorManager;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // ActorManager의 Actor 리스트에 등록
            actorManager = FindFirstObjectByType<ActorManager>();

            // 리스트 등록 여부 체크
            if(actorManager && actorManager.Actors.Contains(this) == false)
            {
                // 리스트 추가
                actorManager.Actors.Add(this);
            }
        }

        private void OnDestroy()
        {
            // 킬 되면 ActorManager의 Actor 리스트에서 제거
            if(actorManager)
            {
                actorManager.Actors.Remove(this);
            }
        }
        #endregion
    }
}

