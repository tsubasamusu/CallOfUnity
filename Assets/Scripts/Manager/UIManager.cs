using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// UI�𐧌䂷��
    /// </summary>
    public class UIManager : MonoBehaviour, ISetUp
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
            public LogoType logoType;//���S�̎��
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

        [SerializeField]
        private CanvasGroup cgSettings;//�ݒ�̃L�����o�X�O���[�v

        [SerializeField]
        private Slider sldLookSensitivity;//���_���x�̃X���C�_�[

        [SerializeField]
        private Slider sldLookSmooth;//���_�̊��炩���̃X���C�_�[

        [SerializeField]
        private Text txtData;//�f�[�^�̃e�L�X�g

        [SerializeField]
        private Slider sldHp;//HP�̃X���C�_�[

        [SerializeField]
        private Text txtBulletCount;//�c�e���̃e�L�X�g

        [SerializeField]
        private WeaponButtonDetail BtnWeaponPrefab;//����̃{�^���̃v���t�@�u

        [HideInInspector]
        public ReactiveProperty<bool> EndedStartPerformance = new(false);//�Q�[���X�^�[�g���o���I��������ǂ���

        private List<WeaponButtonDetail> btnWeaponList = new();//����̃{�^���̃��X�g

        /// <summary>
        /// UIManager�̏����ݒ���s��
        /// </summary>
        public void SetUp()
        {
            //���C���{�^���Ƃ��̑��̃{�^����񊈐�������
            btnMain.interactable = cgOtherButtons.interactable = false;

            //�s�K�v�ȃL�����o�X�O���[�v���\���ɂ���
            cgGameUI.alpha = cgOtherButtons.alpha = 0f;

            //�ݒ��\������
            cgSettings.alpha = 1f;

            //�X���C�_�[�������l�ɐݒ肷��
            sldLookSensitivity.value = GameData.instance.lookSensitivity / 10f;
            sldLookSmooth.value = GameData.instance.lookSmooth;
            sldHp.value = 1f;

            //���_�̃e�L�X�g�������l�ɐݒ肷��
            UpdateTxtScore();

            //�ݒ�̃L�����o�X�O���[�v��񊈐�������
            cgSettings.gameObject.SetActive(false);

            //�f�[�^�̃e�L�X�g���\���ɂ���
            txtData.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

            //�w�i�𔒐F�ɐݒ肷��
            imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

            //���S���^�C�g���ɐݒ肷��
            imgLogo.sprite = GetLogoSprite(LogoType.GameTitle);

            //���C���{�^����F�ɐݒ肷��
            imgMainButton.color = Color.blue;

            //���C���{�^���̃e�L�X�g���uGame Start�v�ɐݒ肷��
            txtMainButton.text = "Game Start";

            //�f�[�^�̃e�L�X�g���X�V����
            txtData.text
                = "Total Kill : "
                + GameData.instance.playerTotalKillCount.ToString()
                + "\n"
                + "Kill-Death Ratio : "
                + (GameData.instance.playerTotalKillCount
                / (GameData.instance.playerTotalDeathCount == 0 ? 1f : GameData.instance.playerTotalDeathCount))
                .ToString("F2")
                + "\n"
                + "Hit Rate : "
                + (GameData.instance.playerTotalAttackCount
                / (GameData.instance.playerTotalShotCount == 0 ? 1f : GameData.instance.playerTotalShotCount))
                .ToString("F2")
                + "%";

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
                .Subscribe(_ =>
                {
                    //���킪�I������Ă��Ȃ����
                    if (GameData.instance.playerWeaponInfo.info0.data == null || GameData.instance.playerWeaponInfo.info1.data == null)
                    {
                        //����I���{�^���̃A�j���[�V�������s��
                        btnChooseWeapon.gameObject.transform.DOScale(1.3f, 0.25f).SetLoops(2, LoopType.Yoyo);

                        //�ȍ~�̏������s��Ȃ�
                        return;
                    }

                    //�S�Ẵ{�^����񊈐�������
                    btnMain.interactable = cgOtherButtons.interactable = false;

                    //�w�i����莞�Ԃ����Ĕ�\���ɂ���
                    imgBackground.DOFade(0f, 1f);

                    //���S����莞�Ԃ����Ĕ�\���ɂ���
                    imgLogo.DOFade(0f, 1f);

                    //�S�Ẵ{�^������莞�Ԃ����Ĕ�\���ɂ��A���o�I����Ԃɐ؂�ւ���
                    cgOtherButtons.DOFade(0f, 1f);
                    txtMainButton.DOFade(0f, 1f).OnComplete(() => cgGameUI.alpha = 1f);
                    imgMainButton.DOFade(0f, 1f).OnComplete(() => EndedStartPerformance.Value = true);
                })
                .AddTo(this);

            //�ݒ�{�^���������ꂽ�ۂ̏���
            btnSetting.OnClickAsObservable()
                .Where(_ => txtData.color.a == 0)
                .Subscribe(_ =>
                {
                    //�ݒ�̃L�����o�X�O���[�v���\������Ă���Ȃ�
                    if (cgSettings.gameObject.activeSelf)
                    {
                        //���_���x���X�V
                        GameData.instance.lookSensitivity = sldLookSensitivity.value * 10f;

                        //���_�̊��炩�����X�V
                        GameData.instance.lookSmooth = sldLookSmooth.value;

                        //�ݒ�̃L�����o�X�O���[�v��񊈐�������
                        cgSettings.gameObject.SetActive(false);

                        //���C���{�^���̃Q�[���I�u�W�F�N�g������������
                        btnMain.gameObject.SetActive(true);
                    }
                    //�ݒ�̃L�����o�X�O���[�v���\������Ă��Ȃ��Ȃ�
                    else
                    {
                        //�ݒ�̃L�����o�X�O���[�v������������
                        cgSettings.gameObject.SetActive(true);

                        //���C���{�^���̃Q�[���I�u�W�F�N�g��񊈐�������
                        btnMain.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);

            //�f�[�^�{�^���������ꂽ�ۂ̏���
            btnData.OnClickAsObservable()
                .Where(_ => !cgSettings.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    //�f�[�^���\������Ă���Ȃ�
                    if (txtData.color.a == 1f)
                    {
                        //�f�[�^�̃e�L�X�g���\���ɂ���
                        txtData.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

                        //���C���{�^���̃Q�[���I�u�W�F�N�g������������
                        btnMain.gameObject.SetActive(true);
                    }
                    //�f�[�^���\������Ă��Ȃ��Ȃ�
                    else
                    {
                        //�f�[�^�̃e�L�X�g��\������
                        txtData.color = Color.black;

                        //���C���{�^���̃Q�[���I�u�W�F�N�g��񊈐�������
                        btnMain.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);

            //�L�����o�X�̈ʒu�����擾����
            RectTransform canvasRectTran = GameObject.Find("Canvas").GetComponent<RectTransform>();

            //����I���{�^���������ꂽ�ۂ̏���
            btnChooseWeapon.OnClickAsObservable()
                .Where(_ => txtData.color.a == 0 && !cgSettings.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    //�S�Ẵ{�^����񊈐�������
                    btnMain.interactable = btnSetting.interactable = btnChooseWeapon.interactable = btnData.interactable = false;

                    //�S�Ă̕s�K�v��UI����莞�Ԃ����Ĕ�\���ɂ���
                    imgLogo.DOFade(0f, 1f);
                    imgMainButton.DOFade(0f, 1f);
                    txtMainButton.DOFade(0f, 1f);
                    cgOtherButtons.DOFade(0f, 1f)
                    .OnComplete(() =>
                    {
                        //�S�Ă̕���̐������J��Ԃ�
                        for (int i = 0; i < GameData.instance.WeaponDataSO.weaponDataList.Count; i++)
                        {
                            //����̃{�^���𐶐�
                            WeaponButtonDetail btnWeapon = Instantiate(BtnWeaponPrefab);

                            //���������{�^���̏����ݒ���s��
                            btnWeapon.SetUpWeaponButton(GameData.instance.WeaponDataSO.weaponDataList[i], this);

                            RectTransform btnWeaponRectTran = btnWeapon.GetComponent<RectTransform>();

                            //���������{�^���̐e��ݒ肷��
                            btnWeaponRectTran.SetParent(canvasRectTran);

                            //�K�؂�y���W���擾����
                            float y = -400f + (800f / (GameData.instance.WeaponDataSO.weaponDataList.Count - 1) * i);

                            //���������{�^���̈ʒu��ݒ肷��
                            btnWeaponRectTran.localPosition = new Vector3(0f, y, 0f);

                            //���������{�^�������X�g�ɒǉ�����
                            btnWeaponList.Add(btnWeapon);
                        }
                    });
                })
                .AddTo(this);

            //�c�e���̃e�L�X�g�̍X�V����
            this.UpdateAsObservable()
                .Subscribe(_ => txtBulletCount.text = GameData.instance.PlayerControllerBase.GetBulletcCount().ToString())
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
            return logoDatasList.Find(x => x.logoType == logoType).sprite;
        }

        /// <summary>
        /// ����I�����I����
        /// </summary>
        public void EndChooseWeapon()
        {
            //����̃{�^���̐������J��Ԃ�
            while (btnWeaponList.Count > 0)
            {
                //����̃{�^��������
                Destroy(btnWeaponList[0].gameObject);

                //���X�g������
                btnWeaponList.RemoveAt(0);
            }

            //����I���{�^��������
            Destroy(btnChooseWeapon.gameObject);

            //�S�Ă�UI���ĕ\������
            imgLogo.DOFade(1f, 1f);
            imgMainButton.DOFade(1f, 1f);
            txtMainButton.DOFade(1f, 1f);
            cgOtherButtons.DOFade(1f, 1f)
                .OnComplete(() =>
                    btnMain.interactable = btnSetting.interactable = btnData.interactable = true);
        }

        /// <summary>
        /// HP�̃X���C�_�[��ݒ肷��
        /// </summary>
        /// <param name="setValue">�ݒ�l�i0�`1�j</param>
        public void SetSldHp(float setValue)
        {
            //HP�̃X���C�_�[�̒l��ݒ肷��
            sldHp.DOValue(setValue, 0.25f);
        }

        /// <summary>
        /// ���_�̃e�L�X�g���X�V����
        /// </summary>
        public void UpdateTxtScore()
        {
            //�`�[��0�̓��_�̃e�L�X�g���X�V����
            txtScoreTeam0.text = GameData.instance.score.team0.ToString();

            //�`�[��1�̓��_�̃e�L�X�g���X�V����
            txtScoreTeam1.text = GameData.instance.score.team1.ToString();
        }
    }
}
