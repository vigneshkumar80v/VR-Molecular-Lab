// AtomController.cs (Fixed — no false proximity color on grab from tray)
// Place in: Assets/Scripts/Atoms/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MolecularLab
{
    [RequireComponent(typeof(XRGrabInteractable))]
    [RequireComponent(typeof(Rigidbody))]
    public class AtomController : MonoBehaviour
    {
        [Header("Atom Identity")]
        public AtomType atomType;

        [Header("Proximity Detection")]
        public float bondDetectionRadius = 0.12f;

        [Tooltip("How far the atom must travel from its grab point before proximity checks start.")]
        public float minTravelDistance = 0.08f;

        [Header("Visual Feedback")]
        public Renderer atomRenderer;
        public Color normalColor    = Color.white;
        public Color grabbedColor   = new Color(1f, 0.9f, 0.4f);   // soft yellow
        public Color proximityColor = new Color(0.4f, 1f, 1f);     // cyan

        // Internal
        private XRGrabInteractable            _grabInteractable;
        private bool                          _isGrabbed    = false;
        private bool                          _hasMoved     = false;  // true once moved minTravelDistance
        private Vector3                       _grabPosition;          // position when grabbed
        private readonly List<AtomController> _nearbyAtoms  = new List<AtomController>();

        // Static registry of all active atoms
        private static readonly List<AtomController> _allAtoms = new List<AtomController>();

        [HideInInspector] public BondManager bondManager;

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Awake()
        {
            _grabInteractable = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            _allAtoms.Add(this);
            _grabInteractable.selectEntered.AddListener(OnGrabbed);
            _grabInteractable.selectExited.AddListener(OnReleased);
        }

        private void OnDisable()
        {
            _allAtoms.Remove(this);
            _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            _grabInteractable.selectExited.RemoveListener(OnReleased);
        }

        private void Update()
        {
            if (!_isGrabbed) return;

            // Wait until atom has moved away from its grab origin
            // This prevents false proximity detection on the tray
            if (!_hasMoved)
            {
                float travelDist = Vector3.Distance(transform.position, _grabPosition);
                if (travelDist < minTravelDistance)
                {
                    SetColor(grabbedColor); // stay yellow while on tray
                    return;
                }
                _hasMoved = true;
            }

            // Now do proximity check
            _nearbyAtoms.Clear();
            foreach (var other in _allAtoms)
            {
                if (other == this) continue;
                if (!other.gameObject.activeSelf) continue;

                float dist = Vector3.Distance(transform.position, other.transform.position);
                if (dist <= bondDetectionRadius)
                    _nearbyAtoms.Add(other);
            }

            // Update color
            SetColor(_nearbyAtoms.Count > 0 ? proximityColor : grabbedColor);
        }

        // ── Grab Events ──────────────────────────────────────────────────

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            _isGrabbed    = true;
            _hasMoved     = false;
            _grabPosition = transform.position;  // record where we picked it up
            SetColor(grabbedColor);
            AudioManager.Instance?.PlayGrabSound();
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            _isGrabbed = false;
            AudioManager.Instance?.PlayReleaseSound();

            if (_nearbyAtoms.Count > 0 && bondManager != null)
                bondManager.TryFormBond(this, _nearbyAtoms);

            SetColor(normalColor);
            _nearbyAtoms.Clear();
            _hasMoved = false;
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private void SetColor(Color color)
        {
            if (atomRenderer != null)
                atomRenderer.material.color = color;
        }

        public void Consume()
        {
            _isGrabbed = false;
            _nearbyAtoms.Clear();
            gameObject.SetActive(false);
        }

        public void ResetAtom()
        {
            _isGrabbed = false;
            _hasMoved  = false;
            _nearbyAtoms.Clear();
            SetColor(normalColor);
            gameObject.SetActive(true);
        }
    }
}