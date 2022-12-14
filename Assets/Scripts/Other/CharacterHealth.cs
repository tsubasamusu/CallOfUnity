using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// キャラクターの体力を管理する
    /// </summary>
    public class CharacterHealth : MonoBehaviour, ISetUp
    {
        /// <summary>
        /// CharacterHealthの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //HP保持用
            float hp = 100f;

            //Rigidbodyを取得する
            Rigidbody rb = GetComponent<Rigidbody>();

            //ControllerBaseを取得する
            ControllerBase controllerBase = GetComponent<ControllerBase>();

            //弾に触れた際の処理
            this.OnCollisionEnterAsObservable()
                .Where(collision => collision.transform.TryGetComponent(out BulletDetailBase _))
                .Subscribe(collision =>
                {
                    //物理演算を無効化する
                    if (!rb.isKinematic) rb.isKinematic = true;

                    //BulletdetailBaseを取得する
                    BulletDetailBase bulletDetailBase = collision.transform.GetComponent<BulletDetailBase>();

                    //触れた弾が敵チームの弾なら
                    if (bulletDetailBase.MyTeamNo != controllerBase.myTeamNo)
                    {
                        //HPを更新する
                        hp = Mathf.Clamp(hp - bulletDetailBase.WeaponData.attackPower, 0f, 100f);

                        //自分がプレイヤーなら
                        if (controllerBase.IsPlayer)
                        {
                            //HPのスライダーを設定する
                            GameData.instance.UiManager.SetSldHp(hp / 100f);

                            //効果音を再生する
                            SoundManager.instance.PlaySound(SoundDataSO.SoundName.被弾した時の音);
                        }
                        //触れた弾の持ち主がプレイヤーなら
                        else if (bulletDetailBase.IsPlayerBullet)
                        {
                            //UIのアニメーションを行う
                            GameData.instance.UiManager.PlayTxtGaveDamageAnimation(bulletDetailBase.WeaponData.attackPower);
                        }
                    }

                    //触れた弾を消す
                    Destroy(collision.gameObject);

                    //HPが0なら死亡処理を行う
                    if (hp == 0f) Die(ref hp, controllerBase, bulletDetailBase);
                })
                .AddTo(this);
        }

        /// <summary>
        /// 死亡処理
        /// </summary>
        /// <param name="hp">自分のHP</param>
        /// <param name="controllerBase">自分のControllerBase</param>
        /// <param name="bulletDetailBase">接触相手のBulletDetailBase</param>
        public void Die(ref float hp, ControllerBase controllerBase, BulletDetailBase bulletDetailBase = null)
        {
            //自分がプレイヤーなら
            if (controllerBase.IsPlayer)
            {
                //プレイヤーのデス数を「1」増やす
                GameData.instance.playerTotalDeathCount++;

                //リロードゲージのアニメーションを止める
                GameData.instance.UiManager.StopReloadGaugeAnimation();
            }

            //プレイヤーの弾によって死亡したら
            if (bulletDetailBase != null && bulletDetailBase.IsPlayerBullet)
            {
                //プレイヤーのキル数を「1」増やす
                GameData.instance.playerTotalKillCount++;
            }

            //自分がチーム0なら
            if (controllerBase.myTeamNo == 0)
            {
                //チーム1の得点を増やす
                GameData.instance.Score.Value
                    = (GameData.instance.Score.Value.team0, (GameData.instance.Score.Value.team1 + 1));
            }
            //自分がチーム1なら
            else
            {
                //チーム0の得点を増やす
                GameData.instance.Score.Value
                    = ((GameData.instance.Score.Value.team0 + 1), GameData.instance.Score.Value.team1);
            }

            //再設定する
            controllerBase.ReSetUp();

            //HPを初期値に戻す
            hp = 100f;

            //自分がプレイヤーなら、HPのスライダーを初期値に設定する
            if (controllerBase.IsPlayer) GameData.instance.UiManager.SetSldHp(1f);

            //得点のテキストを更新する
            GameData.instance.UiManager.UpdateTxtScore();

            //リスポーンする
            transform.position = controllerBase.myTeamNo == 0 ?
                GameData.instance.RespawnTransList[0].position
                : GameData.instance.RespawnTransList[1].position;
        }
    }
}
