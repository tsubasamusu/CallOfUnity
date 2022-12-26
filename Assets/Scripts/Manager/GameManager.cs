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
    /// ゲームの進行を制御する
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList0 = new();//ISetUpインターフェイスのリスト0

        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList1 = new();//ISetUpインターフェイスのリスト1

        /// <summary>
        /// ゲーム開始直後に呼び出される
        /// </summary>
        private void Start()
        {
            //各クラスの初期設定を行う（1回目）
            SetUp(0);

            //ゲームスタート演出が終わった際の処理
            GameData.instance.UiManager.EndedGameStartPerformance
                .Where(_ => GameData.instance.UiManager.EndedGameStartPerformance.Value == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //得点の監視処理
            GameData.instance.Score
                .Subscribe(_ =>
                {
                    if (GameData.instance.Score.Value.team0 >= ConstData.WIN_SCORE) EndGame(true);
                    if (GameData.instance.Score.Value.team1 >= ConstData.WIN_SCORE) EndGame(false);
                })
                .AddTo(this);

            //試合を開始する
            void StartGame()
            {
                //各クラスの初期設定を行う（2回目）
                SetUp(1);
            }

            //各クラスの初期設定を行う
            void SetUp(int setUpNo)
            {
                //ISetUpインターフェイスのリストの要素数だけ繰り返す
                for (int i = 0; i < (setUpNo == 0 ? iSetUpList0 : iSetUpList1).Count; i++)
                {
                    //各クラスの初期設定を行う
                    (setUpNo == 0 ? iSetUpList0 : iSetUpList1)[i].Value.SetUp();
                }
            }

            //ゲームを終了する
            void EndGame(bool isGameClear)
            {
                //カメラを独立させる
                Camera.main.transform.parent = null;

                //不要なゲームオブジェクトを全て消す
                {
                    Destroy(GameData.instance.TemporaryObjectContainerTran.gameObject);
                    while (GameData.instance.npcControllerBaseList.Count > 0)
                    {
                        Destroy(GameData.instance.npcControllerBaseList[0].gameObject);
                        GameData.instance.npcControllerBaseList.RemoveAt(0);
                    }
                    Destroy(GameData.instance.PlayerControllerBase.gameObject);
                }

                //データを保存する
                GameData.instance.SaveData();

                //ゲームを終了演出を行う
                GameData.instance.UiManager.PlayGameEndPerformance(isGameClear);

                //ゲーム終了演出が終った際の処理
                GameData.instance.UiManager.EndedGameEndPerformance
                    .Where(_ => GameData.instance.UiManager.EndedGameEndPerformance.Value == true)
                    .Subscribe(_ => SceneManager.LoadScene("Main"))
                    .AddTo(this);
            }
        }
    }
}
