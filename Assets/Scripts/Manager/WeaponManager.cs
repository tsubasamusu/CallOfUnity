using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// ����E�e�Ɋւ��鏈�����s��
    /// </summary>
    public class WeaponManager : MonoBehaviour,ISetUp
    {
        public static WeaponManager instance;//�C���X�^���X

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
        /// WeaponManager�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            
        }
    }
}
