

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

namespace MolecularLab
{
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class MoleculeObject : MonoBehaviour
    {
        [Header("Inspector UI (World Space)")]
        public GameObject      inspectorPanel;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI formulaText;
        public TextMeshProUGUI bondTypeText;
        public TextMeshProUGUI descriptionText;
        public CanvasGroup     inspectorCanvasGroup;  // For fade animation

        [Header("Animation")]
        public UIAnimator uiAnimator;                 // Attach UIAnimator to this GO

        [Header("Reset")]
        public float shakeThreshold = 2.0f;

        // Internal
        private MoleculeData         _data;
        private List<AtomController> _atoms;
        private BondManager          _bondManager;
        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable   _grab;
        private Rigidbody            _rb;
        private Vector3              _lastVelocity;
        private bool                 _canBreak = false;

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Awake()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _rb   = GetComponent<Rigidbody>();

            if (inspectorPanel != null)
                inspectorPanel.SetActive(false);
        }

        private void Start()
{
    uiAnimator?.PlayMoleculePop();
    Invoke(nameof(EnableBreak), 1.5f);
}

private void EnableBreak() => _canBreak = true;
        private void OnEnable()
        {
            _grab.selectEntered.AddListener(OnGrabbed);
            _grab.selectExited.AddListener(OnReleased);
            _grab.hoverEntered.AddListener(OnHoverEnter);
            _grab.hoverExited.AddListener(OnHoverExit);
        }

        private void OnDisable()
        {
            _grab.selectEntered.RemoveListener(OnGrabbed);
            _grab.selectExited.RemoveListener(OnReleased);
            _grab.hoverEntered.RemoveListener(OnHoverEnter);
            _grab.hoverExited.RemoveListener(OnHoverExit);
        }

       private void FixedUpdate()
{
    if (!_canBreak || _rb == null) return;
    float acceleration = (_rb.linearVelocity - _lastVelocity).magnitude / Time.fixedDeltaTime;
    if (acceleration > shakeThreshold)
        BreakApart();
    _lastVelocity = _rb.linearVelocity;
}

        // ── Public API ───────────────────────────────────────────────────

        public void Initialize(MoleculeData data, List<AtomController> atoms, BondManager manager)
        {
            _data        = data;
            _atoms       = atoms;
            _bondManager = manager;
            UpdateInspectorUI();
        }

        public void BreakApart()
        {
            if (uiAnimator != null)
            {
                // Animate break then destroy
                uiAnimator.PlayBreakApart(() =>
                {
                    _bondManager?.ReturnAtoms(_atoms, transform.position);
                    Destroy(gameObject);
                });
            }
            else
            {
                _bondManager?.ReturnAtoms(_atoms, transform.position);
                Destroy(gameObject);
            }
        }

        private string FormatFormula(string formula)
{
    string result = "";
    foreach (char c in formula)
    {
        if (char.IsDigit(c))
            result += $"<size=60%><voffset=-0.2em>{c}</voffset></size>";
        else
            result += c;
    }
    return result;
}

        private void UpdateInspectorUI()
        {
            if (_data == null) return;
            if (nameText        != null) nameText.text        = _data.moleculeName;
            if (formulaText != null) formulaText.text = FormatFormula(_data.formula);
            if (bondTypeText    != null) bondTypeText.text    = _data.bondType.ToString() + " Covalent";
            if (descriptionText != null) descriptionText.text = _data.description;
        }

        private void OnHoverEnter(HoverEnterEventArgs args)
        {
            if (uiAnimator != null && inspectorCanvasGroup != null)
                uiAnimator.FadeInInspector(inspectorCanvasGroup);
            else if (inspectorPanel != null)
                inspectorPanel.SetActive(true);
        }

        private void OnHoverExit(HoverExitEventArgs args)
        {
            if (uiAnimator != null && inspectorCanvasGroup != null)
                uiAnimator.FadeOutInspector(inspectorCanvasGroup);
            else if (inspectorPanel != null)
                inspectorPanel.SetActive(false);
        }

        private void OnGrabbed(SelectEnterEventArgs args) { }
        private void OnReleased(SelectExitEventArgs args) { }
    }
}
