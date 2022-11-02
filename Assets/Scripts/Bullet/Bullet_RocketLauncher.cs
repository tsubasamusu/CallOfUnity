using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// ���P�b�g�����`���[�e�p�̃N���X
    /// </summary>
    public class Bullet_RocketLauncher : BulletDetailBase
    {
        /// <summary>
        /// ���P�b�g�����`���[�̒e�̐�������ɌĂяo�����
        /// </summary>
        private void Start()
        {
            //�G�t�F�N�g�𐶐�
            Transform effectTran = Instantiate(GameData.instance.ObjRocketLauncherEffect.transform);

            //�G�t�F�N�g�̌�����ݒ�
            effectTran.forward = transform.forward;

            //�G�t�F�N�g�̈ړ�
            this.UpdateAsObservable()
                .Subscribe(_ => effectTran.position = transform.position)
                .AddTo(this);

            //�G�t�F�N�g�̍폜
            this.OnDestroyAsObservable()
                .Subscribe(_ => Destroy(effectTran.gameObject))
                .AddTo(this);
        }
    }
}
