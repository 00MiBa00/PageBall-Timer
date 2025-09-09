using UnityEngine;
using DG.Tweening;

namespace Views.Init
{
    public class BallIdleAnimation : MonoBehaviour
    {
        [SerializeField] private float moveY = 0.3f;
        [SerializeField] private float moveX = 0.2f;
        [SerializeField] private float scaleFactor = 1.05f;
        [SerializeField] private float duration = 1.5f;

        private Tween moveYTween;
        private Tween moveXTween;
        private Tween scaleTween;

        void OnEnable()
        {
            moveYTween = transform.DOMoveY(transform.position.y + moveY, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            moveXTween = transform.DOMoveX(transform.position.x + moveX, duration * 2f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);

            scaleTween = transform.DOScale(Vector3.one * scaleFactor, duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }

        void OnDisable()
        {
            moveYTween?.Kill();
            moveXTween?.Kill();
            scaleTween?.Kill();
        }
    }
}