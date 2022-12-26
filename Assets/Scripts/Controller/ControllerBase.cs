using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// PlayerController�N���X��NPCController�N���X�̐e�N���X
    /// </summary>
    public class ControllerBase : MonoBehaviour, ISetUp
    {
        [HideInInspector]
        public int myTeamNo;//�����̃`�[���ԍ�

        protected WeaponDataSO.WeaponData currentWeaponData;//�g�p���̕���

        protected int currentWeapoonNo;//�g�p���̕���̔ԍ�

        protected int bulletCountForNpc;//NPC�p�̎c�e��

        protected Transform weaponTran;//����̈ʒu

        private GameObject objWeapon;//����̃I�u�W�F�N�g

        protected bool isReloading;//�����[�h�����ǂ���

        protected bool isPlayer;//�������v���C���[���ǂ���

        /// <summary>
        /// �u�������v���C���[���ǂ����v�̎擾�p
        /// </summary>
        public bool IsPlayer { get => isPlayer; }

        /// <summary>
        /// ControllerBase�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //����̈ʒu���擾
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //�q�N���X�̏����ݒ���s��
            SetUpController();

            //�Đݒ肷��
            ReSetUp();

            //����̃I�u�W�F�N�g��\������
            DisplayObjWeapon();
        }

        /// <summary>
        /// ����̃I�u�W�F�N�g��\������
        /// </summary>
        protected void DisplayObjWeapon()
        {
            //����̃I�u�W�F�N�g�����ɐ������Ă���Ȃ�A���������
            if (objWeapon != null) Destroy(objWeapon);

            //����̃I�u�W�F�N�g�𐶐�����
            objWeapon = Instantiate(currentWeaponData.objWeapon);

            //������������̐e��ݒ�
            objWeapon.transform.SetParent(weaponTran.transform);

            //������������̈ʒu�E�p�x��ݒ�
            objWeapon.transform.localPosition = objWeapon.transform.localEulerAngles = Vector3.zero;
        }

        /// <summary>
        /// �Đݒ肷��
        /// </summary>
        public virtual void ReSetUp()
        {
            //�q�N���X�ŏ������L�q����
        }

        /// <summary>
        /// �q�N���X�̏����ݒ���s��
        /// </summary>
        protected virtual void SetUpController()
        {
            //�q�N���X�ŏ������L�q����
        }

        /// <summary>
        /// �ڒn������s��
        /// </summary>
        /// <returns>�ڒn���Ă�����true</returns>
        protected bool CheckGrounded()
        {
            //�������쐬 
            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

            //���������̃R���C�_�[�ɐڐG������true��Ԃ�
            return Physics.Raycast(ray, 0.15f);
        }

        /// <summary>
        /// �����[�h����
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>�҂�����</returns>
        protected async UniTaskVoid ReloadAsync(CancellationToken token)
        {
            //�����[�h���ɕύX����
            isReloading = true;

            //�������v���C���[�Ȃ�A�����[�h�Q�[�W�̃A�j���[�V�������s��
            if(isPlayer)GameData.instance.UiManager.PlayImgReloadGaugeAnimation(currentWeaponData.reloadTime);

            //��莞�ԑ҂�
            await UniTask.Delay(TimeSpan.FromSeconds(currentWeaponData.reloadTime), cancellationToken: token);

            //�g�p���̕���̎c�e���������l�ɐݒ�
            SetBulletCount(currentWeapoonNo, currentWeaponData.ammunitionNo);

            //�����[�h�I����ԂɕύX����
            isReloading = false;
        }

        /// <summary>
        /// �ˌ�����
        /// </summary>
        protected void Shot()
        {
            //�g�p���̕���̎c�e�����u0�v�ȉ��Ȃ�A�ȍ~�̏������s��Ȃ�
            if (GetBulletcCount() <= 0) return;

            //�g�p���̕���̎c�e�����X�V����
            SetBulletCount(currentWeapoonNo,
                Math.Clamp(GetWeaponInfo(currentWeapoonNo).bulletCount - 1, 0, currentWeaponData.ammunitionNo));

            //�������v���C���[�Ȃ�
            if (isPlayer)
            {
                //�v���C���[�̑����ː����u1�v���₷
                GameData.instance.playerTotalShotCount++;
            }

            //�e�𐶐�����
            BulletDetailBase bullet = Instantiate(currentWeaponData.bullet);

            //���������e�̏����ݒ���s��
            bullet.SetUpBullet(currentWeaponData, myTeamNo, isPlayer);

            //���������e�̐e��ݒ肷��
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //���������e�̈ʒu��ݒ肷��
            bullet.transform.position = weaponTran.position;

            //���������e�̌�����ݒ肷��
            bullet.transform.forward = weaponTran.forward;

            //�e�𔭎˂���
            bullet.transform.GetComponent<Rigidbody>()
                .AddForce(weaponTran.forward * currentWeaponData.shotPower, ForceMode.Impulse);

            //�G�t�F�N�g�𐶐�����
            Transform effectTran = Instantiate(GameData.instance.ObjMuzzleFlashEffect.transform);

            //���������G�t�F�N�g�̈ʒu��ݒ肷��
            effectTran.position = weaponTran.position;

            //���������G�t�F�N�g�̐e��ݒ肷��
            effectTran.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //���������G�t�F�N�g��0.2�b��ɏ���
            Destroy(effectTran.gameObject, 0.2f);
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̃����[�h���Ԃ��擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̃����[�h����</returns>
        protected float GetReloadTime()
        {
            //���ݎg�p���Ă��镐��̃����[�h���Ԃ�Ԃ�
            return currentWeaponData.reloadTime;
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̎c�e�����擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̎c�e��</returns>
        public int GetBulletcCount()
        {
            //���ݎg�p���Ă��镐��̎c�e����Ԃ�
            return GetWeaponInfo(currentWeapoonNo).bulletCount;
        }

        /// <summary>
        /// ����̏����擾����
        /// </summary>
        /// <param name="weaponNo">����̔ԍ�</param>
        /// <returns>�i�f�[�^�E�c�e���j</returns>
        protected (WeaponDataSO.WeaponData weaponData, int bulletCount) GetWeaponInfo(int weaponNo)
        {
            //������NPC�Ȃ�ANPC�p�̏���Ԃ�
            if (!isPlayer) return (currentWeaponData, bulletCountForNpc);

            //�󂯎�����ԍ��ɉ����Ė߂�l��ύX
            return weaponNo switch
            {
                0 => GameData.instance.playerWeaponInfo.info0,
                1 => GameData.instance.playerWeaponInfo.info1,
                _ => (null, -1)
            };
        }

        /// <summary>
        /// ����̎c�e����ݒ肷��
        /// </summary>
        /// <param name="weaponNo">�ݒ肵������������̔ԍ�</param>
        /// <param name="bulletCount">�ݒ肵��������̎c�e��</param>
        protected void SetBulletCount(int weaponNo, int bulletCount)
        {
            //������NPC�Ȃ�
            if (!isPlayer)
            {
                //NPC�p�̎c�e����ݒ肷��
                bulletCountForNpc = bulletCount;

                //�ȍ~�̏������s��Ȃ�
                return;
            }

            //�󂯎������������̔ԍ��ɉ����ď�����ύX
            switch (weaponNo)
            {
                case 0: GameData.instance.playerWeaponInfo.info0.bulletCount = bulletCount; break;
                case 1: GameData.instance.playerWeaponInfo.info1.bulletCount = bulletCount; break;
                default: Debug.Log("�K�؂ȕ���̔ԍ����w�肵�Ă�������"); break;
            }
        }
    }
}
