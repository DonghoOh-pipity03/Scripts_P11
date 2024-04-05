using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Project_11;
using Andtech.ProTracer;

namespace Project_11
{
    public class Weapon : MonoBehaviour
    {
        #region variable
        [Header("무기")]
        [SerializeField] LayerMask enemyLayer;
        [SerializeField] Transform pos_Weapon_Logic;  // 공격범위_UI 기준, 무기 로직 기준
        [SerializeField] Transform pos_weapon_muzzle; // 이펙트를 위한 총구 위치
        [SerializeField] Transform pos_weapon_Horizontal; // 애니메이션을 위한 포탑 위치 - 수평
        [SerializeField] Transform pos_weapon_Vertical; // 애니메이션을 위한 포탑 위치 - 수직
        Transform curTarget = null;  // 현재 공격 대상
        float firstFireTime;
        int totalFiredCount;

        [Header("UI")]
        [SerializeField] LayerMask cursorLayer; // 커서 위치를 찾기위한, 헬퍼의 레이어
        [SerializeField] float maxDist_CursorRay = 100;  // 커서 위치를 찾기 위한, 레이의 최대거리
        [SerializeField] GameObject fireRangeUI;    // 공격범위_UI - 게임오브젝트
        RectTransform fireRangeUI_Rect;    // 공격 범위 UI - RectTransform
        Image fireRangeUI_Image; // 공격 범위 UI - Image
        float fireRange_HalfAngle;    // 공격 범위 UI의 절반 각도
        Vector3 cursorWorldPos;    // 커서의 월드 위치, (단, 무기 로직의 월드 높이와 항상 같음)

        [Header("VFX")]
        [SerializeField] Bullet raytracer;  // 예광탄 - 프리팹
        [SerializeField] SmokeTrail smokeTrailPrefab; // 총알 연기 - 프리팹
        [SerializeField] float tracerSpeed = 100;

        [Header("개발옵션")]
        public bool useVFX = true;
        #endregion


        #region Singleton
        // 싱글톤
        private static Weapon DW;
        public static Weapon singleton
        {
            get
            {
                if (DW == null)
                {
                    DW = FindObjectOfType<Weapon>();
                    if (DW == null) Debug.Log("Damage_weapon을 사용하려 했지만, 없어요.");
                }
                return DW;
            }
        }
        #endregion


        #region Lifcycle_Function
        private void Awake()
        {
            fireRangeUI_Rect = fireRangeUI.GetComponent<RectTransform>();
            fireRangeUI_Image = fireRangeUI.GetComponent<Image>();
        }

        private void Start()
        {
            fireRange_HalfAngle = PlayerManager.singleton.fireAngle / 2;
            Set_FireRangeUI_Range(PlayerManager.singleton.fireRange);
            Set_FireRangeUI_HalfAngle(fireRange_HalfAngle);
        }

        void Update()
        {
            if (PlayerManager.singleton.isDead) return;

            Get_CursorWorldPos();

            RotateWeapon();
            Sync_Weapon_UI();
            Sync_Weapon_Anim();
            
            Find_ClosestEnemy();
            Attack();
        }
        #endregion


