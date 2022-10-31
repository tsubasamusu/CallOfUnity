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
        [HideInInspector]
        public WeaponDataSO.WeaponData weaponData;//武器のデータ

        /// <summary>
        /// 移動する
        /// </summary>
        public void Move()
        {
            //移動
            this.UpdateAsObservable()
                .Subscribe(_ => transform.Translate(transform.forward * weaponData.bulletVelocity * Time.deltaTime))
                .AddTo(this);
        }
    }
}
