using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using UniRx;

namespace CallOfUnity
{
    /// <summary>
    /// UI�𐧌䂷��
    /// </summary>
    public class UIManager : MonoBehaviour,ISetUp
    {
        /// <summary>
        /// ���S�̎��
        /// </summary>
        private enum LogoType
        {
            GameTitle,//�Q�[���^�C�g��
            GameOver,//�Q�[���I�[�o�[
            GameClear//�Q�[���N���A
        }

        /// <summary>
        /// ���S�̃f�[�^���Ǘ����� 
        /// </summary>
        [Serializable]
        private class LogoData
        {
            public LogoType LogoType;//���S�̎��
            public Sprite sprite;//�X�v���C�g
        }

        [SerializeField]
        private List<LogoData> logoDatasList = new();//���S�̃f�[�^�̃��X�g

        [SerializeField]
        private Image imgLogo;//���S

        [SerializeField]
        private Image imgBackground;//�w�i

        [SerializeField]
        private CanvasGroup cgGameUI;//��������UI�̃L�����o�X�O���[�v

        [SerializeField]
        private Text txtScoreTeam0;//�`�[��0�̓��_�̃e�L�X�g

        [SerializeField]
        private Text txtScoreTeam1;//�`�[��1�̓��_�̃e�L�X�g

        [SerializeField]
        private Button btnMain;//���C���{�^��

        [SerializeField]
        private Image imgMainButton;//���C���{�^���̃C���[�W

        [SerializeField]
        private Text txtMainButton;//�{�^���̃e�L�X�g

        /// <summary>
        /// UIManager�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {

        }

        /// <summary>
        /// ���S�̃X�v���C�g���擾����
        /// </summary>
        /// <param name="logoType">���S�̎��</param>
        /// <returns>���S�̃X�v���C�g</returns>
        private Sprite GetLogoSprite(LogoType logoType)
        {
            //�K�؂ȃ��S�̃X�v���C�g��Ԃ�
            return logoDatasList.Find(x => x.LogoType == logoType).sprite;
        }