        #region GETSET
        // 커서의 월드 위치를 가져온다. (단, 위치는 무기로직 오브젝트의 Y 높이와 항상 같다.)
        void Get_CursorWorldPos()
        {
            // 인게임 UI 모드가 아닐시, 리턴
            if (!GameManager.singleton.isInGameUIMode) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxDist_CursorRay, cursorLayer))
            {
                cursorWorldPos = hit.point;

            }
        }

        public void Set_FireRangeUI_Range(float _range)
        {
            var newVal = _range * 2;
            fireRangeUI_Rect.localScale = new Vector3(newVal, newVal, newVal);
        }

        public void Set_FireRangeUI_HalfAngle(float _halfAngle)
        {
            fireRange_HalfAngle = Mathf.Abs(_halfAngle);
            fireRangeUI_Image.fillAmount = _halfAngle * 2 / 360;
        }
        #endregion


        #region Function
        // 커서 방향으로 무기를 회전한다.
        void RotateWeapon()
        {
            // 커서 방향 벡터 (월드 기준) 
            Vector3 targetDirection = cursorWorldPos - pos_Weapon_Logic.position;

            // 커서 방향 벡터를 무기 좌표계로 변환
            Vector3 targetDirection_local = pos_Weapon_Logic.InverseTransformPoint(pos_Weapon_Logic.position + targetDirection);

            // 커서 방향 벡터를 무기 좌표계에 수평이 되도록 수정
            targetDirection_local.y = 0;

            // Debug.DrawLine(pos_weapon_logic.position, pos_weapon_logic_parent.TransformPoint(targetDirection_local), Color.red);

            // 커서 방향 벡터를 월드 기준으로 변환
            Vector3 targetDirection_world = pos_Weapon_Logic.TransformDirection(targetDirection_local);

            // 회전을 위한 쿼터니언 생성
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection_world);

            // 회전
            pos_Weapon_Logic.rotation = Quaternion.RotateTowards(pos_Weapon_Logic.rotation, targetRotation, PlayerManager.singleton.weaponRotAngle_PerSec * Time.deltaTime);
        }

        // 사격 범위 내의 가장 가까운 적을 찾아 저장한다.
        void Find_ClosestEnemy()
        {
            curTarget = null;

            Collider[] colliders = Physics.OverlapSphere(pos_Weapon_Logic.position, PlayerManager.singleton.fireRange, enemyLayer);
            float closestDistance = PlayerManager.singleton.fireRange;

            foreach (var collider in colliders)
            {
                Transform newEnemyTransform = collider.transform;

                // 거리 계산
                float distance = Vector3.Distance(pos_Weapon_Logic.position, newEnemyTransform.position);

                // 가장 가까운 적 발견
                if (distance < closestDistance)
                {
                    // 적과의 방향벡터 구하기 - 월드 기준, y축은 사용안함
                    Vector3 targetDirection = newEnemyTransform.position - pos_Weapon_Logic.position;
                    targetDirection.y = 0;

                    // 무기의 방향벡터 구하기 - 월드 기준, y축은 사용안함 
                    Vector3 weaponDirection = pos_Weapon_Logic.transform.forward;
                    weaponDirection.y = 0;

                    // 적과 무기 사이의 각도
                    float angleToTarget = Vector3.Angle(targetDirection, weaponDirection);

                    // 사격 범위 내에 있을 경우
                    if (angleToTarget < fireRange_HalfAngle)
                    {
                        // closestObject에 가장 가까운 물체가 저장된다
                        curTarget = newEnemyTransform;
                        closestDistance = distance;
                    }
                }
            }
        }

        // 공격대상을 공격한다.
        void Attack()
        {
            // 1. 공격 대상이 없으면, 리턴
            if (curTarget == null)
            {
                // 공격이 끝났으므로, 발사 횟수를 초기화
                totalFiredCount = 0;
                return;
            }

            // 2. 공격 쿨타임 중이면 리턴
            // 첫 발사 시간을 초기화
            if (totalFiredCount == 0) firstFireTime = Time.time;

            // 첫 발사 때부터 지금까지, 발사해야 하는 횟수
            int fireCnt_Haveto = Mathf.FloorToInt((Time.time - firstFireTime) / PlayerManager.singleton.weaponCooltime) + 1;

            // 지금 프레임에서 발사해야 하는 횟수
            int fireCount_CurFrame = fireCnt_Haveto - totalFiredCount;

            // 현재 프레임에서 발사할 것이 없으면 리턴
            if (fireCount_CurFrame == 0) return;

            // 3. 공격 처리
            // 발사횟수 초기화
            totalFiredCount += fireCount_CurFrame;
            // 공격 - 현재 프레임에서 공격해야하는 만큼 공격처리
            for (int i = 0; i < fireCount_CurFrame; i++)
            {
                // 공격처리
                PlayerManager.singleton.SendDamage_P2E(curTarget.GetComponent<Health>(), PlayerManager.singleton.weaponDamage,
                                WeaponKind.Weapon, PlayerManager.singleton.damageKind, PlayerManager.singleton.explosionDist);

                // 공격 그래픽 처리
                // 미완성 구현
                // VFX - 예광탄 - 현재 방식으로는 FPS를 많이 소비함, 인스펙터에서 몇발당 1발이 예광탄을 정해서 구현해야함
                if (useVFX)
                {
                    Bullet bullet = Instantiate(raytracer);
                    SmokeTrail smokeTrail = Instantiate(smokeTrailPrefab);
                    bullet.Completed += OnCompleted;
                    smokeTrail.Completed += OnCompleted;

                    var visualTarget = curTarget.position + Random.onUnitSphere * PlayerManager.singleton.VisualAccuracy * (curTarget.position - pos_weapon_muzzle.position).magnitude;
                    bullet.DrawLine(pos_weapon_muzzle.position, visualTarget, tracerSpeed, 0);
                    smokeTrail.DrawLine(pos_weapon_muzzle.position, visualTarget, tracerSpeed, 0);
                }

                // 타겟 재탐색
                Find_ClosestEnemy();
                if (curTarget == null) return;
            }
        }

        // 외부 패키지 코드_TracerDemo
        private void OnCompleted(object sender, System.EventArgs e)
        {
            // Handle complete event here
            if (sender is TracerObject tracerObject) Destroy(tracerObject.gameObject);
        }
        #endregion


        #region UI
        // 사격 범위 UI의 방향을 무기와 일치시킨다.
        void Sync_Weapon_UI()
        {
            // 무기의 월드 회전 값을 Vector3로 가져오기 
            Quaternion worldRotation = pos_Weapon_Logic.rotation;
            Vector3 weaponDirection_Vector3 = worldRotation.eulerAngles;

            // 방향 벡터를 각도로 변환
            float angle = weaponDirection_Vector3.y;

            // 보정
            angle = angle + fireRange_HalfAngle;

            // UI 회전
            fireRangeUI_Rect.localEulerAngles = new Vector3(0, 0, angle);
        }

        // 현재 타겟의 위치로 회전한다.
        // 타겟이 없으면 UI위치와 일치시킨다.
        void Sync_Weapon_Anim()
        {
            Vector3 aimPos = Vector3.zero;

            if (curTarget != null) aimPos = curTarget.position;
            else aimPos = pos_Weapon_Logic.position + pos_Weapon_Logic.forward * 5;
            
            // 수평 회전
            // 'pos_weapon_Horizontal'과 'curTarget'과의 '월드 방향 벡터'를 구한다.
            Vector3 direction = (aimPos - pos_weapon_Horizontal.position).normalized;

            // 'pos_weapon_Horizontal의 부모'의 '로컬 방향 벡터'로 변환한다.
            Vector3 localDirection = pos_weapon_Horizontal.parent.InverseTransformDirection(direction);

            // y축을 제거한다.
            localDirection.y = 0;

            // 목표 회전을 계산
            Quaternion targetRotation = Quaternion.LookRotation(localDirection, Vector3.up);

            // 현재 회전에서 목표 회전까지 부드럽게 회전
            pos_weapon_Horizontal.localRotation
                = Quaternion.Slerp(pos_weapon_Horizontal.localRotation, targetRotation, Time.deltaTime * PlayerManager.singleton.weapon_rotationSpeed_H);


            // 수직 회전
            // 'pos_weapon_Vertical'과 'curTarget'과의 월드 방향 벡터를 구함
            Vector3 worldDirection = aimPos - pos_weapon_Vertical.position;

            // 월드 방향 벡터를 'pos_weapon_Horizontal'의 로컬 방향 벡터로 변환
            Vector3 localDirection_v = pos_weapon_Horizontal.InverseTransformDirection(worldDirection);

            // 로컬 방향 벡터와 'pos_weapon_Horizontal'의 xz 평면과의 각도를 구함
            float angle = Vector3.Angle(pos_weapon_Horizontal.up, localDirection_v);
            angle = angle - 90;

            // 각도 제한
            angle = Mathf.Clamp(angle, -PlayerManager.singleton.weapon_maxVertical, PlayerManager.singleton.weapon_maxVertical);

            // 위에서 구한 각도를 'pos_weapon_Vertical'의 로컬 x 축에 적용 (y와 z의 회전 값은 항상 0)
            Quaternion targetRotation_V = Quaternion.Euler(angle, 0, 0);
            pos_weapon_Vertical.localRotation
                = Quaternion.Slerp(pos_weapon_Vertical.localRotation, targetRotation_V, Time.deltaTime * PlayerManager.singleton.weapon_rotationSpeed_V);
        }

        // 사격 범위 UI를 끈다.
        public void SetActive_Weapon_UI(bool _act) => fireRangeUI.SetActive(_act);
        #endregion
    }
}