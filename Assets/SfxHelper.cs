using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxHelper : MonoBehaviour
{

    public int PlayRandomAudioOneshot(List<AudioClip> sources, float minVolume, float maxVolume, float minPitch, float maxPitch)
    {
        if (sources.Count > 0)
        {
            var idx = UnityEngine.Random.Range(0, sources.Count);
            AudioManager.Instance.PlaySFX(
                sources[idx],
                gameObject,
                UnityEngine.Random.Range(minVolume, maxVolume),
                UnityEngine.Random.Range(minPitch, maxPitch),
                0f);
            return idx;
        }
        else
        {
            Debug.LogError("Tried to play audioclip from an empty list");
            return -1;
        }
    }
}
