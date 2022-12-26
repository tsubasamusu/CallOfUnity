using System.Collections.Generic;
using System;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// ���̃f�[�^���Ǘ�����
    /// </summary>
    [CreateAssetMenu(fileName = "SoundDataSO", menuName = "Create SoundDataSO")]
    public class SoundDataSO : ScriptableObject
    {
        /// <summary>   
        /// ���̖��O   
        /// </summary>   
        public enum SoundName
        {
            �Q�[���X�^�[�g�{�^�������������̉�,
            ���ʂ̃{�^�������������̉�,
            �����ȃ{�^�������������̉�,
            �Q�[���N���A�̉�,
            �Q�[���I�[�o�[�̉�,
            ��e�������̉�,
            �����������̉�,
            �����[�h���鎞�̉�,
            ��������BGM
        }

        /// <summary>   
        /// ���̃f�[�^���Ǘ����� 
        /// </summary>   
        [Serializable]
        public class SoundData
        {
            public SoundName name;//���O  
            public AudioClip clip;//�N���b�v 
        }

        public List<SoundData> soundDataList = new();//���̃f�[�^�̃��X�g  
    }
}