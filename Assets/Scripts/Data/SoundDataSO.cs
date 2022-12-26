using System.Collections.Generic;
using System;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 音のデータを管理する
    /// </summary>
    [CreateAssetMenu(fileName = "SoundDataSO", menuName = "Create SoundDataSO")]
    public class SoundDataSO : ScriptableObject
    {
        /// <summary>   
        /// 音の名前   
        /// </summary>   
        public enum SoundName
        {

        }

        /// <summary>   
        /// 音のデータを管理する 
        /// </summary>   
        [Serializable]
        public class SoundData
        {
            public SoundName name;//名前  
            public AudioClip clip;//クリップ 
        }

        public List<SoundData> soundDataList = new();//音のデータのリスト  
    }
}