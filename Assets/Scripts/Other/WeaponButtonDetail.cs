using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace CallOfUnity
{
    /// <summary>
    /// ����̃{�^���̏������s��
    /// </summary>
    public class WeaponButtonDetail : MonoBehaviour
    {
        private WeaponDataSO.WeaponData weaponData;//����̃f�[�^

        /// <summary>
        /// �u�����̕���̃f�[�^�v�̎擾�p
        /// </summary>
        public WeaponDataSO.WeaponData WeaponData { get => weaponData; }

        /// <summary>
        /// ����̃{�^���̏����ݒ���s��
        /// </summary>
        /// <param name="weaponData">����̃f�[�^</param>
        /// <param name="uIManager">UIManager</param>
        public void SetUpWeaponButton(WeaponDataSO.WeaponData weaponData,UIManager uIManager)
        {
            //����̃f�[�^���擾����
            this.weaponData = weaponData;

            //�{�^�����擾����
            Button btnWeapon = GetComponent<Button>();

            //�{�^���̃e�L�X�g��ݒ肷��
            transform.GetChild(0).GetComponent<Text>().text = weaponData.name.ToString();

            //�{�^���������ꂽ�ۂ̏���
            btnWeapon.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    //�{�^����񊈐�������
                    GetComponent<Button>().interactable = false;

                    //�v���C���[�̏�������u0�v����Ȃ�
                    if (GameData.instance.playerWeaponInfo.info0.data == null)
                    {
                        //�v���C���[�̏�������u0�v��ݒ肷��
                        GameData.instance.playerWeaponInfo.info0.data = weaponData;
                    }
                    //�v���C���[�̏�������u1�v����Ȃ�
                    else if (GameData.instance.playerWeaponInfo.info1.data == null)
                    {
                        //�v���C���[�̏����u1�v��ݒ肷��
                        GameData.instance.playerWeaponInfo.info1.data = weaponData;
                    }

                    //�v���C���[�̕��킪2�Ƃ��I�����ꂽ��
                    if(GameData.instance.playerWeaponInfo.info0.data!=null&& GameData.instance.playerWeaponInfo.info1.data != null)
                    {
                        //����̑I�����I����
                        uIManager.EndChooseWeapon();
                    }
                })
                .AddTo(this);
        }
    }
}
