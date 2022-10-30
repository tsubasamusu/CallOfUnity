using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

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
                        * (Input.GetKey(ConstData.runKey) ? ConstData.runSpeed : ConstData.walkSpeed);

                    //かがむキーが押されたら
                    if (Input.GetKeyDown(ConstData.stoopKey))
                    {
                        //かがむ
                        Stoop();
                    }
                    //かがむキーが離されたら
                    else if (Input.GetKeyUp(ConstData.stoopKey))
                    {
                        //かがむのをやめる
                        StopStoop();
                    }
                })
                .AddTo(this);

            //リロード
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.reloadKey))
                .ThrottleFirst(System.TimeSpan.FromSeconds(3.0f))//TODO:リロード時間取得処理
                .Subscribe(_ => Reload())
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
    }
}
