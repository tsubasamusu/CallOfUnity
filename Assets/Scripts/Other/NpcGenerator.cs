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

            //�����ʒu�̃��X�g�̗v�f�������J��Ԃ�
            for (int i = 0; i < spawnPosList.Count; i++)
            {
                //NPC�𐶐�����
                ControllerBase npcControllerBase = Instantiate(GameData.instance.NpcControllerBase);

                //��������NPC�̈ʒu��ݒ肷��
                npcControllerBase.transform.position = spawnPosList[i];

                //��������NPC�Ƀ`�[���ԍ���^����
                npcControllerBase.myTeamNo = i <= ConstData.TEAMMATE_NUMBER - 2 ? 0 : 1;

                //�`�[��1��NPC���X�e�[�W�����Ɍ�������
                if (npcControllerBase.myTeamNo == 1) npcControllerBase.transform.Rotate(0, 180f, 0);

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
            //���X�g��p�ӂ���
            List<Vector3> spawnPosList = new();

            //�K�v��NPC�̐������J��Ԃ�
            for (int i = 0; i < ConstData.TEAMMATE_NUMBER * 2 - 1; i++)
            {
                //�`�[��0�̐����ʒu���쐬���Ă���Ȃ�
                if (i <= ConstData.TEAMMATE_NUMBER - 2)
                {
                    //�ŏ��̐����ʒu��x���W���擾
                    float firstPosX = -2f * ((ConstData.TEAMMATE_NUMBER - 1) / 2f);

                    //�쐬�������W�����X�g�ɒǉ�����
                    spawnPosList.Add(new Vector3(firstPosX + (2f * i), 0f, -25f));

                    //���̌J��Ԃ������Ɉڂ�
                    continue;
                }

                //�ŏ��̐����ʒu��x���W���擾
                float firstPosX2 = -2f * (ConstData.TEAMMATE_NUMBER / 2f);

                //�쐬�������W�����X�g�ɒǉ�����
                spawnPosList.Add(new Vector3(firstPosX2 + (2f * (i - (ConstData.TEAMMATE_NUMBER - 1))), 0f, 25f));
            }

            //�쐬�������X�g��Ԃ�
            return spawnPosList;
        }
    }
}
