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
            uIManager.EndedStartPerformance
                .Where(_ => uIManager.EndedStartPerformance.Value == true)
                .Subscribe(_ => StartGame())
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
                }
            }
        }
    }
}
