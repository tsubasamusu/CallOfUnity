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

                    //かがむキーが押されたら
                    if (Input.GetKeyDown(ConstData.STOOP_KEY))
                    {
                        //かがむ
                        Stoop();
                    }
                    //かがむキーが離されたら
                    else if (Input.GetKeyUp(ConstData.STOOP_KEY))
                    {
                        //かがむのをやめる
                        StopStoop();
                    }

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
                .ThrottleFirst(TimeSpan.FromSeconds(GetRateOfFire()))
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
        /// リセット時に呼び出される
        /// </summary>
        private void Reset()
        {
            //Rigidbodyを取得
            rb = GetComponent<Rigidbody>();

            //自分のチーム番号を設定
            myTeamNo = 0;
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
        /// かがむ
        /// </summary>
        private void Stoop()
        {
            //TODO:かがむ処理
            Debug.Log("かがむ");
        }

        /// <summary>
        /// かがむのをやめる
        /// </summary>
        private void StopStoop()
        {
            //TODO:かがむのをやめる処理
            Debug.Log("かがむのをやめる");
        }

        /// <summary>
        /// 武器をチェンジする
        /// </summary>
        private void ChangeWeapon()
        {
            //リロード中は以降の処理を行わない
            if(isReloading) return;

            //武器をチェンジする処理
            Debug.Log("武器をチェンジ");
        }
    }
}
