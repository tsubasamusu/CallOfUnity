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
    /// UIを制御する
    /// </summary>
    public class UIManager : MonoBehaviour, ISetUp
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
            public LogoType logoType;//ロゴの種類
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

        [SerializeField]
        private Text txtData;//データのテキスト

        [SerializeField]
        private Slider sldHp;//HPのスライダー

        [SerializeField]
        private Text txtBulletCount;//残弾数のテキスト

        [SerializeField]
        private WeaponButtonDetail BtnWeaponPrefab;//武器のボタンのプレファブ

        [HideInInspector]
        public ReactiveProperty<bool> EndedStartPerformance = new(false);//ゲームスタート演出が終わったかどうか

        private List<WeaponButtonDetail> btnWeaponList = new();//武器のボタンのリスト

        /// <summary>
        /// UIManagerの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //メインボタンとその他のボタンを非活性化する
            btnMain.interactable = cgOtherButtons.interactable = false;

            //不必要なキャンバスグループを非表示にする
            cgGameUI.alpha = cgOtherButtons.alpha = 0f;

            //設定を表示する
            cgSettings.alpha = 1f;

            //スライダーを初期値に設定する
            sldLookSensitivity.value = GameData.instance.lookSensitivity / 10f;
            sldLookSmooth.value = GameData.instance.lookSmooth;
            sldHp.value = 1f;

            //得点のテキストを初期値に設定する
            UpdateTxtScore();

            //設定のキャンバスグループを非活性化する
            cgSettings.gameObject.SetActive(false);

            //データのテキストを非表示にする
            txtData.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

            //背景を白色に設定する
            imgBackground.color = new Color(Color.white.r, Color.white.g, Color.white.b, 1f);

            //ロゴをタイトルに設定する
            imgLogo.sprite = GetLogoSprite(LogoType.GameTitle);

            //メインボタンを青色に設定する
            imgMainButton.color = Color.blue;

            //メインボタンのテキストを「Game Start」に設定する
            txtMainButton.text = "Game Start";

            //データのテキストを更新する
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
                .Subscribe(_ =>
                {
                    //武器が選択されていなければ
                    if (GameData.instance.playerWeaponInfo.info0.data == null || GameData.instance.playerWeaponInfo.info1.data == null)
                    {
                        //武器選択ボタンのアニメーションを行う
                        btnChooseWeapon.gameObject.transform.DOScale(1.3f, 0.25f).SetLoops(2, LoopType.Yoyo);

                        //以降の処理を行わない
                        return;
                    }

                    //全てのボタンを非活性化する
                    btnMain.interactable = cgOtherButtons.interactable = false;

                    //背景を一定時間かけて非表示にする
                    imgBackground.DOFade(0f, 1f);

                    //ロゴを一定時間かけて非表示にする
                    imgLogo.DOFade(0f, 1f);

                    //全てのボタンを一定時間かけて非表示にし、演出終了状態に切り替える
                    cgOtherButtons.DOFade(0f, 1f);
                    txtMainButton.DOFade(0f, 1f).OnComplete(() => cgGameUI.alpha = 1f);
                    imgMainButton.DOFade(0f, 1f).OnComplete(() => EndedStartPerformance.Value = true);
                })
                .AddTo(this);

            //設定ボタンが押された際の処理
            btnSetting.OnClickAsObservable()
                .Where(_ => txtData.color.a == 0)
                .Subscribe(_ =>
                {
                    //設定のキャンバスグループが表示されているなら
                    if (cgSettings.gameObject.activeSelf)
                    {
                        //視点感度を更新
                        GameData.instance.lookSensitivity = sldLookSensitivity.value * 10f;

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

            //データボタンが押された際の処理
            btnData.OnClickAsObservable()
                .Where(_ => !cgSettings.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    //データが表示されているなら
                    if (txtData.color.a == 1f)
                    {
                        //データのテキストを非表示にする
                        txtData.color = new Color(Color.black.r, Color.black.g, Color.black.b, 0f);

                        //メインボタンのゲームオブジェクトを活性化する
                        btnMain.gameObject.SetActive(true);
                    }
                    //データが表示されていないなら
                    else
                    {
                        //データのテキストを表示する
                        txtData.color = Color.black;

                        //メインボタンのゲームオブジェクトを非活性化する
                        btnMain.gameObject.SetActive(false);
                    }
                })
                .AddTo(this);

            //キャンバスの位置情報を取得する
            RectTransform canvasRectTran = GameObject.Find("Canvas").GetComponent<RectTransform>();

            //武器選択ボタンを押された際の処理
            btnChooseWeapon.OnClickAsObservable()
                .Where(_ => txtData.color.a == 0 && !cgSettings.gameObject.activeSelf)
                .Subscribe(_ =>
                {
                    //全てのボタンを非活性化する
                    btnMain.interactable = btnSetting.interactable = btnChooseWeapon.interactable = btnData.interactable = false;

                    //全ての不必要なUIを一定時間かけて非表示にする
                    imgLogo.DOFade(0f, 1f);
                    imgMainButton.DOFade(0f, 1f);
                    txtMainButton.DOFade(0f, 1f);
                    cgOtherButtons.DOFade(0f, 1f)
                    .OnComplete(() =>
                    {
                        //全ての武器の数だけ繰り返す
                        for (int i = 0; i < GameData.instance.WeaponDataSO.weaponDataList.Count; i++)
                        {
                            //武器のボタンを生成
                            WeaponButtonDetail btnWeapon = Instantiate(BtnWeaponPrefab);

                            //生成したボタンの初期設定を行う
                            btnWeapon.SetUpWeaponButton(GameData.instance.WeaponDataSO.weaponDataList[i], this);

                            RectTransform btnWeaponRectTran = btnWeapon.GetComponent<RectTransform>();

                            //生成したボタンの親を設定する
                            btnWeaponRectTran.SetParent(canvasRectTran);

                            //適切なy座標を取得する
                            float y = -400f + (800f / (GameData.instance.WeaponDataSO.weaponDataList.Count - 1) * i);

                            //生成したボタンの位置を設定する
                            btnWeaponRectTran.localPosition = new Vector3(0f, y, 0f);

                            //生成したボタンをリストに追加する
                            btnWeaponList.Add(btnWeapon);
                        }
                    });
                })
                .AddTo(this);

            //残弾数のテキストの更新処理
            this.UpdateAsObservable()
                .Subscribe(_ => txtBulletCount.text = GameData.instance.PlayerControllerBase.GetBulletcCount().ToString())
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
            return logoDatasList.Find(x => x.logoType == logoType).sprite;
        }

        /// <summary>
        /// 武器選択を終える
        /// </summary>
        public void EndChooseWeapon()
        {
            //武器のボタンの数だけ繰り返す
            while (btnWeaponList.Count > 0)
            {
                //武器のボタンを消す
                Destroy(btnWeaponList[0].gameObject);

                //リストを消す
                btnWeaponList.RemoveAt(0);
            }

            //武器選択ボタンを消す
            Destroy(btnChooseWeapon.gameObject);

            //全てのUIを再表示する
            imgLogo.DOFade(1f, 1f);
            imgMainButton.DOFade(1f, 1f);
            txtMainButton.DOFade(1f, 1f);
            cgOtherButtons.DOFade(1f, 1f)
                .OnComplete(() =>
                    btnMain.interactable = btnSetting.interactable = btnData.interactable = true);
        }

        /// <summary>
        /// HPのスライダーを設定する
        /// </summary>
        /// <param name="setValue">設定値（0～1）</param>
        public void SetSldHp(float setValue)
        {
            //HPのスライダーの値を設定する
            sldHp.DOValue(setValue, 0.25f);
        }

        /// <summary>
        /// 得点のテキストを更新する
        /// </summary>
        public void UpdateTxtScore()
        {
            //チーム0の得点のテキストを更新する
            txtScoreTeam0.text = GameData.instance.score.team0.ToString();

            //チーム1の得点のテキストを更新する
            txtScoreTeam1.text = GameData.instance.score.team1.ToString();
        }
    }
}
