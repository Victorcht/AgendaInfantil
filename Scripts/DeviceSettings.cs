using UnityEngine;

//  DEVICE SETTINGS
//  Configuraciones de FPS y cosas del dispostivo
public class DeviceSettings : MonoBehaviour
{
    // Start
    void Start()
    {
        // Sincroniza con la tasa de refresco de pantalla
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Update
    void Update()
    {
        
    }
}
