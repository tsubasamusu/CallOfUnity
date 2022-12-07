using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CallOfUnity
{
    /// <summary>
    /// プレイヤーの動きを制御する
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : ControllerBase
    {
        private Rigidbody rb;//Rigidbody

        /// <summary>
        /// PlayerControllerの初期設定を行う
        /// </summary>
        protected override void SetUpController()
        {
            //リセット時の処理を呼び出す 
            Reset();

            //移動・かがむ・武器チェンジ・ジャンプ
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //移動する
                    transform.Translate(
                        //水平方向の入力を取得
                        Vector3.Scale((Camera.main.transform.right * Input.GetAxis("Horizontal")
                        //垂直方向の入力を取得
                        + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                        //y成分を0にする
                        , new Vector3(1f, 0f, 1f))
                        //移動スピードを取得
                        * (Input.GetKey(ConstData.RUN_KEY) ? ConstData.RUN_SPEED : ConstData.WALK_SPEED)
                        //時間を掛ける
                        * Time.deltaTime);

                    //武器チェンジキーが押されたら武器をチェンジする
                    if (Input.GetKeyDown(ConstData.CHANGE_WEAPON_KEY)) ChangeWeapon();

                    //構えるキーが離されたら
                    if (Input.GetKeyUp(ConstData.STANCE_KEY))
                    {
                        //構えるのをやめる
                        Camera.main.DOFieldOfView(ConstData.NORMAL_FOV, ConstData.STANCE_TIME);
                    }
                })
                .AddTo(this);

            //重力を生成する
            this.UpdateAsObservable()
                .Where(_ => !CheckGrounded())
                .Subscribe(_ => transform.Translate(Vector3.down * GameData.instance.gravity * Time.deltaTime))
                .AddTo(this);

            //リロード
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.RELOAD_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(GetReloadTime()))
                .Subscribe(_ => ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget())
                .AddTo(this);

            //射撃
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(ConstData.SHOT_KEY) && GetAmmunitionRemaining() > 0)
                .ThrottleFirst(TimeSpan.FromSeconds(currentWeaponData.rateOfFire))
                .Subscribe(_ => Shot())
                .AddTo(this);

            //構える
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.STANCE_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(ConstData.STANCE_TIME))
                .Subscribe(_ =>Camera.main.DOFieldOfView(ConstData.STANCE_FOV, ConstData.STANCE_TIME))
                .AddTo(this);

            //重力の初期値を取得
            float firstGravity = GameData.instance.gravity;

            //ジャンプ
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.JUMP_KEY) && CheckGrounded())
                .Subscribe(_ => JumpAsync(this.GetCancellationTokenOnDestroy(), firstGravity).Forget())
                .AddTo(this);
        }

        /// <summary>
        /// 再設定する
        /// </summary>
        public override void ReSetUp()
        {
            //親クラスの処理を行う
            base.ReSetUp();

            //各武器の残弾数を最大値に設定する
            UpdateBulletCount(0, GetWeaponInformation(0).weaponData.ammunitionNo);
            UpdateBulletCount(1, GetWeaponInformation(1).weaponData.ammunitionNo);
        }

        /// <summary>
        /// リセット時に呼び出される
        /// </summary>
        private void Reset()
        {
            //Rigidbodyを取得
            rb = GetComponent<Rigidbody>();

            //自分のチーム番号を設定
            myTeamNo = 0;

            //使用中の武器の番号を初期値に設定
            currentWeapoonNo= 0;

            //使用中の武器のデータを初期値に設定
            currentWeaponData = GetWeaponInformation(0).weaponData;
        }

        /// <summary>
        /// ジャンプする
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <param name="firstGravity">重力の初期値</param>
        /// <returns>待ち時間</returns>
        private async UniTaskVoid JumpAsync(CancellationToken token,float firstGravity)
        {
            //物理演算を開始
            rb.isKinematic = false;

            //力を加える
            rb.AddForce(Vector3.up * ConstData.JUMP_POWER, ForceMode.Impulse);

            //重力を無効化
            GameData.instance.gravity = 0f;

            //完全にジャンプするまで待つ
            await UniTask.Delay(TimeSpan.FromSeconds(ConstData.WAIT_JUMP_TIME), cancellationToken: token);

            //キャラクターの角度を初期化
            transform.eulerAngles = Vector3.zero;

            //物理演算を終了
            rb.isKinematic = true;

            //重力を初期値に設定
            GameData.instance.gravity = firstGravity;

            //着地するまで待つ
            await UniTask.WaitUntil(() => CheckGrounded(), cancellationToken: token);
        }

        /// <summary>
        /// 武器をチェンジする
        /// </summary>
        private void ChangeWeapon()
        {
            //リロード中か、射撃中は以降の処理を行わない
            if(isReloading||Input.GetKey(ConstData.SHOT_KEY)) return;

            //使用中の武器のデータを更新する
            currentWeaponData = currentWeapoonNo == 0 ?
                GetWeaponInformation(1).weaponData : GetWeaponInformation(0).weaponData;

            //使用中の武器の番号を更新する
            currentWeapoonNo = currentWeapoonNo == 0 ? 1 : 0;

            ///武器のオブジェクトを表示する
            DisplayObjWeapon();
        }
    }
}
