using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// NPC�𐶐�����
    /// </summary>
    public class NpcGenerator : MonoBehaviour, ISetUp
    {
        /// <summary>
        /// NpcGenerator�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //�����ʒu�̃��X�g���擾
            List<Vector3> spawnPosList = GetSpawnPosList();

            //�擾�������X�g�̗v�f�����������Ȃ�������
            if (spawnPosList.Count != ConstData.TEAMMATE_NUMBER * 2 - 1)
            {
                //�����
                Debug.Log("�K�؂Ȑ��̐����ʒu���쐬���Ă�������");

                //�ȍ~�̏������s��Ȃ�
                return;
            }

            //�����ʒu�̃��X�g�̗v�f�������J��Ԃ�
            for (int i = 0; i < spawnPosList.Count; i++)
            {
                //NPC�𐶐�����
                ControllerBase npcControllerBase = Instantiate(GameData.instance.NpcControllerBase);

                //��������NPC�̈ʒu��ݒ肷��
                npcControllerBase.transform.position = spawnPosList[i];

                //��������NPC�Ƀ`�[���ԍ���^����
                npcControllerBase.myTeamNo = i <= ConstData.TEAMMATE_NUMBER - 2 ? 0 : 1;

                //��������NPC�̏����ݒ���s��
                npcControllerBase.SetUp();

                //��������NPC�����X�g�ɒǉ�
                GameData.instance.npcControllerBaseList.Add(npcControllerBase);
            }
        }

        /// <summary>
        /// �����ʒu�̃��X�g���擾����
        /// </summary>
        /// <returns>�����ʒu�̃��X�g</returns>
        private List<Vector3> GetSpawnPosList()
        {
            //�i���j
            return new List<Vector3> { Vector3.zero };

            //TODO:�����ʒu�̃��X�g���擾���鏈��
        }
    }
}
