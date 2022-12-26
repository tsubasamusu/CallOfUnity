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
            //Rigidbody���擾
            Rigidbody rb = GetComponent<Rigidbody>();

            //���Z�b�g���̏������Ăяo�� 
            Reset();

            //�ړ��E�����ށE����`�F���W�E�W�����v
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�X�e�[�W���痣�ꂷ������
                    if (Mathf.Abs((transform.position - Vector3.zero).magnitude) > ConstData.MAX_LENGTH_FROM_CENTER)
                    {
                        //��
                        float num = 0f;

                        //����
                        GetComponent<CharacterHealth>().Die(ref num,this);
                    }

                    //�ړ�����
                    Move();

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
                .Subscribe(_ => Camera.main.DOFieldOfView(ConstData.STANCE_FOV, ConstData.STANCE_TIME).SetLink(gameObject))
                .AddTo(this);

            //�ړ�����
            void Move()
            {
                //�v���C���[�ɓ��͂��ꂽ�ړ��������擾����
                Vector3 enteredMovement =
                    Vector3.Scale((Camera.main.transform.right * Input.GetAxis("Horizontal")
                    + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                    , new Vector3(1f, 0f, 1f));

                //�������Ԃł̈ړ���h�~����
                rb.isKinematic = enteredMovement.magnitude == 0f;

                //�ړ������s����
                rb.MovePosition(rb.position + (enteredMovement * GetMoveVelocity() * Time.deltaTime));

                //�ړ����x���擾����
                float GetMoveVelocity()
                {
                    //�i�s�����ɏ�Q��������Ȃ�ړ����x���u0�v�ɂ���i�ړ����Ȃ��j
                    if (CheckObstacles()) return 0f;

                    //�ړ����x��Ԃ�
                    return Input.GetKey(ConstData.RUN_KEY) ? ConstData.RUN_SPEED : ConstData.WALK_SPEED;
                }

                //�i�s�����ɏ�Q�����Ȃ������m�F����
                bool CheckObstacles()
                {
                    //�������쐬  
                    var ray = new Ray(transform.position + (Vector3.up * 1f), enteredMovement.normalized);

                    //���������̃R���C�_�[�ɐڐG������true��Ԃ� 
                    return Physics.Raycast(ray, 1f);
                }
            }
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
            //�������v���C���[�ɐݒ肷��
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

            //����̃I�u�W�F�N�g��\������
            DisplayObjWeapon();
        }
    }
}
