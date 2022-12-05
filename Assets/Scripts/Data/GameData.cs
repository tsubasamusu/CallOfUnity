using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// �ϓ��l���Ǘ�
    /// </summary>
    public class GameData : MonoBehaviour
    {
        [SerializeField, Header("���_���x"), Range(0.1f, 10f)]
        private float lookSensitivity;//���_���x

        [SerializeField, Header("���_�̊��炩��"), Range(0.1f, 1f)]
        private float lookSmooth;//���_�̊��炩��

        [Header("�d��")]
        public float gravity;//�d��

        [SerializeField,Header("���P�b�g�����`���[�̃G�t�F�N�g")]
        private GameObject objRocketLauncherEffect;//���P�b�g�����`���[�̃G�t�F�N�g

        [SerializeField,Header("�Q�[���I�u�W�F�N�g�̈ꎞ�I�Ȑe")]
        private Transform temporaryObjectContainerTran;

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        /// <summary>
        /// �u���_���x�v�̎擾�E�ݒ�p
        /// </summary>
        public float LookSensitivity { get => lookSensitivity; set => lookSensitivity = value; }

        /// <summary>
        /// �u���_�̊��炩���v�̎擾�E�ݒ�p
        /// </summary>
        public float LookSmooth { get => lookSmooth; set => lookSmooth = value; }

        /// <summary>
        /// �u���P�b�g�����`���[�̃G�t�F�N�g�v�̎擾�p
        /// </summary>
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

        /// <summary>
        /// �u�Q�[���I�u�W�F�N�g�̈ꎞ�I�Ȑe�v�̎擾�p
        /// </summary>
        public Transform TemporaryObjectContainerTran { get => temporaryObjectContainerTran; }

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