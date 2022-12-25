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
        private Text txtScoreTeam0;//チーム0の得点のテキスト

        [SerializeField]
        private Text txtScoreTeam1;//チーム1の得点のテキスト

        [SerializeField]
        private Button btnMain;//メインボタン

        [SerializeField]
        private Image imgMainButton;//メインボタンのイメージ

        [SerializeField]
        private Text txtMainButton;//ボタンのテキスト

        /// <summary>
        /// UIManagerの初期設定を行う
        /// </summary>
        public void SetUp()
        {

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

        /// <summary>
        /// ゲームスタート演出を行う
        /// </summary>
        /// <returns>待ち時間</returns>
        public IEnumerator PlayGameStart()
        {
            //ゲームスタート演出終了判定用
            bool end = false;

            //メインボタンを非活性化する
            btnMain.interactable = false;

            //試合中のUIのキャンバスグループを非表示にする
            cgGameUI.alpha = 0f;

            //背景を白色に設定する
            imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

            //ロゴをタイトルに設定する
            imgLogo.sprite = GetLogoSprite(LogoType.GameTitle);

            //メインボタンを青色に設定する
            imgMainButton.color = Color.blue;

            //メインボタンのテキストを「Open」に設定する
            txtMainButton.text = "Open";

            //メインボタンを非表示にする
            imgMainButton.DOFade(0f, 0f);
            txtMainButton.DOFade(0f, 0f);

            //ロゴを一定時間かけて表示する
            imgLogo.DOFade(1f, 1f);

            //メインボタンを一定時間かけて表示し、メインボタンを活性化する
            txtMainButton.DOFade(1f, 1f);
            imgMainButton.DOFade(1f, 1f).OnComplete(() => btnMain.interactable = true);

            //メインボタンが押された際の処理
            btnMain.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //メインボタンを非活性化する
                    btnMain.interactable = false;

                    //背景を一定時間かけて非表示にする
                    imgBackground.DOFade(0f, 1f);

                    //ロゴを一定時間かけて非表示にする
                    imgLogo.DOFade(0f, 1f);

                    //メインボタンを一定時間かけて非表示にし、演出終了状態に切り替える
                    txtMainButton.DOFade(0f, 1f);
                    imgMainButton.DOFade(0f, 1f).OnComplete(() => end = true);
                })
                .AddTo(this);

            //ゲームスタート演出が終わるまで待つ
            yield return new WaitUntil(() => end);
        }

        ///// <summary>
        ///// ゲームオーバー演出を行う
        ///// </summary>
        ///// <returns>待ち時間</returns>
        //public IEnumerator PlayGameOver()
        //{
        //    //ゲームオーバー演出終了判定用
        //    bool end = false;

        //    //背景を黒色に設定
        //    imgBackground.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

        //    //ロゴをゲームオーバーに設定
        //    imgLogo.sprite = GetLogoSprite(LogoType.GameOver);

        //    //ボタンを赤色に設定
        //    imgMainButton.color = new Color(Color.red.r, Color.red.g, Color.red.b, 0f);

        //    //ボタンのテキストを「Restart」に設定
        //    txtMainButton.text = "Restart";

        //    //ボタンに登録されている処理を削除
        //    mainButton.onClick.RemoveAllListeners();

        //    //ボタンが押された際の処理を設定
        //    mainButton.onClick.AddListener(() => ClickedButton());

        //    //ボタンを非活性化する
        //    mainButton.interactable = false;

        //    //得点のキャンバスグループを一定時間かけて非表示にする
        //    cgGameUI.DOFade(0f, 1f);

        //    //ロゴを非表示にする
        //    imgLogo.DOFade(0f, 0f)

        //    //背景を一定時間かけて表示する
        //    .OnComplete(() => imgBackground.DOFade(1f, 1f)

        //        //ロゴを一定時間かけて表示する
        //        .OnComplete(() => imgLogo.DOFade(1f, 1f)

        //        .OnComplete(() =>
        //        {
        //            //ボタンのイメージを一定時間かけて表示する
        //            { imgMainButton.DOFade(1f, 1f); }

        //            {
        //                //ボタンのキャンバスグループを一定時間かけて表示する
        //                cgMainButton.DOFade(1f, 1f)

        //                //ボタンを活性化する
        //                .OnComplete(() => mainButton.interactable = true);
        //            }

        //        })));

        //    //ボタンが押された際の処理
        //    void ClickedButton()
        //    {
        //        //効果音を再生
        //        SoundManager.instance.PlaySound(SoundDataSO.SoundName.GameRestartSE);

        //        //背景を一定時間かけて白色にする
        //        imgBackground.DOColor(Color.white, 1f);

        //        //ロゴを一定時間かけて非表示にする
        //        imgLogo.DOFade(0f, 1f);

        //        //ボタンのキャンバスグループを一定時間かけて非表示にする
        //        cgMainButton.DOFade(0f, 1f)

        //            //ゲームオーバー演出が終了した状態に切り替える
        //            .OnComplete(() => end = true);

        //        //ボタンを非活性化する
        //        mainButton.interactable = false;
        //    }

        //    //ゲームオーバー演出が終わるまで待つ
        //    yield return new WaitUntil(() => end == true);
        //}

        ///// <summary>
        ///// ゲームクリア演出を行う
        ///// </summary>
        ///// <returns>待ち時間</returns>
        //public IEnumerator PlayGameClear()
        //{
        //    //ゲームクリア演出終了判定用
        //    bool end = false;

        //    //背景を白色に設定
        //    imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 0f);

        //    //ロゴをゲームクリアに設定
        //    imgLogo.sprite = GetLogoSprite(LogoType.GameClear);

        //    //ボタンを黄色に設定
        //    imgMainButton.color = new Color(Color.yellow.r, Color.yellow.g, Color.yellow.b, 0f);

        //    //ボタンのテキストを「Restart」に設定
        //    txtMainButton.text = "Restart";

        //    //ボタンに登録されている処理を削除
        //    mainButton.onClick.RemoveAllListeners();

        //    //ボタンが押された際の処理を設定
        //    mainButton.onClick.AddListener(() => ClickedButton());

        //    //ボタンを非活性化する
        //    mainButton.interactable = false;

        //    //得点のテキストを一定時間かけて青色に変える
        //    txtScoreTeam0.DOColor(Color.blue, 2f);

        //    //ロゴを非表示にする
        //    imgLogo.DOFade(0f, 0f)

        //    //背景を一定時間かけて表示する
        //    .OnComplete(() => imgBackground.DOFade(1f, 1f)

        //        //ロゴを一定時間かけて表示する
        //        .OnComplete(() => imgLogo.DOFade(1f, 1f)

        //        .OnComplete(() =>
        //        {
        //            //ボタンのイメージを一定時間かけて表示する
        //            { imgMainButton.DOFade(1f, 1f); }

        //            {
        //                //ボタンのキャンバスグループを一定時間かけて表示する
        //                cgMainButton.DOFade(1f, 1f)

        //                //ボタンを活性化する
        //                .OnComplete(() => mainButton.interactable = true);
        //            }

        //        })));

        //    //ボタンが押された際の処理
        //    void ClickedButton()
        //    {
        //        //効果音を再生
        //        SoundManager.instance.PlaySound(SoundDataSO.SoundName.GameRestartSE);

        //        //得点のキャンバスグループを一定時間かけて非表示にする
        //        cgGameUI.DOFade(0f, 1f);

        //        //ロゴを一定時間かけて非表示にする
        //        imgLogo.DOFade(0f, 1f);

        //        //ボタンのキャンバスグループを一定時間かけて非表示にする
        //        cgMainButton.DOFade(0f, 1f)

        //            //ゲームクリア演出が終了した状態に切り替える
        //            .OnComplete(() => end = true);

        //        //ボタンを非活性化する
        //        mainButton.interactable = false;
        //    }

        //    //ゲームクリア演出が終わるまで待つ
        //    yield return new WaitUntil(() => end == true);
        //}

        ///// <summary>
        ///// 得点の表示を更新する準備を行う
        ///// </summary>
        //public void PrepareUpdateTxtScore()
        //{
        //    //得点の表示を更新する
        //    StartCoroutine(UpDateTxtScore());
        //}

        ///// <summary>
        ///// 得点の表示を更新する
        ///// </summary>
        ///// <returns>待ち時間</returns>
        //private IEnumerator UpDateTxtScore()
        //{
        //    //得点のテキストを設定する
        //    txtScoreTeam0.text = GameData.instance.score.playerScore.ToString() + ":" + GameData.instance.score.enemyScore.ToString();

        //    //得点のキャンバスグループを一定時間かけて表示する
        //    cgGameUI.DOFade(1f, 0.25f);

        //    //得点を一定時間、表示し続ける
        //    yield return new WaitForSeconds(0.25f + GameData.instance.DisplayScoreTime);

        //    //プレイヤーが勝利したら
        //    if (GameData.instance.score.playerScore == GameData.instance.MaxScore)
        //    {
        //        //以降の処理を行わない
        //        yield break;
        //    }

        //    //得点のキャンバスグループを一定時間かけて非表示にする
        //    cgGameUI.DOFade(0f, 0.25f);
        //}
    }
}
