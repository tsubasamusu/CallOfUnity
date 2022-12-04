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
        private List<SerializableInterface<ISetUp>> iSetUpList = new();//ISetUpインターフェイスのリスト

        [SerializeField]
        private ControllerBase playerControllerBase;//プレイヤーのControllerBase

        /// <summary>
        /// ゲーム開始直後に呼び出される
        /// </summary>
        void Start()
        {
            //TODO:プレイヤーの所持武器の指定の確認

            //TODO:プレイヤーの所持武器の設定

            //仮
            playerControllerBase.weaponDatas[0] = GameData.instance.WeaponDataSO.weaponDataList[0];
            playerControllerBase.weaponDatas[1] = GameData.instance.WeaponDataSO.weaponDataList[2];

            //各クラスの初期設定を行う
            SetUp();

            //各クラスの初期設定を行う
            void SetUp()
            {
                //ISetUpインターフェイスのリストの要素数だけ繰り返す
                for (int i = 0; i < iSetUpList.Count; i++)
                {
                    //各クラスの初期設定を行う
                    iSetUpList[i].Value.SetUp();
                }
            }
        }
    }
}
