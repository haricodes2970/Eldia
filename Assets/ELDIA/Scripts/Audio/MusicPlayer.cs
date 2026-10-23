using UnityEngine;

namespace ELDIA.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class MusicPlayer : MonoBehaviour
    {
        private static MusicPlayer instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            AudioSource source = GetComponent<AudioSource>();
            source.clip = CreateAmbientPad();
            source.loop = true;
            source.volume = 0.15f;
            source.playOnAwake = true;
            source.Play();
        }

        private static AudioClip CreateAmbientPad()
        {
            const int sampleRate = 44100;
            const int seconds = 8;
            int sampleCount = sampleRate * seconds;
            AudioClip clip = AudioClip.Create("ELDIA Ambient Pad", sampleCount, 1, sampleRate, false);
            float[] samples = new float[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                float time = i / (float)sampleRate;
                float breath = 0.65f + 0.35f * Mathf.Sin(time * Mathf.PI * 2f / seconds);
                samples[i] = (Mathf.Sin(time * Mathf.PI * 2f * 110f) +
                    Mathf.Sin(time * Mathf.PI * 2f * 165f)) * 0.045f * breath;
            }

            clip.SetData(samples, 0);
            return clip;
        }
    }
}
