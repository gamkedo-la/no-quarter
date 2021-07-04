using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StoreTerminal : Interactable
{
    public override event InteractionStarted started;
    public override event InteractionFinished finished;

    public override void HandleInteraction(GameObject caller)
    {
        SceneManager.LoadSceneAsync("Scenes/Store", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += (scene, mode) => started?.Invoke();
        SceneManager.sceneUnloaded += (scene) => finished?.Invoke();
    }
}