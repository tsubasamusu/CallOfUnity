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

        /// <summary>
        /// �u�g�p���ꂽ����̃f�[�^�v�̎擾�p
        /// </summary>
        public WeaponDataSO.WeaponData WeaponData { get => weaponData; }

        /// <summary>
        /// �e�̏����ݒ���s��
        /// </summary>
        /// <param name="weaponData">�g�p��������̃f�[�^</param>
        public void SetUpBullet(WeaponDataSO.WeaponData weaponData)
        {
            //�g�p���ꂽ����̃f�[�^���擾
            this.weaponData = weaponData;

            //�����ݒ���s��
            SetUp();

            //����
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�e��y���W�����O�ɍs������
                    if (transform.position.y < 0f || transform.position.y > 5f)
                    {
                        //�e������
                        Destroy(gameObject);

                        //�ȍ~�̏������s��Ȃ�
                        return;
                    }

                    //�X�e�[�W���痣�ꂷ������
                    if (Mathf.Abs((transform.position - Vector3.zero).magnitude) > ConstData.MAX_LENGTH_FROM_CENTER)
                    {
                        //���ł���
                        Destroy(gameObject);
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

                        ///���������G�t�F�N�g�̈ʒu��ݒ肷��
                        effectTran.position = transform.position;

                        //���������G�t�F�N�g�̌�����ݒ肷��
                        effectTran.forward = -transform.forward;

                        //���������G�t�F�N�g�̐e��ݒ肷��
                        effectTran.SetParent(controllerBase.transform);

                        //���������G�t�F�N�g������
                        Destroy(effectTran.gameObject, 0.2f);
                    }

                    //�e������
                    Destroy(gameObject);
                })
                .AddTo(this);
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
