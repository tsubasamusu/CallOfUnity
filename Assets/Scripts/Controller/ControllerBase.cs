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
        public WeaponDataSO.WeaponData[] weaponDatas = new WeaponDataSO.WeaponData[2];//��������̃f�[�^

        [HideInInspector]
        public WeaponDataSO.WeaponData currentWeapon;//�g�p���̕���

        private int allBulletCount;//���c�e��

        private int[] bulletCounts = new int[2];//���ꂼ��̕���̎c�e��

        protected Transform weaponTran;//����̈ʒu

        /// <summary>
        /// �u���c�e���v�̎擾�p
        /// </summary>
        public int AllBulletCount { get => allBulletCount; }

        /// <summary>
        /// �u���ꂼ��̕���̎c�e���v�̎擾�p
        /// </summary>
        public int[] BulletCounts { get => bulletCounts; }

        /// <summary>
        /// ControllerBase�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //����̈ʒu���擾
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //���c�e���������l�ɐݒ�
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

            //�q�N���X�̏����ݒ���s��
            SetUpController();

            //��������̐������J��Ԃ�
            for (int i = 0; i < weaponDatas.Length; i++)
            {
                //�������킪�ݒ肳��Ă���Ȃ�
                if (weaponDatas[i] != null)
                {
                    //�g�p���̕����ݒ�
                    if (i == 0) currentWeapon = weaponDatas[i];
                }
                //�������킪�ݒ肳��Ă��Ȃ��Ȃ�
                else
                {
                    //�����
                    Debug.Log("�������킪�ݒ肳��Ă��܂���");
                }
            }
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
        protected void Reload()
        {
            
        }

        /// <summary>
        /// �ˌ�����
        /// </summary>
        protected void Shot()
        {
            //TODO:�ˌ�����
            Debug.Log("�ˌ�");
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̃����[�h���Ԃ��擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̃����[�h����</returns>
        protected float GetReloadTime()
        {
            //TODO:�����[�h���Ԏ擾����
            return 3f;//�i���j
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̘A�ˑ��x���擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̘A�ˑ��x</returns>
        protected float GetRateOfFire()
        {
            //TODO:�A�ˑ��x�擾����
            return 1f;//�i���j
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̎c�e�����擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̎c�e��</returns>
        protected float GetAmmunitionRemaining()
        {
            //TODO:�c�e���̎擾����
            return 100f;//�i���j
        }
    }
}
