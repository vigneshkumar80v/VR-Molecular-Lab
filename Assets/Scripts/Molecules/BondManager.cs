// BondManager.cs (Fixed — uses Physics.OverlapSphere to find ALL nearby atoms)
// Place in: Assets/Scripts/Molecules/
// Attach to a single empty GameObject named "BondManager"

using System.Collections.Generic;
using UnityEngine;

namespace MolecularLab
{
    public class BondManager : MonoBehaviour
    {
        [Header("References")]
        public MoleculeDatabase moleculeDatabase;
        public AudioManager     audioManager;
        public UIManager        uiManager;

        [Header("Bond Detection")]
        [Tooltip("Radius used to scan for ALL nearby atoms when a bond attempt is made.")]
        public float bondScanRadius = 0.25f;

        [Header("Spawn Settings")]
        public float spawnHeightOffset = 0.1f;

        // Layer mask — set this to the layer your atoms are on (default = Everything)
        [Header("Atom Layer")]
        public LayerMask atomLayerMask = ~0;

        // ── Unity Lifecycle ──────────────────────────────────────────────

        private void Start()
        {
            // Inject this manager into all atoms in the scene
            var atoms = FindObjectsByType<AtomController>(FindObjectsSortMode.None);
            foreach (var atom in atoms)
                atom.bondManager = this;
        }

        // ── Public API ───────────────────────────────────────────────────

        /// <summary>
        /// Called by AtomController when released.
        /// Uses OverlapSphere to find ALL atoms near the drop point,
        /// then checks every possible subset against the molecule database.
        /// </summary>
        public void TryFormBond(AtomController initiator, List<AtomController> nearby)
        {
            // Use Physics.OverlapSphere at the initiator's position
            // to collect ALL active atoms in range (more reliable than trigger lists)
            var allNearby = GetAllAtomsInRange(initiator.transform.position);

            // Always include the initiator itself
            if (!allNearby.Contains(initiator))
                allNearby.Insert(0, initiator);

            // Count atoms by type in the full nearby set
            int h = 0, o = 0, c = 0, n = 0;
            CountAtoms(allNearby, ref h, ref o, ref c, ref n);

            Debug.Log($"[BondManager] Nearby atoms — H:{h} O:{o} C:{c} N:{n}");

            // First try exact match with all nearby atoms
            var match = moleculeDatabase.FindMatch(h, o, c, n);

            if (match != null)
            {
                FormMolecule(match, allNearby);
                return;
            }

            // If no full match, try subsets (e.g. player dropped H near H+O — try H2O)
            // Try every subset of allNearby that includes the initiator
            var subsetMatch = TryFindSubsetMatch(initiator, allNearby);
            if (subsetMatch.data != null)
            {
                FormMolecule(subsetMatch.data, subsetMatch.atoms);
                return;
            }

            // No valid combination found
            Debug.Log("[BondManager] No valid molecule found.");
            audioManager?.PlayFailSound();
        }

        /// <summary>
        /// Called by MoleculeObject when broken apart — re-activates atoms.
        /// </summary>
        public void ReturnAtoms(List<AtomController> atoms, Vector3 spawnOrigin)
        {
            float offset = 0.1f;
            for (int i = 0; i < atoms.Count; i++)
            {
                atoms[i].transform.position = spawnOrigin + Vector3.right * (i * offset);
                atoms[i].ResetAtom();

                // Re-inject bond manager reference
                atoms[i].bondManager = this;
            }
        }

        // ── Private Helpers ──────────────────────────────────────────────

        /// <summary>
        /// Uses Physics.OverlapSphere to find all active AtomControllers in range.
        /// </summary>
        private List<AtomController> GetAllAtomsInRange(Vector3 center)
        {
            var result  = new List<AtomController>();
            var hits    = Physics.OverlapSphere(center, bondScanRadius, atomLayerMask);

            foreach (var hit in hits)
            {
                var atom = hit.GetComponent<AtomController>();
                if (atom != null && atom.gameObject.activeSelf)
                {
                    if (!result.Contains(atom))
                        result.Add(atom);
                }
            }
            return result;
        }

        /// <summary>
        /// Tries all subsets of nearby atoms (that include the initiator)
        /// to find a valid molecule match.
        /// </summary>
        private (MoleculeData data, List<AtomController> atoms) TryFindSubsetMatch(
            AtomController initiator, List<AtomController> pool)
        {
            int count = pool.Count;

            // Try subsets from largest to smallest (prefer bigger molecules)
            for (int size = count; size >= 2; size--)
            {
                var subsets = GetSubsets(pool, size);
                foreach (var subset in subsets)
                {
                    // Must include the initiator
                    if (!subset.Contains(initiator)) continue;

                    int h = 0, o = 0, c = 0, n = 0;
                    CountAtoms(subset, ref h, ref o, ref c, ref n);

                    var match = moleculeDatabase.FindMatch(h, o, c, n);
                    if (match != null)
                        return (match, subset);
                }
            }

            return (null, null);
        }

        private void FormMolecule(MoleculeData data, List<AtomController> usedAtoms)
        {
            // Calculate midpoint
            Vector3 center = Vector3.zero;
            foreach (var atom in usedAtoms)
                center += atom.transform.position;
            center /= usedAtoms.Count;
            center.y += spawnHeightOffset;

            // Spawn molecule prefab
            if (data.moleculePrefab != null)
            {
                var molecule = Instantiate(data.moleculePrefab, center, Quaternion.identity);
                var molObj   = molecule.GetComponent<MoleculeObject>();
                molObj?.Initialize(data, usedAtoms, this);
            }

            // Consume atoms
            foreach (var atom in usedAtoms)
                atom.Consume();

            audioManager?.PlaySuccessSound();
            uiManager?.OnMoleculeDiscovered(data);

            Debug.Log($"[BondManager] ✅ Formed: {data.moleculeName} ({data.formula})");
        }

        private void CountAtoms(List<AtomController> atoms, ref int h, ref int o, ref int c, ref int n)
        {
            foreach (var atom in atoms)
            {
                switch (atom.atomType)
                {
                    case AtomType.Hydrogen:  h++; break;
                    case AtomType.Oxygen:    o++; break;
                    case AtomType.Carbon:    c++; break;
                    case AtomType.Nitrogen:  n++; break;
                }
            }
        }

        /// <summary>
        /// Returns all subsets of a given size from the pool.
        /// </summary>
        private List<List<AtomController>> GetSubsets(List<AtomController> pool, int size)
        {
            var result = new List<List<AtomController>>();
            GetSubsetsRecursive(pool, size, 0, new List<AtomController>(), result);
            return result;
        }

        private void GetSubsetsRecursive(
            List<AtomController> pool,
            int size,
            int start,
            List<AtomController> current,
            List<List<AtomController>> result)
        {
            if (current.Count == size)
            {
                result.Add(new List<AtomController>(current));
                return;
            }

            for (int i = start; i < pool.Count; i++)
            {
                current.Add(pool[i]);
                GetSubsetsRecursive(pool, size, i + 1, current, result);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}