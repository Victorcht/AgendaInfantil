using TMPro;
using UnityEngine;
using UnityEngine.UI;

//  EVENTO SCRIPT
//  Este script va en el objeto de evento y recibe sus datos y los asigna a los objetos de interfaz que lleva dentro
public class EventoScript : MonoBehaviour
{

    // Referencias a los objetos de UI que tiene dentro
    public string remainderName;
    public string remainderDate;
    public string remainderFullDate;
    public string remainderHour;

    // Variables del evento
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI hourText;

    // Update
    void Start()
    {
        // Asigna los valores
        nameText.text = remainderName;
        dateText.text = remainderDate;
        hourText.text = remainderHour;
    }
}
