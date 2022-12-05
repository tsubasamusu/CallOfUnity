using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

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

            //射撃とリロードの制御を開始
            ShotReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

            //移動
            this.UpdateAsObservable()
                .Subscribe(_ => SetTargetPos(GetNearEnemyPos()))
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

            //WeaponDataSOを取得
            WeaponDataSO weaponDataSO = GameData.instance.WeaponDataSO;

            //所持武器の数だけ繰り返す
            for(int i=0;i<weaponDatas.Length;i++) 
            {
                //所持武器をランダムに決定
                weaponDatas[i] = weaponDataSO.weaponDataList[UnityEngine.Random.Range(0, weaponDataSO.weaponDataList.Count)];
            }

            //（デバック用）
            myTeamNo = 1;
        }

        /// <summary>
        /// 射撃とリロードを制御する
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>待ち時間</returns>
        private async UniTask ShotReloadAsync(CancellationToken token)
        {
            //無限に繰り返す
            while(true)
            {
                //残弾数が0なら
                if(GetAmmunitionRemaining()==0)
                {
                    //リロードする
                    ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

                    //リロードが終わるまで待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(GetReloadTime()), cancellationToken: token);
                }

                //射線上に敵がいて、弾が残っているなら
                if (CheckEnemy() && GetAmmunitionRemaining() > 0)
                {
                    //次弾が撃てるまで待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(GetRateOfFire()), cancellationToken: token);

                    //射撃する
                    Shot();
                }

                //1フレーム待つ
                await UniTask.Yield(token);
            }
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
            var ray = new Ray(weaponTran.position, transform.forward);

            //光線を発射し、光線が何にも触れなかったらfalseを返す
            if (!Physics.Raycast(ray, out RaycastHit hit, currentWeapon.firingRange)) return false;

            //光線の接触相手のチーム番号が異なるならtrueを返す
            return (hit.transform.TryGetComponent(out ControllerBase controller) && myTeamNo != controller.myTeamNo);
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
