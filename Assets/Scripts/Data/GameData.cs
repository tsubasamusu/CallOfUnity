using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 変動値を管理
    /// </summary>
    public class GameData : MonoBehaviour
    {
        [Header("視点感度"), Range(0.1f, 10f)]
        public float lookSensitivity;//視点感度

        [Header("視点の滑らかさ"), Range(0.1f, 1f)]
        public float lookSmooth;//視点の滑らかさ

        [Header("重力")]
        public float gravity;//重力

        [HideInInspector]
        public (int team0 ,int team1) score;//得点

        [SerializeField, Header("リスポーン地点のリスト")]
        private List<Transform> respawnTransList = new();

        [SerializeField,Header("ロケットランチャーのエフェクト")]
        private GameObject objRocketLauncherEffect;//ロケットランチャーのエフェクト

        [SerializeField,Header("ゲームオブジェクトの一時的な親")]
        private Transform temporaryObjectContainerTran;

        [SerializeField, Header("NPCのプレファブ")]
        private ControllerBase npcControllerBase;//NPCのプレファブ

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        [HideInInspector]
        public List<ControllerBase> npcList = new();//NPCのリスト

        [HideInInspector]
        public List<WeaponDataSO.WeaponData> weaponDataListForPlayer = new();//プレーヤー用の所持武器のリスト

        /// <summary>
        /// 「リスポーン地点のリスト」の取得用
        /// </summary>
        public List<Transform> RespawnTransList { get => respawnTransList; }

        /// <summary>
        /// 「ロケットランチャーのエフェクト」の取得用
        /// </summary>
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

        /// <summary>
        /// 「ゲームオブジェクトの一時的な親」の取得用
        /// </summary>
        public Transform TemporaryObjectContainerTran { get => temporaryObjectContainerTran; }

        /// <summary>
        /// 「NPCのプレファブ」の取得用
        /// </summary>
        public ControllerBase NpcControllerBase { get => npcControllerBase; }

        /// <summary>
        /// 「WeaponDataSO」の取得用
        /// </summary>
        public WeaponDataSO WeaponDataSO { get => weaponDataSO; }

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