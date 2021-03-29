using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private string _fxName;


    private void OnCollisionEnter2D(Collision2D other)
    {
        //Normal si pas que des ronds

        Vector2 dir = other.transform.position - transform.position;
        dir.Normalize();

        other.gameObject.GetComponent<Ball>()?.Bump(dir,  _force);

        GameplayManager.Instance.ShakyCam(false);

        GameplayManager.Instance.FXManager.Instantiate(_fxName, transform.position, Quaternion.identity, null);
    }
}
