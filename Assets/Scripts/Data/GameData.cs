using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// �ϓ��l���Ǘ�
    /// </summary>
    public class GameData : MonoBehaviour
    {
        [Range(0.1f, 10f)]
        public float lookSensitivity;//���_���x

        [Range(0.1f, 1f)]
        public float lookSmooth;//���_�̊��炩��

        [SerializeField]
        private List<Transform> respawnTransList = new();

        [SerializeField]
        private GameObject objRocketLauncherEffect;//���P�b�g�����`���[�̃G�t�F�N�g

        [SerializeField]
        private GameObject objExplosionEffect;//�����̃G�t�F�N�g

        [SerializeField]
        private Transform temporaryObjectContainerTran;

        [SerializeField]
        private ControllerBase npcControllerBase;//NPC�̃v���t�@�u

        [SerializeField]
        private ControllerBase playerControllerBase;//�v���C���[

        [SerializeField]
        private Material team0Material;//�`�[��0�̃}�e���A��

        [SerializeField]
        private Material team1Material;//�`�[��1�̃}�e���A��

        [HideInInspector]
        public (int team0, int team1) score;//���_

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        [HideInInspector]
        public List<ControllerBase> npcControllerBaseList = new();//NPC�̃��X�g

        [HideInInspector]
        public ((WeaponDataSO.WeaponData data, int bulletCount) info0,
            (WeaponDataSO.WeaponData data, int bulletCount) info1)
            playerWeaponInfo;//�v���C���[�̏�������̏��

        /// <summary>
        /// �u���X�|�[���n�_�̃��X�g�v�̎擾�p
        /// </summary>
        public List<Transform> RespawnTransList { get => respawnTransList; }

        /// <summary>
        /// �u���P�b�g�����`���[�̃G�t�F�N�g�v�̎擾�p
        /// </summary>
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

        /// <summary>
        /// �u�����̃G�t�F�N�g�v�̎擾�p
        /// </summary>
        public GameObject ObjExplosionEffect { get => objExplosionEffect; }

        /// <summary>
        /// �u�Q�[���I�u�W�F�N�g�̈ꎞ�I�Ȑe�v�̎擾�p
        /// </summary>
        public Transform TemporaryObjectContainerTran { get => temporaryObjectContainerTran; }

        /// <summary>
        /// �uNPC�̃v���t�@�u�v�̎擾�p
        /// </summary>
        public ControllerBase NpcControllerBase { get => npcControllerBase; }

        /// <summary>
        /// �u�v���C���[�v�̎擾�p
        /// </summary>
        public ControllerBase PlayerControllerBase { get => playerControllerBase; }

        /// <summary>
        /// �u�`�[��0�̃}�e���A���v�̎擾�p
        /// </summary>
        public Material Team0Material { get => team0Material; }

        /// <summary>
        /// �u�`�[��1�̃}�e���A���v�̎擾�p
        /// </summary>
        public Material Team1Material { get => team1Material; }

        /// <summary>
        /// �uWeaponDataSO�v�̎擾�p
        /// </summary>
        public WeaponDataSO WeaponDataSO { get => weaponDataSO; }

        public static GameData instance;//�C���X�^���X

        /// <summary>
        /// Start���\�b�h���O�ɌĂяo�����
        /// </summary>
        private void Awake()
        {
            //�ȉ��A�V���O���g���ɕK�{�̋L�q
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}