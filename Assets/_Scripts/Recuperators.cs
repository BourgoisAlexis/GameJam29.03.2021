using UnityEngine;

public class Recuperators : MonoBehaviour
{


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball == null)
            return;

        ball.LevelUp();
    }
}
