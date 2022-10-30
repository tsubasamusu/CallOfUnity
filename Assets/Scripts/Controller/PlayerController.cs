using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace CallOfUnity
{
    /// <summary>
    /// �v���C���[�̓����𐧌䂷��
    /// </summary>
    public class PlayerController : ControllerBase
    {
        /// <summary>
        /// PlayerController�̏����ݒ���s��
        /// </summary>
        protected override void SetUpController()
        {
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�ړ�������ݒ�
                    moveDir
                    //���������̓��͂��擾
                    = Vector3.Scale((Camera.main.transform.right * Input.GetAxis("Horizontal")
                        //���������̓��͂��擾
                        + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                        //y������0�ɂ���
                        , new Vector3(1f, 0f, 1f))
                        //�ړ��X�s�[�h���擾
                        * (Input.GetKey(ConstData.runKey) ? ConstData.runSpeed : ConstData.walkSpeed);

                    //�����ރL�[�������ꂽ��
                    if (Input.GetKeyDown(ConstData.stoopKey))
                    {
                        //������
                        Stoop();
                    }
                    //�����ރL�[�������ꂽ��
                    else if (Input.GetKeyUp(ConstData.stoopKey))
                    {
                        //�����ނ̂���߂�
                        StopStoop();
                    }
                })
                .AddTo(this);

            //�����[�h
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.reloadKey))
                .ThrottleFirst(System.TimeSpan.FromSeconds(3.0f))//TODO:�����[�h���Ԏ擾����
                .Subscribe(_ => Reload())
                .AddTo(this);
        }

        /// <summary>
        /// ������
        /// </summary>
        private void Stoop()
        {
            //TODO:�����ޏ���
            Debug.Log("������");
        }

        /// <summary>
        /// �����ނ̂���߂�
        /// </summary>
        private void StopStoop()
        {
            //TODO:�����ނ̂���߂鏈��
            Debug.Log("�����ނ̂���߂�");
        }
    }
}
