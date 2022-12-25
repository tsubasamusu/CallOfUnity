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
    /// UIを制御する
    /// </summary>
    public class UIManager : MonoBehaviour,ISetUp
    {
        /// <summary>
        /// ロゴの種類
        /// </summary>
        private enum LogoType
        {
            GameTitle,//ゲームタイトル
            GameOver,//ゲームオーバー
            GameClear//ゲームクリア
        }

        /// <summary>
        /// ロゴのデータを管理する 
        /// </summary>
        [Serializable]
        private class LogoData
        {
            public LogoType LogoType;//ロゴの種類
            public Sprite sprite;//スプライト
        }

        [SerializeField]
        private List<LogoData> logoDatasList = new();//ロゴのデータのリスト

        [SerializeField]
        private Image imgLogo;//ロゴ

        [SerializeField]
        private Image imgBackground;//背景

        [SerializeField]
        private CanvasGroup cgGameUI;//試合中のUIのキャンバスグループ

        [SerializeField]
        private CanvasGroup cgOtherButtons;//その他のボタン

        [SerializeField]
        private Text txtScoreTeam0;//チーム0の得点のテキスト

        [SerializeField]
        private Text txtScoreTeam1;//チーム1の得点のテキスト

        [SerializeField]
        private Button btnMain;//メインボタン

        [SerializeField]
        private Button btnSetting;//設定ボタン

        [SerializeField]
        private Button btnChooseWeapon;//武器選択ボタン

        [SerializeField]
        private Button btnData;//データボタン

        [SerializeField]
        private Image imgMainButton;//メインボタンのイメージ

        [SerializeField]
        private Text txtMainButton;//メインボタンのテキスト

        [SerializeField]
        private CanvasGroup cgSettings;//設定のキャンバスグループ

        [SerializeField]
        private Slider sldLookSensitivity;//視点感度のスライダー

        [SerializeField]
        private Slider sldLookSmooth;//視点の滑らかさのスライダー

        [HideInInspector]
        public ReactiveProperty<bool> endedStartPerformance = new(false);//ゲームスタート演出が終わったかどうか

        /// <summary>
        /// UIManagerの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //メインボタンとその他のボタンを非活性化する
            btnMain.interactable = cgOtherButtons.interactable = false;

            //不必要なキャンバスグループを非表示にする
            cgGameUI.alpha = cgOtherButtons.alpha= 0f;

            //設定を表示する
            cgSettings.alpha = 1f;

            //設定のキャンバスグループを非活性化する
            cgSettings.gameObject.SetActive(false);

            //背景を白色に設定する
            imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

            //ロゴをタイトルに設定する
            imgLogo.sprite = GetLogoSprite(LogoType.GameTitle);

            //メインボタンを青色に設定する
            imgMainButton.color = Color.blue;

            //メインボタンのテキストを「Game Start」に設定する
            txtMainButton.text = "Game Start";

            //メインボタンを非表示にする
            imgMainButton.DOFade(0f, 0f);
            txtMainButton.DOFade(0f, 0f);

            //ロゴを一定時間かけて表示する
            imgLogo.DOFade(1f, 1f);

            //全てののボタンを一定時間かけて表示し、活性化する
            txtMainButton.DOFade(1f, 1f);
            cgOtherButtons.DOFade(1f, 1f).OnComplete(() => cgOtherButtons.interactable = true);
            imgMainButton.DOFade(1f, 1f).OnComplete(() => btnMain.interactable = true);

            //メインボタンが押された際の処理
            btnMain.OnClickAsObservable()
                .Where(_ => GameData.instance.playerWeaponInfo.info0.data != null && GameData.instance.playerWeaponInfo.info1.data != null)
                .Subscribe(_ =>
                {
                    //全てのボタンを非活性化する
                    btnMain.interactable = cgOtherButtons.interactable = false;

                    //背景を一定時間かけて非表示にする
                    imgBackground.DOFade(0f, 1f);

                    //ロゴを一定時間かけて非表示にする
                    imgLogo.DOFade(0f, 1f);

                    //全てのボタンを一定時間かけて非表示にし、演出終了状態に切り替える
                    cgOtherButtons.DOFade(0f, 1f);
                    txtMainButton.DOFade(0f, 1f).OnComplete(() => cgGameUI.alpha = 1f);
                    imgMainButton.DOFade(0f, 1f).OnComplete(() => endedStartPerformance.Value = true);
                })
                .AddTo(this);

            //設定ボタンが押された際の処理
            btnSetting.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //設定のキャンバスグループが表示されているなら
                    if(cgSettings.gameObject.activeSelf)
                    {
                        //視点感度を更新
                        GameData.instance.lookSensitivity= sldLookSensitivity.value * 10f;

                        //視点の滑らかさを更新
                        GameData.instance.lookSmooth = sldLookSmooth.value;

                        //設定のキャンバスグループを非活性化する
                        cgSettings.gameObject.SetActive(false);

                        //メインボタンのゲームオブジェクトを活性化する
                        btnMain.gameObject.SetActive(true);
                    }
                    //設定のキャンバスグループが表示されていないなら
                    else
                    {
                        //設定のキャンバスグループを活性化する
                        cgSettings.gameObject.SetActive(true);

                        //メインボタンのゲームオブジェクトを非活性化する
                        btnMain.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// ロゴのスプライトを取得する
        /// </summary>
        /// <param name="logoType">ロゴの種類</param>
        /// <returns>ロゴのスプライト</returns>
        private Sprite GetLogoSprite(LogoType logoType)
        {
            //適切なロゴのスプライトを返す
            return logoDatasList.Find(x => x.LogoType == logoType).sprite;
        }
    }
}
