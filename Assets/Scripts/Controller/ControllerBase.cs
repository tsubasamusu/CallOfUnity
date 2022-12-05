using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// PlayerControllerクラスとNPCControllerクラスの親クラス
    /// </summary>
    public class ControllerBase : MonoBehaviour, ISetUp
    {
        [HideInInspector]
        public int myTeamNo;//自分のチーム番号

        [HideInInspector]
        public WeaponDataSO.WeaponData[] weaponDatas = new WeaponDataSO.WeaponData[2];//所持武器のデータ

        [HideInInspector]
        public WeaponDataSO.WeaponData currentWeapon;//使用中の武器

        private int allBulletCount;//総残弾数

        private int[] bulletCounts = new int[2];//それぞれの武器の残弾数

        protected Transform weaponTran;//武器の位置

        protected bool isReloading;//リロード中かどうか

        protected bool isPlayer;//プレイヤーかどうか

        /// <summary>
        /// 「総残弾数」の取得用
        /// </summary>
        public int AllBulletCount { get => allBulletCount; }

        /// <summary>
        /// 「それぞれの武器の残弾数」の取得用
        /// </summary>
        public int[] BulletCounts { get => bulletCounts; }

        /// <summary>
        /// ControllerBaseの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //武器の位置を取得
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //総残弾数を初期値に設定
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

            //子クラスの初期設定を行う
            SetUpController();

            //所持武器の数だけ繰り返す
            for (int i = 0; i < weaponDatas.Length; i++)
            {
                //所持武器が設定されているなら
                if (weaponDatas[i] != null)
                {
                    //使用中の武器を設定
                    if (i == 0) currentWeapon = weaponDatas[i];
                }
                //所持武器が設定されていないなら
                else
                {
                    //問題を報告
                    Debug.Log("所持武器が設定されていません");
                }
            }
        }

        /// <summary>
        /// 子クラスの初期設定を行う
        /// </summary>
        protected virtual void SetUpController()
        {
            //子クラスで処理を記述する
        }

        /// <summary>
        /// 接地判定を行う
        /// </summary>
        /// <returns>接地していたらtrue</returns>
        protected bool CheckGrounded()
        {
            //光線を作成 
            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

            //光線が他のコライダーに接触したらtrueを返す
            return Physics.Raycast(ray, 0.15f);
        }

        /// <summary>
        /// リロードする
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>待ち時間</returns>
        protected async UniTaskVoid ReloadAsync(CancellationToken token)
        {
            //リロードできるだけの弾が残っていなければ、以降の処理を行わない
            if (allBulletCount < GetRequiredBulletCount()) return;

            //リロード中に変更する
            isReloading = true;

            //一定時間待つ
            await UniTask.Delay(TimeSpan.FromSeconds(currentWeapon.reloadTime), cancellationToken: token);

            //総残弾数を更新する
            allBulletCount = Math.Clamp(allBulletCount - GetRequiredBulletCount(), 0, ConstData.FIRST_ALL_BULLET_COUNT);

            //使用中の武器の残弾数を初期値に設定
            bulletCounts[GetCurrentWeaponNo()] = currentWeapon.ammunitionNo;

            //リロード終了状態に変更する
            isReloading = false;

            //リロードに必要な弾数を取得する
            int GetRequiredBulletCount()
            {
                //使用中の武器の最大装弾数と、使用中の武器の現在の装弾数との差を返す
                return currentWeapon.ammunitionNo - GetAmmunitionRemaining();
            }
        }

        /// <summary>
        /// 射撃する
        /// </summary>
        protected void Shot()
        {
            //リロード中か、残弾数が「0」なら、以降の処理を行わない
            if (isReloading || GetAmmunitionRemaining() == 0) return;

            //総残弾数を更新する
            allBulletCount = Math.Clamp(allBulletCount - 1, 0, ConstData.FIRST_ALL_BULLET_COUNT);

            //使用中の武器の残弾数を更新する
            bulletCounts[GetCurrentWeaponNo()] = Math.Clamp(bulletCounts[GetCurrentWeaponNo()] - 1, 0, currentWeapon.ammunitionNo);

            //弾を生成する
            BulletDetailBase bullet = Instantiate(currentWeapon.bullet);

            //生成した弾の親を設定する
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //生成した弾の位置を設定する
            bullet.transform.position = weaponTran.position;

            //生成した弾の向きを設定する
            bullet.transform.forward = isPlayer ? Camera.main.transform.forward : transform.forward;

            //生成した弾を飛ばす
            bullet.Move();
        }

        /// <summary>
        /// 現在使用している武器のリロード時間を取得する
        /// </summary>
        /// <returns>現在使用している武器のリロード時間</returns>
        protected float GetReloadTime()
        {
            //リロード中か、残弾数が「0」なら、リロード時間を「0」で返す
            if (isReloading || GetAmmunitionRemaining() == 0) return 0f;

            //現在使用している武器のリロード時間を返す
            return currentWeapon.reloadTime;
        }

        /// <summary>
        /// 現在使用している武器の連射速度を取得する
        /// </summary>
        /// <returns>現在使用している武器の連射速度</returns>
        protected float GetRateOfFire()
        {
            //現在使用している武器の連射速度を返す
            return currentWeapon.rateOfFire;
        }

        /// <summary>
        /// 使用中の武器の番号を取得する
        /// </summary>
        /// <returns>使用中の武器の番号（0or1）</returns>
        private int GetCurrentWeaponNo()
        {
            //所持武器の数だけ繰り返す
            for (int i = 0; i < weaponDatas.Length; i++)
            {
                //繰り返し処理で取得した武器が使用中の武器と一致したなら
                if (weaponDatas[i] == currentWeapon)
                {
                    //繰り返し回数を返す
                    return i;
                }
            }

            //問題を報告
            Debug.Log("使用中の武器が所持武器の中にありません");

            //仮
            return 0;
        }

        /// <summary>
        /// 現在使用している武器の残弾数を取得する
        /// </summary>
        /// <returns>現在使用している武器の残弾数</returns>
        protected int GetAmmunitionRemaining()
        {
            //現在使用している武器の残弾数を返す
            return bulletCounts[GetCurrentWeaponNo()];
        }
    }
}
