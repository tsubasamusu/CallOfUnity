using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TNRD;
using UniRx;
using UnityEngine;
using System.Threading;

namespace CallOfUnity
{
    /// <summary>
    /// �Q�[���̐i�s�𐧌䂷��
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList0 = new();//ISetUp�C���^�[�t�F�C�X�̃��X�g0

        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList1 = new();//ISetUp�C���^�[�t�F�C�X�̃��X�g1

        [SerializeField]
        private ControllerBase playerControllerBase;//�v���C���[��ControllerBase

        [SerializeField]
        private UIManager uIManager;//UIManager

        /// <summary>
        /// �Q�[���J�n����ɌĂяo�����
        /// </summary>
        private void Start()
        {
            //�e�N���X�̏����ݒ���s���i1��ځj
            SetUp(0);

            //�Q�[���X�^�[�g���o���I������ۂ̏���
            uIManager.EndedStartPerformance
                .Where(_ => uIManager.EndedStartPerformance.Value == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //�������J�n����
            void StartGame()
            {
                //�e�N���X�̏����ݒ���s���i2��ځj
                SetUp(1);
            }

            //�e�N���X�̏����ݒ���s��
            void SetUp(int setUpNo)
            {
                //ISetUp�C���^�[�t�F�C�X�̃��X�g�̗v�f�������J��Ԃ�
                for (int i = 0; i < (setUpNo == 0 ? iSetUpList0 : iSetUpList1).Count; i++)
                {
                    //�e�N���X�̏����ݒ���s��
                    (setUpNo == 0 ? iSetUpList0 : iSetUpList1)[i].Value.SetUp();
                }
            }

            //�Q�[�����I������
            void EndGame(EndGameType endGameType)
            {
                //�Q�[���I���̎�ނɉ����ď�����ύX����
                switch (endGameType)
                {
                    //�Q�[���N���A�Ȃ�
                    case EndGameType.GameClear:
                        break;

                    //�Q�[���I�[�o�[�Ȃ�
                    case EndGameType.GameOver:
                        break;
                }
            }
        }
    }
}
