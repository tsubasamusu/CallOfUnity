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
        /// <summary>
        /// �Q�[���I���̎��
        /// </summary>
        private enum EndGameType
        {
            GameClear,//�Q�[���N���A
            GameOver,//�Q�[���I�[�o�[
            GameDraw//��������
        }

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
            uIManager.endedStartPerformance
                .Where(_ => uIManager.endedStartPerformance.Value == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //�������J�n����
            void StartGame()
            {
                //�e�N���X�̏����ݒ���s���i2��ځj
                SetUp(1);

                //�������Ԃ̍X�V���J�n����
                UpdateTimeLimitAsync(this.GetCancellationTokenOnDestroy()).Forget();

                //�������Ԃ��X�V����
                async UniTaskVoid UpdateTimeLimitAsync(CancellationToken token)
                {
                    //�������ԕێ��p
                    float timeLimit = ConstData.TIME_LIMIT;

                    //�����ɌJ��Ԃ�
                    while (true)
                    {
                        //�������Ԃ��X�V����
                        timeLimit -= Time.deltaTime;

                        //�������Ԃ̃e�L�X�g���X�V����
                        uIManager.SetTxtTimeLimit(timeLimit);

                        //�������Ԃ��I������
                        if(timeLimit<=0f)
                        {
                            //�J��Ԃ��������甲���o��
                            break;
                        }

                        //�`�[��0������������
                        if(GameData.instance.score.team0>=ConstData.WIN_SCORE)
                        {
                            //�Q�[�����I������
                            EndGame(EndGameType.GameClear);

                            //�J��Ԃ��������甲���o��
                            break;
                        }
                        //�`�[��1������������
                        else if(GameData.instance.score.team1>=ConstData.WIN_SCORE) 
                        {
                            //�Q�[�����I������
                            EndGame(EndGameType.GameOver);

                            //�J��Ԃ��������甲���o��
                            break;
                        }

                        //1�t���[���҂�
                        await UniTask.Yield(token);
                    }

                    //�������Ԃ��c���Ă����Ȃ�
                    if(timeLimit>0f)
                    {
                        //�ȍ~�̏������s��Ȃ�
                        return;
                    }

                    //TODO:�������Ԃɂ�鎎���I���̏���
                }
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

                    //���������Ȃ�
                    case EndGameType.GameDraw:
                        break;
                }
            }
        }
    }
}
