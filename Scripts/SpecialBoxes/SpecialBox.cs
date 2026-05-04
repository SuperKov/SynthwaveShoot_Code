using UnityEngine;

public abstract class SpecialBox : MonoBehaviour
{
    public abstract void Use();

    public virtual bool CanBeUsed() { return true; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || !CanBeUsed()) return;
        Use();
        Destroy(gameObject);
    }
}
