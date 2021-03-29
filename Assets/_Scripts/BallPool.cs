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

    private IEnumerator LaunchCorout()
    {
        int delay = 2;

        while (_reserve.Count > 0)
        {
            GameObject instance = _reserve.Dequeue();
            instance.transform.localPosition = Vector3.zero;
            instance.SetActive(true);
            float rnd = Random.Range(5f, 9f);
            instance.GetComponent<Ball>().Bump(Vector2.up, rnd);
            _ingame.Add(instance);

            yield return new WaitForSeconds(delay);
        }
    }
}
