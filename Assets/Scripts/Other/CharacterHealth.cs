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
        /// <summary>
        /// CharacterHealth�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //HP�ێ��p
            float hp = 100f;

            //Rigidbody���擾����
            Rigidbody rb = GetComponent<Rigidbody>();

            //ControllerBase���擾����
            ControllerBase controllerBase = GetComponent<ControllerBase>();

            //�e�ɐG�ꂽ�ۂ̏���
            this.OnCollisionEnterAsObservable()
                .Where(collision => collision.transform.TryGetComponent(out BulletDetailBase _))
                .Subscribe(collision =>
                {
                    //�������Z�𖳌�������
                    if (!rb.isKinematic) rb.isKinematic = true;

                    //BulletdetailBase���擾����
                    BulletDetailBase bulletDetailBase = collision.transform.GetComponent<BulletDetailBase>();

                    //�G�ꂽ�e���G�`�[���̒e�Ȃ�
                    if (bulletDetailBase.MyTeamNo != controllerBase.myTeamNo)
                    {
                        //HP���X�V����
                        hp = Mathf.Clamp(hp - bulletDetailBase.WeaponData.attackPower, 0f, 100f);
                    }

                    //�������v���C���[�Ȃ�AHP�̃X���C�_�[��ݒ肷��
                    if (controllerBase.IsPlayer) GameData.instance.UiManager.SetSldHp(hp / 100f);

                    //�G�ꂽ�e������
                    Destroy(collision.gameObject);

                    //HP��0�Ȃ玀�S�������s��
                    if (hp == 0f) Die( ref hp, controllerBase,bulletDetailBase);
                })
                .AddTo(this);
        }

        /// <summary>
        /// ���S����
        /// </summary>
        /// <param name="hp">������HP</param>
        /// <param name="controllerBase">������ControllerBase</param>
        /// <param name="bulletDetailBase">�ڐG�����BulletDetailBase</param>
        public void Die(ref float hp,ControllerBase controllerBase,BulletDetailBase bulletDetailBase = null)
        {
            //�������v���C���[�Ȃ�A�v���C���[�̃f�X�����u1�v���₷
            if (controllerBase.IsPlayer) GameData.instance.playerTotalDeathCount++;

            //�v���C���[�̒e�ɂ���Ď��S������
            if (bulletDetailBase != null && bulletDetailBase.IsPlayerBullet)
            {
                //�v���C���[�̃L�������u1�v���₷
                GameData.instance.playerTotalKillCount++;
            }

            //�������`�[��0�Ȃ�
            if (controllerBase.myTeamNo == 0)
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

            //�Đݒ肷��
            controllerBase.ReSetUp();

            //HP�������l�ɖ߂�
            hp = 100f;

            //�������v���C���[�Ȃ�AHP�̃X���C�_�[�������l�ɐݒ肷��
            if (controllerBase.IsPlayer) GameData.instance.UiManager.SetSldHp(1f);

            //���X�|�[������
            transform.position = controllerBase.myTeamNo == 0 ?
                GameData.instance.RespawnTransList[0].position
                : GameData.instance.RespawnTransList[1].position;
        }
    }
}
