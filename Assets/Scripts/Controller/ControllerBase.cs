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
        public WeaponDataSO.WeaponData currentWeaponData;//使用中の武器

        private int allBulletCount;//総残弾数

        protected int currentWeapoonNo;//使用中の武器の番号

        protected Transform weaponTran;//武器の位置

        protected GameObject objWeapon;//武器のオブジェクト

        protected bool isReloading;//リロード中かどうか

        /// <summary>
        /// 「総残弾数」の取得用
        /// </summary>
        public int AllBulletCount { get => allBulletCount; }

        /// <summary>
        /// ControllerBaseの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //武器の位置を取得
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //子クラスの初期設定を行う
            SetUpController();

            //再設定する
            ReSetUp();

            //武器のオブジェクトを表示する
            DisplayObjWeapon();
        }

        /// <summary>
        /// 武器のオブジェクトを表示する
        /// </summary>
        protected void DisplayObjWeapon()
        {
            //武器のオブジェクトが既に生成してあるなら、それを消す
            if (objWeapon != null) Destroy(objWeapon);

            //武器のオブジェクトを生成する
            objWeapon = Instantiate(currentWeaponData.objWeapon);

            //生成した武器の親を設定
            objWeapon.transform.SetParent(weaponTran.transform);

            //生成した武器の位置・角度を設定
            objWeapon.transform.localPosition = objWeapon.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// 再設定する
        /// </summary>
        public virtual void ReSetUp()
        {
            //総残弾数を初期値に設定
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

            //子クラスで処理を記述する
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
            await UniTask.Delay(TimeSpan.FromSeconds(currentWeaponData.reloadTime), cancellationToken: token);

            //総残弾数を更新する
            allBulletCount = Math.Clamp(allBulletCount - GetRequiredBulletCount(), 0, ConstData.FIRST_ALL_BULLET_COUNT);

            //使用中の武器の残弾数を初期値に設定
            UpdateBulletCount(currentWeapoonNo, currentWeaponData.ammunitionNo);

            //リロード終了状態に変更する
            isReloading = false;

            //リロードに必要な弾数を取得する
            int GetRequiredBulletCount()
            {
                //使用中の武器の最大装弾数と、使用中の武器の現在の装弾数との差を返す
                return currentWeaponData.ammunitionNo - GetAmmunitionRemaining();
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
            UpdateBulletCount(currentWeapoonNo,
                Math.Clamp(GetWeaponInformation(currentWeapoonNo).bulletCount - 1, 0, currentWeaponData.ammunitionNo));

            //弾を生成する
            BulletDetailBase bullet = Instantiate(currentWeaponData.bullet);

            //生成した弾の親を設定する
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //生成した弾の位置を設定する
            bullet.transform.position = weaponTran.position;

            //生成した弾の進行方向を設定する
            Vector3 moveDir = TryGetComponent(out PlayerController _) ? Camera.main.transform.forward : transform.forward;

            //生成した弾の初期設定を行う
            bullet.SetUpBullet(currentWeaponData, moveDir);
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
            return currentWeaponData.reloadTime;
        }

        /// <summary>
        /// 現在使用している武器の残弾数を取得する
        /// </summary>
        /// <returns>現在使用している武器の残弾数</returns>
        protected int GetAmmunitionRemaining()
        {
            //現在使用している武器の残弾数を返す
            return GetWeaponInformation(currentWeapoonNo).bulletCount;
        }

        /// <summary>
        /// 武器の情報を取得する
        /// </summary>
        /// <param name="weaponNo">武器の番号</param>
        /// <returns>（データ・残弾数）</returns>
        protected (WeaponDataSO.WeaponData weaponData, int bulletCount) GetWeaponInformation(int weaponNo)
        {
            //受け取った番号に応じて戻り値を変更
            return weaponNo switch
            {
                0 => GameData.instance.playerWeaponInfo.infomation0,
                1 => GameData.instance.playerWeaponInfo.infomation1,
                _ => (null, -1)
            };
        }

        /// <summary>
        /// 武器の残弾数を更新する
        /// </summary>
        /// <param name="weaponNo">設定したい所持武器の番号</param>
        /// <param name="bulletCount">設定したい武器の残弾数</param>
        protected void UpdateBulletCount(int weaponNo, int bulletCount)
        {
            //受け取った所持武器の番号に応じて処理を変更
            switch (weaponNo)
            {
                case 0: GameData.instance.playerWeaponInfo.infomation0.bulletCount0 = bulletCount; break;
                case 1: GameData.instance.playerWeaponInfo.infomation1.bulletCount1 = bulletCount; break;
                default: Debug.Log("適切な武器の番号を指定してください"); break;
            }
        }
    }
}
