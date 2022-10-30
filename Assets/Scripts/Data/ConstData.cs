using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 固定値を管理
    /// </summary>
    public static class ConstData
    {
        public const float WALK_SPEED = 10f;//歩くスピード

        public const float RUN_SPEED = 30f;//走るスピード

        public const float GRAVITY = 5f;//重力

        public const KeyCode RUN_KEY = KeyCode.Q;//ダッシュキー

        public const KeyCode RELOAD_KEY = KeyCode.R;//リロードキー

        public const KeyCode STOOP_KEY = KeyCode.E;//かがむキー

        public const KeyCode CHANGE_WEAPON_KEY = KeyCode.LeftShift;//武器チェンジキー

        public const KeyCode SHOT_KEY = KeyCode.Mouse0;//射撃キー
    }
}