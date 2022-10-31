using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// PlayerControllerクラスとNPCControllerクラスの親クラス
    /// </summary>
    public class ControllerBase : MonoBehaviour, ISetUp
    {
        [HideInInspector]
        public int myTeamNo;//自分のチーム番号

        /// <summary>
        /// ControllerBaseの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //重力を生成する
            this.UpdateAsObservable()
                .Where(_ => !CheckGrounded())
                .Subscribe(_ => transform.Translate(Vector3.down * ConstData.GRAVITY * Time.deltaTime))
                .AddTo(this);

            //子クラスの初期設定を行う
            SetUpController();
        }

        /// <summary>
        /// 子クラスの初期設定を行う
        /// </summary>
        protected virtual void SetUpController()
        {
            //子クラスで処理を記述する
        }

        /// <summary>
        /// 接地判定を行う
        /// </summary>
        /// <returns>接地していたらtrue</returns>
        protected bool CheckGrounded()
        {
            //光線を作成 
            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

            //光線が他のコライダーに接触したらtrueを返す
            return Physics.Raycast(ray, 0.15f);
        }

        /// <summary>
        /// リロードする
        /// </summary>
        protected void Reload()
        {
            //TODO:リロード処理
            Debug.Log("リロード");
        }

        /// <summary>
        /// 射撃する
        /// </summary>
        protected void Shot()
        {
            //TODO:射撃処理
            Debug.Log("射撃");
        }

        /// <summary>
        /// 現在使用している武器のリロード時間を取得する
        /// </summary>
        /// <returns>現在使用している武器のリロード時間</returns>
        protected float GetReloadTime()
        {
            //TODO:リロード時間取得処理
            return 3f;//（仮）
        }

        /// <summary>
        /// 現在使用している武器の連射速度を取得する
        /// </summary>
        /// <returns>現在使用している武器の連射速度</returns>
        protected float GetRateOfFire()
        {
            //TODO:連射速度取得処理
            return 1f;//（仮）
        }

        /// <summary>
        /// 現在使用している武器の残弾数を取得する
        /// </summary>
        /// <returns>現在使用している武器の残弾数</returns>
        protected float GetAmmunitionRemaining()
        {
            //TODO:残弾数の取得処理
            return 100f;//（仮）
        }
    }
}
