using UnityEngine;

public class Recuperators : MonoBehaviour
{
    [SerializeField] private string _vfxName;
    [SerializeField] private string _sfxName;

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball == null)
            return;

        GameplayManager.Instance.AudioManager.PlaySFX(_sfxName);
        GameplayManager.Instance.FXManager.Instantiate(_vfxName, transform.position, Quaternion.identity, null);
        ball.LevelUp();
    }
}
