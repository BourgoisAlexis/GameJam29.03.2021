using System.Collections;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private int _points;
    [SerializeField] private string _vfxName;
    [SerializeField] private string _sfxName;
    [SerializeField] private Sprite[] _sprites;

    private Coroutine _corout;
    private SpriteRenderer _visual;

    private void Awake()
    {
        _visual = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Normal si pas que des ronds

        Ball ball = other.gameObject.GetComponent<Ball>();

        if (ball == null)
            return;

        Vector2 dir = other.transform.position - transform.position;
        dir.Normalize();

        ball.Bump(dir,  _force);
        GameplayManager.Instance.ShakyCam(false);
        GameplayManager.Instance.UpdateScore(_points * (ball.Level + 1));
        GameplayManager.Instance.AudioManager.PlaySFX(_sfxName);
        GameplayManager.Instance.FXManager.Instantiate(_vfxName, transform.position, Quaternion.identity, null);

        if (_force <= 0)
            return;

        if (_corout != null)
            StopCoroutine(_corout);

        _corout = StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        _visual.sprite = _sprites[1];
        yield return new WaitForSeconds(0.05f);
        _visual.sprite = _sprites[0];
    }
}
