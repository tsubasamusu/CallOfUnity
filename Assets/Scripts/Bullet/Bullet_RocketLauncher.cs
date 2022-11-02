using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// ロケットランチャー弾用のクラス
    /// </summary>
    public class Bullet_RocketLauncher : BulletDetailBase
    {
        /// <summary>
        /// ロケットランチャーの弾の生成直後に呼び出される
        /// </summary>
        private void Start()
        {
            //エフェクトを生成
            Transform effectTran = Instantiate(GameData.instance.ObjRocketLauncherEffect.transform);

            //エフェクトの向きを設定
            effectTran.forward = transform.forward;

            //エフェクトの移動
            this.UpdateAsObservable()
                .Subscribe(_ => effectTran.position = transform.position)
                .AddTo(this);

            //エフェクトの削除
            this.OnDestroyAsObservable()
                .Subscribe(_ => Destroy(effectTran.gameObject))
                .AddTo(this);
        }
    }
}
