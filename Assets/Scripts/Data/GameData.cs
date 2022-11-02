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

        //�u���_���x�v�̎擾�E�ݒ�p
        public float LookSensitivity { get => lookSensitivity; set => lookSensitivity = value; }

        //�u���_�̊��炩���v�̎擾�E�ݒ�p
        public float LookSmooth { get => lookSmooth; set => lookSmooth = value; }

        //�u���P�b�g�����`���[�̃G�t�F�N�g�v�̎擾�p
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

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