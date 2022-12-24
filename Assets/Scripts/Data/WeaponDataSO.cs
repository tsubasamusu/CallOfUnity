using CallOfUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///����̃f�[�^���Ǘ�����
/// </summary>
[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Create WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    /// <summary>   
    /// ����̖��O   
    /// </summary>   
    public enum WeaponName
    {
        AssaultRifle,//�A�T���g���C�t��
        Pistol,//�s�X�g��
        RocketLauncher,//���P�b�g�����`���[
        Shotgun,//�V���b�g�K��
        SniperRifle,//�X�i�C�p�[���C�t��
        SubMachineGun//�T�u�}�V���K��
    }

    /// <summary>   
    /// ����̃f�[�^���Ǘ����� 
    /// </summary>   
    [Serializable]
    public class WeaponData
    {
        public WeaponName name;//���O  
        public GameObject objWeapon;//����̃I�u�W�F�N�g
        public GameObject impactEffect;//���e�G�t�F�N�g
        public Sprite sprWeapon;//����̃X�v���C�g
        public BulletDetailBase bullet;//�e
        public int ammunitionNo;//���e��
        public float reloadTime;//�����[�h����
        public float rateOfFire;//�A�ˑ��x
        public float firingRange;//�˒�����
        public float shotPower;//���˗�
        [Range(0f,100f)]
        public float attackPower;//�U����
    }

    public List<WeaponData> weaponDataList = new();//����̃f�[�^�̃��X�g  
}
