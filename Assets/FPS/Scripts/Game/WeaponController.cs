using System;
using UnityEngine;

namespace Unity.FPS.Game
{
    // 크로스헤어 데이터 구조체
    [Serializable]
    public struct CrosshairData
    {
        public Sprite crosshairSprite;
        public float crosshairSize;
        public Color crosshairColor;
    }

    // 무기 슈팅 타입 enum
    public enum WeaponShootType
    {
        Manual,
        Automatic,
        Charge,
        Sniper
    }

    // 무기를 제어하는 클래스, 모든 무기에 부착한다
    public class WeaponController : MonoBehaviour
    {
        #region Variables
        // 무기 비주얼 활성화, 비활성화
        public GameObject weaponRoot;
        public Transform weaponMuzzle;

        private AudioSource shootAudioSource;
        public AudioClip switchWeaponSfx;                           // 무기 교환 시 효과음

        // 크로스헤어
        public CrosshairData defaultCrosshair;                      // 기존 크로스헤어
        public CrosshairData targetInSightCrosshair;                // 적이 타기팅 되었을 때의 크로스헤어

        // 조준 Aim
        [Range(0, 1)]
        public float aimZoomRatio = 1f;                             // 조준 시 줌 확대 배율
        public Vector3 aimOffset;                                   // 조준 위치로 이동 시 무기별 위치 offset(조정)값

        // 슈팅
        [SerializeField] private WeaponShootType shootType;

        [SerializeField] private float maxAmmo = 8f;
        private float currentAmmo;

        [SerializeField] private float delayBetweenShots = 0.5f;    // 연사 시 발사 간격
        private float lastTimeShot;                                 // 마지막 발사 시간

        // 발사 효과
        public GameObject muzzleFlashPrefab;                        // VFX
        public AudioClip shootSfx;                                  // SFX

        // 반동 Recoil
        public float recoilForce = 0.5f;

        // 발사체 Projectile
        public ProjectileBase projectilePrefab;

        // 한 번 방아쇠를 당길 때(쏘는 데) 필요한 bullet의 개수
        [SerializeField] private int bulletsPerShot = 1;
        // 발사체가 발사될 때 퍼져 나가는 각도
        [SerializeField] private float bulletSpreadAngle = 0f;

        private Vector3 lastMuzzlePosition;                         //지난 프레임의 Muzzle의 위치

        // 재장전 - Reload
        // 자동 재장전
        [SerializeField] private bool automaticReload = true;
        private float ammoReloadRate = 1f;                          // 초당 재장전되는 Ammo의 양
        private float ammoReloadDelay = 2f;                         // 총을 쏜 후 딜레이 시간 이후 재장전 시간

        // 충전 - Charge
        private float ammoUseOnStartCharge = 1f;                    // 충전 발사 버튼을 누르기 시작할 때 소모되는 양, 차지를 시작하기 위한 입장료
        private float ammoUseRateWhileCharging = 1f;                // 충전하고 있는 동안 Ammo가 소비되는 양
        private float maxChargeDuration = 2f;                       // 충전하는 총 시간

        public float lastChargeTriggerTimesTemp;                    // 충전하는 시간을 저장하는 임시 변수
        #endregion

        #region Property
        public GameObject Owner { get; set; }           // 무기를 장착한 주인 오브젝트
        public GameObject SourcePrefab { get; set; }    // 무기를 생성한 원본 프리팹
        public bool IsWeaponActive { get; set; }        // 현재 이 무기가 액티브 무기인지

        // Projectile
        public Vector3 MuzzleWorldVelocity { get; private set; }    // 발사 시 총구의 이독 속도

        // 충전
        public bool IsCharging { get; private set; }
        public float CurrentCharge { get; private set; }            // 슛 타입의 무기 발사 충전량, 0 ~ 1

        public WeaponShootType ShootType => shootType;              // 슛 타입 읽기 전용

        public float CurrentAmmoRate { get; private set; }          // 현재 소유한 Ammo 비율
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            shootAudioSource = this.GetComponent<AudioSource>();
        }

        private void Start()
        {
            // 초기화
            currentAmmo = maxAmmo;
            lastTimeShot = Time.time;
        }

        private void Update()
        {
            UpdateCharge();
            UpdateAmmo();

            if (Time.deltaTime > 0f)
            {
                // 이번 프레임의 Muzzle 속도 구하기
                MuzzleWorldVelocity = (weaponMuzzle.position - lastMuzzlePosition) / Time.deltaTime;

                // Muzzle 위치 저장
                lastMuzzlePosition = weaponMuzzle.position;
            }
        }
        #endregion

        #region Custom Method
        // 충전
        private void UpdateCharge()
        {
            if (IsCharging == false)
                return;

            // Current Charge : 0(0%) ~ 1(100% 충전)
            if (CurrentCharge < 1f)
            {
                // 현재 앞으로 충전할 양
                float chargeLeft = 1 - CurrentCharge;

                // 이번 프레임에 충전할 양
                float chargeAdd = 0;
                if (maxChargeDuration <= 0f)
                {
                    chargeAdd = chargeLeft;
                }
                else
                {
                    chargeAdd = (1f / maxChargeDuration) * Time.deltaTime;
                }
                chargeAdd = Mathf.Clamp(chargeAdd, 0f, chargeLeft);

                // Charge Add에 따른 Ammo 소비량을 구한다
                float ammoChargeRequire = chargeAdd * ammoUseRateWhileCharging;
                if (ammoChargeRequire <= currentAmmo)
                {
                    UseAmmo(ammoChargeRequire);

                    CurrentCharge += chargeAdd;
                    CurrentCharge = Mathf.Clamp01(CurrentCharge);
                }
            }
        }

