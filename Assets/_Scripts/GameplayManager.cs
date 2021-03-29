using DG.Tweening;
using UnityEngine;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Variables
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;
    [SerializeField] private float _bottomLimit;
    [SerializeField] private SpriteRenderer _machineVisual;
    [SerializeField] private SpriteRenderer _screenVisual;
    [SerializeField] private Sprite[] _machineSprites;
    [SerializeField] private Sprite[] _screenSprites;

    private Vector3 _basePos;

    private Transform _camTransform;
    private FXManager _fxManager;
    private AudioManager _audioManager;
    private BallPool _ballPool;
    [SerializeField] private UIManager _uiManager;

    private int _current;
    private int _target;
    private int _pv = 30;
    private int _evolve;
    private bool _canSlap;

    //Accessors
    public FXManager FXManager => _fxManager;
    public UIManager UIManager => _uiManager;
    public AudioManager AudioManager => _audioManager;
    public BallPool BallPool => _ballPool;
    public float BottomLimit => _bottomLimit;
    #endregion


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        _camTransform = Camera.main.transform;
        _basePos = _camTransform.position;

        _fxManager = GetComponent<FXManager>();
        _audioManager = GetComponent<AudioManager>();
        _ballPool = GetComponent<BallPool>();
        _uiManager.UpdateScore(0);

        _canSlap = true;
    }

    private void Update()
    {
        if (_canSlap)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Slap(true);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Slap(false);
        }

        if (_current != _target)
        {
            int multiplier = 1;
            int diff = Mathf.Abs(_target - _current);

            if (diff > 10)
                multiplier = diff / 10;

            _current += (int)Mathf.Sign(_target - _current) * multiplier;
            _uiManager.UpdateScore(_current);
        }
    }


    public void UpdateScore(int value)
    {
        _target += value;
    }


    private void Slap(bool left)
    {
        _canSlap = false;
        if (_pv > 0)
            _pv--;

        int step = 30 / 4;


        if (_pv == 0)
        {
            _machineVisual.sprite = _machineSprites[3];
            _uiManager.UpdatePV(_pv);
            _ballPool.Stop();
            return;
        }
        else if (_pv < step * 2)
            _machineVisual.sprite = _machineSprites[2];
        else if (_pv < step * 3)
            _machineVisual.sprite = _machineSprites[1];


        _uiManager.UpdatePV(_pv);

        if (left)
            _rightHand.DOMoveX(8.7f, 0.05f).onComplete += RightSlap;
        else
            _leftHand.DOMoveX(-8.7f, 0.05f).onComplete += LeftSlap;
    }

    private void RightSlap()
    {
        _audioManager.PlaySFX("SFX_Slam", null, _rightHand.position);

        _canSlap = true;
        ShakyCam(true);

        _ballPool.BumpAll(new Vector2(-1, 0), 2);

        _rightHand.DOMoveX(15, 0.05f);
    }

    private void LeftSlap()
    {
        _audioManager.PlaySFX("SFX_Slam", null, _leftHand.position);

        _canSlap = true;
        ShakyCam(true);

        _ballPool.BumpAll(new Vector2(1, 0), 2);

        _leftHand.DOMoveX(-15, 0.05f);
    }

    public void ShakyCam(bool super)
    {
        Sequence seq = DOTween.Sequence();
        float intensity = 0.05f * (super ? 3 : 1);
        int iterrations = 6 * (super ? 3 : 1);

        for (int i = 0; i < iterrations; i++)
        {
            Vector3 random = new Vector3(Random.Range(intensity, -intensity), Random.Range(intensity, -intensity), 0);
            seq.Append(_camTransform.DOMove(_basePos + random, 0.01f));
        }

        seq.Append(_camTransform.DOMove(_basePos, 0.05f));
    }

    public void Evolve()
    {
        if (_evolve < 2)
            _evolve++;

        _screenVisual.sprite = _screenSprites[_evolve];
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 pos = -Vector3.up * _bottomLimit;
        Gizmos.DrawLine(pos + Vector3.left * 5, pos - Vector3.left * 5);
    }
}
