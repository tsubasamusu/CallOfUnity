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
        public List<WeaponDataSO.WeaponData> weaponDataList = new();//��������̃f�[�^�̃��X�g

        [HideInInspector]
        public WeaponDataSO.WeaponData currentWeapon;//�g�p���̕���

        private int allBulletCount;//���c�e��

        private List<int> bulletCountList = new ();//���ꂼ��̕���̎c�e���̃��X�g

        protected Transform weaponTran;//����̈ʒu

        protected bool isReloading;//�����[�h�����ǂ���

        protected bool isPlayer;//�v���C���[���ǂ���

        /// <summary>
        /// �u���c�e���v�̎擾�p
        /// </summary>
        public int AllBulletCount { get => allBulletCount; }

        /// <summary>
        /// �u���ꂼ��̕���̎c�e���v�̎擾�p
        /// </summary>
        public List<int> BulletCounts { get => bulletCountList; }

        /// <summary>
        /// ControllerBase�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //����̈ʒu���擾
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //�����ł��镐��̐������J��Ԃ�
            for (int i = 0; i < ConstData.WEAPONS_NUMBER_I_HAVE; i++)
            {
                //��������̃��X�g�ɋ󔠂����
                weaponDataList.Add(null);

                //���ꂼ��̕���̎c�e���̃��X�g�ɋ󔠂����
                bulletCountList.Add(0);
            }

            //��
            weaponDataList[0] = GameData.instance.WeaponDataSO.weaponDataList[0];
            weaponDataList[1] = GameData.instance.WeaponDataSO.weaponDataList[2];

            //�Đݒ肷��
            ReSetUp();

            //�q�N���X�̏����ݒ���s��
            SetUpController();
        }

        /// <summary>
        /// �Đݒ肷��
        /// </summary>
        public void ReSetUp()
        {
            //���c�e���������l�ɐݒ�
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

            //���ꂼ��̕���̎c�e���̃��X�g�̗v�f�������J��Ԃ�
            for (int i = 0;i<bulletCountList.Count;i++)
            {
                //���ꂼ��̕���̎c�e�����ő吔�ɂ���
                bulletCountList[i] = weaponDataList[i].ammunitionNo;
            }

            //�g�p���̕���������l�ɐݒ�
            currentWeapon = weaponDataList[0];
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
            await UniTask.Delay(TimeSpan.FromSeconds(currentWeapon.reloadTime), cancellationToken: token);

            //���c�e�����X�V����
            allBulletCount = Math.Clamp(allBulletCount - GetRequiredBulletCount(), 0, ConstData.FIRST_ALL_BULLET_COUNT);

            //�g�p���̕���̎c�e���������l�ɐݒ�
            bulletCountList[GetCurrentWeaponNo()] = currentWeapon.ammunitionNo;

            //�����[�h�I����ԂɕύX����
            isReloading = false;

            //�����[�h�ɕK�v�Ȓe�����擾����
            int GetRequiredBulletCount()
            {
                //�g�p���̕���̍ő呕�e���ƁA�g�p���̕���̌��݂̑��e���Ƃ̍���Ԃ�
                return currentWeapon.ammunitionNo - GetAmmunitionRemaining();
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
            bulletCountList[GetCurrentWeaponNo()]
                = Math.Clamp(bulletCountList[GetCurrentWeaponNo()] - 1, 0, currentWeapon.ammunitionNo);

            //�e�𐶐�����
            BulletDetailBase bullet = Instantiate(currentWeapon.bullet);

            //���������e�̐e��ݒ肷��
            bullet.transform.SetParent(GameData.instance.TemporaryObjectContainerTran);

            //���������e�̈ʒu��ݒ肷��
            bullet.transform.position = weaponTran.position;

            //���������e�̌�����ݒ肷��
            bullet.transform.forward = isPlayer ? Camera.main.transform.forward : transform.forward;

            //���������e�̏����ݒ���s��
            bullet.SetUpBullet(currentWeapon);
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
            return currentWeapon.reloadTime;
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̘A�ˑ��x���擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̘A�ˑ��x</returns>
        protected float GetRateOfFire()
        {
            //���ݎg�p���Ă��镐��̘A�ˑ��x��Ԃ�
            return currentWeapon.rateOfFire;
        }

        /// <summary>
        /// �g�p���̕���̔ԍ����擾����
        /// </summary>
        /// <returns>�g�p���̕���̔ԍ�</returns>
        protected int GetCurrentWeaponNo()
        {
            //��������̔ԍ���Ԃ�
            return weaponDataList.IndexOf(currentWeapon) == -1 ? 0 : weaponDataList.IndexOf(currentWeapon);
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̎c�e�����擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̎c�e��</returns>
        protected int GetAmmunitionRemaining()
        {
            //���ݎg�p���Ă��镐��̎c�e����Ԃ�
            return bulletCountList[GetCurrentWeaponNo()];
        }
    }
}
