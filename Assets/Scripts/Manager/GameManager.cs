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

        /// <summary>
        /// �Q�[���J�n����ɌĂяo�����
        /// </summary>
        void Start()
        {
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
