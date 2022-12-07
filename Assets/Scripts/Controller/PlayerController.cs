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

            //ジャンプ
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.JUMP_KEY) && CheckGrounded())
                .Subscribe(_ => Jump())
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
        private void Jump()
        {
            //y座標を変更する
            transform.DOMoveY(transform.position.y + 1f, 0.5f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo);
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
