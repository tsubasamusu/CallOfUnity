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
            //体の位置情報を取得
            Transform bodyTran = transform.GetChild(0);

            //リセット時の処理を呼び出す
            Reset();

            //射撃とリロードの制御を開始する
            ShotReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

            //移動・体の向きの調整
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //近くの敵の座標を取得する
                    Vector3 nearEnemyPos = GetNearEnemyPos();

                    //目標地点を設定する
                    SetTargetPos(nearEnemyPos);

                    //適切な体の向きを取得する
                    Vector3 bodyDir =
                    ((nearEnemyPos + new Vector3(0f, 1.5f, 0f)) - (transform.position + new Vector3(0f, 1.5f, 0f))).normalized;

                    //NPCを敵の方向へ滑らかに向かせる
                    bodyTran.forward = Vector3.Lerp(bodyTran.forward, bodyDir, Time.deltaTime * ConstData.WEAPON_ROT_SMOOTH);
                })
                .AddTo(this);
        }

        /// <summary>
        /// リセット時に呼び出される
        /// </summary>
        private void Reset()
        {
            //NavMeshAgentを取得する
            agent = GetComponent<NavMeshAgent>();

            //使用武器をランダムに決定
            currentWeaponData = GameData.instance.WeaponDataSO
                .weaponDataList[UnityEngine.Random.Range(0, GameData.instance.WeaponDataSO.weaponDataList.Count)];

            //自分の色を設定する
            transform.GetChild(1).GetComponent<MeshRenderer>().material = myTeamNo == 0 ?
                GameData.instance.Team0Material : GameData.instance.Team1Material;

            //CharacterHealthの初期設定を行う
            GetComponent<CharacterHealth>().SetUp();
        }

        /// <summary>
        /// 再設定する
        /// </summary>
        public override void ReSetUp()
        {
            //残弾数を最大値に設定
            bulletCountForNpc = currentWeaponData.ammunitionNo;
        }

        /// <summary>
        /// 射撃とリロードを制御する
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>待ち時間</returns>
        private async UniTask ShotReloadAsync(CancellationToken token)
        {
            //無限に繰り返す
            while (true)
            {
                //残弾数が「0」以下なら
                if (GetBulletcCount() <= 0)
                {
                    //リロードする
                    ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

                    //リロードが終わるまで待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(GetReloadTime()), cancellationToken: token);
                }

                //射線上に敵がいて、弾が残っており、リロード中でないなら
                if (CheckEnemy() && GetBulletcCount() >= 1 && !isReloading)
                {
                    //次弾が撃てるまで待つ
                    await UniTask.Delay(TimeSpan.FromSeconds(currentWeaponData.rateOfFire), cancellationToken: token);

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
            var ray = new Ray(weaponTran.position, weaponTran.forward);

            //光線を発射し、光線が何にも触れなかったらfalseを返す
            if (!Physics.Raycast(ray, out RaycastHit hit, currentWeaponData.firingRange)) return false;

            //光線の接触相手のチーム番号が異なるならtrueを返す
            return (hit.transform.TryGetComponent(out ControllerBase controller) && myTeamNo != controller.myTeamNo);
        }

        /// <summary>
        /// 最も近くにいる敵の位置を取得する
        /// </summary>
        /// <returns>最も近くにいる敵の位置</returns>
        private Vector3 GetNearEnemyPos()
        {
            //「最も近くにいる敵」を仮登録する
            ControllerBase nearEnemy = myTeamNo == 0 ?
                GameData.instance.npcControllerBaseList[ConstData.TEAMMATE_NUMBER - 1]
                : GameData.instance.PlayerControllerBase;

            //「最も近くにいる敵との距離」を仮登録する
            float nearLength =
                ((myTeamNo == 0 ?
                GameData.instance.npcControllerBaseList[ConstData.TEAMMATE_NUMBER - 1]
                : GameData.instance.PlayerControllerBase).transform.position - transform.position).magnitude;

            //NPCの数だけ繰り返す
            for (int i = 0; i < GameData.instance.npcControllerBaseList.Count; i++)
            {
                //繰り返し処理で取得したNPCが味方なら、次の繰り返し処理に移る
                if (GameData.instance.npcControllerBaseList[i].myTeamNo == myTeamNo) continue;

                //繰り返し処理で取得したNPCとの距離を取得する
                float length = (GameData.instance.npcControllerBaseList[i].transform.position - transform.position).magnitude;

                //記録を更新したら
                if (length < nearLength) 
                {
                    //「最も近くにいる敵」を更新する
                    nearEnemy = GameData.instance.npcControllerBaseList[i];

                    //「最も近くにいる敵との距離」を更新する
                    nearLength = length; 
                }
            }

            //「最も近くにいる敵の位置」を返す
            return nearEnemy.transform.position;
        }
    }
}
