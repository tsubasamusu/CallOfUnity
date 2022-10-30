using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// �Œ�l���Ǘ�
    /// </summary>
    public static class ConstData
    {
        public const float WALK_SPEED = 10f;//�����X�s�[�h

        public const float RUN_SPEED = 30f;//����X�s�[�h

        public const float GRAVITY = 5f;//�d��

        public const float STANCE_TIME = 0.25f;//�\���鎞��

        public const float NORMAL_FOV = 60f;//��{�̎���p

        public const float STANCE_FOV = 30f;//�\���鎞�̎���p

        public const KeyCode RUN_KEY = KeyCode.Q;//�_�b�V���L�[

        public const KeyCode RELOAD_KEY = KeyCode.R;//�����[�h�L�[

        public const KeyCode STOOP_KEY = KeyCode.E;//�����ރL�[

        public const KeyCode CHANGE_WEAPON_KEY = KeyCode.LeftShift;//����`�F���W�L�[

        public const KeyCode SHOT_KEY = KeyCode.Mouse0;//�ˌ��L�[

        public const KeyCode STANCE_KEY = KeyCode.Mouse1;//�\����L�[
    }
}