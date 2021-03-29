using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEmitter : MonoBehaviour
{
    private AudioSource audiosource;
    private AudioReverbFilter reverb;

    private void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        audiosource.playOnAwake = false;

        reverb = gameObject.AddComponent<AudioReverbFilter>();
    }

    public void AdjustVolume(float _volume)
    {
        audiosource.volume = _volume;
    }

    public void AdjustReverb(float _reverb)
    {
        if (_reverb <= 0)
            reverb.reverbPreset = AudioReverbPreset.Off;
        else
        {
            reverb.reverbPreset = AudioReverbPreset.User;
            reverb.dryLevel = 0;
            reverb.room = 0;
            reverb.roomHF = 0;
            reverb.roomLF = 0;
            reverb.decayTime = 4 - (3 - 3 * _reverb);
            reverb.decayHFRatio = 1 - (0.8f - 0.8f * _reverb);
            reverb.reflectionsLevel = -600;
            reverb.reflectionsDelay = 0;
            reverb.reverbDelay = 0.05f;
            reverb.hfReference = 5000;
            reverb.lfReference = 250;
            reverb.diffusion = 70;
            reverb.density = 70;

            reverb.reverbLevel = -2000 + 2000 * _reverb;
        }
    }

    public void PlaySound(AudioClip _clip)
    {
        audiosource.clip = _clip;
        audiosource.Play();
    }

    public void StopSound()
    {
        audiosource.Stop();
    }
}
