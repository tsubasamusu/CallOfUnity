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
    /// ゲームの進行を制御する
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// ゲーム終了の種類
        /// </summary>
        private enum EndGameType
        {
            GameClear,//ゲームクリア
            GameOver,//ゲームオーバー
            GameDraw//引き分け
        }

        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList0 = new();//ISetUpインターフェイスのリスト0

        [SerializeField]
        private List<SerializableInterface<ISetUp>> iSetUpList1 = new();//ISetUpインターフェイスのリスト1

        [SerializeField]
        private ControllerBase playerControllerBase;//プレイヤーのControllerBase

        [SerializeField]
        private UIManager uIManager;//UIManager

        /// <summary>
        /// ゲーム開始直後に呼び出される
        /// </summary>
        private void Start()
        {
            //各クラスの初期設定を行う（1回目）
            SetUp(0);

            //ゲームスタート演出が終わった際の処理
            uIManager.endedStartPerformance
                .Where(_ => uIManager.endedStartPerformance.Value == true)
                .Subscribe(_ => StartGame())
                .AddTo(this);

            //試合を開始する
            void StartGame()
            {
                //各クラスの初期設定を行う（2回目）
                SetUp(1);

                //制限時間の更新を開始する
                UpdateTimeLimitAsync(this.GetCancellationTokenOnDestroy()).Forget();

                //制限時間を更新する
                async UniTaskVoid UpdateTimeLimitAsync(CancellationToken token)
                {
                    //制限時間保持用
                    float timeLimit = ConstData.TIME_LIMIT;

                    //無限に繰り返す
                    while (true)
                    {
                        //制限時間を更新する
                        timeLimit -= Time.deltaTime;

                        //制限時間のテキストを更新する
                        uIManager.SetTxtTimeLimit(timeLimit);

                        //制限時間が終ったら
                        if(timeLimit<=0f)
                        {
                            //繰り返し処理から抜け出す
                            break;
                        }

                        //チーム0が勝利したら
                        if(GameData.instance.score.team0>=ConstData.WIN_SCORE)
                        {
                            //ゲームを終了する
                            EndGame(EndGameType.GameClear);

                            //繰り返し処理から抜け出す
                            break;
                        }
                        //チーム1が勝利したら
                        else if(GameData.instance.score.team1>=ConstData.WIN_SCORE) 
                        {
                            //ゲームを終了する
                            EndGame(EndGameType.GameOver);

                            //繰り返し処理から抜け出す
                            break;
                        }

                        //1フレーム待つ
                        await UniTask.Yield(token);
                    }

                    //制限時間が残っていたなら
                    if(timeLimit>0f)
                    {
                        //以降の処理を行わない
                        return;
                    }

                    //TODO:制限時間による試合終了の処理
                }
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
            void EndGame(EndGameType endGameType)
            {
                //ゲーム終了の種類に応じて処理を変更する
                switch (endGameType)
                {
                    //ゲームクリアなら
                    case EndGameType.GameClear:
                        break;

                    //ゲームオーバーなら
                    case EndGameType.GameOver:
                        break;

                    //引き分けなら
                    case EndGameType.GameDraw:
                        break;
                }
            }
        }
    }
}
