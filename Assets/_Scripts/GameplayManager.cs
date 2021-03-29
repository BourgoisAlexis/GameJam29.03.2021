using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Variables
    [Header("UI")]
    [SerializeField] private GraphicRaycaster _GRayCaster;
    [SerializeField] private EventSystem _eventSystem;

    [Header("Hands")]
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    [Header("Adaptative Visuals")]
    [SerializeField] private SpriteRenderer _machineVisual;
    [SerializeField] private SpriteRenderer _screenVisual;
    [SerializeField] private Sprite[] _machineSprites;
    [SerializeField] private Sprite[] _screenSprites;
    [SerializeField] private float _bottomLimit;


    private Camera _mainCamera;
    private PointerEventData _pointerEventData;

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

        _mainCamera = Camera.main;
        _camTransform = _mainCamera.transform;
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
            if (Input.GetMouseButtonDown(1))
                Slap(true);
            else if (Input.GetMouseButtonDown(0) && DetectUI(Input.mousePosition) == false)
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
        else
            return;

        int step = 30 / 4;


        if (_pv == 0)
        {
            _machineVisual.sprite = _machineSprites[3];
            _ballPool.Stop();
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

        _fxManager.Instantiate("P_VFX_Evolution", _screenVisual.transform.position - Vector3.forward, Quaternion.identity, null);
        _screenVisual.sprite = _screenSprites[_evolve];
    }


    private bool DetectUI(Vector2 _position)
    {
        _pointerEventData = new PointerEventData(_eventSystem);
        _pointerEventData.position = _position;

        List<RaycastResult> results = new List<RaycastResult>();

        _GRayCaster.Raycast(_pointerEventData, results);

        foreach (RaycastResult result in results)
            return true;

        return false;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 pos = -Vector3.up * _bottomLimit;
        Gizmos.DrawLine(pos + Vector3.left * 5, pos - Vector3.left * 5);
    }
}
