using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Triggers;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace CallOfUnity
{
    /// <summary>
    /// NPC�̓����𐧌䂷��
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCController : ControllerBase
    {
        private NavMeshAgent agent;//NavMeshAgent

        /// <summary>
        /// NPCController�̏����ݒ���s��
        /// </summary>
        protected override void SetUpController()
        {
            //���Z�b�g���̏������Ăяo��
            Reset();

            //�ˌ��ƃ����[�h�̐�����J�n
            ShotReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

            //�ړ�
            this.UpdateAsObservable()
                .Subscribe(_ => SetTargetPos(GetNearEnemyPos()))
                .AddTo(this);
        }

        /// <summary>
        /// ���Z�b�g���ɌĂяo�����
        /// </summary>
        private void Reset()
        {
            //NavMeshAgent���擾
            agent = GetComponent<NavMeshAgent>();

            //��~������ݒ�
            agent.stoppingDistance = ConstData.STOPPING_DISTANCE;

            //�i�f�o�b�N�p�j
            myTeamNo = 1;
        }

        /// <summary>
        /// �ˌ��ƃ����[�h�𐧌䂷��
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>�҂�����</returns>
        private async UniTask ShotReloadAsync(CancellationToken token)
        {
            //�����ɌJ��Ԃ�
            while(true)
            {
                //�c�e����0�Ȃ�
                if(GetAmmunitionRemaining()==0)
                {
                    //�����[�h����
                    Reload();

                    //�����[�h���I���܂ő҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(GetReloadTime()), cancellationToken: token);
                }

                //�ː���ɓG�����āA�e���c���Ă���Ȃ�
                if (CheckEnemy() && GetAmmunitionRemaining() > 0)
                {
                    //���e�����Ă�܂ő҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(GetRateOfFire()), cancellationToken: token);

                    //�ˌ�����
                    Shot();
                }

                //1�t���[���҂�
                await UniTask.Yield(token);
            }
        }

        /// <summary>
        /// �ڕW�n�_��ݒ肷��
        /// </summary>
        /// <param name="targetPos">�ڕW�n�_</param>
        private void SetTargetPos(Vector3 targetPos)
        {
            //�ڕW�n�_��ݒ�
            agent.destination = targetPos;
        }

        /// <summary>
        /// �G���ː���ɂ��邩���ׂ�
        /// </summary>
        /// <returns>�G���ΐ���ɋ�����true</returns>
        private bool CheckEnemy()
        {
            //�������쐬  
            var ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);

            //�����𔭎˂��A���������ɂ��G��Ȃ�������false��Ԃ�
            if (!Physics.Raycast(ray, out RaycastHit hit, GetRange())) return false;

            //�����̐ڐG����̃`�[���ԍ����قȂ�Ȃ�true��Ԃ�
            return (hit.transform.TryGetComponent(out ControllerBase controller) && myTeamNo != controller.myTeamNo);
        }

        /// <summary>
        /// ���ݎg�p���Ă��镐��̎˒��������擾����
        /// </summary>
        /// <returns>���ݎg�p���Ă��镐��̎˒�����</returns>
        private float GetRange()
        {
            //TODO:�˒������̎擾����
            return 100f;//�i���j
        }

        /// <summary>
        /// �ł��߂��ɂ���G�̈ʒu���擾����
        /// </summary>
        /// <returns>�ł��߂��ɂ���G�̈ʒu</returns>
        private Vector3 GetNearEnemyPos()
        {
            //TODO:�߂��̓G��T������
            return Vector3.zero;//�i���j
        }
    }
}
