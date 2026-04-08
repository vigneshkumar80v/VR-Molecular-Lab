
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MolecularLab
{
    public class UIManager : MonoBehaviour
    {
        [Header("Library Panel")]
        public GameObject      libraryPanel;
        public Transform       libraryEntryContainer;
        public GameObject      libraryEntryPrefab;
        public UIAnimator      libraryAnimator;         // Attach UIAnimator to LibraryPanel

        [Header("Discovery Counter")]
        public TextMeshProUGUI discoveryCounterText;
        public RectTransform   counterRect;             // For bounce animation

        [Header("Flash Effect")]
        public CanvasGroup flashGroup;                  // Optional full-panel flash overlay

        [Header("Database Reference")]
        public MoleculeDatabase moleculeDatabase;

        // Internal
        private readonly HashSet<string> _discovered = new HashSet<string>();
        private bool _libraryOpen = false;

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Start()
        {
            UpdateDiscoveryCounter();

            if (libraryPanel != null)
             libraryPanel.SetActive(false);
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
        public void OnMoleculeDiscovered(MoleculeData data)
        {
            if (_discovered.Contains(data.moleculeName)) return;

            _discovered.Add(data.moleculeName);
            AddLibraryEntry(data);
            UpdateDiscoveryCounter();

            // Flash + counter bounce
            libraryAnimator?.PlayDiscoveryFlash();
            if (libraryAnimator != null && counterRect != null)
                libraryAnimator.BounceCounter();

            // Auto-open library on first discovery
            if (!_libraryOpen)
                ShowLibrary();
        }

        public void ToggleLibrary()
        {
            if (_libraryOpen) HideLibrary();
            else              ShowLibrary();
        }

        public void ShowLibrary()
        {
            _libraryOpen = true;
            if (libraryAnimator != null)
                libraryAnimator.SlideIn();
            else
                libraryPanel?.SetActive(true);
        }

        public void HideLibrary()
        {
            _libraryOpen = false;
            if (libraryAnimator != null)
                libraryAnimator.SlideOut();
            else
                libraryPanel?.SetActive(false);
        }

        // ── Private Helpers ──────────────────────────────────────────────

        private void AddLibraryEntry(MoleculeData data)
        {
            if (libraryEntryPrefab == null || libraryEntryContainer == null) return;

            var entry = Instantiate(libraryEntryPrefab, libraryEntryContainer);
            var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
               texts[0].text = $"{data.moleculeName}  <color=#AAFFAA>{FormatFormula(data.formula)}</color>";
                texts[1].text = data.bondType.ToString();
            }
            else if (texts.Length == 1)
            {
                texts[0].text = $"{data.moleculeName}  {FormatFormula(data.formula)}";
            }

            var icon = entry.GetComponentInChildren<Image>();
            if (icon != null && data.moleculeIcon != null)
                icon.sprite = data.moleculeIcon;

            // Animate the new entry sliding in
            var entryRect = entry.GetComponent<RectTransform>();
            libraryAnimator?.AnimateEntryIn(entryRect);
        }

        private void UpdateDiscoveryCounter()
        {
            if (discoveryCounterText != null && moleculeDatabase != null)
            {
                discoveryCounterText.text =
                    $"Discovered: {_discovered.Count} / {moleculeDatabase.TotalCount}";
            }
        }
    }
}
