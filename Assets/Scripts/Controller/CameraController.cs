using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// �J�����𐧌�
    /// </summary>
    public class CameraController : MonoBehaviour,ISetUp
    {
        private float yRot;//���͂��ꂽy�p�x
        private float xRot;//���͂��ꂽx�p�x

        private float currentYRot;//���݂�y�p�x
        private float currentXRot;//���݂�x�p�x

        private float yRotVelocity;//y�̉�]���x
        private float xRotVelocity;//x�̉�]���x

        /// <summary>
        /// �����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //���t���[���A���b�Z�[�W�𔭍s����
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�}�E�X�̉��ړ����擾
                    yRot += Input.GetAxis("Mouse X") * GameData.instance.LookSensitivity;

                    //�}�E�X�̏c�ړ����擾
                    xRot -= Input.GetAxis("Mouse Y") * GameData.instance.LookSensitivity;

                    //���炩��x�p�x���擾
                    currentXRot = Mathf.SmoothDamp(currentXRot, xRot, ref xRotVelocity, GameData.instance.LookSmooth);

                    //���炩��y�p�x���擾
                    currentYRot = Mathf.SmoothDamp(currentYRot, yRot, ref yRotVelocity, GameData.instance.LookSmooth);

                    //�J��������]������
                    transform.rotation = Quaternion.Euler(currentXRot, currentYRot, 0);
                }
                ).AddTo(this);
        }
    }
}
