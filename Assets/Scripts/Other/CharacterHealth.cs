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
                .Where(collision => collision.transform.TryGetComponent(out BulletDetailBase _))
                .Subscribe(collision =>
                {
                    //ダメージを取得
                    float damage = collision.transform.GetComponent<BulletDetailBase>().WeaponData.attackPower;

                    //HPを更新
                    hp = Mathf.Clamp(hp - damage, 0f, 100f);

                    //HPが0なら死亡処理を行う
                    if (hp == 0f) Die();
                })
                .AddTo(this);

            //死亡処理
            void Die()
            {
                //ControllerBaseを取得できなかったら
                if(!TryGetComponent(out ControllerBase controllerBase))
                {
                    //問題を報告
                    Debug.Log("ControllerBaseを取得できませんでした");

                    //以降の処理を行わない
                    return;
                }

                //自分がチーム0なら
                if(controllerBase.myTeamNo== 0)
                {
                    //チーム1の得点を増やす
                    GameData.instance.score.team1++;
                }
                //自分がチーム1なら
                else
                {
                    //チーム0の得点を増やす
                    GameData.instance.score.team0++;
                }

                //チーム0が勝利したら
                if(GameData.instance.score.team0 >=ConstData.WIN_SCORE)
                {
                    //TODO:チーム0勝利時の処理
                    Debug.Log("チーム0の勝ち");

                    //以降の処理を行わない
                    return;
                }
                //チーム1が勝利したら
                else if(GameData.instance.score.team1>=ConstData.WIN_SCORE)
                {
                    //TODO:チーム1勝利時の処理
                    Debug.Log("チーム1の勝ち");

                    //以降の処理を行わない
                    return;
                }

                //再設定する
                controllerBase.ReSetUp();

                //リスポーンする
                transform.position = controllerBase.myTeamNo == 0 ? 
                    GameData.instance.RespawnTransList[0].position
                    : GameData.instance.RespawnTransList[1].position;
            }
        }
    }
}
