using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace CallOfUnity
{
    /// <summary>
    /// �ϓ��l���Ǘ�
    /// </summary>
    public class GameData : MonoBehaviour, ISetUp
    {
        [Range(0f, 10f)]
        public float lookSensitivity;//���_���x

        [Range(0f, 1f)]
        public float lookSmooth;//���_�̊��炩��

        [HideInInspector]
        public int playerTotalKillCount;//�v���C���[�̑��L����

        [HideInInspector]
        public int playerTotalDeathCount;//�v���C���[�̑��f�X��

        [HideInInspector]
        public int playerTotalAttackCount;//�v���C���[�̑�������

        [HideInInspector]
        public int playerTotalShotCount;//�v���C���[�̑����ː�

        [SerializeField]
        private List<Transform> respawnTransList = new();

        [SerializeField]
        private GameObject objMuzzleFlashEffect;//���ˌ��̃G�t�F�N�g

        [SerializeField]
        private GameObject objBleedingEffect;//�o���̃G�t�F�N�g

        [SerializeField]
        private GameObject objRocketLauncherEffect;//���P�b�g�����`���[�̃G�t�F�N�g

        [SerializeField]
        private GameObject objExplosionEffect;//�����̃G�t�F�N�g

        [SerializeField]
        private Transform temporaryObjectContainerTran;//�Q�[���I�u�W�F�N�g�̈ꎞ�I�Ȑe

        [SerializeField]
        private ControllerBase npcControllerBase;//NPC�̃v���t�@�u

        [SerializeField]
        private ControllerBase playerControllerBase;//�v���C���[��ControllerBase

        [SerializeField]
        private CharacterHealth playerCharacterHealth;//�v���C���[��CharacterHealth

        [SerializeField]
        private UIManager uiManager;//UIManager

        [SerializeField]
        private Material team0Material;//�`�[��0�̃}�e���A��

        [SerializeField]
        private Material team1Material;//�`�[��1�̃}�e���A��

        [HideInInspector]
        public ReactiveProperty<(int team0, int team1)> Score = new((0, 0));//���_

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        [SerializeField]
        private SoundDataSO soundDataSO;//SoundDataSO

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
        /// �u���ˌ��̃G�t�F�N�g�v�̎擾�p
        /// </summary>
        public GameObject ObjMuzzleFlashEffect { get => objMuzzleFlashEffect; }

        /// <summary>
        /// �u�o���̃G�t�F�N�g�v�̎擾�p
        /// </summary>
        public GameObject ObjBleedingEffect { get => objBleedingEffect; }

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
        /// �u�v���C���[��ControllerBase�v�̎擾�p
        /// </summary>
        public ControllerBase PlayerControllerBase { get => playerControllerBase; }

        /// <summary>
        /// �u�v���C���[��CharacterHealth�v�̎擾�p
        /// </summary>
        public CharacterHealth PlayerCharacterHealth { get => playerCharacterHealth; }

        /// <summary>
        /// �uUIManager�v�̎擾�p
        /// </summary>
        public UIManager UiManager { get => uiManager; }

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

        /// <summary>
        /// �uSoundDataSO�v�̎擾�p
        /// </summary>
        public SoundDataSO SoundDataSO { get => soundDataSO; }

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

        /// <summary>
        /// GameData�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //���Z�b�g����
            Reset();
        }

        /// <summary>
        /// ���Z�b�g����
        /// </summary>
        private void Reset()
        {
            //�f�[�^�����[�h����
            if (PlayerPrefs.HasKey("Kill")) playerTotalKillCount = PlayerPrefs.GetInt("Kill");
            if (PlayerPrefs.HasKey("Death")) playerTotalDeathCount = PlayerPrefs.GetInt("Death");
            if (PlayerPrefs.HasKey("Attack")) playerTotalAttackCount = PlayerPrefs.GetInt("Attack");
            if (PlayerPrefs.HasKey("Shot")) playerTotalShotCount = PlayerPrefs.GetInt("Shot");
            if (PlayerPrefs.HasKey("LookSensitivity")) lookSensitivity = PlayerPrefs.GetFloat("LookSensitivity");
            if (PlayerPrefs.HasKey("LookSmooth")) lookSmooth = PlayerPrefs.GetFloat("LookSmooth");
        }

        /// <summary>
        /// �f�[�^��ۑ�����
        /// </summary>
        public void SaveData()
        {
            PlayerPrefs.SetInt("Kill", playerTotalKillCount);
            PlayerPrefs.SetInt("Death", playerTotalDeathCount);
            PlayerPrefs.SetInt("Attack", playerTotalAttackCount);
            PlayerPrefs.SetInt("Shot", playerTotalShotCount);
            PlayerPrefs.SetFloat("LookSensitivity", lookSensitivity);
            PlayerPrefs.SetFloat("LookSmooth", lookSmooth);
        }
    }
}