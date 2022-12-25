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
        private CanvasGroup cgOtherButtons;//���̑��̃{�^��

        [SerializeField]
        private Text txtScoreTeam0;//�`�[��0�̓��_�̃e�L�X�g

        [SerializeField]
        private Text txtScoreTeam1;//�`�[��1�̓��_�̃e�L�X�g

        [SerializeField]
        private Button btnMain;//���C���{�^��

        [SerializeField]
        private Button btnSetting;//�ݒ�{�^��

        [SerializeField]
        private Button btnChooseWeapon;//����I���{�^��

        [SerializeField]
        private Button btnData;//�f�[�^�{�^��

        [SerializeField]
        private Image imgMainButton;//���C���{�^���̃C���[�W

        [SerializeField]
        private Text txtMainButton;//���C���{�^���̃e�L�X�g

        [HideInInspector]
        public ReactiveProperty<bool> endedStartPerformance = new(false);//�Q�[���X�^�[�g���o���I��������ǂ���

        /// <summary>
        /// UIManager�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //���C���{�^���Ƃ��̑��̃{�^����񊈐�������
            btnMain.interactable = cgOtherButtons.interactable = false;

            //��������UI�̃L�����o�X�O���[�v�ƁA���̑��̃{�^���̃L�����o�X�O���[�v���\���ɂ���
            cgGameUI.alpha = cgOtherButtons.alpha = 0f;

            //�w�i�𔒐F�ɐݒ肷��
            imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

            //���S���^�C�g���ɐݒ肷��
            imgLogo.sprite = GetLogoSprite(LogoType.GameTitle);

            //���C���{�^����F�ɐݒ肷��
            imgMainButton.color = Color.blue;

            //���C���{�^���̃e�L�X�g���uGame Start�v�ɐݒ肷��
            txtMainButton.text = "Game Start";

            //���C���{�^�����\���ɂ���
            imgMainButton.DOFade(0f, 0f);
            txtMainButton.DOFade(0f, 0f);

            //���S����莞�Ԃ����ĕ\������
            imgLogo.DOFade(1f, 1f);

            //�S�Ă̂̃{�^������莞�Ԃ����ĕ\�����A����������
            txtMainButton.DOFade(1f, 1f);
            cgOtherButtons.DOFade(1f, 1f).OnComplete(() => cgOtherButtons.interactable = true);
            imgMainButton.DOFade(1f, 1f).OnComplete(() => btnMain.interactable = true);

            //���C���{�^���������ꂽ�ۂ̏���
            btnMain.OnClickAsObservable()
                .Where(_ => GameData.instance.playerWeaponInfo.info0.data != null && GameData.instance.playerWeaponInfo.info1.data != null)
                .Subscribe(_ =>
                {
                    //�S�Ẵ{�^����񊈐�������
                    btnMain.interactable = cgOtherButtons.interactable = false;

                    //�w�i����莞�Ԃ����Ĕ�\���ɂ���
                    imgBackground.DOFade(0f, 1f);

                    //���S����莞�Ԃ����Ĕ�\���ɂ���
                    imgLogo.DOFade(0f, 1f);

                    //�S�Ẵ{�^������莞�Ԃ����Ĕ�\���ɂ��A���o�I����Ԃɐ؂�ւ���
                    cgOtherButtons.DOFade(0f, 1f);
                    txtMainButton.DOFade(0f, 1f).OnComplete(() => cgGameUI.alpha = 1f);
                    imgMainButton.DOFade(0f, 1f).OnComplete(() => endedStartPerformance.Value = true);
                })
                .AddTo(this);
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
    }
}
