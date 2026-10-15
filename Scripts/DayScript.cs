using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//  DAY SCRIPT
//  Se le aplica a los objetos de los siete dias que aparecen en la agenda,
//  este identifica que dia de la semana es y pone su número las tareas que tiene
//  y mira si es un dia festivo para cambiar su color.
public class DayScript : MonoBehaviour
{
    // Variables
    public int dayNum;
    public bool isHoliday = false;
    public Color themeColor;
    public int taskCount;
    public int remainderCount;

    // Referencias
    public GameObject UI;
    public GameObject camara;
    public GameObject numObj;
    public GameObject nameObj;
    public GameObject whiteBackObject;
    public Sprite blackBack;
    public Sprite whiteBack;
    public GameObject taskImage;
    public GameObject remainderImage;
    public TextMeshProUGUI taskNumber;
    public TextMeshProUGUI RemainderNumber;

    // Estructura para almacenar los dias festivos que recibimos des una base de datos externa
    public class Holiday
    {
        public string date;
        public string localName;
        public string name;
        public string countryCode;
        public bool fixedValue; // "fixed" es palabra reservada en C#
        public bool global;
        public string[] counties;
        public int? launchYear; // Nullable int
        public string[] types;
    }

    public class HolidayArray
    {
        public Holiday[] holidays;
    }

    // Variables y referencias privadas
    private DateTime date;
    private TextMeshProUGUI text;
    private bool wasPaused = false;
    private Coroutine festivosCoroutine;
    private ThemeScript themeScript;
    private Animator anim;
    private UIFunctions uiScript;
    private bool loaded = false;
    private PlayerDataScript playerData;


    // Start
    void Start()
    {
        // Cargamos componentes
        themeScript = UI.GetComponent<ThemeScript>();
        text = numObj.GetComponent<TextMeshProUGUI>();
        anim = GetComponent<Animator>();
        uiScript = UI.GetComponent<UIFunctions>();
        playerData = camara.GetComponent<PlayerDataScript>();
        date = DateTime.Now;

        // Cargamos los dias
        reloadDays();
    }

    // Al activarse
    private void OnEnable()
    {
        // Loadad lo ponemos en false para que se recargue
        loaded = false;

        // Ejecutamos la función que muestra cuantas tareas tiene el dia de manera asyncrona
        StartCoroutine(ShowTaskEventCount());
    }

