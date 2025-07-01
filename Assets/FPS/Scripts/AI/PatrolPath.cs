using UnityEngine;
using System.Collections.Generic;

namespace Unity.FPS.AI
{
    // 패트롤 웨이 포인트(node)를 관리하는 클래스
    public class PatrolPath : MonoBehaviour
    {
        #region Variables
        // 웨이 포인트 리스트
        public List<Transform> pathNodes = new List<Transform>();

        // 패트롤 패스를 패트롤하는 Enemy 리스트
        public List<EnemyController> enemiesToAssign = new List<EnemyController>();
        #endregion

        #region Unity Event Method
        // Patrol Path 그리기 - 노드와 노드를 라인으로 그린다
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < pathNodes.Count; i++)
            {
                // i, i + 1 라인 연결하여 그린다
                int nextIndex = i + 1;
                // i가 마지막 노드이면
                if(nextIndex >= pathNodes.Count)
                {
                    nextIndex -= pathNodes.Count;
                }
                Gizmos.DrawLine(pathNodes[i].position, pathNodes[nextIndex].position);
                Gizmos.DrawSphere(pathNodes[i].position, 0.2f);
            }
        }
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 등록된 enemy와 patrolPath를 저장
            foreach (var enemy in enemiesToAssign)
            {
                enemy.patrolPath = this;
            }
        }
        #endregion

        #region Custom Method
        // 특정 위치와 지정된 노드와의 거리 구하기
        public float GetDistanceToNode(Vector3 originPosition, int targetNodeIndex)
        {
            // targetNodeIndex 체크
            if(targetNodeIndex < 0 || targetNodeIndex >= pathNodes.Count
                || pathNodes[targetNodeIndex] == null)
            {
                return -1f;
            }

            return (pathNodes[targetNodeIndex].position - originPosition).magnitude;
        }

        // 지정된 노드의 위치 구하기
        public Vector3 GetPositionOfPathNode(int targetNodeIndex)
        {
            // targetNodeIndex 체크
            if (targetNodeIndex < 0 || targetNodeIndex >= pathNodes.Count
                || pathNodes[targetNodeIndex] == null)
            {
                return Vector3.zero;
            }

            return pathNodes[targetNodeIndex].position;
        }
        #endregion
    }
}

