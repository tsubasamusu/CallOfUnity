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

        /// <summary>
        /// ゲーム開始直後に呼び出される
        /// </summary>
        void Start()
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
