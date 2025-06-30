using UnityEngine;

namespace Unity.FPS.Game
{
    // 오디오 플레이 관련 클래스
    public class AudioUtility
    {
        // 게임 오브젝트 생성하여 지정하는 효과음을 플레이
        public static void CreateSFX(AudioClip clip, Vector3 point, float spatialBlend, float rolloffDistanceMin = 1f)
        {
            // 하이라키 창에서 빈 오브젝트 만들기
            GameObject impactSfxInstance = new GameObject();
            impactSfxInstance.transform.position = point;       // 위치 지정

            // 새로 생성한 게임 오브젝트에 Audio Source 컴포넌트 추가
            AudioSource source = impactSfxInstance.AddComponent<AudioSource>();
            source.clip = clip;                         // 플레이할 오디오 클립
            source.spatialBlend = spatialBlend;         // 3D 사운드 효과 설정
            source.minDistance = rolloffDistanceMin;    // 3D 사운드 효과 최소 거리
            source.Play();

            // 사운드 플레이 후 자동 킬
            TimeSelfDestruct timeSelfDestruct = impactSfxInstance.AddComponent<TimeSelfDestruct>();
            timeSelfDestruct.lifeTime = clip.length;
        }
    }
}
