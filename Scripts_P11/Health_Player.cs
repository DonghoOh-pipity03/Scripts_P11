using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Project_11;

namespace Project_11
{
    public class Health_Player : Health
    {
        [HideInInspector]
        private new int maxHP => PlayerManager.singleton.maxHP;

        [Header("UI")]
        [SerializeField] Image[] HP_Bars;
        float init_HP_Bar_Width;
        float init_HP_Bar_Height;

        [Header("죽음")]
        [SerializeField] GameObject liveRenderer;
        [SerializeField] GameObject deadRenderer;

        #region Singleton
        // 싱글톤
        private static Health_Player HP;
        public static Health_Player singleton
        {
            get
            {
                if (HP == null)
                {
                    HP = FindObjectOfType<Health_Player>();
                    if (HP == null) Debug.Log("Health_Player를 사용하려 했지만, 없어요.");
                }
                return HP;
            }
        }
        #endregion


        protected override void Awake()
        {
            base.Awake();

            if (HP_Bars != null)
            {
                init_HP_Bar_Width = HP_Bars[0].rectTransform.sizeDelta.x;
                init_HP_Bar_Height = HP_Bars[0].rectTransform.sizeDelta.y;
            }
        }

        /* private new void OnEnable()
        {
            curHP = maxHP;
        } */

        protected override void Update()
        {
            base.Update();

            if (HP_Bars != null) Update_HP_UI();
        }

        public override void GetDamage(int _Damage, WeaponKind _DamageKind)
        {
            if (isDead) return;

            curHP -= _Damage;
            if (curHP <= 0) Dead();
        }

        void Update_HP_UI()
        {
            int barCount = Mathf.Max(((int)curHP - 1), 0) / 100;    // 체력의 백 자리 수
            int curHP_00 = (int)curHP % 100;    // 체력의 십과 일의 자리 수
            curHP_00 = (curHP_00 == 0) ? 100 : curHP_00;

            // 체력 백 자리 수에 따라, 체력바 컬러를 표시
            for (int i = 0; i < HP_Bars.Length; i++)
            {
                if (i <= barCount)
                {
                    HP_Bars[i].enabled = true;
                    HP_Bars[i].rectTransform.sizeDelta = new Vector2(init_HP_Bar_Width, init_HP_Bar_Height);
                }
                else HP_Bars[i].enabled = false;
            }

            // 체력 십과 일의 자리 수에 따라, 체력바 길이를 표시
            if (curHP != 0)
                HP_Bars[barCount].rectTransform.sizeDelta = new Vector2(init_HP_Bar_Width / 100 * curHP_00, init_HP_Bar_Height);
            else HP_Bars[barCount].rectTransform.sizeDelta = new Vector2(0, init_HP_Bar_Height);
        }

        protected override void Dead()
        {
            base.Dead();
            PlayerManager.singleton.PlayerDead();

            if (liveRenderer != null) liveRenderer.SetActive(false);
            if (deadRenderer != null) deadRenderer.SetActive(true);

            // 자폭
            SelfExplosion(PlayerManager.singleton.selfExp_damage, PlayerManager.singleton.selfExp_dist);

            // 이펙트 - 폭발
            GameObject obj = EffectManager.singleton.GetPooledObject(EffectKind.Explosion);
            if (obj != null)
            {
                obj.transform.position = transform.position + Vector3.up;
                obj.transform.localScale = Vector3.one * PlayerManager.singleton.selfExp_dist;
            }

            // UI끄기
            Weapon.singleton.SetActive_Weapon_UI(false);
            GameManager.singleton.Cursor_Menu();
        }
    }
}