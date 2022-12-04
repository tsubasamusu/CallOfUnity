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

        [HideInInspector]
        public WeaponDataSO.WeaponData[] weaponDatas = new WeaponDataSO.WeaponData[2];//所持武器のデータ

        [HideInInspector]
        public WeaponDataSO.WeaponData currentWeapon;//使用中の武器

        private int allBulletCount;//総残弾数

        private int[] bulletCounts = new int[2];//それぞれの武器の残弾数

        protected Transform weaponTran;//武器の位置

        /// <summary>
        /// 「総残弾数」の取得用
        /// </summary>
        public int AllBulletCount { get => allBulletCount; }

        /// <summary>
        /// 「それぞれの武器の残弾数」の取得用
        /// </summary>
        public int[] BulletCounts { get => bulletCounts; }

        /// <summary>
        /// ControllerBaseの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //武器の位置を取得
            weaponTran = transform.GetChild(0).transform.GetChild(0).transform;

            //総残弾数を初期値に設定
            allBulletCount = ConstData.FIRST_ALL_BULLET_COUNT;

            //子クラスの初期設定を行う
            SetUpController();

            //所持武器の数だけ繰り返す
            for (int i = 0; i < weaponDatas.Length; i++)
            {
                //所持武器が設定されているなら
                if (weaponDatas[i] != null)
                {
                    //使用中の武器を設定
                    if (i == 0) currentWeapon = weaponDatas[i];
                }
                //所持武器が設定されていないなら
                else
                {
                    //問題を報告
                    Debug.Log("所持武器が設定されていません");
                }
            }
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
