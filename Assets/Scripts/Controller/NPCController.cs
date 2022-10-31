using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// NPCの動きを制御する
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCController : ControllerBase
    {
        private NavMeshAgent agent;//NavMeshAgent

        /// <summary>
        /// NPCControllerの初期設定を行う
        /// </summary>
        protected override void SetUpController()
        {
            //リセット時の処理を呼び出す
            Reset();

            //移動・射撃
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //目標地点を設定
                    SetTargetPos(GetNearEnemyPos());

                    //射線上に敵がいて、弾が残っているなら射撃する
                    if (CheckEnemy() && GetAmmunitionRemaining() > 0) Shot();
                })
                .AddTo(this);
        }

        /// <summary>
        /// リセット時に呼び出される
        /// </summary>
        private void Reset()
        {
            //NavMeshAgentを取得
            agent = GetComponent<NavMeshAgent>();

            //停止距離を設定
            agent.stoppingDistance = ConstData.STOPPING_DISTANCE;

            //（デバック用）
            myTeamNo = 1;
        }

        /// <summary>
        /// 目標地点を設定する
        /// </summary>
        /// <param name="targetPos">目標地点</param>
        private void SetTargetPos(Vector3 targetPos)
        {
            //目標地点を設定
            agent.destination = targetPos;
        }

        /// <summary>
        /// 敵が射線上にいるか調べる
        /// </summary>
        /// <returns>敵が斜線上に居たらtrue</returns>
        private bool CheckEnemy()
        {
            //光線を作成  
            var ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);

            //光線を発射し、光線が何にも触れなかったらfalseを返す
            if (!Physics.Raycast(ray, out RaycastHit hit, GetRange())) return false;

            //光線の接触相手のチーム番号が異なるならtrueを返す
            return (hit.transform.TryGetComponent(out ControllerBase controller) && myTeamNo != controller.myTeamNo);
        }

        /// <summary>
        /// 現在使用している武器の射程距離を取得する
        /// </summary>
        /// <returns>現在使用している武器の射程距離</returns>
        private float GetRange()
        {
            //TODO:射程距離の取得処理
            return 100f;//（仮）
        }

        /// <summary>
        /// 最も近くにいる敵の位置を取得する
        /// </summary>
        /// <returns>最も近くにいる敵の位置</returns>
        private Vector3 GetNearEnemyPos()
        {
            //TODO:近くの敵を探す処理
            return Vector3.zero;//（仮）
        }
    }
}
