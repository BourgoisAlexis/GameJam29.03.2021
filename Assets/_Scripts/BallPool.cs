using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BallPool : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject _prefab;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _starting;

    private Queue<GameObject> _reserve = new Queue<GameObject>();
    private Queue<GameObject> _lost = new Queue<GameObject>();
    private List<GameObject> _ingame = new List<GameObject>();

    private float delay = 1;
    private bool stop;
    #endregion


    private void Awake()
    {
        for (int i = 0; i < _starting; i++)
        {
            GameObject instance = Instantiate(_prefab, _parent);
            _reserve.Enqueue(instance);
            instance.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(LaunchCorout());
    }


    public void BumpAll(Vector2 dir, float force)
    {
        List<GameObject> balls = new List<GameObject>(_ingame);

        foreach (GameObject g in balls)
            g.GetComponent<Ball>().Bump(dir, force);
    }

    public void LoseBall(GameObject ball)
    {
        if (stop)
            return;

        GameplayManager.Instance.AudioManager.PlaySFX("SFX_LostBall");

        _lost.Enqueue(ball);
        _ingame.Remove(ball);

        ball.SetActive(false);
    }

    public void EnqueueBall(GameObject ball)
    {
        if (stop)
            return;

        _reserve.Enqueue(ball);
        _ingame.Remove(ball);

        ball.SetActive(false);
        UpdateCount();
    }

    public void ChangeSpeed(Slider slider)
    {
        delay = slider.value;
    }

    private IEnumerator LaunchCorout()
    {
        while (_reserve.Count > 0)
        {
            GameObject instance = _reserve.Dequeue();
            UpdateCount();
            instance.transform.localPosition = Vector3.zero;
            instance.SetActive(true);
            float rnd = Random.Range(4f, 6f);
            instance.GetComponent<Ball>().Bump(Vector2.up, rnd);
            _ingame.Add(instance);

            yield return new WaitForSeconds(delay);
        }
    }

    private void UpdateCount()
    {
        GameplayManager.Instance.UIManager.UpdateBallCount(_reserve.Count);
    }

    public void Stop()
    {
        stop = true;
        StopAllCoroutines();
        StartCoroutine(Destroying());
    }

    private IEnumerator Destroying()
    {
        for (int i = _ingame.Count - 1; i >= 0; i--)
        {
            _ingame[i].SetActive(false);
            GameplayManager.Instance.FXManager.Instantiate("P_VFX_Sparkles_Ball_Collision", _ingame[i].transform.position, Quaternion.identity, null);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
