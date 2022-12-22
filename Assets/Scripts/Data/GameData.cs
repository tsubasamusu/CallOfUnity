using System.Collections.Generic;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 変動値を管理
    /// </summary>
    public class GameData : MonoBehaviour
    {
        [Range(0.1f, 10f)]
        public float lookSensitivity;//視点感度

        [Range(0.1f, 1f)]
        public float lookSmooth;//視点の滑らかさ

        [SerializeField]
        private List<Transform> respawnTransList = new();

        [SerializeField]
        private GameObject objRocketLauncherEffect;//ロケットランチャーのエフェクト

        [SerializeField]
        private GameObject objExplosionEffect;//爆発のエフェクト

        [SerializeField]
        private Transform temporaryObjectContainerTran;

        [SerializeField]
        private ControllerBase npcControllerBase;//NPCのプレファブ

        [SerializeField]
        private ControllerBase playerControllerBase;//プレイヤー

        [SerializeField]
        private Material team0Material;//チーム0のマテリアル

        [SerializeField]
        private Material team1Material;//チーム1のマテリアル

        [HideInInspector]
        public (int team0, int team1) score;//得点

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        [HideInInspector]
        public List<ControllerBase> npcControllerBaseList = new();//NPCのリスト

        [HideInInspector]
        public ((WeaponDataSO.WeaponData data, int bulletCount) info0,
            (WeaponDataSO.WeaponData data, int bulletCount) info1)
            playerWeaponInfo;//プレイヤーの所持武器の情報

        /// <summary>
        /// 「リスポーン地点のリスト」の取得用
        /// </summary>
        public List<Transform> RespawnTransList { get => respawnTransList; }

        /// <summary>
        /// 「ロケットランチャーのエフェクト」の取得用
        /// </summary>
        public GameObject ObjRocketLauncherEffect { get => objRocketLauncherEffect; }

        /// <summary>
        /// 「爆発のエフェクト」の取得用
        /// </summary>
        public GameObject ObjExplosionEffect { get => objExplosionEffect; }

        /// <summary>
        /// 「ゲームオブジェクトの一時的な親」の取得用
        /// </summary>
        public Transform TemporaryObjectContainerTran { get => temporaryObjectContainerTran; }

        /// <summary>
        /// 「NPCのプレファブ」の取得用
        /// </summary>
        public ControllerBase NpcControllerBase { get => npcControllerBase; }

        /// <summary>
        /// 「プレイヤー」の取得用
        /// </summary>
        public ControllerBase PlayerControllerBase { get => playerControllerBase; }

        /// <summary>
        /// 「チーム0のマテリアル」の取得用
        /// </summary>
        public Material Team0Material { get => team0Material; }

        /// <summary>
        /// 「チーム1のマテリアル」の取得用
        /// </summary>
        public Material Team1Material { get => team1Material; }

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