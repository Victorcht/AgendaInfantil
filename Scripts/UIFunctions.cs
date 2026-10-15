using System.Collections;
using UnityEngine;

// UIFUNCIONS
// Funciones que maneja la interfaz para gestionar las pestañas
public class UIFunctions : MonoBehaviour
{
    // Lista de estados
    public enum UIState
    {
        load,
        agenda,
        eventos,
        tareas,
        config,
        recompensas
    }

    // Variables
    public UIState uiState;
    private UIState lastActiveTab;  // Sirve para saber cual fue el último estado y cerrar la pestaña

    // Referencias de objetos
    public GameObject agendaObj;
    public GameObject tareasObj;
    public GameObject eventosObj;
    public GameObject configObj;

    // Start
    void Start()
    {
        // Activamos la agenda por defecto
        uiState = UIState.agenda;
        lastActiveTab = UIState.agenda;
        agendaObj.SetActive(true);
    }

    // Update
    private void Update()
    {
        // Recargamos las pestañas
        ReloadTabs();
    }

    // Función que mira si ha habido cambios en la pestaña que queremos abrir y carga las ventanas
    void ReloadTabs()
    {
        // Comparamos con el anterior para detectar cambios
        if (lastActiveTab != uiState)
        {
            // Si hay cambios desactivamos la pestaña anterior
            switch (lastActiveTab)
            {
                case UIState.agenda:
                    StartCoroutine(CloseTab(agendaObj));
                    break;
                case UIState.tareas:
                    StartCoroutine(CloseTab(tareasObj));
                    break;
                case UIState.eventos:
                    StartCoroutine(CloseTab(eventosObj));
                    break;
                case UIState.config:
                    StartCoroutine(CloseTab(configObj));
                    break;
            }

            // Activamos la pestaña que queremos ver
            switch (uiState)
            {
                case UIState.agenda:
                    StartCoroutine(OpenTab(agendaObj));
                    break;
                case UIState.tareas:
                    StartCoroutine(OpenTab(tareasObj));
                    break;
                case UIState.eventos:
                    StartCoroutine(OpenTab(eventosObj));
                    break;
                case UIState.config:
                    StartCoroutine(OpenTab(configObj));
                    break;
            }

            // Guardamos la pestaña actual
            lastActiveTab = uiState;
        }
    }

    // Función que espera y cierra la pestaña
    IEnumerator CloseTab(GameObject tab)
    {
        Animator anim;
        if(tab != agendaObj)
        {
            anim = tab.GetComponent<Animator>();
            anim.SetBool("close", true);
        }

        yield return new WaitForSeconds(0.5f);
        tab.SetActive(false);
    }

    // Función que espera y abre la pestaña
    IEnumerator OpenTab(GameObject tab)
    {
        yield return new WaitForSeconds(0.5f);
        tab.SetActive(true);
    }
}
