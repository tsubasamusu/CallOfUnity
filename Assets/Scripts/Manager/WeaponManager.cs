using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 武器・弾に関する処理を行う
    /// </summary>
    public class WeaponManager : MonoBehaviour,ISetUp
    {
        public static WeaponManager instance;//インスタンス

        /// <summary>
        /// Startメソッドより前に呼び出される
        /// </summary>
        private void Awake()
        {
            //以下、シングルトンに必須の記述
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// WeaponManagerの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            
        }
    }
}
