using UnityEngine;
 
namespace MolecularLab
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource sfxSource;       // For one-shot effects
 
        [Header("Sound Clips")]
        public AudioClip successBondClip;   // Played on valid molecule formed
        public AudioClip failBondClip;      // Played on invalid combination
        public AudioClip grabClip;          // Played when atom is picked up
        public AudioClip releasClip;        // Played when atom is dropped
        public AudioClip uiClickClip;       // Played on UI button interaction
        public AudioClip breakApartClip;    // Played when molecule is broken
 
        [Header("Volume Settings")]
        [Range(0f, 1f)] public float masterVolume = 1f;
 
        // Singleton for easy access (optional)
        public static AudioManager Instance { get; private set; }
 
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
 
            if (sfxSource == null)
                sfxSource = gameObject.AddComponent<AudioSource>();
 
            sfxSource.playOnAwake = false;
        }
 
        // ── Public API ───────────────────────────────────────────────────
 
        public void PlaySuccessSound()   => PlayClip(successBondClip);
        public void PlayFailSound()      => PlayClip(failBondClip);
        public void PlayGrabSound()      => PlayClip(grabClip);
        public void PlayReleaseSound()   => PlayClip(releasClip);
        public void PlayUIClickSound()   => PlayClip(uiClickClip);
        public void PlayBreakSound()     => PlayClip(breakApartClip);
 
        private void PlayClip(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, masterVolume);
        }
    }
}