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
        private Rigidbody rb;//Rigidbody

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

                    //�����ރL�[�������ꂽ��
                    if (Input.GetKeyDown(ConstData.STOOP_KEY))
                    {
                        //������
                        Stoop();
                    }
                    //�����ރL�[�������ꂽ��
                    else if (Input.GetKeyUp(ConstData.STOOP_KEY))
                    {
                        //�����ނ̂���߂�
                        StopStoop();
                    }

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

            //�d�͂𐶐�����
            this.UpdateAsObservable()
                .Where(_ => !CheckGrounded())
                .Subscribe(_ => transform.Translate(Vector3.down * GameData.instance.gravity * Time.deltaTime))
                .AddTo(this);

            //�����[�h
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.RELOAD_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(GetReloadTime()))
                .Subscribe(_ => ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget())
                .AddTo(this);

            //�ˌ�
            this.UpdateAsObservable()
                .Where(_ => Input.GetKey(ConstData.SHOT_KEY) && GetAmmunitionRemaining() > 0)
                .ThrottleFirst(TimeSpan.FromSeconds(GetRateOfFire()))
                .Subscribe(_ => Shot())
                .AddTo(this);

            //�\����
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.STANCE_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(ConstData.STANCE_TIME))
                .Subscribe(_ =>Camera.main.DOFieldOfView(ConstData.STANCE_FOV, ConstData.STANCE_TIME))
                .AddTo(this);

            //�d�͂̏����l���擾
            float firstGravity = GameData.instance.gravity;

            //�W�����v
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.JUMP_KEY) && CheckGrounded())
                .Subscribe(_ => JumpAsync(this.GetCancellationTokenOnDestroy(), firstGravity).Forget())
                .AddTo(this);
        }

        /// <summary>
        /// ���Z�b�g���ɌĂяo�����
        /// </summary>
        private void Reset()
        {
            //Rigidbody���擾
            rb = GetComponent<Rigidbody>();

            //�����̃`�[���ԍ���ݒ�
            myTeamNo = 0;
        }

        /// <summary>
        /// �W�����v����
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <param name="firstGravity">�d�͂̏����l</param>
        /// <returns>�҂�����</returns>
        private async UniTaskVoid JumpAsync(CancellationToken token,float firstGravity)
        {
            //�������Z���J�n
            rb.isKinematic = false;

            //�͂�������
            rb.AddForce(Vector3.up * ConstData.JUMP_POWER, ForceMode.Impulse);

            //�d�͂𖳌���
            GameData.instance.gravity = 0f;

            //���S�ɃW�����v����܂ő҂�
            await UniTask.Delay(TimeSpan.FromSeconds(ConstData.WAIT_JUMP_TIME), cancellationToken: token);

            //�L�����N�^�[�̊p�x��������
            transform.eulerAngles = Vector3.zero;

            //�������Z���I��
            rb.isKinematic = true;

            //�d�͂������l�ɐݒ�
            GameData.instance.gravity = firstGravity;

            //���n����܂ő҂�
            await UniTask.WaitUntil(() => CheckGrounded(), cancellationToken: token);
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Stoop()
        {
            //TODO:�����ޏ���
            Debug.Log("������");
        }

        /// <summary>
        /// �����ނ̂���߂�
        /// </summary>
        private void StopStoop()
        {
            //TODO:�����ނ̂���߂鏈��
            Debug.Log("�����ނ̂���߂�");
        }

        /// <summary>
        /// ������`�F���W����
        /// </summary>
        private void ChangeWeapon()
        {
            //�����[�h���͈ȍ~�̏������s��Ȃ�
            if(isReloading) return;

            //������`�F���W���鏈��
            Debug.Log("������`�F���W");
        }
    }
}
