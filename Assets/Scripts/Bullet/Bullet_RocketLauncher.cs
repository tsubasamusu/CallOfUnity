using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

namespace CallOfUnity
{
    /// <summary>
    /// ���P�b�g�����`���[�e�p�̃N���X
    /// </summary>
    public class Bullet_RocketLauncher : BulletDetailBase
    {
        /// <summary>
        /// �����ݒ���s��
        /// </summary>
        protected override void SetUp()
        {
            //�G�t�F�N�g�𐶐�����
            Transform effectTran = Instantiate(GameData.instance.ObjRocketLauncherEffect.transform);

            //���������G�t�F�N�g�̐e��ݒ肷��
            effectTran.SetParent(transform);

            //�G�t�F�N�g�̌�����ݒ肷��
            effectTran.forward = -transform.forward;

            //�G�t�F�N�g�̈ړ�
            this.UpdateAsObservable()
                .Subscribe(_ => effectTran.position = transform.position)
                .AddTo(this);

            //����
            this.OnCollisionEnterAsObservable()
                .Subscribe(_ => 
                {
                    //�����̃G�t�F�N�g�𐶐�����
                    Transform explosionEffectTran = Instantiate(GameData.instance.ObjExplosionEffect.transform);

                    //�����̃G�t�F�N�g�̈ʒu��ݒ肷��
                    explosionEffectTran.position = transform.position;

                    //���������G�t�F�N�g�̐e��ݒ�
                    explosionEffectTran.SetParent(GameData.instance.TemporaryObjectContainerTran);

                    //���������G�t�F�N�g��3�b��ɏ���
                    Destroy(explosionEffectTran.gameObject, 3f);
                })
                .AddTo(this);
        }
    }
}