        /// <summary>
        /// �Q�[���X�^�[�g���o���s��
        /// </summary>
        /// <returns>�҂�����</returns>
        public IEnumerator PlayGameStart()
        {
            //�Q�[���X�^�[�g���o�I������p
            bool end = false;

            //���C���{�^����񊈐�������
            btnMain.interactable = false;

            //��������UI�̃L�����o�X�O���[�v���\���ɂ���
            cgGameUI.alpha = 0f;

            //�w�i�𔒐F�ɐݒ肷��
            imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

            //���S���^�C�g���ɐݒ肷��
            imgLogo.sprite = GetLogoSprite(LogoType.GameTitle);

            //���C���{�^����F�ɐݒ肷��
            imgMainButton.color = Color.blue;

            //���C���{�^���̃e�L�X�g���uOpen�v�ɐݒ肷��
            txtMainButton.text = "Open";

            //���C���{�^�����\���ɂ���
            imgMainButton.DOFade(0f, 0f);
            txtMainButton.DOFade(0f, 0f);

            //���S����莞�Ԃ����ĕ\������
            imgLogo.DOFade(1f, 1f);

            //���C���{�^������莞�Ԃ����ĕ\�����A���C���{�^��������������
            txtMainButton.DOFade(1f, 1f);
            imgMainButton.DOFade(1f, 1f).OnComplete(() => btnMain.interactable = true);

            //���C���{�^���������ꂽ�ۂ̏���
            btnMain.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //���C���{�^����񊈐�������
                    btnMain.interactable = false;

                    //�w�i����莞�Ԃ����Ĕ�\���ɂ���
                    imgBackground.DOFade(0f, 1f);

                    //���S����莞�Ԃ����Ĕ�\���ɂ���
                    imgLogo.DOFade(0f, 1f);

                    //���C���{�^������莞�Ԃ����Ĕ�\���ɂ��A���o�I����Ԃɐ؂�ւ���
                    txtMainButton.DOFade(0f, 1f);
                    imgMainButton.DOFade(0f, 1f).OnComplete(() => end = true);
                })
                .AddTo(this);

            //�Q�[���X�^�[�g���o���I���܂ő҂�
            yield return new WaitUntil(() => end);
        }

        ///// <summary>
        ///// �Q�[���I�[�o�[���o���s��
        ///// </summary>
        ///// <returns>�҂�����</returns>
        //public IEnumerator PlayGameOver()
        //{
        //    //�Q�[���I�[�o�[���o�I������p
        //    bool end = false;

        //    //�w�i�����F�ɐݒ�
        //    imgBackground.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

        //    //���S���Q�[���I�[�o�[�ɐݒ�
        //    imgLogo.sprite = GetLogoSprite(LogoType.GameOver);

        //    //�{�^����ԐF�ɐݒ�
        //    imgMainButton.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0f);

        //    //�{�^���̃e�L�X�g���uRestart�v�ɐݒ�
        //    txtMainButton.text = "Restart";

        //    //�{�^���ɓo�^����Ă��鏈�����폜
        //    mainButton.onClick.RemoveAllListeners();

        //    //�{�^���������ꂽ�ۂ̏�����ݒ�
        //    mainButton.onClick.AddListener(() => ClickedButton());

        //    //�{�^����񊈐�������
        //    mainButton.interactable = false;

        //    //���_�̃L�����o�X�O���[�v����莞�Ԃ����Ĕ�\���ɂ���
        //    cgGameUI.DOFade(0f, 1f);

        //    //���S���\���ɂ���
        //    imgLogo.DOFade(0f, 0f)

        //    //�w�i����莞�Ԃ����ĕ\������
        //    .OnComplete(() => imgBackground.DOFade(1f, 1f)

        //        //���S����莞�Ԃ����ĕ\������
        //        .OnComplete(() => imgLogo.DOFade(1f, 1f)

        //        .OnComplete(() =>
        //        {
        //            //�{�^���̃C���[�W����莞�Ԃ����ĕ\������
        //            { imgMainButton.DOFade(1f, 1f); }

        //            {
        //                //�{�^���̃L�����o�X�O���[�v����莞�Ԃ����ĕ\������
        //                cgMainButton.DOFade(1f, 1f)

        //                //�{�^��������������
        //                .OnComplete(() => mainButton.interactable = true);
        //            }

        //        })));

        //    //�{�^���������ꂽ�ۂ̏���
        //    void ClickedButton()
        //    {
        //        //���ʉ����Đ�
        //        SoundManager.instance.PlaySound(SoundDataSO.SoundName.GameRestartSE);

        //        //�w�i����莞�Ԃ����Ĕ��F�ɂ���
        //        imgBackground.DOColor(Color.white, 1f);

        //        //���S����莞�Ԃ����Ĕ�\���ɂ���
        //        imgLogo.DOFade(0f, 1f);

        //        //�{�^���̃L�����o�X�O���[�v����莞�Ԃ����Ĕ�\���ɂ���
        //        cgMainButton.DOFade(0f, 1f)

        //            //�Q�[���I�[�o�[���o���I��������Ԃɐ؂�ւ���
        //            .OnComplete(() => end = true);

        //        //�{�^����񊈐�������
        //        mainButton.interactable = false;
        //    }

        //    //�Q�[���I�[�o�[���o���I���܂ő҂�
        //    yield return new WaitUntil(() => end == true);
        //}

        ///// <summary>
        ///// �Q�[���N���A���o���s��
        ///// </summary>
        ///// <returns>�҂�����</returns>
        //public IEnumerator PlayGameClear()
        //{
        //    //�Q�[���N���A���o�I������p
        //    bool end = false;

        //    //�w�i�𔒐F�ɐݒ�
        //    imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);

        //    //���S���Q�[���N���A�ɐݒ�
        //    imgLogo.sprite = GetLogoSprite(LogoType.GameClear);

        //    //�{�^�������F�ɐݒ�
        //    imgMainButton.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0f);

        //    //�{�^���̃e�L�X�g���uRestart�v�ɐݒ�
        //    txtMainButton.text = "Restart";

        //    //�{�^���ɓo�^����Ă��鏈�����폜
        //    mainButton.onClick.RemoveAllListeners();

        //    //�{�^���������ꂽ�ۂ̏�����ݒ�
        //    mainButton.onClick.AddListener(() => ClickedButton());

        //    //�{�^����񊈐�������
        //    mainButton.interactable = false;

        //    //���_�̃e�L�X�g����莞�Ԃ����ĐF�ɕς���
        //    txtScoreTeam0.DOColor(Color.blue, 2f);

        //    //���S���\���ɂ���
        //    imgLogo.DOFade(0f, 0f)

        //    //�w�i����莞�Ԃ����ĕ\������
        //    .OnComplete(() => imgBackground.DOFade(1f, 1f)

        //        //���S����莞�Ԃ����ĕ\������
        //        .OnComplete(() => imgLogo.DOFade(1f, 1f)

        //        .OnComplete(() =>
        //        {
        //            //�{�^���̃C���[�W����莞�Ԃ����ĕ\������
        //            { imgMainButton.DOFade(1f, 1f); }

        //            {
        //                //�{�^���̃L�����o�X�O���[�v����莞�Ԃ����ĕ\������
        //                cgMainButton.DOFade(1f, 1f)

        //                //�{�^��������������
        //                .OnComplete(() => mainButton.interactable = true);
        //            }

        //        })));

        //    //�{�^���������ꂽ�ۂ̏���
        //    void ClickedButton()
        //    {
        //        //���ʉ����Đ�
        //        SoundManager.instance.PlaySound(SoundDataSO.SoundName.GameRestartSE);

        //        //���_�̃L�����o�X�O���[�v����莞�Ԃ����Ĕ�\���ɂ���
        //        cgGameUI.DOFade(0f, 1f);

        //        //���S����莞�Ԃ����Ĕ�\���ɂ���
        //        imgLogo.DOFade(0f, 1f);

        //        //�{�^���̃L�����o�X�O���[�v����莞�Ԃ����Ĕ�\���ɂ���
        //        cgMainButton.DOFade(0f, 1f)

        //            //�Q�[���N���A���o���I��������Ԃɐ؂�ւ���
        //            .OnComplete(() => end = true);

        //        //�{�^����񊈐�������
        //        mainButton.interactable = false;
        //    }

        //    //�Q�[���N���A���o���I���܂ő҂�
        //    yield return new WaitUntil(() => end == true);
        //}

        ///// <summary>
        ///// ���_�̕\�����X�V���鏀�����s��
        ///// </summary>
        //public void PrepareUpdateTxtScore()
        //{
        //    //���_�̕\�����X�V����
        //    StartCoroutine(UpDateTxtScore());
        //}

        ///// <summary>
        ///// ���_�̕\�����X�V����
        ///// </summary>
        ///// <returns>�҂�����</returns>
        //private IEnumerator UpDateTxtScore()
        //{
        //    //���_�̃e�L�X�g��ݒ肷��
        //    txtScoreTeam0.text = GameData.instance.score.playerScore.ToString() + ":" + GameData.instance.score.enemyScore.ToString();

        //    //���_�̃L�����o�X�O���[�v����莞�Ԃ����ĕ\������
        //    cgGameUI.DOFade(1f, 0.25f);

        //    //���_����莞�ԁA�\����������
        //    yield return new WaitForSeconds(0.25f + GameData.instance.DisplayScoreTime);

        //    //�v���C���[������������
        //    if (GameData.instance.score.playerScore == GameData.instance.MaxScore)
        //    {
        //        //�ȍ~�̏������s��Ȃ�
        //        yield break;
        //    }

        //    //���_�̃L�����o�X�O���[�v����莞�Ԃ����Ĕ�\���ɂ���
        //    cgGameUI.DOFade(0f, 0.25f);
        //}
    }
}
