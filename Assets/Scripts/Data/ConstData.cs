using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// �Œ�l���Ǘ�
    /// </summary>
    public static class ConstData
    {
        public const float WALK_SPEED = 5f;//�����X�s�[�h

        public const float RUN_SPEED = 10f;//����X�s�[�h

        public const float STANCE_TIME = 0.25f;//�\���鎞��

        public const float NORMAL_FOV = 60f;//��{�̎���p

        public const float STANCE_FOV = 30f;//�\���鎞�̎���p
        
        public const float MAX_LENGTH_FROM_CENTER = 50f;//�I�u�W�F�N�g�����݂ł���X�e�[�W��������̍ő勗��

        public const float WEAPON_ROT_SMOOTH = 0.8f;//����̉�]�̊��炩��

        public const float BGM_VOLUME = 0.5f;//BGM�̉���

        public const int WIN_SCORE = 1;//�������_

        public const int TEAMMATE_NUMBER = 10;//1�`�[���̐l��

        public const KeyCode RUN_KEY = KeyCode.Q;//�_�b�V���L�[

        public const KeyCode RELOAD_KEY = KeyCode.R;//�����[�h�L�[

        public const KeyCode CHANGE_WEAPON_KEY = KeyCode.LeftShift;//����`�F���W�L�[

        public const KeyCode SHOT_KEY = KeyCode.Mouse0;//�ˌ��L�[

        public const KeyCode STANCE_KEY = KeyCode.Mouse1;//�\����L�[
    }
}