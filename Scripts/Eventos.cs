using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//  EVENTOS
//  Maneja la pestaña de eventos, el botón de crear, la navegación y la lista de eventos
//  La lista de eventos de rellena utilizando un PreFab de evento un objeto de interfaz preconstruido 
//  que representa un evento al que le asignamos su titulo, hora y fecha
public class Eventos : MonoBehaviour
{
    // Referencias
    public TextMeshProUGUI monthName;
    public TextMeshProUGUI yearNum;
    public GameObject UI;
    public GameObject eventPrefab;   // Prefab de evento, el objeto que copiamos y pegamos por cada evento que haya con sus valores
    public GameObject content;
    public GameObject camara;
    public GameObject arrowLeft;
    public GameObject arrowRight;
    public Button addButton;
    public GameObject newEvent;

    // Variables y referencias privadas
    private NewEventScript newEventScript;
    private ThemeScript themeScript;
    private DateTime date;
    private int dateOffset = 0;
    private TextMeshProUGUI dayTMP;
    private bool loaded = false;
    private PlayerDataScript playerData;
    private List<EventoScript> remainderList;
    private Image arrowLeftImg;
    private Image arrowRightImg;
    private bool newEventActive = false;

    // Start
    void Start()
    {
        // Recibimos los componentes necesarios
        themeScript = UI.GetComponent<ThemeScript>();
        playerData = camara.GetComponent<PlayerDataScript>();
        arrowLeftImg = arrowLeft.GetComponent<Image>();
        arrowRightImg = arrowRight.GetComponent<Image>();
        newEventScript = newEvent.GetComponent<NewEventScript>();

        // Asignamos la fecha a el dia de hoy
        date = DateTime.Now;

        // Ponemos el nombre del mes en español en la parte superior
        string monthNameString = date.ToString("MMMM", new CultureInfo("es-ES"));
        monthName.text = char.ToUpper(monthNameString[0]) + monthNameString.Substring(1);
        yearNum.text = date.ToString("yyyy");

        // Asignamos una función a los botones
        addButton.onClick.AddListener(AddEvent);
        arrowRight.GetComponent<Button>().onClick.AddListener(MoveDayRight);
        arrowLeft.GetComponent<Button>().onClick.AddListener(MoveDayLeft);
    }

    // Update
    void Update()
    {
        // Ponemos el color del tema a los objetos
        monthName.color = themeScript.themeColor;
        arrowLeftImg.color = themeScript.themeColor;
        arrowRightImg.color = themeScript.themeColor;

        // Si no hemos cargado los eventos
        if (!loaded)
        {
            // Variables para gestionar los dias
            string actualDay = date.ToString("yyyy-MM-dd");
            string monthNameString = date.ToString("MMMM", new CultureInfo("es-ES"));
            monthName.text = char.ToUpper(monthNameString[0]) + monthNameString.Substring(1);
            yearNum.text = date.ToString("yyyy");

            // Lista de objetos de evento
            remainderList = GetComponentsInChildren<EventoScript>().ToList();
            var remainderListOrdered = remainderList.Select(h => h.gameObject)
                                 .ToList();

            // Destruimos todos los objetos que habia antes para recargarlos
            foreach (var remainder in remainderListOrdered)
            {
                Destroy(remainder);
            }

            // Cargamos los recordatorios según los datos de cada dia del PlayerData
            foreach (PlayerDataScript.DayData day in playerData.playerData)
            {
                DateTime day1 = DateTime.Parse(day.date);
                DateTime day2 = DateTime.Parse(actualDay);

                string diaName = day1.ToString("dddd", new CultureInfo("es-ES"));
                diaName = char.ToUpper(diaName[0]) + diaName.Substring(1);

                // Si el dia esta en el mes y año del menú
                if (day1.Year == day2.Year && day1.Month == day2.Month)
                {
                    // Creamos los remainders
                    foreach (PlayerDataScript.reminder remainder in day.remainders)
                    {
                        // Instanciamos el objeto y le asignamos su nombre, fecha y hora
                        GameObject obj = Instantiate(eventPrefab, content.transform);
                        var objScipt = obj.GetComponent<EventoScript>();

                        objScipt.remainderName = remainder.name;
                        objScipt.remainderFullDate = day.date;
                        objScipt.remainderDate = day1.ToString("dd").TrimStart('0') + " " + diaName;
                        objScipt.remainderHour = remainder.time;
                    }
                }
            }

            // Ordenamos los eventos según el dia
            RemainderReorder();
            loaded = true;
        }

        // Si estamos creando un evento y vemos que el pop up da el okey
        if (newEventActive && newEventScript.confirmed)
        {
            // Creamos un nuevo evento
            EventCreated(newEventScript.nombre, newEventScript.dia, newEventScript.hora, newEventScript.minutos);
            newEventActive = false;
        }        
    }

