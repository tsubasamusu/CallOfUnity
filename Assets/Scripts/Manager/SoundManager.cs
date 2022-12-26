using CallOfUnity;
using DG.Tweening;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary> 
    /// ���̏������s��
    /// </summary> 
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;//�C���X�^���X 

        private AudioSource[] audioSources;//���Đ��p��AudioSource�̔z�� 

        /// <summary> 
        /// Start���\�b�h���O�ɌĂяo����� 
        /// </summary> 
        void Awake()
        {
            //���Đ��p��AudioSource�̔z��̗v�f����ݒ� 
            audioSources = new AudioSource[GameData.instance.SoundDataSO.soundDataList.Count];

            //���Đ��p��AudioSource�̔z��̗v�f�������J��Ԃ� 
            for (int i = 0; i < audioSources.Length; i++)
            {
                //AudioSorce�R���|�[�l���g���쐬���A���g�ɃA�^�b�`������ɁA�z��Ɋi�[���� 
                audioSources[i] = gameObject.AddComponent<AudioSource>();
            }

            //�ȉ��A�V���O���g���ɕK�{�̋L�q 
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
        /// �����Đ����� 
        /// </summary> 
        /// <param name="name">���̖��O</param> 
        /// <param name="volume">���̃{�����[��</param> 
        /// <param name="loop">�J��Ԃ����ǂ���</param> 
        public void PlaySound(SoundDataSO.SoundName name, float volume = 1f, bool loop = false)
        {
            //���Đ��p��AudioSource�̔z��̗v�f��1�����o�� 
            foreach (AudioSource source in audioSources)
            {
                //���o����AudioSource���Đ����ł͂Ȃ��i�g�p����Ă��Ȃ��j�Ȃ� 
                if (source.isPlaying == false)
                {
                    //���̃N���b�v���擾���āA�o�^���� 
                    source.clip = GameData.instance.SoundDataSO.soundDataList.Find(x => x.name == name).clip;

                    //���̃{�����[����ݒ� 
                    source.volume = volume;

                    //�J��Ԃ�
                    if (loop) source.loop = true;

                    //�����Đ����� 
                    source.Play();

                    //�J��Ԃ��������甲���o��
                    break;
                }
            }
        }

        /// <summary> 
        /// �S�Ẳ����~���� 
        /// </summary> 
        public void StopSound()
        {
            //���Đ��p��AudioSource�̔z��̗v�f��1�����o�� 
            foreach (AudioSource source in audioSources)
            {
                //�����t�F�[�h�A�E�g������ 
                source.DOFade(0f, ConstData.FADE_SOUND_OUT_TIME)
                    .OnComplete(() =>
                    {
                        //�������S�Ɏ~�߂� 
                        source.Stop();

                        //���o����AudioSource�̃N���b�v����ɂ��� 
                        source.clip = null;
                    });
            }
        }
    }
}