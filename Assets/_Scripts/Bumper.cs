using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private int _points;
    [SerializeField] private string _fxName;


    private void OnCollisionEnter2D(Collision2D other)
    {
        //Normal si pas que des ronds

        Vector2 dir = other.transform.position - transform.position;
        dir.Normalize();

        other.gameObject.GetComponent<Ball>()?.Bump(dir,  _force);

        GameplayManager.Instance.ShakyCam(false);
        GameplayManager.Instance.UpdateScore(_points);
        GameplayManager.Instance.FXManager.Instantiate(_fxName, transform.position, Quaternion.identity, null);
    }
}
