using System.Collections.Generic;
using UnityEngine;

namespace MolecularLab
{
    public class AtomSpawner : MonoBehaviour
    {
        [Header("Atom Prefabs")]
        public GameObject hydrogenPrefab;
        public GameObject oxygenPrefab;
        public GameObject carbonPrefab;
        public GameObject nitrogenPrefab;

        [Header("Spawn Count Per Type")]
        [Tooltip("How many of each atom to spawn on the tray at once.")]
        public int hydrogenCount  = 6;  // Need up to 5 for Glycine
        public int oxygenCount    = 3;  // Need up to 2 for CO2
        public int carbonCount    = 2;  // Need up to 2 for Cyanogen
        public int nitrogenCount  = 2;  // Need up to 2 for N2

        [Header("Layout")]
        [Tooltip("Spacing between atoms on the tray.")]
        public float slotSpacing  = 0.15f;
        [Tooltip("Row spacing between atom type rows.")]
        public float rowSpacing   = 0.18f;

        [Header("References")]
        public BondManager bondManager;

        // Track spawned atoms so we can clear them on refill
        private readonly List<GameObject> _spawnedAtoms = new List<GameObject>();

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Start()
        {
            SpawnAllAtoms();
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Clears all existing atoms on the tray and spawns a fresh set.
        /// Wire this to a "Refill" button in the scene.
        /// </summary>
        public void RefillAtoms()
        {
            // Destroy any remaining atoms on the tray
            foreach (var atom in _spawnedAtoms)
            {
                if (atom != null)
                    Destroy(atom);
            }
            _spawnedAtoms.Clear();

            SpawnAllAtoms();
        }

        // ── Private Helpers ──────────────────────────────────────────────

        private void SpawnAllAtoms()
        {
            // Row 0 — Hydrogen (white)
            SpawnRow(hydrogenPrefab,  hydrogenCount,  0);

            // Row 1 — Oxygen (red)
            SpawnRow(oxygenPrefab,    oxygenCount,    1);

            // Row 2 — Carbon (dark)
            SpawnRow(carbonPrefab,    carbonCount,    2);

            // Row 3 — Nitrogen (blue)
            SpawnRow(nitrogenPrefab,  nitrogenCount,  3);
        }

        private void SpawnRow(GameObject prefab, int count, int row)
        {
            if (prefab == null) return;

            for (int i = 0; i < count; i++)
            {
               Vector3 spawnPos = transform.position
    + transform.right   * (i   * slotSpacing)
    + transform.forward * (row  * rowSpacing)
    + Vector3.up        * 0.05f;

var atom = Instantiate(prefab, spawnPos, Quaternion.identity);

               

                // Inject BondManager
                var controller = atom.GetComponent<AtomController>();
                if (controller != null && bondManager != null)
                    controller.bondManager = bondManager;

                _spawnedAtoms.Add(atom);
            }
        }
    }
}