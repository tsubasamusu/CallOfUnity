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

        public const float STANCE_TIME = 0.25f;//構える時間

        public const float NORMAL_FOV = 60f;//基本の視野角

        public const float STANCE_FOV = 30f;//構える時の視野角

        public const float JUMP_POWER = 5f;//ジャンプ力

        public const float WAIT_JUMP_TIME = 0.4f;//ジャンプ後、物理演算終了の判断を行うまでの時間

        public const float STOPPING_DISTANCE = 5f;//停止距離

        public const int FIRST_ALL_BULLET_COUNT = 1000;//初期総残弾数

        public const int WIN_SCORE = 50;//勝利得点

        public const KeyCode RUN_KEY = KeyCode.Q;//ダッシュキー

        public const KeyCode RELOAD_KEY = KeyCode.R;//リロードキー

        public const KeyCode CHANGE_WEAPON_KEY = KeyCode.LeftShift;//武器チェンジキー

        public const KeyCode SHOT_KEY = KeyCode.Mouse0;//射撃キー

        public const KeyCode STANCE_KEY = KeyCode.Mouse1;//構えるキー

        public const KeyCode JUMP_KEY = KeyCode.Space;//ジャンプキー
    }
}