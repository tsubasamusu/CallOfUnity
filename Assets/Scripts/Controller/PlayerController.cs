using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CallOfUnity
{
    /// <summary>
    /// �v���C���[�̓����𐧌䂷��
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// PlayerController�̏����ݒ���s��
        /// </summary>
        protected override void SetUpController()
        {
            //���Z�b�g���̏������Ăяo�� 
            Reset();

            //�ړ��E�����ށE����`�F���W�E�W�����v
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�ړ�����
                    transform.Translate(
                        //���������̓��͂��擾
                        Vector3.Scale((Camera.main.transform.right * Input.GetAxis("Horizontal")
                        //���������̓��͂��擾
                        + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                        //y������0�ɂ���
                        , new Vector3(1f, 0f, 1f))
                        //�ړ��X�s�[�h���擾
                        * (Input.GetKey(ConstData.RUN_KEY) ? ConstData.RUN_SPEED : ConstData.WALK_SPEED)
                        //���Ԃ��|����
                        * Time.deltaTime);

                    //����`�F���W�L�[�������ꂽ�畐����`�F���W����
                    if (Input.GetKeyDown(ConstData.CHANGE_WEAPON_KEY)) ChangeWeapon();

                    //�\����L�[�������ꂽ��
                    if (Input.GetKeyUp(ConstData.STANCE_KEY))
                    {
                        //�\����̂���߂�
                        Camera.main.DOFieldOfView(ConstData.NORMAL_FOV, ConstData.STANCE_TIME);
                    }
                })
                .AddTo(this);

            //�����[�h
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.RELOAD_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(GetReloadTime()))
                .Subscribe(_ => ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget())
                .AddTo(this);

            //�ˌ�
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(ConstData.SHOT_KEY) && GetBulletcCount() >= 1 && !isReloading)
                .ThrottleFirst(TimeSpan.FromSeconds(currentWeaponData.rateOfFire))
                .Subscribe(_ => Shot())
                .AddTo(this);

            //�\����
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.STANCE_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(ConstData.STANCE_TIME))
                .Subscribe(_ => Camera.main.DOFieldOfView(ConstData.STANCE_FOV, ConstData.STANCE_TIME))
                .AddTo(this);
        }

        /// <summary>
        /// �Đݒ肷��
        /// </summary>
        public override void ReSetUp()
        {
            //�e����̎c�e�����ő�l�ɐݒ肷��
            SetBulletCount(0, GetWeaponInfo(0).weaponData.ammunitionNo);
            SetBulletCount(1, GetWeaponInfo(1).weaponData.ammunitionNo);
        }

        /// <summary>
        /// ���Z�b�g���ɌĂяo�����
        /// </summary>
        private void Reset()
        {
            //�������v���[���[�ɐݒ肷��
            isPlayer = true;

            //�����̃`�[���ԍ���ݒ�
            myTeamNo = 0;

            //�g�p���̕���̔ԍ��������l�ɐݒ�
            currentWeapoonNo = 0;

            //�g�p���̕���̃f�[�^�������l�ɐݒ�
            currentWeaponData = GetWeaponInfo(0).weaponData;
        }

        /// <summary>
        /// ������`�F���W����
        /// </summary>
        private void ChangeWeapon()
        {
            //�����[�h�����A�ˌ����͈ȍ~�̏������s��Ȃ�
            if (isReloading || Input.GetKey(ConstData.SHOT_KEY)) return;

            //�g�p���̕���̃f�[�^���X�V����
            currentWeaponData = currentWeapoonNo == 0 ?
                GetWeaponInfo(1).weaponData : GetWeaponInfo(0).weaponData;

            //�g�p���̕���̔ԍ����X�V����
            currentWeapoonNo = currentWeapoonNo == 0 ? 1 : 0;

            ///����̃I�u�W�F�N�g��\������
            DisplayObjWeapon();
        }
    }
}
