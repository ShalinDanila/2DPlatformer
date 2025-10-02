using System.Collections;
using UnityEngine;

public class RespawnSystem : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPoint;

    public void Respawn(GameObject instigator)
    {
        // Destroy current player (gameObject)
        if (instigator != null)
        {
            Destroy(instigator);
        }

        // Create new instance of the player
        if (playerPrefab != null && spawnPoint != null)
        {
            GameObject newPlayer = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
            EnableAllComponents(newPlayer);
        }
    }

    private void EnableAllComponents(GameObject obj)
    {
        foreach (var comp in obj.GetComponentsInChildren<Component>(true))
        {
            var type = comp.GetType();
            var enableProp = type.GetProperty("enabled");
            if (enableProp != null && enableProp.PropertyType == typeof(bool))
            {
                enableProp.SetValue(comp, true);
            }
        }
    }
}