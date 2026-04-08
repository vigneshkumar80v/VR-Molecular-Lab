using UnityEngine;
 
namespace MolecularLab
{
    public enum BondType
    {
        Single,
        Double,
        Triple,
        Mixed   // for molecules like Urea, Glycine with multiple bond types
    }
 
    [CreateAssetMenu(fileName = "NewMolecule", menuName = "MolecularLab/Molecule Data")]
    public class MoleculeData : ScriptableObject
    {
        [Header("Identity")]
        public string moleculeName;         // e.g. "Water"
        public string formula;              // e.g. "H₂O"
        public BondType bondType;
 
        [Header("Atom Requirements")]
        public int hydrogenCount;           // H
        public int oxygenCount;             // O
        public int carbonCount;             // C
        public int nitrogenCount;           // N
 
        [Header("Visuals & Audio")]
        public GameObject moleculePrefab;   // 3D prefab to spawn on bond
        public Sprite moleculeIcon;         // Optional icon for library panel
 
        [Header("Description")]
        [TextArea(2, 4)]
        public string description;          // Short info for the inspector panel
 
        /// <summary>
        /// Returns true if the given atom counts exactly match this molecule's recipe.
        /// </summary>
        public bool Matches(int h, int o, int c, int n)
        {
            return h == hydrogenCount &&
                   o == oxygenCount   &&
                   c == carbonCount   &&
                   n == nitrogenCount;
        }
    }
}