using DG.Tweening;
using UnityEngine;
using System.Collections;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    #region Variables
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private Vector3 _basePos;
    private bool canSlap;

    private Transform _camTransform;
    private FXManager _fxManager;
    private BallPool _ballPool;
    [SerializeField] private UIManager _uiManager;

    //Score
    private int current;
    private int target;

    //Accessors
    public FXManager FXManager => _fxManager;
    public UIManager UIManager => _uiManager;
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
        _ballPool = GetComponent<BallPool>();
        _uiManager.UpdateScore(0);

        canSlap = true;
    }

    private void Update()
    {
        if (canSlap)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Slap(true);
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Slap(false);
        }

        if (current != target)
        {
            int multiplier = 1;
            int diff = Mathf.Abs(target - current);

            if (diff > 10)
                multiplier = diff / 10;

            current += (int)Mathf.Sign(target - current) * multiplier;
            _uiManager.UpdateScore(current);
        }
    }


    public void UpdateScore(int value)
    {
        target += value;
    }


    private void Slap(bool left)
    {
        canSlap = false;

        if (left)
            rightHand.DOMoveX(2.5f, 0.05f).onComplete += RightSlap;
        else
            leftHand.DOMoveX(-2.5f, 0.05f).onComplete += LeftSlap;
    }

    private void RightSlap()
    {
        canSlap = true;
        ShakyCam(true);

        _ballPool.BumpAll(new Vector2(-1, 0), 2);

        rightHand.DOMoveX(7f, 0.05f);
    }

    private void LeftSlap()
    {
        canSlap = true;
        ShakyCam(true);

        _ballPool.BumpAll(new Vector2(1, 0), 2);

        leftHand.DOMoveX(-7f, 0.05f);
    }

    public void ShakyCam(bool super)
    {
        Sequence seq = DOTween.Sequence();
        float intensity = 0.08f * (super ? 3 : 1);
        int iterrations = 6 * (super ? 3 : 1);

        for (int i = 0; i < iterrations; i++)
        {
            Vector3 random = new Vector3(Random.Range(intensity, -intensity), Random.Range(intensity, -intensity), 0);
            seq.Append(_camTransform.DOMove(_basePos + random, 0.01f));
        }

        seq.Append(_camTransform.DOMove(_basePos, 0.05f));
    }
}
