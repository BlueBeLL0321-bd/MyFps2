using UnityEngine;
using System.Collections.Generic;

namespace Unity.FPS.Game
{
    // 신에 있는 모든 Actor 관리
    public class ActorManager : MonoBehaviour
    {
        #region Property
        // 신에 있는 Actor 리스트
        public List<Actor> Actors { get; set; }
        public GameObject Player { get; private set; }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 리스트 생성
            Actors = new List<Actor>();
        }
        #endregion

        #region Custom Method
        public void SetPlayer(GameObject player)
        {
            Player = player;
        }
        #endregion
    }
}

