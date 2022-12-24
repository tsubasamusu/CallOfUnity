using CallOfUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///武器のデータを管理する
/// </summary>
[CreateAssetMenu(fileName = "WeaponDataSO", menuName = "Create WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    /// <summary>   
    /// 武器の名前   
    /// </summary>   
    public enum WeaponName
    {
        AssaultRifle,//アサルトライフル
        Pistol,//ピストル
        RocketLauncher,//ロケットランチャー
        Shotgun,//ショットガン
        SniperRifle,//スナイパーライフル
        SubMachineGun//サブマシンガン
    }

    /// <summary>   
    /// 武器のデータを管理する 
    /// </summary>   
    [Serializable]
    public class WeaponData
    {
        public WeaponName name;//名前  
        public GameObject objWeapon;//武器のオブジェクト
        public GameObject impactEffect;//着弾エフェクト
        public Sprite sprWeapon;//武器のスプライト
        public BulletDetailBase bullet;//弾
        public int ammunitionNo;//装弾数
        public float reloadTime;//リロード時間
        public float rateOfFire;//連射速度
        public float firingRange;//射程距離
        public float shotPower;//発射力
        [Range(0f,100f)]
        public float attackPower;//攻撃力
    }

    public List<WeaponData> weaponDataList = new();//武器のデータのリスト  
}
