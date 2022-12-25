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

        private ControllerBase controllerBase;//ControllerBase

        //HPの取得用
        public float Hp { get => hp; }

        /// <summary>
        /// CharacterHealthの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //Rigidbodyを取得する
            Rigidbody rb = GetComponent<Rigidbody>();

            //ControllerBaseを取得する
            controllerBase = GetComponent<ControllerBase>();

            //弾に触れた際の処理
            this.OnCollisionEnterAsObservable()
                .Where(collision => collision.transform.TryGetComponent(out BulletDetailBase _))
                .Subscribe(collision =>
                {
                    //物理演算を無効化する
                    if (!rb.isKinematic) rb.isKinematic = true;

                    //BulletdetailBaseを取得する
                    BulletDetailBase bulletDetailBase = collision.transform.GetComponent<BulletDetailBase>();

                    //触れた弾が敵チームの弾なら
                    if (bulletDetailBase.MyTeamNo != controllerBase.myTeamNo)
                    {
                        //HPを更新する
                        hp = Mathf.Clamp(hp - bulletDetailBase.WeaponData.attackPower, 0f, 100f);
                    }

                    //触れた弾を消す
                    Destroy(collision.gameObject);

                    //HPが0なら死亡処理を行う
                    if (hp == 0f) Die(bulletDetailBase);
                })
                .AddTo(this);
        }

        /// <summary>
        /// 死亡処理
        /// </summary>
        /// <param name="bulletDetailBase">BulletDetailBase</param>
        public void Die(BulletDetailBase bulletDetailBase = null)
        {
            //自分がプレイヤーなら、プレイヤーのデス数を「1」増やす
            if(controllerBase.IsPlayer)GameData.instance.playerDieCount++;

            //プレイヤーの弾によって死亡したら
            if (bulletDetailBase != null && bulletDetailBase.IsPlayerBullet)
            {
                //プレイヤーのキル数を「1」増やす
                GameData.instance.playerKillCount++;
            }

            //自分がチーム0なら
            if (controllerBase.myTeamNo == 0)
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
            if (GameData.instance.score.team0 >= ConstData.WIN_SCORE)
            {
                //TODO:チーム0勝利時の処理

                //以降の処理を行わない
                return;
            }
            //チーム1が勝利したら
            else if (GameData.instance.score.team1 >= ConstData.WIN_SCORE)
            {
                //TODO:チーム1勝利時の処理

                //以降の処理を行わない
                return;
            }

            //再設定する
            controllerBase.ReSetUp();

            //HPを初期値に戻す
            hp = 100f;

            //リスポーンする
            transform.position = controllerBase.myTeamNo == 0 ?
                GameData.instance.RespawnTransList[0].position
                : GameData.instance.RespawnTransList[1].position;
        }
    }
}
