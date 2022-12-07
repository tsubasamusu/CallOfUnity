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

            //移動
            this.UpdateAsObservable()
                .Subscribe(_ =>transform.Translate(transform.forward * weaponData.bulletVelocity * Time.deltaTime))
                .AddTo(this);

            //着弾
            this.OnCollisionEnterAsObservable()
                .Subscribe(collision =>
                {
                    //プレイヤーとNPC以外のものに接触したら
                    if (!collision.transform.TryGetComponent(out ControllerBase controllerBase))
                    {
                        //弾を消す
                        Destroy(gameObject);
                    }
                })
                .AddTo(this);
        }
    }
}