    // Función que detecta festivos
    IEnumerator Festivos()
    {
        // Url de la base de datos externa
        string url = $"https://date.nager.at/api/v3/PublicHolidays/{date.Year}/ES";

        // Recibimos los datos
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        // Si recibimos algo
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;

            // Extraemos los datos
            List<string> fechas = new List<string>();
            Regex regex = new Regex("\"date\"\\s*:\\s*\"(.*?)\"");
            MatchCollection matches = regex.Matches(json);

            foreach (Match match in matches)
            {
                fechas.Add(match.Groups[1].Value);
            }

            // Por cada fecha si alguna coincide con la de este dia lo marcamos como festivo
            foreach (var fecha in fechas)
            {
                isHoliday = false;
                int day = dayMath();
                if (fecha == $"{date.Year}-{date.Month.ToString("00")}-{day.ToString("00")}")
                    isHoliday = true;
            }

            // Pone el fondo oscuro por ser festivo
            if (isHoliday)
                GetComponent<Image>().sprite = blackBack;
            else
                GetComponent<Image>().sprite = whiteBack;
        }
        // Error si no puede recibir datos
        else
        {
            Debug.LogError("Error al obtener festivos: " + request.error);
        }
    }

    // Cálculo del día
    int dayMath()
    {
        int currentDay = date.Day;
        int day = 0;
        int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        // Según el dia que sea hoy calcula qué dia que es el objeto
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Monday: day = currentDay + (dayNum - 1); break;
            case DayOfWeek.Tuesday: day = currentDay + (dayNum - 2); break;
            case DayOfWeek.Wednesday: day = currentDay + (dayNum - 3); break;
            case DayOfWeek.Thursday: day = currentDay + (dayNum - 4); break;
            case DayOfWeek.Friday: day = currentDay + (dayNum - 5); break;
            case DayOfWeek.Saturday: day = currentDay + (dayNum - 6); break;
            case DayOfWeek.Sunday: day = currentDay + (dayNum - 7); break;
        }

        // Suma o resta el dia según el mes y tiene cuidado de los dias del mes
        if (day > daysInMonth)
            day -= daysInMonth;

        if(day <= 0)
            day = DateTime.DaysInMonth(date.Year, date.Month - 1) + day;

        return day;
    }

    // Recarga los dias
    void reloadDays()
    {
        // Pone el número del dia
        text.text = dayMath().ToString();

        // Función que activa si este es el dia de hoy
        todayAnimation();

        // Cancelar corrutina previa
        if (festivosCoroutine != null)
            StopCoroutine(festivosCoroutine);

        // Activamos la función de los festivos
        festivosCoroutine = StartCoroutine(Festivos());
    }

    // Se llama cuando la app entra o sale de pausa
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            wasPaused = true;
        }
        else if (wasPaused)
        {
            // Si vuelve de la pausa la app recarga los dias por si ha cambiado el dia durante la ejecución
            wasPaused = false;
            reloadDays();
        }
    }

    // Función asyncrona que pone las tareas y eventos que tiene el dia
    IEnumerator ShowTaskEventCount()
    {
        bool dayExist = false; // Si el dia existe en la base de datos
        date = DateTime.Now;

        // Espera a que la base de datos haya cargado los datos
        yield return new WaitUntil(() => playerData != null);

        // Para cada dia de la base de datos
        foreach (PlayerDataScript.DayData day in playerData.playerData)
        {
            // Busca el dia que coincida y mira cuantos eventos o tareas tiene para ponerselo en el objeto
            if (day.date == date.ToString("yyyy-MM") + "-" + dayMath().ToString("D2"))
            {
                taskCount = day.tasks.Count(t => t.done == false);
                remainderCount = day.remainders.Count;

                if (taskCount == 0)
                    taskImage.SetActive(false);
                else
                {
                    taskImage.SetActive(true);
                    taskNumber.text = taskCount.ToString();
                }

                if (remainderCount == 0)
                    remainderImage.SetActive(false);
                else
                {
                    remainderImage.SetActive(true);
                    RemainderNumber.text = remainderCount.ToString();
                }

                dayExist = true;
            }
        }

        // Si no existe el dia desactiva los iconos
        if (!dayExist)
        {
            remainderImage.SetActive(false);
            taskImage.SetActive(false);
        }
    }

    // Función que aplica la flecha blanca al dia activo
    void todayAnimation()
    {   
        if(dayMath() == date.Day)
        {
            Color color;
            if (UnityEngine.ColorUtility.TryParseHtmlString("#00000099", out color))
            {
                numObj.GetComponent<TextMeshProUGUI>().color = color;
            }
            nameObj.GetComponent<TextMeshProUGUI>().color = themeColor;

            whiteBackObject.SetActive(true);
        }
    }

    // Update
    private void Update()
    {
        // Cambia el color del nombre del dia según el tema y si es el dia de hoy
        if (dayMath() == date.Day)
        {
            themeColor = themeScript.themeColor;
            nameObj.GetComponent<TextMeshProUGUI>().color = themeColor;
        }

        // Carga la agenda con su animación
        if(!loaded && uiScript.uiState == UIFunctions.UIState.agenda)
        {
            StartCoroutine(LoadAgenda());
            loaded = true;
        }

        // Cierra la agenda con su animación
        if (loaded && uiScript.uiState != UIFunctions.UIState.agenda)
        {
            StartCoroutine(CloseAgenda());
            loaded = false;
        }
    }

    // Animación de abrir agenda
    IEnumerator LoadAgenda()
    {
        yield return new WaitForSeconds(0.1f * dayNum);
        anim.SetBool("dayStart", true);
    }

    // Animación de cerrar agenda
    IEnumerator CloseAgenda()
    {
        yield return new WaitForSeconds(0.03f * dayNum);
        anim.SetBool("dayStart", false);
        anim.SetBool("itsToday", false);
    }
}
