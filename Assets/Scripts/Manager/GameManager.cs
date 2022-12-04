using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// �Q�[���̐i�s�𐧌䂷��
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList = new();//ISetUp�C���^�[�t�F�C�X�̃��X�g

        [SerializeField]
        private ControllerBase playerControllerBase;//�v���C���[��ControllerBase

        /// <summary>
        /// �Q�[���J�n����ɌĂяo�����
        /// </summary>
        void Start()
        {
            //TODO:�v���C���[�̏�������̎w��̊m�F

            //TODO:�v���C���[�̏�������̐ݒ�

            //��
            playerControllerBase.weaponDatas[0] = GameData.instance.WeaponDataSO.weaponDataList[0];
            playerControllerBase.weaponDatas[1] = GameData.instance.WeaponDataSO.weaponDataList[2];

            //�e�N���X�̏����ݒ���s��
            SetUp();

            //�e�N���X�̏����ݒ���s��
            void SetUp()
            {
                //ISetUp�C���^�[�t�F�C�X�̃��X�g�̗v�f�������J��Ԃ�
                for (int i = 0; i < iSetUpList.Count; i++)
                {
                    //�e�N���X�̏����ݒ���s��
                    iSetUpList[i].Value.SetUp();
                }
            }
        }
    }
}
