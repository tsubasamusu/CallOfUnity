using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace CallOfUnity
{
    /// <summary>
    /// 変動値を管理
    /// </summary>
    public class GameData : MonoBehaviour, ISetUp
    {
        [Range(0f, 10f)]
        public float lookSensitivity;//視点感度

        [Range(0f, 1f)]
        public float lookSmooth;//視点の滑らかさ

        [HideInInspector]
        public int playerTotalKillCount;//プレイヤーの総キル数

        [HideInInspector]
        public int playerTotalDeathCount;//プレイヤーの総デス数

        [HideInInspector]
        public int playerTotalAttackCount;//プレイヤーの総命中数

        [HideInInspector]
        public int playerTotalShotCount;//プレイヤーの総発射数

        [SerializeField]
        private List<Transform> respawnTransList = new();

        [SerializeField]
        private GameObject objMuzzleFlashEffect;//発射口のエフェクト

        [SerializeField]
        private GameObject objBleedingEffect;//出血のエフェクト

        [SerializeField]
        private GameObject objRocketLauncherEffect;//ロケットランチャーのエフェクト

        [SerializeField]
        private GameObject objExplosionEffect;//爆発のエフェクト

        [SerializeField]
        private Transform temporaryObjectContainerTran;//ゲームオブジェクトの一時的な親

        [SerializeField]
        private ControllerBase npcControllerBase;//NPCのプレファブ

        [SerializeField]
        private ControllerBase playerControllerBase;//プレイヤーのControllerBase

        [SerializeField]
        private CharacterHealth playerCharacterHealth;//プレイヤーのCharacterHealth

        [SerializeField]
        private UIManager uiManager;//UIManager

        [SerializeField]
        private Material team0Material;//チーム0のマテリアル

        [SerializeField]
        private Material team1Material;//チーム1のマテリアル

        [HideInInspector]
        public ReactiveProperty<(int team0, int team1)> Score = new((0, 0));//得点

        [SerializeField]
        private WeaponDataSO weaponDataSO;//WeaponDataSO

        [SerializeField]
        private SoundDataSO soundDataSO;//SoundDataSO

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
        /// 「発射口のエフェクト」の取得用
        /// </summary>
        public GameObject ObjMuzzleFlashEffect { get => objMuzzleFlashEffect; }

        /// <summary>
        /// 「出血のエフェクト」の取得用
        /// </summary>
        public GameObject ObjBleedingEffect { get => objBleedingEffect; }

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
        /// 「プレイヤーのControllerBase」の取得用
        /// </summary>
        public ControllerBase PlayerControllerBase { get => playerControllerBase; }

        /// <summary>
        /// 「プレイヤーのCharacterHealth」の取得用
        /// </summary>
        public CharacterHealth PlayerCharacterHealth { get => playerCharacterHealth; }

        /// <summary>
        /// 「UIManager」の取得用
        /// </summary>
        public UIManager UiManager { get => uiManager; }

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

        /// <summary>
        /// 「SoundDataSO」の取得用
        /// </summary>
        public SoundDataSO SoundDataSO { get => soundDataSO; }

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

        /// <summary>
        /// GameDataの初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //リセットする
            Reset();
        }

        /// <summary>
        /// リセットする
        /// </summary>
        private void Reset()
        {
            //データをロードする
            if (PlayerPrefs.HasKey("Kill")) playerTotalKillCount = PlayerPrefs.GetInt("Kill");
            if (PlayerPrefs.HasKey("Death")) playerTotalDeathCount = PlayerPrefs.GetInt("Death");
            if (PlayerPrefs.HasKey("Attack")) playerTotalAttackCount = PlayerPrefs.GetInt("Attack");
            if (PlayerPrefs.HasKey("Shot")) playerTotalShotCount = PlayerPrefs.GetInt("Shot");
            if (PlayerPrefs.HasKey("LookSensitivity")) lookSensitivity = PlayerPrefs.GetFloat("LookSensitivity");
            if (PlayerPrefs.HasKey("LookSmooth")) lookSmooth = PlayerPrefs.GetFloat("LookSmooth");
        }

        /// <summary>
        /// データを保存する
        /// </summary>
        public void SaveData()
        {
            PlayerPrefs.SetInt("Kill", playerTotalKillCount);
            PlayerPrefs.SetInt("Death", playerTotalDeathCount);
            PlayerPrefs.SetInt("Attack", playerTotalAttackCount);
            PlayerPrefs.SetInt("Shot", playerTotalShotCount);
            PlayerPrefs.SetFloat("LookSensitivity", lookSensitivity);
            PlayerPrefs.SetFloat("LookSmooth", lookSmooth);
        }
    }
}