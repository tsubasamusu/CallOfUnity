using System.Collections;
using System.Collections.Generic;
using TNRD;
using UnityEngine;

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
        /// <returns>待ち時間</returns>
        private IEnumerator Start()
        {
            //各クラスの初期設定を行う（1回目）
            SetUp(0);

            //ゲームスタート演出を行う
            yield return StartCoroutine(uIManager.PlayGameStart());

            //TODO:プレイヤーの所持武器の指定の確認と設定
            GameData.instance.playerWeaponInfo.info0.data = GameData.instance.WeaponDataSO.weaponDataList[0];
            GameData.instance.playerWeaponInfo.info1.data = GameData.instance.WeaponDataSO.weaponDataList[2];

            //各クラスの初期設定を行う（2回目）
            SetUp(1);

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
        }
    }
}
