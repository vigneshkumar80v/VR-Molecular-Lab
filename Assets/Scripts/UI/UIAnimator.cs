using UnityEngine;
using DG.Tweening;

namespace MolecularLab
{
    public class UIAnimator : MonoBehaviour
    {
        [Header("Panel Animation")]
        public float showDuration = 0.4f;
        public float hideDuration = 0.3f;
        public Ease  showEase     = Ease.OutBack;
        public Ease  hideEase     = Ease.InBack;

        [Header("Molecule Pop Animation")]
        public float popDuration = 0.35f;
        public float popScale    = 1.3f;

        [Header("Discovery Flash")]
        public CanvasGroup flashGroup;
        public float flashDuration = 0.2f;

        [Header("Counter Bounce")]
        public RectTransform counterRect;
        public float bounceDuration = 0.25f;

        private Sequence _currentSequence;
        private bool     _isVisible   = false;
        private Vector3  _originalScale;

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Awake()
        {
            // Store the REAL scale (e.g. 0.001, 0.001, 0.001) on startup
            _originalScale = transform.localScale;
        }

        // ── Panel Show / Hide ────────────────────────────────────────────

        public void SlideIn()
        {
            if (_isVisible) return;
            _isVisible = true;

            _currentSequence?.Kill();
            gameObject.SetActive(true);

            // Start from zero, animate back to original scale (0.001)
            transform.localScale = Vector3.zero;

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(transform.DOScale(_originalScale, showDuration).SetEase(showEase))
                .SetUpdate(true);
        }

        public void SlideOut()
        {
            if (!_isVisible) return;
            _isVisible = false;

            _currentSequence?.Kill();

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(transform.DOScale(Vector3.zero, hideDuration).SetEase(hideEase))
                .OnComplete(() => gameObject.SetActive(false))
                .SetUpdate(true);
        }

        public void TogglePanel()
        {
            if (_isVisible) SlideOut();
            else            SlideIn();
        }

        // ── Molecule Pop ─────────────────────────────────────────────────

        public void PlayMoleculePop()
        {
            // Store original scale of the molecule prefab
            var originalMoleculeScale = transform.localScale;
            transform.localScale      = Vector3.zero;

            DOTween.Sequence()
                .Append(transform.DOScale(originalMoleculeScale * popScale, popDuration * 0.6f).SetEase(Ease.OutBack))
                .Append(transform.DOScale(originalMoleculeScale,            popDuration * 0.4f).SetEase(Ease.InOutSine));
        }

        public void PlayBreakApart(System.Action onComplete = null)
        {
            var originalMoleculeScale = transform.localScale;

            DOTween.Sequence()
                .Append(transform.DOScale(originalMoleculeScale * 1.2f, 0.1f).SetEase(Ease.OutSine))
                .Append(transform.DOScale(Vector3.zero,                  0.2f).SetEase(Ease.InBack))
                .OnComplete(() => onComplete?.Invoke());
        }

        // ── Library Entry ────────────────────────────────────────────────

        public void AnimateEntryIn(RectTransform entry)
        {
            if (entry == null) return;

            var cg = entry.GetComponent<CanvasGroup>();
            if (cg == null) cg = entry.gameObject.AddComponent<CanvasGroup>();

            cg.alpha         = 0f;
            entry.localScale = new Vector3(0.8f, 0.8f, 1f);

            DOTween.Sequence()
                .Append(entry.DOScaleX(1f, 0.3f).SetEase(Ease.OutCubic))
                .Join(entry.DOScaleY(1f,   0.3f).SetEase(Ease.OutCubic))
                .Join(cg.DOFade(1f,        0.25f))
                .SetUpdate(true);
        }

        // ── Discovery Flash ──────────────────────────────────────────────

        public void PlayDiscoveryFlash()
        {
            if (flashGroup == null) return;

            flashGroup.alpha = 0f;
            flashGroup.gameObject.SetActive(true);

            DOTween.Sequence()
                .Append(flashGroup.DOFade(0.5f, flashDuration * 0.3f))
                .Append(flashGroup.DOFade(0f,   flashDuration * 0.7f))
                .OnComplete(() => flashGroup.gameObject.SetActive(false));
        }

        // ── Counter Bounce ───────────────────────────────────────────────

        public void BounceCounter()
        {
            if (counterRect == null) return;

            counterRect.DOKill();
            counterRect.localScale = Vector3.one;

            DOTween.Sequence()
                .Append(counterRect.DOScale(1.3f, bounceDuration * 0.4f).SetEase(Ease.OutSine))
                .Append(counterRect.DOScale(1.0f, bounceDuration * 0.6f).SetEase(Ease.InOutBounce));
        }

        // ── Inspector Fade ───────────────────────────────────────────────

        public void FadeInInspector(CanvasGroup cg)
        {
            if (cg == null) return;
            cg.gameObject.SetActive(true);
            cg.alpha = 0f;
            cg.DOFade(1f, 0.2f).SetUpdate(true);
        }

        public void FadeOutInspector(CanvasGroup cg)
        {
            if (cg == null) return;
            cg.DOFade(0f, 0.15f)
              .SetUpdate(true)
              .OnComplete(() => cg.gameObject.SetActive(false));
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
            transform.DOKill();
        }
    }
}