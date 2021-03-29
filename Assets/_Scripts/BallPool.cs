using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private int _starting;

    private Queue<GameObject> _reserve = new Queue<GameObject>();
    private Queue<GameObject> _used = new Queue<GameObject>();
    private List<GameObject> _ingame = new List<GameObject>();

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
        LaunchBall(10);
    }

    public void BumpAll(Vector2 dir, float force)
    {
        foreach (GameObject g in _ingame)
            g.GetComponent<Ball>().Bump(dir, force);
    }

    private void LaunchBall(int delay)
    {
        StartCoroutine(LaunchCorout(delay));
    }

    private IEnumerator LaunchCorout(int delay)
    {
        for (int i = 0;  i < delay; i++)
        {
            GameObject instance = _reserve.Dequeue();
            instance.SetActive(true);
            _ingame.Add(instance);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
