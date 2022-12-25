using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// �S�Ă̒e�̐e�N���X
    /// </summary>
    public class BulletDetailBase : MonoBehaviour
    {
        private WeaponDataSO.WeaponData weaponData;//�g�p���ꂽ����̃f�[�^

        private int myTeamNo;//�����̃`�[���ԍ�

        private bool isPlayerBullet;//�v���C���[�̒e���ǂ���

        /// <summary>
        /// �u�g�p���ꂽ����̃f�[�^�v�̎擾�p
        /// </summary>
        public WeaponDataSO.WeaponData WeaponData { get => weaponData; }

        /// <summary>
        /// �u�����̃`�[���ԍ��v�̎擾�p
        /// </summary>
        public int MyTeamNo { get => myTeamNo; }

        /// <summary>
        /// �u�v���C���[�̒e���ǂ����v�̎擾�p
        /// </summary>
        public bool IsPlayerBullet { get => isPlayerBullet; }

        /// <summary>
        /// �e�̏����ݒ���s��
        /// </summary>
        /// <param name="weaponData">�g�p��������̃f�[�^</param>
        /// <param name="myTeamNo">�����̃`�[���ԍ�</param>
        /// <param name="isPlayerBullet">�v���C���[�̒e���ǂ���</param>
        public void SetUpBullet(WeaponDataSO.WeaponData weaponData,int myTeamNo,bool isPlayerBullet)
        {
            //�g�p���ꂽ����̃f�[�^���擾����
            this.weaponData = weaponData;

            //�����̃`�[���ԍ����擾����
            this.myTeamNo = myTeamNo;

            //�u�v���C���[�̒e���ǂ����v���擾����
            this.isPlayerBullet = isPlayerBullet;

            //�����ݒ���s��
            SetUp();

            //����
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�e��y���W�����O�ɍs������
                    if (transform.position.y < 0f || transform.position.y > 5f)
                    {
                        //���ł���
                        DestroyBullet(false);

                        //�ȍ~�̏������s��Ȃ�
                        return;
                    }

                    //�X�e�[�W���痣�ꂷ������
                    if (Mathf.Abs((transform.position - Vector3.zero).magnitude) > ConstData.MAX_LENGTH_FROM_CENTER)
                    {
                        //���ł���
                        DestroyBullet(false);
                    }
                })
                .AddTo(this);

            //���e
            this.OnCollisionEnterAsObservable()
                .Subscribe(collision =>
                {
                    //�v���C���[��NPC�ɐڐG������
                    if (collision.transform.TryGetComponent(out ControllerBase controllerBase))
                    {
                        //�G�t�F�N�g�𐶐�����
                        Transform effectTran = Instantiate(GameData.instance.ObjBleedingEffect.transform);

                        //���������G�t�F�N�g�̈ʒu��ݒ肷��
                        effectTran.position = transform.position;

                        //���������G�t�F�N�g�̌�����ݒ肷��
                        effectTran.forward = -transform.forward;

                        //���������G�t�F�N�g�̐e��ݒ肷��
                        effectTran.SetParent(controllerBase.transform);

                        //���������G�t�F�N�g������
                        Destroy(effectTran.gameObject, 0.2f);

                        //���ł���
                        DestroyBullet(controllerBase.myTeamNo != myTeamNo);
                    }

                    //���ł���
                    DestroyBullet(false);
                })
                .AddTo(this);

            //���ł���
            void DestroyBullet(bool attackedEnemy)
            {
                Debug.Log(GameData.instance.playerAttackCount);

                //�G�ɍU�����Ă��Ȃ��Ȃ�
                if (!attackedEnemy)
                {
                    //�e������
                    Destroy(gameObject);
                }
                //�������v���C���[���A�G�ɍU�������Ȃ�
                else if(isPlayerBullet&&attackedEnemy)
                {
                    //�v���C���[�̖��������u1�v���₷
                    GameData.instance.playerAttackCount++;
                }
            }
        }

        /// <summary>
        /// �����ݒ���s��
        /// </summary>
        protected virtual void SetUp()
        {
            //�e�q�N���X�ŏ������L�q����
        }
    }
}
