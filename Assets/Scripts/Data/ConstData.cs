using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 固定値を管理
    /// </summary>
    public static class ConstData
    {
        public const float walkSpeed = 10f;//歩くスピード

        public const float runSpeed = 30f;//走るスピード

        public const float gravity = 5f;//重力

        public const KeyCode runKey = KeyCode.Q;//ダッシュキー

        public const KeyCode reloadKey = KeyCode.R;//リロードキー
    }
}