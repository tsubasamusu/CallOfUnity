using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// カメラを制御する
    /// </summary>
    public class CameraController : MonoBehaviour,ISetUp
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
            //カメラを操作する
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //マウスの横移動を取得
                    yRot += Input.GetAxis("Mouse X") * GameData.instance.LookSensitivity;

                    //マウスの縦移動を取得
                    xRot -= Input.GetAxis("Mouse Y") * GameData.instance.LookSensitivity;

                    //滑らかにxの回転を取得
                    currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, GameData.instance.LookSmooth);

                    //滑らかにyの回転を取得
                    currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, GameData.instance.LookSmooth);

                    //カメラを回転させる
                    transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
                })
                .AddTo(this);
        }
    }
}
