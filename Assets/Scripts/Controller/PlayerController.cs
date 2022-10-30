using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;

namespace CallOfUnity
{
    /// <summary>
    /// プレイヤーの動きを制御する
    /// </summary>
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// PlayerControllerの初期設定を行う
        /// </summary>
        protected override void SetUpController()
        {
            //移動・かがむ・武器チェンジ
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //移動方向を設定
                    moveDir
                    //水平方向の入力を取得
                    = Vector3.Scale((Camera.main.transform.right * Input.GetAxis("Horizontal")
                        //垂直方向の入力を取得
                        + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                        //y成分を0にする
                        , new Vector3(1f, 0f, 1f))
                        //移動スピードを取得
                        * (Input.GetKey(ConstData.RUN_KEY) ? ConstData.RUN_SPEED : ConstData.WALK_SPEED);

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
                    if(Input.GetKeyDown(ConstData.CHANGE_WEAPON_KEY))ChangeWeapon();
                })
                .AddTo(this);

            //リロード
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.RELOAD_KEY))
                .ThrottleFirst(System.TimeSpan.FromSeconds(GetReloadTime()))
                .Subscribe(_ => Reload())
                .AddTo(this);

            //射撃
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(ConstData.SHOT_KEY))
                .ThrottleFirst(System.TimeSpan.FromSeconds(GetRateOfFire()))
                .Subscribe(_ => Shot())
                .AddTo(this);

            //構える
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.STANCE_KEY) || Input.GetKeyUp(ConstData.STANCE_KEY))
                .ThrottleFirst(System.TimeSpan.FromSeconds(ConstData.STANCE_TIME))
                .Subscribe(_ =>
                {
                    //構えるキーが押されたら
                    if(Input.GetKeyDown(ConstData.STANCE_KEY))
                    {
                        //構える
                        Camera.main.DOFieldOfView(ConstData.STANCE_FOV,ConstData.STANCE_TIME);
                    }
                    //構えるキーが離されたら
                    else if(Input.GetKeyUp(ConstData.STANCE_KEY))
                    {
                        //構えるのをやめる
                        Camera.main.DOFieldOfView(ConstData.NORMAL_FOV, ConstData.STANCE_TIME);
                    }
                })
                .AddTo(this);
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
            //武器をチェンジする処理
            Debug.Log("武器をチェンジ");
        }
    }
}
