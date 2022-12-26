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

            //生成位置のリストの要素数だけ繰り返す
            for (int i = 0; i < spawnPosList.Count; i++)
            {
                //NPCを生成する
                ControllerBase npcControllerBase = Instantiate(GameData.instance.NpcControllerBase);

                //生成したNPCの位置を設定する
                npcControllerBase.transform.position = spawnPosList[i];

                //生成したNPCにチーム番号を与える
                npcControllerBase.myTeamNo = i <= ConstData.TEAMMATE_NUMBER - 2 ? 0 : 1;

                //チーム1のNPCをステージ内側に向かせる
                if (npcControllerBase.myTeamNo == 1) npcControllerBase.transform.Rotate(0, 180f, 0);

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
            //リストを用意する
            List<Vector3> spawnPosList = new();

            //必要なNPCの数だけ繰り返す
            for (int i = 0; i < ConstData.TEAMMATE_NUMBER * 2 - 1; i++)
            {
                //チーム0の生成位置を作成しているなら
                if (i <= ConstData.TEAMMATE_NUMBER - 2)
                {
                    //最初の生成位置のx座標を取得
                    float firstPosX = -2f * ((ConstData.TEAMMATE_NUMBER - 1) / 2f);

                    //作成した座標をリストに追加する
                    spawnPosList.Add(new Vector3(firstPosX + (2f * i), 0f, -25f));

                    //次の繰り返し処理に移る
                    continue;
                }

                //最初の生成位置のx座標を取得
                float firstPosX2 = -2f * (ConstData.TEAMMATE_NUMBER / 2f);

                //作成した座標をリストに追加する
                spawnPosList.Add(new Vector3(firstPosX2 + (2f * (i - (ConstData.TEAMMATE_NUMBER - 1))), 0f, 25f));
            }

            //作成したリストを返す
            return spawnPosList;
        }
    }
}
