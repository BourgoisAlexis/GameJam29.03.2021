using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    #region Variables
    [Header("Space Settings")]
    [SerializeField] private bool is2D;
    [SerializeField] private GameObject emitterPrefab;
    [Header("Volume Settings")]
    [SerializeField] [Range(0, 1)] private float musicVolume;
    [SerializeField] private int transition;
    [SerializeField] [Range(0, 1)] private float sfxVolume;
    [Header("Sounds")]
    [SerializeField] private List<AudioClip> clips;

    private int simultaneous = 20;
    private List<AudioSource> musics = new List<AudioSource>();
    private Queue<AudioEmitter> emitters = new Queue<AudioEmitter>();
    private Queue<AudioEmitter> emittersLoop = new Queue<AudioEmitter>();
    private Dictionary<string, AudioClip> repertory = new Dictionary<string, AudioClip>();

    private Transform camTransform;
    #endregion


    private void Awake()
    {
        camTransform = Camera.main.transform;

        SetupMusic();
        SetupOwner();

        foreach (AudioClip clip in clips)
        {
            repertory.Add(clip.name, clip);
        }
    }


    private void SetupMusic()
    {
        for (int i = 0; i < 2; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            musics.Add(source);
            source.playOnAwake = false;
            source.loop = true;
        }
    }

    private void SetupOwner()
    {
        for (int i = 0; i < simultaneous; i++)
        {
            GameObject instance = Instantiate(emitterPrefab, transform);
            instance.transform.localPosition = Vector3.zero;
            emitters.Enqueue(instance.GetComponent<AudioEmitter>());

            instance = Instantiate(emitterPrefab, transform);
            instance.transform.localPosition = Vector3.zero;
            emittersLoop.Enqueue(instance.GetComponent<AudioEmitter>());

            instance.GetComponent<AudioSource>().loop = true;
        }
    }


    public void PlayMusic(string _name)
    {
        int index = musics[0].isPlaying ? 1 : 0;
        AudioSource toUse = musics[index];

        if (repertory.ContainsKey("Music_" + _name))
        {
            toUse.volume = 0;
            toUse.clip = repertory["Music_" + _name];
            toUse.Play();

            StartCoroutine(Transition(index));
        }

    }

    private IEnumerator Transition(int _index)
    {
        AudioSource down = musics[_index == 0 ? 1 : 0];
        AudioSource up = musics[_index == 0 ? 0 : 1];
        
        int transitionning = transition;
        float step = musicVolume / transition;

        while (transitionning > 0)
        {
            down.volume -= step;
            up.volume += step;
            transitionning--;
            yield return new WaitForFixedUpdate();
        }

        down.volume = 0;
        down.Stop();

        up.volume = musicVolume;
    }


    public void PlaySFX(string _name, Transform _parent = null, Vector3 _position = default(Vector3), float _reverb = 0)
    {
        if (repertory.ContainsKey(_name))
        {
            AudioEmitter instance = emitters.Dequeue();
            Transform inst = instance.transform;
            emitters.Enqueue(instance);

            if (is2D == true)
                inst.position = new Vector3(_position.x, _position.y, transform.position.z);
            else
            {
                if (_position == default(Vector3))
                    inst.position = camTransform.position + camTransform.forward;
                else
                    inst.position = _position;
            }

            if (_parent == null)
            {
                if (_position == default(Vector3))
                    inst.SetParent(camTransform);
                else
                    inst.SetParent(transform);
            }
            else
                inst.SetParent(_parent);

            instance.AdjustVolume(sfxVolume);
            instance.AdjustReverb(_reverb);
            instance.PlaySound(repertory[_name]);
        }
    }

    public AudioEmitter PlaySFXLoop(string _name, Transform _parent = null, Vector3 _position = default(Vector3), float _reverb = 0)
    {
        if (repertory.ContainsKey(_name))
        {
            AudioEmitter instance = emittersLoop.Dequeue();
            Transform inst = instance.transform;

            if (is2D == true)
                inst.position = new Vector3(_position.x, _position.y, transform.position.z);
            else
            {
                if (_position == default(Vector3))
                    inst.position = camTransform.position + camTransform.forward;
                else
                    inst.position = _position;
            }

            if (_parent == null)
            {
                if (_position == default(Vector3))
                    inst.SetParent(camTransform);
                else
                    inst.SetParent(transform);
            }
            else
                inst.SetParent(_parent);

            instance.AdjustVolume(sfxVolume);
            instance.AdjustReverb(_reverb);
            instance.PlaySound(repertory[_name]);

            return instance;
        }

        return null;
    }

    public void EnqueueEmitter(AudioEmitter _emitter)
    {
        if (_emitter != null)
        {
            emittersLoop.Enqueue(_emitter);
            _emitter.StopSound();
        }
    }
}