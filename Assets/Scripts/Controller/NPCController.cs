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
            //�̂̈ʒu�����擾
            Transform bodyTran = transform.GetChild(0);

            //���Z�b�g���̏������Ăяo��
            Reset();

            //�ˌ��ƃ����[�h�̐�����J�n����
            ShotReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

            //�ړ��E�̂̌����̒���
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    //�߂��̓G�̍��W���擾����
                    Vector3 nearEnemyPos = GetNearEnemyPos();

                    //�ڕW�n�_��ݒ肷��
                    SetTargetPos(nearEnemyPos);

                    //�K�؂ȑ̂̌������擾����
                    Vector3 bodyDir =
                    ((nearEnemyPos + new Vector3(0f, 1.5f, 0f)) - (transform.position + new Vector3(0f, 1.5f, 0f))).normalized;

                    //NPC��G�̕����֊��炩�Ɍ�������
                    bodyTran.forward = Vector3.Lerp(bodyTran.forward, bodyDir, Time.deltaTime * ConstData.WEAPON_ROT_SMOOTH);
                })
                .AddTo(this);
        }

        /// <summary>
        /// ���Z�b�g���ɌĂяo�����
        /// </summary>
        private void Reset()
        {
            //NavMeshAgent���擾����
            agent = GetComponent<NavMeshAgent>();

            //�g�p����������_���Ɍ���
            currentWeaponData = GameData.instance.WeaponDataSO
                .weaponDataList[UnityEngine.Random.Range(0, GameData.instance.WeaponDataSO.weaponDataList.Count)];

            //�����̐F��ݒ肷��
            transform.GetChild(1).GetComponent<MeshRenderer>().material = myTeamNo == 0 ?
                GameData.instance.Team0Material : GameData.instance.Team1Material;

            //CharacterHealth�̏����ݒ���s��
            GetComponent<CharacterHealth>().SetUp();
        }

        /// <summary>
        /// �Đݒ肷��
        /// </summary>
        public override void ReSetUp()
        {
            //�c�e�����ő�l�ɐݒ�
            bulletCountForNpc = currentWeaponData.ammunitionNo;
        }

        /// <summary>
        /// �ˌ��ƃ����[�h�𐧌䂷��
        /// </summary>
        /// <param name="token">CancellationToken</param>
        /// <returns>�҂�����</returns>
        private async UniTask ShotReloadAsync(CancellationToken token)
        {
            //�����ɌJ��Ԃ�
            while (true)
            {
                //�c�e�����u0�v�ȉ��Ȃ�
                if (GetBulletcCount() <= 0)
                {
                    //�����[�h����
                    ReloadAsync(this.GetCancellationTokenOnDestroy()).Forget();

                    //�����[�h���I���܂ő҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(GetReloadTime()), cancellationToken: token);
                }

                //�ː���ɓG�����āA�e���c���Ă���A�����[�h���łȂ��Ȃ�
                if (CheckEnemy() && GetBulletcCount() >= 1 && !isReloading)
                {
                    //���e�����Ă�܂ő҂�
                    await UniTask.Delay(TimeSpan.FromSeconds(currentWeaponData.rateOfFire), cancellationToken: token);

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
            var ray = new Ray(weaponTran.position, weaponTran.forward);

            //�����𔭎˂��A���������ɂ��G��Ȃ�������false��Ԃ�
            if (!Physics.Raycast(ray, out RaycastHit hit, currentWeaponData.firingRange)) return false;

            //�����̐ڐG����̃`�[���ԍ����قȂ�Ȃ�true��Ԃ�
            return (hit.transform.TryGetComponent(out ControllerBase controller) && myTeamNo != controller.myTeamNo);
        }

        /// <summary>
        /// �ł��߂��ɂ���G�̈ʒu���擾����
        /// </summary>
        /// <returns>�ł��߂��ɂ���G�̈ʒu</returns>
        private Vector3 GetNearEnemyPos()
        {
            //�u�ł��߂��ɂ���G�v�����o�^����
            ControllerBase nearEnemy = myTeamNo == 0 ?
                GameData.instance.npcControllerBaseList[ConstData.TEAMMATE_NUMBER - 1]
                : GameData.instance.PlayerControllerBase;

            //�u�ł��߂��ɂ���G�Ƃ̋����v�����o�^����
            float nearLength =
                ((myTeamNo == 0 ?
                GameData.instance.npcControllerBaseList[ConstData.TEAMMATE_NUMBER - 1]
                : GameData.instance.PlayerControllerBase).transform.position - transform.position).magnitude;

            //NPC�̐������J��Ԃ�
            for (int i = 0; i < GameData.instance.npcControllerBaseList.Count; i++)
            {
                //�J��Ԃ������Ŏ擾����NPC�������Ȃ�A���̌J��Ԃ������Ɉڂ�
                if (GameData.instance.npcControllerBaseList[i].myTeamNo == myTeamNo) continue;

                //�J��Ԃ������Ŏ擾����NPC�Ƃ̋������擾����
                float length = (GameData.instance.npcControllerBaseList[i].transform.position - transform.position).magnitude;

                //�L�^���X�V������
                if (length < nearLength) 
                {
                    //�u�ł��߂��ɂ���G�v���X�V����
                    nearEnemy = GameData.instance.npcControllerBaseList[i];

                    //�u�ł��߂��ɂ���G�Ƃ̋����v���X�V����
                    nearLength = length; 
                }
            }

            //�u�ł��߂��ɂ���G�̈ʒu�v��Ԃ�
            return nearEnemy.transform.position;
        }
    }
}
