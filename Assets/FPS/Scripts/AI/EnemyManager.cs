using UnityEngine;
using System.Collections.Generic;

namespace Unity.FPS.AI
{
    // 신에 있는 Enemy들을 관리하는 클래스
    public class EnemyManager : MonoBehaviour
    {
        #region Property
        // 신에 있는 Enemy 리스트
        public List<EnemyController> Enemies { get; private set; }
        // 신에서 생성된 Enemy의 총 수량
        public int NumberOfEnemiesTotal {  get; private set; }
        // 현재 신에서 살아 있는 Enemy의 수량
        public int NumberOfEnemiesRemaining => Enemies.Count;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 리스트 생성
            Enemies = new List<EnemyController>();
        }
        #endregion

        #region Custom Method
        // Enemy 리스트 등록
        public void RegisterEnemy(EnemyController newEnemy)
        {
            Enemies.Add(newEnemy);

            // 카운팅
            NumberOfEnemiesTotal++;
        }

        // Enemy 리스트 제거
        public void UnregisterEnemy(EnemyController killedEnemy)
        {
            Enemies.Remove(killedEnemy);
        }
        #endregion
    }
}

