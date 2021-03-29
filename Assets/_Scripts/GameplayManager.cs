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
    private Ball _ball;
    private Transform _camTransform;
    private FXManager _fxManager;

    private bool canSlap;

    //Accessors
    public FXManager FXManager => _fxManager;
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
    }


    public void SetupBall(Ball ball)
    {
        _ball = ball;
    }


    private void Slap(bool left)
    {
        if (left)
            rightHand.DOMoveX(2.5f, 0.05f).onComplete += LeftSlap;
        else
            leftHand.DOMoveX(-2.5f, 0.05f).onComplete += RightSlap;
    }

    private void LeftSlap()
    {
        ShakyCam(true);

        if (_ball != null)
            _ball.Bump(new Vector2(-1, 0), 2);
    }

    private void RightSlap()
    {
        ShakyCam(true);

        if (_ball != null)
            _ball.Bump(new Vector2(1, 0), 2);

        leftHand.DOMoveX(-2.5f, 0.05f).onComplete += RightSlap;
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
