using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 変動値を管理
    /// </summary>
    public class GameData : MonoBehaviour
    {
        [SerializeField, Header("視点感度"), Range(0.1f, 10f)]
        private float lookSensitivity;//視点感度

        [SerializeField, Header("視点の滑らかさ"), Range(0.1f, 1f)]
        private float lookSmooth;//視点の滑らかさ

        [Header("重力")]
        public float gravity;//重力

        [SerializeField,Header("ロケットランチャーのエフェクト")]
        private GameObject objRocketLauncherEffect;//ロケットランチャーのエフェクト

        //「視点感度」の取得・設定用
        public float LookSensitivity { get => lookSensitivity; set => lookSensitivity = value; }

        //「視点の滑らかさ」の取得・設定用
        public float LookSmooth { get => lookSmooth; set => lookSmooth = value; }

        //「ロケットランチャーのエフェクト」の取得用
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

        public static GameData instance;//インスタンス

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
    }
}