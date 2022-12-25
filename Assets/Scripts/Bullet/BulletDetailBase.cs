using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// 全ての弾の親クラス
    /// </summary>
    public class BulletDetailBase : MonoBehaviour
    {
        private WeaponDataSO.WeaponData weaponData;//使用された武器のデータ

        private int myTeamNo;//自分のチーム番号

        private bool isPlayerBullet;//プレイヤーの弾かどうか

        /// <summary>
        /// 「使用された武器のデータ」の取得用
        /// </summary>
        public WeaponDataSO.WeaponData WeaponData { get => weaponData; }

        /// <summary>
        /// 「自分のチーム番号」の取得用
        /// </summary>
        public int MyTeamNo { get => myTeamNo; }

        /// <summary>
        /// 「プレイヤーの弾かどうか」の取得用
        /// </summary>
        public bool IsPlayerBullet { get => isPlayerBullet; }

        /// <summary>
        /// 弾の初期設定を行う
        /// </summary>
        /// <param name="weaponData">使用した武器のデータ</param>
        /// <param name="myTeamNo">自分のチーム番号</param>
        /// <param name="isPlayerBullet">プレイヤーの弾かどうか</param>
        public void SetUpBullet(WeaponDataSO.WeaponData weaponData,int myTeamNo,bool isPlayerBullet)
        {
            //使用された武器のデータを取得する
            this.weaponData = weaponData;

            //自分のチーム番号を取得する
            this.myTeamNo = myTeamNo;

            //「プレイヤーの弾かどうか」を取得する
            this.isPlayerBullet = isPlayerBullet;

            //初期設定を行う
            SetUp();

            //自滅
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //弾のy座標が戦場外に行ったら
                    if (transform.position.y < 0f || transform.position.y > 5f)
                    {
                        //自滅する
                        DestroyBullet(false);

                        //以降の処理を行わない
                        return;
                    }

                    //ステージから離れすぎたら
                    if (Mathf.Abs((transform.position - Vector3.zero).magnitude) > ConstData.MAX_LENGTH_FROM_CENTER)
                    {
                        //自滅する
                        DestroyBullet(false);
                    }
                })
                .AddTo(this);

            //着弾
            this.OnCollisionEnterAsObservable()
                .Subscribe(collision =>
                {
                    //プレイヤーかNPCに接触したら
                    if (collision.transform.TryGetComponent(out ControllerBase controllerBase))
                    {
                        //エフェクトを生成する
                        Transform effectTran = Instantiate(GameData.instance.ObjBleedingEffect.transform);

                        //生成したエフェクトの位置を設定する
                        effectTran.position = transform.position;

                        //生成したエフェクトの向きを設定する
                        effectTran.forward = -transform.forward;

                        //生成したエフェクトの親を設定する
                        effectTran.SetParent(controllerBase.transform);

                        //生成したエフェクトを消す
                        Destroy(effectTran.gameObject, 0.2f);

                        //自滅する
                        DestroyBullet(controllerBase.myTeamNo != myTeamNo);
                    }

                    //自滅する
                    DestroyBullet(false);
                })
                .AddTo(this);

            //自滅する
            void DestroyBullet(bool attackedEnemy)
            {
                Debug.Log(GameData.instance.playerAttackCount);

                //敵に攻撃していないなら
                if (!attackedEnemy)
                {
                    //弾を消す
                    Destroy(gameObject);
                }
                //自分がプレイヤーかつ、敵に攻撃したなら
                else if(isPlayerBullet&&attackedEnemy)
                {
                    //プレイヤーの命中数を「1」増やす
                    GameData.instance.playerAttackCount++;
                }
            }
        }

        /// <summary>
        /// 初期設定を行う
        /// </summary>
        protected virtual void SetUp()
        {
            //各子クラスで処理を記述する
        }
    }
}
