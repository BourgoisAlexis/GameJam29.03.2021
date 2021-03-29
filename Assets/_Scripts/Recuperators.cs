using UnityEngine;

public class Recuperators : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball == null)
            return;

        ball.LevelUp();
    }
}