    // Función que ordena los eventos según su dia del mes
    void RemainderReorder()
    {
        remainderList = GetComponentsInChildren<EventoScript>().ToList();
        var remainderListOrdered = remainderList
            .OrderBy(e =>
            {
                if (!string.IsNullOrEmpty(e.remainderFullDate))
                {
                    string[] partes = e.remainderFullDate.Split('-');
                    if (partes.Length == 3 && int.TryParse(partes[2], out int dia))
                        return dia;
                }
                return int.MaxValue;
            })
            .Select(e => e.transform)
            .ToList();

        for (int i = 0; i < remainderListOrdered.Count; i++)
        {
            remainderListOrdered[i].SetSiblingIndex(i);
        }
    }

    // Función que carga el pop up de crear un evento
    void AddEvent()
    {
        newEvent.SetActive(true);
        newEvent.GetComponent<NewEventScript>().maxDay = DateTime.DaysInMonth(date.Year, date.Month);
        newEventActive = true;
    }

    // Función que crea el evento nuevo y recibe los datos necesarios que vienen del formulario del pop up
    void EventCreated(String nombre, String dia, String hora, String minutos)
    {
        string actualDay = date.ToString("yyyy-MM") + "-" + dia;
        bool sameRemainder = false;

        // Mira que no exista un mismo evento con el mismo nombre
        foreach (var day in playerData.playerData)
        {
            if (day.date == actualDay)
            {
                foreach (var remainder in day.remainders)
                {
                    if (remainder.name == nombre)
                    {
                        sameRemainder = true;
                    }
                }
            }
        }

        // Si no existe ya crea un nuevo objeto de evento con los datos
        if (!sameRemainder)
        {
            DateTime parsedDay = DateTime.Parse(actualDay);
            string diaName = parsedDay.ToString("dddd", new CultureInfo("es-ES"));
            diaName = char.ToUpper(diaName[0]) + diaName.Substring(1);

            GameObject obj = Instantiate(eventPrefab, content.transform);
            var objScipt = obj.GetComponent<EventoScript>();

            objScipt.remainderName = nombre;
            objScipt.remainderDate = dia.TrimStart('0') + " " + diaName;
            objScipt.remainderFullDate = actualDay;
            objScipt.remainderHour = hora + ":" + minutos;

            SaveEventData(nombre, hora + ":" + minutos, dia);

            // Reordena los eventos junto al nuevo
            RemainderReorder();
            newEvent.SetActive(false);
        }
    }

    // Guarda los datos del evento en la base de datos
    void SaveEventData(String remainderName, String remainderHour, String dia)
    {
        string actualDay = date.ToString("yyyy-MM") + "-" + dia;
        bool dayExist = false;

        // Busca el dia y si exitse le añade un evento
        foreach (PlayerDataScript.DayData day in playerData.playerData)
        {
            if (day.date == actualDay)
            {
                day.remainders.Add(new PlayerDataScript.reminder { name = remainderName, time = remainderHour });
                dayExist = true;
            }
        }

        // Si el dia no existe en la base de datos crea el dia
        if (dayExist == false)
        {
            PlayerDataScript.DayData newDay = new PlayerDataScript.DayData();
            newDay.date = actualDay;
            newDay.remainders.Add(new PlayerDataScript.reminder { name = remainderName, time = remainderHour });
            playerData.playerData.Add(newDay);
        }

        // Guarda los datos en el documento
        playerData.SaveData(playerData.playerData);
    }

    // Mueve un mes para delante al clicar la flecha ->
    void MoveDayRight()
    {
        dateOffset += 1;

        date = DateTime.Now.AddMonths(dateOffset);

        loaded = false;
    }

    // Mueve un mes para atras al clicar la flecha <-
    void MoveDayLeft()
    {
        dateOffset -= 1;

        date = DateTime.Now.AddMonths(dateOffset);

        loaded = false;
    }
}
