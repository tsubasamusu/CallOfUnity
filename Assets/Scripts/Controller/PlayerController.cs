using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CallOfUnity
{
    /// <summary>
    /// プレイヤーの動きを制御する
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// PlayerControllerの初期設定を行う
        /// </summary>
        protected override void SetUpController()
        {
            //リセット時の処理を呼び出す 
            Reset();

            //Rigidbodyを取得
            Rigidbody rb = GetComponent<Rigidbody>();

            //移動・かがむ・武器チェンジ・ジャンプ
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //ステージから離れすぎたら
                    if (Mathf.Abs((transform.position - Vector3.zero).magnitude) > ConstData.MAX_LENGTH_FROM_CENTER)
                    {
                        //仮
                        float num = 0f;

                        //死ぬ
                        GetComponent<CharacterHealth>().Die(ref num, this);
                    }

                    //移動する
                    Move();

                    //武器チェンジキーが押されたら武器をチェンジする
                    if (Input.GetKeyDown(ConstData.CHANGE_WEAPON_KEY)) ChangeWeapon();

                    //構えるキーが離されたら
                    if (Input.GetKeyUp(ConstData.STANCE_KEY))
                    {
                        //構えるのをやめる
                        Camera.main.DOFieldOfView(ConstData.NORMAL_FOV, ConstData.STANCE_TIME);
                    }
                })
                .AddTo(this);

            //リロード
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.RELOAD_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(GetReloadTime()))
                .Subscribe(_ => ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget())
                .AddTo(this);

            ////射撃
            //this.UpdateAsObservable()
            //    .Where(_ => Input.GetKey(ConstData.SHOT_KEY) && GetBulletcCount() >= 1 && !isReloading)
            //    .ThrottleFirst(TimeSpan.FromSeconds(currentWeaponData.rateOfFire))
            //    .Subscribe(_ => Shot())
            //    .AddTo(this);

            float timer0 = 100f;//プレイヤーの所持武器「0」用のタイマー
            float timer1 = 100f;//プレイヤーの所持武器「1」用のタイマー

            //射撃処理
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //プレイヤーの所持武器「0」用の時間を計測
                    timer0 += Time.deltaTime;

                    //プレイヤーの所持武器「1」用の時間を計測
                    timer1 += Time.deltaTime;

                    //発射キーが押されていないか、残弾数が「1」より少ないか、リロード中なら、以降の処理を行わない
                    if (!Input.GetKey(ConstData.SHOT_KEY) || GetBulletcCount() < 1 || isReloading) return;

                    //プレイヤーが所持武器「0」を使用しているなら
                    if (currentWeaponData != GameData.instance.playerWeaponInfo.info0.data)
                    {
                        //計測時間「0」が連射時間以上になったら
                        if (timer0 >= currentWeaponData.rateOfFire)
                        {
                            //タイマー「0」を初期化する
                            timer0 = 0f;
                            
                            //射撃する
                            Shot();
                        }
                    }
                    //プレイヤーが所持武器「1」を使用しているなら
                    else
                    {
                        //計測時間「1」が連射時間以上になったら
                        if (timer1 >= currentWeaponData.rateOfFire)
                        {
                            //タイマー「1」を初期化する
                            timer1 = 0f; 
                            
                            //射撃する
                            Shot();
                        }
                    }
                })
                .AddTo(this);

            //構える
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.STANCE_KEY))
                .ThrottleFirst(TimeSpan.FromSeconds(ConstData.STANCE_TIME))
                .Subscribe(_ => Camera.main.DOFieldOfView(ConstData.STANCE_FOV, ConstData.STANCE_TIME).SetLink(gameObject))
                .AddTo(this);

            //武器を変更する
            void ChangeWeapon()
            {
                //リロード中か、射撃中は以降の処理を行わない
                if (isReloading || Input.GetKey(ConstData.SHOT_KEY)) return;

                //使用中の武器のデータを更新する
                currentWeaponData = currentWeapoonNo == 0 ?
                    GetWeaponInfo(1).weaponData : GetWeaponInfo(0).weaponData;

                //使用中の武器の番号を更新する
                currentWeapoonNo = currentWeapoonNo == 0 ? 1 : 0;

                //武器のオブジェクトを表示する
                DisplayObjWeapon();
            }

            //移動する
            void Move()
            {
                //プレイヤーに入力された移動方向を取得する
                Vector3 enteredMovement =
                    Vector3.Scale((Camera.main.transform.right * Input.GetAxis("Horizontal")
                    + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                    , new Vector3(1f, 0f, 1f));

                //無操作状態での移動を防止する
                rb.isKinematic = enteredMovement.magnitude == 0f;

                //移動を実行する
                rb.MovePosition(rb.position + (enteredMovement * GetMoveVelocity() * Time.deltaTime));

                //移動速度を取得する
                float GetMoveVelocity()
                {
                    //進行方向に障害物があるなら移動速度を「0」にする（移動しない）
                    if (CheckObstacles()) return 0f;

                    //移動速度を返す
                    return Input.GetKey(ConstData.RUN_KEY) ? ConstData.RUN_SPEED : ConstData.WALK_SPEED;
                }

                //進行方向に障害物がないかを確認する
                bool CheckObstacles()
                {
                    //光線を作成  
                    var ray = new Ray(transform.position + (Vector3.up * 1f), enteredMovement.normalized);

                    //光線が他のコライダーに接触したらtrueを返す 
                    return Physics.Raycast(ray, 1f);
                }
            }
        }

        /// <summary>
        /// 再設定する
        /// </summary>
        public override void ReSetUp()
        {
            //各武器の残弾数を最大値に設定する
            SetBulletCount(0, GetWeaponInfo(0).weaponData.ammunitionNo);
            SetBulletCount(1, GetWeaponInfo(1).weaponData.ammunitionNo);
        }

        /// <summary>
        /// リセット時に呼び出される
        /// </summary>
        private void Reset()
        {
            //自分をプレイヤーに設定する
            isPlayer = true;

            //自分のチーム番号を設定
            myTeamNo = 0;

            //使用中の武器の番号を初期値に設定
            currentWeapoonNo = 0;

            //使用中の武器のデータを初期値に設定
            currentWeaponData = GetWeaponInfo(0).weaponData;
        }
    }
}
