using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TNRD;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;


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

        /// <summary>
        /// �Q�[���J�n����ɌĂяo�����
        /// </summary>
        private void Start()
        {
            //�e�N���X�̏����ݒ���s���i1��ځj
            SetUp(0);

            //�Q�[���X�^�[�g���o���I������ۂ̏���
            GameData.instance.UiManager.EndedGameStartPerformance
                .Where(_ => GameData.instance.UiManager.EndedGameStartPerformance.Value == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //���_�̊Ď�����
            GameData.instance.Score
                .Subscribe(_ =>
                {
                    if (GameData.instance.Score.Value.team0 >= ConstData.WIN_SCORE) EndGame(true);
                    if (GameData.instance.Score.Value.team1 >= ConstData.WIN_SCORE) EndGame(false);
                })
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
            void EndGame(bool isGameClear)
            {
                //�J������Ɨ�������
                Camera.main.transform.parent = null;

                //�s�v�ȃQ�[���I�u�W�F�N�g��S�ď���
                {
                    Destroy(GameData.instance.TemporaryObjectContainerTran.gameObject);
                    while (GameData.instance.npcControllerBaseList.Count > 0)
                    {
                        Destroy(GameData.instance.npcControllerBaseList[0].gameObject);
                        GameData.instance.npcControllerBaseList.RemoveAt(0);
                    }
                    Destroy(GameData.instance.PlayerControllerBase.gameObject);
                }

                //�f�[�^��ۑ�����
                GameData.instance.SaveData();

                //�Q�[�����I�����o���s��
                GameData.instance.UiManager.PlayGameEndPerformance(isGameClear);

                //�Q�[���I�����o���I�����ۂ̏���
                GameData.instance.UiManager.EndedGameEndPerformance
                    .Where(_ => GameData.instance.UiManager.EndedGameEndPerformance.Value == true)
                    .Subscribe(_ => SceneManager.LoadScene("Main"))
                    .AddTo(this);
            }
        }
    }
}
