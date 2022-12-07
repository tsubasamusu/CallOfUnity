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

            //�ړ�
            this.UpdateAsObservable()
                .Subscribe(_ =>transform.Translate(transform.forward * weaponData.bulletVelocity * Time.deltaTime))
                .AddTo(this);

            //���e
            this.OnCollisionEnterAsObservable()
                .Subscribe(collision =>
                {
                    //�v���C���[��NPC�ȊO�̂��̂ɐڐG������
                    if (!collision.transform.TryGetComponent(out ControllerBase controllerBase))
                    {
                        //�e������
                        Destroy(gameObject);
                    }
                })
                .AddTo(this);
        }
    }
}
