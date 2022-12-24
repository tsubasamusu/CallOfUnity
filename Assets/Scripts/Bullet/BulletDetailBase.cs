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

        /// <summary>
        /// 「使用された武器のデータ」の取得用
        /// </summary>
        public WeaponDataSO.WeaponData WeaponData { get => weaponData; }

        /// <summary>
        /// 弾の初期設定を行う
        /// </summary>
        /// <param name="weaponData">使用した武器のデータ</param>
        public void SetUpBullet(WeaponDataSO.WeaponData weaponData)
        {
            //使用された武器のデータを取得
            this.weaponData = weaponData;

            //初期設定を行う
            SetUp();

            //自滅
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //弾のy座標が戦場外に行ったら
                    if (transform.position.y < 0f || transform.position.y > 5f)
                    {
                        //弾を消す
                        Destroy(gameObject);

                        //以降の処理を行わない
                        return;
                    }

                    //ステージから離れすぎたら
                    if (Mathf.Abs((transform.position - Vector3.zero).magnitude) > ConstData.MAX_LENGTH_FROM_CENTER)
                    {
                        //自滅する
                        Destroy(gameObject);
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

                        ///生成したエフェクトの位置を設定する
                        effectTran.position = transform.position;

                        //生成したエフェクトの向きを設定する
                        effectTran.forward = -transform.forward;

                        //生成したエフェクトの親を設定する
                        effectTran.SetParent(controllerBase.transform);

                        //生成したエフェクトを消す
                        Destroy(effectTran.gameObject, 0.2f);
                    }

                    //弾を消す
                    Destroy(gameObject);
                })
                .AddTo(this);
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
