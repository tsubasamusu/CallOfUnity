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

        protected WeaponDataSO.WeaponData currentWeaponData;//使用中の武器

        protected int currentWeapoonNo;//使用中の武器の番号

        protected int bulletCountForNpc;//NPC用の残弾数

        protected Transform weaponTran;//武器の位置

        private GameObject objWeapon;//武器のオブジェクト

        protected bool isReloading;//リロード中かどうか

        protected bool isPlayer;//自分がプレイヤーかどうか

        /// <summary>
        /// 「自分がプレイヤーかどうか」の取得用
        /// </summary>
        public bool IsPlayer { get => isPlayer; }

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
            //リロード中に変更する
            isReloading = true;

            //自分がプレイヤーなら、リロードゲージのアニメーションを行う
            if(isPlayer)GameData.instance.UiManager.PlayImgReloadGaugeAnimation(currentWeaponData.reloadTime);

            //一定時間待つ
            await UniTask.Delay(TimeSpan.FromSeconds(currentWeaponData.reloadTime), cancellationToken: token);

            //使用中の武器の残弾数を初期値に設定
            SetBulletCount(currentWeapoonNo, currentWeaponData.ammunitionNo);

            //リロード終了状態に変更する
            isReloading = false;
        }

        /// <summary>
        /// 射撃する
        /// </summary>
        protected void Shot()
        {
            //使用中の武器の残弾数が「0」以下なら、以降の処理を行わない
            if (GetBulletcCount() <= 0) return;

            //使用中の武器の残弾数を更新する
            SetBulletCount(currentWeapoonNo,
                Math.Clamp(GetWeaponInfo(currentWeapoonNo).bulletCount - 1, 0, currentWeaponData.ammunitionNo));

            //自分がプレイヤーなら
            if (isPlayer)
            {
                //プレイヤーの総発射数を「1」増やす
                GameData.instance.playerTotalShotCount++;
            }

            //弾を生成する
            BulletDetailBase bullet = Instantiate(currentWeaponData.bullet);

            //生成した弾の初期設定を行う
            bullet.SetUpBullet(currentWeaponData, myTeamNo, isPlayer);

            //生成した弾の親を設定する
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //生成した弾の位置を設定する
            bullet.transform.position = weaponTran.position;

            //生成した弾の向きを設定する
            bullet.transform.forward = weaponTran.forward;

            //弾を発射する
            bullet.transform.GetComponent<Rigidbody>()
                .AddForce(weaponTran.forward * currentWeaponData.shotPower, ForceMode.Impulse);

            //エフェクトを生成する
            Transform effectTran = Instantiate(GameData.instance.ObjMuzzleFlashEffect.transform);

            //生成したエフェクトの位置を設定する
            effectTran.position = weaponTran.position;

            //生成したエフェクトの親を設定する
            effectTran.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //生成したエフェクトを0.2秒後に消す
            Destroy(effectTran.gameObject, 0.2f);
        }

        /// <summary>
        /// 現在使用している武器のリロード時間を取得する
        /// </summary>
        /// <returns>現在使用している武器のリロード時間</returns>
        protected float GetReloadTime()
        {
            //現在使用している武器のリロード時間を返す
            return currentWeaponData.reloadTime;
        }

        /// <summary>
        /// 現在使用している武器の残弾数を取得する
        /// </summary>
        /// <returns>現在使用している武器の残弾数</returns>
        public int GetBulletcCount()
        {
            //現在使用している武器の残弾数を返す
            return GetWeaponInfo(currentWeapoonNo).bulletCount;
        }

        /// <summary>
        /// 武器の情報を取得する
        /// </summary>
        /// <param name="weaponNo">武器の番号</param>
        /// <returns>（データ・残弾数）</returns>
        protected (WeaponDataSO.WeaponData weaponData, int bulletCount) GetWeaponInfo(int weaponNo)
        {
            //自分がNPCなら、NPC用の情報を返す
            if (!isPlayer) return (currentWeaponData, bulletCountForNpc);

            //受け取った番号に応じて戻り値を変更
            return weaponNo switch
            {
                0 => GameData.instance.playerWeaponInfo.info0,
                1 => GameData.instance.playerWeaponInfo.info1,
                _ => (null, -1)
            };
        }

        /// <summary>
        /// 武器の残弾数を設定する
        /// </summary>
        /// <param name="weaponNo">設定したい所持武器の番号</param>
        /// <param name="bulletCount">設定したい武器の残弾数</param>
        protected void SetBulletCount(int weaponNo, int bulletCount)
        {
            //自分がNPCなら
            if (!isPlayer)
            {
                //NPC用の残弾数を設定する
                bulletCountForNpc = bulletCount;

                //以降の処理を行わない
                return;
            }

            //受け取った所持武器の番号に応じて処理を変更
            switch (weaponNo)
            {
                case 0: GameData.instance.playerWeaponInfo.info0.bulletCount = bulletCount; break;
                case 1: GameData.instance.playerWeaponInfo.info1.bulletCount = bulletCount; break;
                default: Debug.Log("適切な武器の番号を指定してください"); break;
            }
        }
    }
}
