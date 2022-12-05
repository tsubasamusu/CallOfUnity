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
        [HideInInspector]
        public WeaponDataSO.WeaponData weaponData;//����̃f�[�^

        /// <summary>
        /// �ړ�����
        /// </summary>
        public void Move()
        {
            //�ړ�
            this.UpdateAsObservable()
                .Subscribe(_ => transform.Translate(transform.forward * weaponData.bulletVelocity * Time.deltaTime))
                .AddTo(this);

            //���e
            this.OnCollisionEnterAsObservable()
                .Subscribe(collision =>
                {
                    //�v���C���[��NPC�ȊO�̂��̂ɐڐG������
                    if(!collision.transform.TryGetComponent(out ControllerBase controllerBase))
                    {
                        //�e������
                        Destroy(gameObject);
                    }
                })
                .AddTo(this);
        }
    }
}
