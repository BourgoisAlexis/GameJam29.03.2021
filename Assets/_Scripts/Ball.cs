using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Variables
    [SerializeField] private Sprite[] _sprites;

    private Transform _transform;
    private Rigidbody2D _rb;
    private Transform _visual;

    private float _bottomLimit;
    private int _level;

    //Accessors
    public int Level => _level;
    #endregion


    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _visual = GetComponentInChildren<SpriteRenderer>().transform;
    }

    private void Start()
    {
        _bottomLimit = GameplayManager.Instance.BottomLimit;
        _visual.GetComponent<SpriteRenderer>().sprite = _sprites[_level];
    }

    private void FixedUpdate()
    {
        //Look At
        Vector3 dir = _rb.velocity;
        dir.Normalize();

        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _visual.rotation = Quaternion.Euler(0f, 0f, rot - 90);

        //Deformation
        float speed = Vector2.SqrMagnitude(_rb.velocity);
        speed /= 1000;
        speed = Mathf.Clamp(speed, 0.5f , 0.8f);

        _visual.localScale = Vector3.Lerp(_transform.localScale, new Vector3(1 - speed, 1, 1 + speed), 0.4f);


        if (_transform.position.y < -_bottomLimit)
        {
            GameplayManager.Instance.BallPool.LoseBall(gameObject);
            if (_level < 4)
                _level++;

            _visual.GetComponent<SpriteRenderer>().sprite = _sprites[_level];
        }
    }


    public void Bump(Vector2 dir, float force)
    {
        _rb.AddForce(dir * force * 100);
    }
}
