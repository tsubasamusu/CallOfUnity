using System;
using System.Collections.Generic;
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
        public List<WeaponDataSO.WeaponData> weaponDataList = new();//所持武器のデータのリスト

        [HideInInspector]
        public WeaponDataSO.WeaponData currentWeapon;//使用中の武器

        private int allBulletCount;//総残弾数

        private List<int> bulletCountList = new ();//それぞれの武器の残弾数のリスト

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
        public List<int> BulletCounts { get => bulletCountList; }

        /// <summary>
        /// ControllerBaseの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //武器の位置を取得
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //所持できる武器の数だけ繰り返す
            for (int i = 0; i < ConstData.WEAPONS_NUMBER_I_HAVE; i++)
            {
                //所持武器のリストに空箱を作る
                weaponDataList.Add(null);

                //それぞれの武器の残弾数のリストに空箱を作る
                bulletCountList.Add(0);
            }

            //仮
            weaponDataList[0] = GameData.instance.WeaponDataSO.weaponDataList[0];
            weaponDataList[1] = GameData.instance.WeaponDataSO.weaponDataList[2];

            //再設定する
            ReSetUp();

            //子クラスの初期設定を行う
            SetUpController();
        }

        /// <summary>
        /// 再設定する
        /// </summary>
        public void ReSetUp()
        {
            //総残弾数を初期値に設定
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

            //それぞれの武器の残弾数のリストの要素数だけ繰り返す
            for (int i = 0;i<bulletCountList.Count;i++)
            {
                //それぞれの武器の残弾数を最大数にする
                bulletCountList[i] = weaponDataList[i].ammunitionNo;
            }

            //使用中の武器を初期値に設定
            currentWeapon = weaponDataList[0];
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
            bulletCountList[GetCurrentWeaponNo()] = currentWeapon.ammunitionNo;

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
            bulletCountList[GetCurrentWeaponNo()]
                = Math.Clamp(bulletCountList[GetCurrentWeaponNo()] - 1, 0, currentWeapon.ammunitionNo);

            //弾を生成する
            BulletDetailBase bullet = Instantiate(currentWeapon.bullet);

            //生成した弾の親を設定する
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //生成した弾の位置を設定する
            bullet.transform.position = weaponTran.position;

            //生成した弾の向きを設定する
            bullet.transform.forward = isPlayer ? Camera.main.transform.forward : transform.forward;

            //生成した弾の初期設定を行う
            bullet.SetUpBullet(currentWeapon);
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
        /// <returns>使用中の武器の番号</returns>
        protected int GetCurrentWeaponNo()
        {
            //所持武器の番号を返す
            return weaponDataList.IndexOf(currentWeapon) == -1 ? 0 : weaponDataList.IndexOf(currentWeapon);
        }

        /// <summary>
        /// 現在使用している武器の残弾数を取得する
        /// </summary>
        /// <returns>現在使用している武器の残弾数</returns>
        protected int GetAmmunitionRemaining()
        {
            //現在使用している武器の残弾数を返す
            return bulletCountList[GetCurrentWeaponNo()];
        }
    }
}
