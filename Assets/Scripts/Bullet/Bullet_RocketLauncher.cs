using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

namespace CallOfUnity
{
    /// <summary>
    /// ロケットランチャー弾用のクラス
    /// </summary>
    public class Bullet_RocketLauncher : BulletDetailBase
    {
        /// <summary>
        /// 初期設定を行う
        /// </summary>
        protected override void SetUp()
        {
            //エフェクトを生成する
            Transform effectTran = Instantiate(GameData.instance.ObjRocketLauncherEffect.transform);

            //生成したエフェクトの親を設定する
            effectTran.SetParent(transform);

            //エフェクトの向きを設定する
            effectTran.forward = -transform.forward;

            //エフェクトの移動
            this.UpdateAsObservable()
                .Subscribe(_ => effectTran.position = transform.position)
                .AddTo(this);

            //爆発
            this.OnCollisionEnterAsObservable()
                .Subscribe(_ => 
                {
                    //爆発のエフェクトを生成する
                    Transform explosionEffectTran = Instantiate(GameData.instance.ObjExplosionEffect.transform);

                    //爆発のエフェクトの位置を設定する
                    explosionEffectTran.position = transform.position;

                    //生成したエフェクトの親を設定
                    explosionEffectTran.SetParent(GameData.instance.TemporaryObjectContainerTran);

                    //生成したエフェクトを3秒後に消す
                    Destroy(explosionEffectTran.gameObject, 3f);
                })
                .AddTo(this);
        }
    }
}
