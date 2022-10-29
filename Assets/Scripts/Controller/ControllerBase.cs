using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace CallOfUnity
{
    /// <summary>
    /// PlayerControllerクラスとNPCControllerクラスの親クラス
    /// </summary>
    public class ControllerBase : MonoBehaviour,ISetUp
    {
        protected Vector3 moveDir;//移動方向

        /// <summary>
        /// 初期設定を行う
        /// </summary>
        public void SetUp()
        {
            //移動する
            this.UpdateAsObservable()
                .Where(_=>moveDir != Vector3.zero)
                .Subscribe(_=>transform.Translate(moveDir))
                .AddTo(this);
        }

        /// <summary>
        /// リロードする
        /// </summary>
        protected void Reload()
        {
            //TODO:リロード処理
        }

        /// <summary>
        /// 射撃する
        /// </summary>
        protected void Shot()
        {
            //TODO:射撃処理
        }
    }
}
