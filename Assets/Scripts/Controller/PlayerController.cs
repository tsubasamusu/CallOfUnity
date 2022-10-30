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
            //�ړ������ƃX�s�[�h���擾
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    moveDir = Vector3.Scale(
                        (Camera.main.transform.right * Input.GetAxis("Horizontal")
                    + Camera.main.transform.forward * Input.GetAxis("Vertical"))
                    , new Vector3(1f, 0f, 1f))
                    * (Input.GetKey(ConstData.runKey) ? ConstData.runSpeed : ConstData.walkSpeed);
                })
                .AddTo(this);

            //�����[�h
            this.UpdateAsObservable()
                .Where(_ => Input.GetKeyDown(ConstData.reloadKey))
                .ThrottleFirst(System.TimeSpan.FromSeconds(3.0f))//TODO:�����[�h���Ԏ擾����
                .Subscribe(_ => Reload())
                .AddTo(this);
        }
    }
}
