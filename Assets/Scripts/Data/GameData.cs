using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// �ϓ��l���Ǘ�
    /// </summary>
    public class GameData : MonoBehaviour
    {
        [Header("���_���x"), Range(0.1f, 10f)]
        public float lookSensitivity;//���_���x

        [Header("���_�̊��炩��"), Range(0.1f, 1f)]
        public float lookSmooth;//���_�̊��炩��

        [Header("�d��")]
        public float gravity;//�d��

        [HideInInspector]
        public (int team0 ,int team1) score;//���_

        [SerializeField, Header("���X�|�[���n�_�̃��X�g")]
        private List<Transform> respawnTransList = new();

        [SerializeField,Header("���P�b�g�����`���[�̃G�t�F�N�g")]
        private GameObject objRocketLauncherEffect;//���P�b�g�����`���[�̃G�t�F�N�g

        [SerializeField,Header("�Q�[���I�u�W�F�N�g�̈ꎞ�I�Ȑe")]
        private Transform temporaryObjectContainerTran;

        [SerializeField, Header("NPC�̃v���t�@�u")]
        private ControllerBase npcControllerBase;//NPC�̃v���t�@�u

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        [HideInInspector]
        public List<ControllerBase> npcList = new();//NPC�̃��X�g

        [HideInInspector]
        public List<WeaponDataSO.WeaponData> weaponDataListForPlayer = new();//�v���[���[�p�̏�������̃��X�g

        /// <summary>
        /// �u���X�|�[���n�_�̃��X�g�v�̎擾�p
        /// </summary>
        public List<Transform> RespawnTransList { get => respawnTransList; }

        /// <summary>
        /// �u���P�b�g�����`���[�̃G�t�F�N�g�v�̎擾�p
        /// </summary>
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

        /// <summary>
        /// �u�Q�[���I�u�W�F�N�g�̈ꎞ�I�Ȑe�v�̎擾�p
        /// </summary>
        public Transform TemporaryObjectContainerTran { get => temporaryObjectContainerTran; }

        /// <summary>
        /// �uNPC�̃v���t�@�u�v�̎擾�p
        /// </summary>
        public ControllerBase NpcControllerBase { get => npcControllerBase; }

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