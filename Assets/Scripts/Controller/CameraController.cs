using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// �J�����𐧌䂷��
    /// </summary>
    public class CameraController : MonoBehaviour, ISetUp
    {
        private float yRot;//���͂��ꂽy�̉�]
        private float xRot;//���͂��ꂽx�̉�]

        private float currentYRot;//���݂�y�̉�]
        private float currentXRot;//���݂�x�̉�]

        private float yRotVelocity;//y�̉�]���x
        private float xRotVelocity;//x�̉�]���x

        /// <summary>
        /// CameraController�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //����p��������
            Camera.main.fieldOfView = ConstData.NORMAL_FOV;

            //�J�����𑀍삷��
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�}�E�X�̉��ړ����擾
                    yRot += Input.GetAxis("Mouse X") * GameData.instance.lookSensitivity;

                    //�}�E�X�̏c�ړ����擾
                    xRot -= Input.GetAxis("Mouse Y") * GameData.instance.lookSensitivity;

                    //���炩��x�̉�]���擾
                    currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, GameData.instance.lookSmooth);

                    //���炩��y�̉�]���擾
                    currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, GameData.instance.lookSmooth);

                    //�J��������]������
                    transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
                })
                .AddTo(this);
        }
    }
}
