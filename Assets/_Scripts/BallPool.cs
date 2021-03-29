using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _starting;
    [SerializeField] private int _startSpawn;

    private Queue<GameObject> _reserve = new Queue<GameObject>();
    private Queue<GameObject> _lost = new Queue<GameObject>();
    private List<GameObject> _ingame = new List<GameObject>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            LaunchBall(_startSpawn);
    }


    public void BumpAll(Vector2 dir, float force)
    {
        List<GameObject> balls = new List<GameObject>(_ingame);

        foreach (GameObject g in balls)
            g.GetComponent<Ball>().Bump(dir, force);
    }

    public void LoseBall(GameObject ball)
    {
        _lost.Enqueue(ball);
        _ingame.Remove(ball);

        ball.SetActive(false);
    }

    public void EnqueueBall(GameObject ball)
    {
        _reserve.Enqueue(ball);
        _ingame.Remove(ball);

        ball.SetActive(false);
    }

    private void LaunchBall(int delay)
    {
        StartCoroutine(LaunchCorout(delay));
    }

    private IEnumerator LaunchCorout(int delay)
    {
        for (int i = 0;  i < delay; i++)
        {
            if (_reserve.Count <= 0)
                break;

            GameObject instance = _reserve.Dequeue();
            instance.transform.localPosition = Vector3.zero;
            instance.SetActive(true);
            instance.GetComponent<Ball>().Bump(Vector2.up, 10);
            _ingame.Add(instance);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
