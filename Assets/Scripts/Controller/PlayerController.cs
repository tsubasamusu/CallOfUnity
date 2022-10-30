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
            //移動方向とスピードを取得
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    moveDir = Vector3.Scale(
                        (Camera.main.transform.right * Input.GetAxis("Horizontal")
                    + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                    , new Vector3(1f, 0f, 1f))
                    * (Input.GetKey(ConstData.runKey) ? ConstData.runSpeed : ConstData.walkSpeed);
                })
                .AddTo(this);

            //リロード
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.reloadKey))
                .ThrottleFirst(System.TimeSpan.FromSeconds(3.0f))//TODO:リロード時間取得処理
                .Subscribe(_ => Reload())
                .AddTo(this);
        }
    }
}
