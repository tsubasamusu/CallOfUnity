using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// キャラクターの体力を管理する
    /// </summary>
    public class CharacterHealth : MonoBehaviour, ISetUp
    {
        private float hp = 100f;//HP

        //HPの取得用
        public float Hp { get => hp; }

        /// <summary>
        /// CharacterHealthの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //弾に触れた際の処理
            this.OnCollisionEnterAsObservable()
                .Where(other => other.transform.TryGetComponent(out BulletDetailBase bullet))
                .Subscribe(other =>
                {
                    //ダメージを取得
                    float damage = other.transform.GetComponent<BulletDetailBase>().weaponData.attackPower;

                    //HPを更新
                    hp = Mathf.Clamp(hp - damage, 0f, 100f);

                    //HPが0なら死亡処理を行う
                    if (hp == 0f) Die();
                })
                .AddTo(this);

            //死亡処理
            void Die()
            {
                //TODO:死亡処理
                Debug.Log("死亡");
            }
        }
    }
}
