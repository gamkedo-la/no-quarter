using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public delegate void InteractionStarted();
    public abstract event InteractionStarted started;

    public delegate void InteractionFinished();
    public abstract event InteractionFinished finished;

    public abstract void HandleInteraction(GameObject caller);
}