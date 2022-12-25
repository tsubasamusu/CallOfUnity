using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace CallOfUnity
{
    /// <summary>
    /// 武器のボタンの処理を行う
    /// </summary>
    public class WeaponButtonDetail : MonoBehaviour
    {
        private WeaponDataSO.WeaponData weaponData;//武器のデータ

        /// <summary>
        /// 「自分の武器のデータ」の取得用
        /// </summary>
        public WeaponDataSO.WeaponData WeaponData { get => weaponData; }

        /// <summary>
        /// 武器のボタンの初期設定を行う
        /// </summary>
        /// <param name="weaponData">武器のデータ</param>
        /// <param name="uIManager">UIManager</param>
        public void SetUpWeaponButton(WeaponDataSO.WeaponData weaponData,UIManager uIManager)
        {
            //武器のデータを取得する
            this.weaponData = weaponData;

            //ボタンを取得する
            Button btnWeapon = GetComponent<Button>();

            //ボタンのテキストを設定する
            transform.GetChild(0).GetComponent<Text>().text = weaponData.name.ToString();

            //ボタンを押された際の処理
            btnWeapon.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //ボタンを非活性化する
                    GetComponent<Button>().interactable = false;

                    //プレイヤーの所持武器「0」が空なら
                    if (GameData.instance.playerWeaponInfo.info0.data == null)
                    {
                        //プレイヤーの所持武器「0」を設定する
                        GameData.instance.playerWeaponInfo.info0.data = weaponData;
                    }
                    //プレイヤーの所持武器「1」が空なら
                    else if (GameData.instance.playerWeaponInfo.info1.data == null)
                    {
                        //プレイヤーの所持「1」を設定する
                        GameData.instance.playerWeaponInfo.info1.data = weaponData;
                    }

                    //プレイヤーの武器が2つとも選択されたら
                    if(GameData.instance.playerWeaponInfo.info0.data!=null&& GameData.instance.playerWeaponInfo.info1.data != null)
                    {
                        //武器の選択を終える
                        uIManager.EndChooseWeapon();
                    }
                })
                .AddTo(this);
        }
    }
}
