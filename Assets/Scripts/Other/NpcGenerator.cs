using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// NPCを生成する
    /// </summary>
    public class NpcGenerator : MonoBehaviour, ISetUp
    {
        /// <summary>
        /// NpcGeneratorの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //生成位置のリストを取得
            List<Vector3> spawnPosList = GetSpawnPosList();

            //取得したリストの要素数が正しくなかったら
            if (spawnPosList.Count != ConstData.TEAMMATE_NUMBER * 2 - 1)
            {
                //問題を報告
                Debug.Log("適切な数の生成位置を作成してください");

                //以降の処理を行わない
                return;
            }

            //生成位置のリストの要素数だけ繰り返す
            for (int i = 0; i < spawnPosList.Count; i++)
            {
                //NPCを生成する
                ControllerBase npcControllerBase = Instantiate(GameData.instance.NpcControllerBase);

                //生成したNPCの位置を設定する
                npcControllerBase.transform.position = spawnPosList[i];

                //生成したNPCにチーム番号を与える
                npcControllerBase.myTeamNo = i <= ConstData.TEAMMATE_NUMBER - 2 ? 0 : 1;

                //生成したNPCの初期設定を行う
                npcControllerBase.SetUp();

                //生成したNPCをリストに追加
                GameData.instance.npcControllerBaseList.Add(npcControllerBase);
            }
        }

        /// <summary>
        /// 生成位置のリストを取得する
        /// </summary>
        /// <returns>生成位置のリスト</returns>
        private List<Vector3> GetSpawnPosList()
        {
            //（仮）
            return new List<Vector3> { Vector3.zero };

            //TODO:生成位置のリストを取得する処理
        }
    }
}
