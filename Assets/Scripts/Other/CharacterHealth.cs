using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// �L�����N�^�[�̗̑͂��Ǘ�����
    /// </summary>
    public class CharacterHealth : MonoBehaviour, ISetUp
    {
        private float hp = 100f;//HP

        //HP�̎擾�p
        public float Hp { get => hp; }

        /// <summary>
        /// CharacterHealth�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //�e�ɐG�ꂽ�ۂ̏���
            this.OnCollisionEnterAsObservable()
                .Where(other => other.transform.TryGetComponent(out BulletDetailBase bullet))
                .Subscribe(other =>
                {
                    //�_���[�W���擾
                    float damage = other.transform.GetComponent<BulletDetailBase>().weaponData.attackPower;

                    //HP���X�V
                    hp = Mathf.Clamp(hp - damage, 0f, 100f);

                    //HP��0�Ȃ玀�S�������s��
                    if (hp == 0f) Die();
                })
                .AddTo(this);

            //���S����
            void Die()
            {
                //TODO:���S����
                Debug.Log("���S");
            }
        }
    }
}
