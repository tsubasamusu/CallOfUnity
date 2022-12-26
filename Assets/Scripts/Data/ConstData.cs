using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// 固定値を管理
    /// </summary>
    public static class ConstData
    {
        public const float WALK_SPEED = 5f;//歩くスピード

        public const float RUN_SPEED = 10f;//走るスピード

        public const float STANCE_TIME = 0.25f;//構える時間

        public const float NORMAL_FOV = 60f;//基本の視野角

        public const float STANCE_FOV = 30f;//構える時の視野角
        
        public const float MAX_LENGTH_FROM_CENTER = 50f;//オブジェクトが存在できるステージ中央からの最大距離

        public const float WEAPON_ROT_SMOOTH = 0.8f;//武器の回転の滑らかさ

        public const float BGM_VOLUME = 0.5f;//BGMの音量

        public const int WIN_SCORE = 1;//勝利得点

        public const int TEAMMATE_NUMBER = 10;//1チームの人数

        public const KeyCode RUN_KEY = KeyCode.Q;//ダッシュキー

        public const KeyCode RELOAD_KEY = KeyCode.R;//リロードキー

        public const KeyCode CHANGE_WEAPON_KEY = KeyCode.LeftShift;//武器チェンジキー

        public const KeyCode SHOT_KEY = KeyCode.Mouse0;//射撃キー

        public const KeyCode STANCE_KEY = KeyCode.Mouse1;//構えるキー
    }
}