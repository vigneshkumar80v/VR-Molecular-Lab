using System.Collections.Generic;
using UnityEngine;
 
namespace MolecularLab
{
    [CreateAssetMenu(fileName = "MoleculeDatabase", menuName = "MolecularLab/Molecule Database")]
    public class MoleculeDatabase : ScriptableObject
    {
        [Tooltip("Add all MoleculeData ScriptableObjects here.")]
        public List<MoleculeData> molecules = new List<MoleculeData>();
 
        /// <summary>
        /// Searches the database for a molecule matching the given atom counts.
        /// Returns null if no match is found.
        /// </summary>
        public MoleculeData FindMatch(int h, int o, int c, int n)
        {
            foreach (var molecule in molecules)
            {
                if (molecule.Matches(h, o, c, n))
                    return molecule;
            }
            return null;
        }
 
        /// <summary>
        /// Returns total number of molecules in the database.
        /// </summary>
        public int TotalCount => molecules.Count;
    }
}