        // Ammo 연산
        private void UpdateAmmo()
        {
            // 재장전 - 자동
            if (automaticReload && currentAmmo < maxAmmo && (lastTimeShot + ammoReloadDelay) < Time.time
                && IsCharging == false)
            {
                currentAmmo += ammoReloadRate * Time.deltaTime;
                currentAmmo = Mathf.Clamp(currentAmmo, 0f, maxAmmo);
            }

            // Current Ammo Rate 연산
            if (maxAmmo == 0 || maxAmmo == Mathf.Infinity)
            {
                CurrentAmmoRate = 1f;
            }
            else
            {
                CurrentAmmoRate = currentAmmo / maxAmmo;
            }
        }

        public void ShowWeapon(bool show)
        {
            weaponRoot.SetActive(show);
            IsWeaponActive = show;

            // 무기 교체
            if (show == true && switchWeaponSfx)
            {
                shootAudioSource.PlayOneShot(switchWeaponSfx);
            }
        }

        // 슛 인풋 처리 : 매개 변수로 fire down, held, released 받아서 슈팅 타입 처리
        public bool HandleShootInput(bool inputDown, bool inputHeld, bool inputUp)
        {
            switch (shootType)
            {
                case WeaponShootType.Manual:
                    if (inputDown == true)
                    {
                        return TryShoot();
                    }
                    break;

                case WeaponShootType.Automatic:
                    if (inputHeld == true)
                    {
                        return TryShoot();
                    }
                    break;

                case WeaponShootType.Charge:
                    if (inputHeld == true)
                    {
                        TryBeginCharge();
                    }
                    else if (inputUp == true)
                    {
                        return TryReleaseCharge();
                    }
                    break;

                case WeaponShootType.Sniper:
                    if (inputDown == true)
                    {
                        return TryShoot();
                    }
                    break;
            }

            return false;
        }

        // 충전 처리 - 시작
        private bool TryBeginCharge()
        {
            if (IsCharging == false && currentAmmo >= ammoUseOnStartCharge
                && (lastTimeShot + delayBetweenShots) < Time.time)
            {
                UseAmmo(ammoUseOnStartCharge);

                lastChargeTriggerTimesTemp = Time.time;
                IsCharging = true;
                return true;
            }

            return false;
        }

        // 충전 처리 - 끝 - 발사
        private bool TryReleaseCharge()
        {
            if (IsCharging == true)
            {
                // 발사
                HandleShoot();

                // 초기화
                CurrentCharge = 0;
                IsCharging = false;
                return true;
            }

            return false;
        }

        // Ammo 연산
        private void UseAmmo(float amount)
        {
            currentAmmo -= amount;
            currentAmmo = Mathf.Clamp(currentAmmo, 0f, maxAmmo);

            // 발사한 시간 저장
            lastTimeShot = Time.time;
        }

        // 발사 처리
        private bool TryShoot()
        {
            if (currentAmmo >= 1f && (lastTimeShot + delayBetweenShots) < Time.time)
            {
                currentAmmo -= 1f;
                Debug.Log($"currentAmmo : {currentAmmo}");

                HandleShoot();
                return true;
            }

            return false;
        }

        // 발사 연출
        private void HandleShoot()
        {
            // fire 시 최종적으로 발생하는 발사체의 개수
            int bulletsPerShotFinal = bulletsPerShot;

            for (int i = 0; i < bulletsPerShotFinal; i++)
            {
                // 발사체가 나가는 방향을 랜덤하게 구한다
                Vector3 shotDirector = GetShotDirectionWithInSpread(weaponMuzzle);
                // 발사체 생성 후 슛
                ProjectileBase projectileBase = Instantiate(projectilePrefab, weaponMuzzle.position,
                    Quaternion.LookRotation(shotDirector));
                projectileBase.Shoot(this);
            }

            // VFX - Muzzle Effect
            if (muzzleFlashPrefab)
            {
                GameObject effectGo = Instantiate(muzzleFlashPrefab, weaponMuzzle.position, weaponMuzzle.rotation, weaponMuzzle);
                Destroy(effectGo, 1f);
            }

            // SFX
            if (shootSfx)
            {
                shootAudioSource.PlayOneShot(shootSfx);
            }

            // 발사한 시간 저장
            lastTimeShot = Time.time;
        }

        // 발사체가 나가는 방향 구하기
        private Vector3 GetShotDirectionWithInSpread(Transform shotTransform)
        {
            float spreadAngleRatio = bulletSpreadAngle / 180f;
            return Vector3.Slerp(shotTransform.forward, UnityEngine.Random.insideUnitSphere, spreadAngleRatio);
        }
        #endregion
    }
}

