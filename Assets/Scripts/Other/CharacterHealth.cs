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
                .Where(collision => collision.transform.TryGetComponent(out BulletDetailBase _))
                .Subscribe(collision =>
                {
                    //�_���[�W���擾
                    float damage = collision.transform.GetComponent<BulletDetailBase>().WeaponData.attackPower;

                    //HP���X�V
                    hp = Mathf.Clamp(hp - damage, 0f, 100f);

                    //HP��0�Ȃ玀�S�������s��
                    if (hp == 0f) Die();
                })
                .AddTo(this);

            //���S����
            void Die()
            {
                //ControllerBase���擾�ł��Ȃ�������
                if(!TryGetComponent(out ControllerBase controllerBase))
                {
                    //�����
                    Debug.Log("ControllerBase���擾�ł��܂���ł���");

                    //�ȍ~�̏������s��Ȃ�
                    return;
                }

                //�������`�[��0�Ȃ�
                if(controllerBase.myTeamNo== 0)
                {
                    //�`�[��1�̓��_�𑝂₷
                    GameData.instance.score.team1++;
                }
                //�������`�[��1�Ȃ�
                else
                {
                    //�`�[��0�̓��_�𑝂₷
                    GameData.instance.score.team0++;
                }

                //�`�[��0������������
                if(GameData.instance.score.team0 >=ConstData.WIN_SCORE)
                {
                    //TODO:�`�[��0�������̏���
                    Debug.Log("�`�[��0�̏���");

                    //�ȍ~�̏������s��Ȃ�
                    return;
                }
                //�`�[��1������������
                else if(GameData.instance.score.team1>=ConstData.WIN_SCORE)
                {
                    //TODO:�`�[��1�������̏���
                    Debug.Log("�`�[��1�̏���");

                    //�ȍ~�̏������s��Ȃ�
                    return;
                }

                //�Đݒ肷��
                controllerBase.ReSetUp();

                //���X�|�[������
                transform.position = controllerBase.myTeamNo == 0 ? 
                    GameData.instance.RespawnTransList[0].position
                    : GameData.instance.RespawnTransList[1].position;
            }
        }
    }
}
