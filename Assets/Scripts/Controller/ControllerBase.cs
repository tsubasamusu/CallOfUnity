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

            //�q�N���X�̏����ݒ���s��
            SetUpController();
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
        }

        /// <summary>
        /// �ˌ�����
        /// </summary>
        protected void Shot()
        {
            //TODO:�ˌ�����
        }
    }
}
