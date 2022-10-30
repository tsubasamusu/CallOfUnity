using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// PlayerController�N���X��NPCController�N���X�̐e�N���X
    /// </summary>
    public class ControllerBase : MonoBehaviour, ISetUp
    {
        protected Vector3 moveDir;//�ړ�����

        /// <summary>
        /// ControllerBase�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //�ړ�����
            this.UpdateAsObservable()
                .Where(_ => moveDir != Vector3.zero)
                .Subscribe(_ => transform.Translate(moveDir * Time.deltaTime))
                .AddTo(this);

            //�d�͂𐶐�����
            this.UpdateAsObservable()
                .Where(_ => !CheckGrounded())
                .Subscribe(_ => transform.Translate(new Vector3(0f, -ConstData.GRAVITY, 0f) * Time.deltaTime))
                .AddTo(this);

            //�q�N���X�̏����ݒ���s��
            SetUpController();
        }

        /// <summary>
        /// �ڒn������s��
        /// </summary>
        /// <returns></returns>
        protected bool CheckGrounded()
        {
            //�������쐬 
            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

            //���������̃R���C�_�[�ɐڐG������true��Ԃ�
            return Physics.Raycast(ray, 0.15f);
        }

        /// <summary>
        /// �q�N���X�̏����ݒ���s��
        /// </summary>
        protected virtual void SetUpController()
        {
            //�q�N���X�ŏ������L�q����
        }

        /// <summary>
        /// �����[�h����
        /// </summary>
        protected void Reload()
        {
            //TODO:�����[�h����
            Debug.Log("�����[�h");
        }

        /// <summary>
        /// �ˌ�����
        /// </summary>
        protected void Shot()
        {
            //TODO:�ˌ�����
            Debug.Log("�ˌ�");
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̃����[�h���Ԃ��擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̃����[�h����</returns>
        protected float GetReloadTime()
        {
            //TODO:�����[�h���Ԏ擾����
            return 3f;//�i���j
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̘A�ˑ��x���擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̘A�ˑ��x</returns>
        protected float GetRateOfFire()
        {
            //TODO:�A�ˑ��x�擾����
            return 1f;//�i���j
        }
    }
}
