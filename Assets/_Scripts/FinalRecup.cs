using UnityEngine;

public class FinalRecup : Recuperators
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        GameplayManager.Instance.UpdateScore(2525);
        GameplayManager.Instance.Evolve();
    }
}
