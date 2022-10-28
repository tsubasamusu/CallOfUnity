using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// カメラを制御
    /// </summary>
    public class CameraController : MonoBehaviour,ISetUp
    {
        private float yRot;//入力されたy角度
        private float xRot;//入力されたx角度

        private float currentYRot;//現在のy角度
        private float currentXRot;//現在のx角度

        private float yRotVelocity;//yの回転速度
        private float xRotVelocity;//xの回転速度

        /// <summary>
        /// 初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //毎フレーム、メッセージを発行する
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //マウスの横移動を取得
                    yRot += Input.GetAxis("Mouse X") * GameData.instance.LookSensitivity;

                    //マウスの縦移動を取得
                    xRot -= Input.GetAxis("Mouse Y") * GameData.instance.LookSensitivity;

                    //滑らかにx角度を取得
                    currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, GameData.instance.LookSmooth);

                    //滑らかにy角度を取得
                    currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, GameData.instance.LookSmooth);

                    //カメラを回転させる
                    transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
                }
                ).AddTo(this);
        }
    }
}
