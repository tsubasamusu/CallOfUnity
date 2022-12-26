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