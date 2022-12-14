using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// カメラを制御する
    /// </summary>
    public class CameraController : MonoBehaviour, ISetUp
    {
        private float yRot;//入力されたyの回転
        private float xRot;//入力されたxの回転

        private float currentYRot;//現在のyの回転
        private float currentXRot;//現在のxの回転

        private float yRotVelocity;//yの回転速度
        private float xRotVelocity;//xの回転速度

        /// <summary>
        /// CameraControllerの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //視野角を初期化
            Camera.main.fieldOfView = ConstData.NORMAL_FOV;

            //カメラを操作する
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //マウスの横移動を取得する
                    yRot += Input.GetAxis("Mouse X") * GameData.instance.lookSensitivity;

                    //マウスの縦移動を取得する
                    xRot -= Input.GetAxis("Mouse Y") * GameData.instance.lookSensitivity;

                    //滑らかにxの回転を取得する
                    currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, GameData.instance.lookSmooth);

                    //滑らかにyの回転を取得する
                    currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, GameData.instance.lookSmooth);

                    //カメラを回転させる
                    transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
                })
                .AddTo(this);
        }
    }
}
