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

        [HideInInspector]
        public WeaponDataSO.WeaponData currentWeaponData;//�g�p���̕���

        private int allBulletCount;//���c�e��

        protected int currentWeapoonNo;//�g�p���̕���̔ԍ�

        protected Transform weaponTran;//����̈ʒu

        protected GameObject objWeapon;//����̃I�u�W�F�N�g

        protected bool isReloading;//�����[�h�����ǂ���

        /// <summary>
        /// �u���c�e���v�̎擾�p
        /// </summary>
        public int AllBulletCount { get => allBulletCount; }

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
            //���c�e���������l�ɐݒ�
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

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
            //�����[�h�ł��邾���̒e���c���Ă��Ȃ���΁A�ȍ~�̏������s��Ȃ�
            if (allBulletCount < GetRequiredBulletCount()) return;

            //�����[�h���ɕύX����
            isReloading = true;

            //��莞�ԑ҂�
            await UniTask.Delay(TimeSpan.FromSeconds(currentWeaponData.reloadTime), cancellationToken: token);

            //���c�e�����X�V����
            allBulletCount = Math.Clamp(allBulletCount - GetRequiredBulletCount(), 0, ConstData.FIRST_ALL_BULLET_COUNT);

            //�g�p���̕���̎c�e���������l�ɐݒ�
            UpdateBulletCount(currentWeapoonNo, currentWeaponData.ammunitionNo);

            //�����[�h�I����ԂɕύX����
            isReloading = false;

            //�����[�h�ɕK�v�Ȓe�����擾����
            int GetRequiredBulletCount()
            {
                //�g�p���̕���̍ő呕�e���ƁA�g�p���̕���̌��݂̑��e���Ƃ̍���Ԃ�
                return currentWeaponData.ammunitionNo - GetAmmunitionRemaining();
            }
        }

        /// <summary>
        /// �ˌ�����
        /// </summary>
        protected void Shot()
        {
            //�����[�h�����A�c�e�����u0�v�Ȃ�A�ȍ~�̏������s��Ȃ�
            if (isReloading || GetAmmunitionRemaining() == 0) return;

            //���c�e�����X�V����
            allBulletCount = Math.Clamp(allBulletCount - 1, 0, ConstData.FIRST_ALL_BULLET_COUNT);

            //�g�p���̕���̎c�e�����X�V����
            UpdateBulletCount(currentWeapoonNo,
                Math.Clamp(GetWeaponInformation(currentWeapoonNo).bulletCount - 1, 0, currentWeaponData.ammunitionNo));

            //�e�𐶐�����
            BulletDetailBase bullet = Instantiate(currentWeaponData.bullet);

            //���������e�̐e��ݒ肷��
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //���������e�̈ʒu��ݒ肷��
            bullet.transform.position = weaponTran.position;

            //���������e�̐i�s������ݒ肷��
            Vector3 moveDir = TryGetComponent(out PlayerController _) ? Camera.main.transform.forward : transform.forward;

            //���������e�̏����ݒ���s��
            bullet.SetUpBullet(currentWeaponData, moveDir);
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̃����[�h���Ԃ��擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̃����[�h����</returns>
        protected float GetReloadTime()
        {
            //�����[�h�����A�c�e�����u0�v�Ȃ�A�����[�h���Ԃ��u0�v�ŕԂ�
            if (isReloading || GetAmmunitionRemaining() == 0) return 0f;

            //���ݎg�p���Ă��镐��̃����[�h���Ԃ�Ԃ�
            return currentWeaponData.reloadTime;
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̎c�e�����擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̎c�e��</returns>
        protected int GetAmmunitionRemaining()
        {
            //���ݎg�p���Ă��镐��̎c�e����Ԃ�
            return GetWeaponInformation(currentWeapoonNo).bulletCount;
        }

        /// <summary>
        /// ����̏����擾����
        /// </summary>
        /// <param name="weaponNo">����̔ԍ�</param>
        /// <returns>�i�f�[�^�E�c�e���j</returns>
        protected (WeaponDataSO.WeaponData weaponData, int bulletCount) GetWeaponInformation(int weaponNo)
        {
            //�󂯎�����ԍ��ɉ����Ė߂�l��ύX
            return weaponNo switch
            {
                0 => GameData.instance.playerWeaponInfo.infomation0,
                1 => GameData.instance.playerWeaponInfo.infomation1,
                _ => (null, -1)
            };
        }

        /// <summary>
        /// ����̎c�e�����X�V����
        /// </summary>
        /// <param name="weaponNo">�ݒ肵������������̔ԍ�</param>
        /// <param name="bulletCount">�ݒ肵��������̎c�e��</param>
        protected void UpdateBulletCount(int weaponNo, int bulletCount)
        {
            //�󂯎������������̔ԍ��ɉ����ď�����ύX
            switch (weaponNo)
            {
                case 0: GameData.instance.playerWeaponInfo.infomation0.bulletCount0 = bulletCount; break;
                case 1: GameData.instance.playerWeaponInfo.infomation1.bulletCount1 = bulletCount; break;
                default: Debug.Log("�K�؂ȕ���̔ԍ����w�肵�Ă�������"); break;
            }
        }
    }
}
