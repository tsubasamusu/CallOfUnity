using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// ƒJƒƒ‰‚ğ§Œä‚·‚é
    /// </summary>
    public class CameraController : MonoBehaviour, ISetUp
    {
        private float yRot;//“ü—Í‚³‚ê‚½y‚Ì‰ñ“]
        private float xRot;//“ü—Í‚³‚ê‚½x‚Ì‰ñ“]

        private float currentYRot;//Œ»İ‚Ìy‚Ì‰ñ“]
        private float currentXRot;//Œ»İ‚Ìx‚Ì‰ñ“]

        private float yRotVelocity;//y‚Ì‰ñ“]‘¬“x
        private float xRotVelocity;//x‚Ì‰ñ“]‘¬“x

        /// <summary>
        /// CameraController‚Ì‰Šúİ’è‚ğs‚¤
        /// </summary>
        public void SetUp()
        {
            //‹–ìŠp‚ğ‰Šú‰»
            Camera.main.fieldOfView = ConstData.NORMAL_FOV;

            //ƒJƒƒ‰‚ğ‘€ì‚·‚é
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //ƒ}ƒEƒX‚Ì‰¡ˆÚ“®‚ğæ“¾
                    yRot += Input.GetAxis("Mouse X") * GameData.instance.lookSensitivity;

                    //ƒ}ƒEƒX‚ÌcˆÚ“®‚ğæ“¾
                    xRot -= Input.GetAxis("Mouse Y") * GameData.instance.lookSensitivity;

                    //ŠŠ‚ç‚©‚Éx‚Ì‰ñ“]‚ğæ“¾
                    currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, GameData.instance.lookSmooth);

                    //ŠŠ‚ç‚©‚Éy‚Ì‰ñ“]‚ğæ“¾
                    currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, GameData.instance.lookSmooth);

                    //ƒJƒƒ‰‚ğ‰ñ“]‚³‚¹‚é
                    transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
                })
                .AddTo(this);
        }
    }
}
