using UnityEngine;

public class LightTriggerGroup : MonoBehaviour
{
    [Header("Lights to turn ON when entering")]
    public Light[] lightsToTurnOn;

    [Header("Lights to turn OFF when entering")]
    public Light[] lightsToTurnOff;

    [Header("Trigger Settings")]
    public string playerTag = "Player";

    void Start()
    {
        // Make sure all lights start turned OFF
        foreach (var light in lightsToTurnOn)
        {
            if (light != null)
                light.enabled = false;
        }

        foreach (var light in lightsToTurnOff)
        {
            if (light != null)
                light.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            Debug.Log("[LightTriggerGroup] Player entered trigger zone.");

            // Turn ON selected lights
            foreach (var light in lightsToTurnOn)
            {
                if (light != null)
                    light.enabled = true;
            }

            // Turn OFF selected lights
            foreach (var light in lightsToTurnOff)
            {
                if (light != null)
                    light.enabled = false;
            }
        }
    }
}