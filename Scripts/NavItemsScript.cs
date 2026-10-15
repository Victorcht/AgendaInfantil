using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//  Nav Items Script
//  Este script se le aplica a cada botón que tiene la barra inferior, asignandole una pestaña
//  y creando una función para su botón que activa y desactiva su pestaña
public class NavItemsScript : MonoBehaviour
{

    // Referencias
    public GameObject UI;
    public GameObject background;
    public GameObject paintedIcon;

    // Variables privadas
    private ThemeScript themeScript;
    private Image img;
    private UIFunctions uiFunctions;

    // Estados/pestañas de la interfaz
    public enum UITab
    {
        agenda,
        eventos,
        tareas,
        config,
        recompensas
    }
    public UITab uiTab;

    // Start
    void Start()
    {
        // Recibir componentes
        uiFunctions = UI.GetComponent<UIFunctions>();
        themeScript = UI.GetComponent<ThemeScript>();

        // Asignar función 
        GetComponent<Button>().onClick.AddListener(ClickEvent);

        // Imagen del botón
        img = paintedIcon.GetComponent<Image>();
    }

    // Update
    void Update()
    {
        img.color = themeScript.themeColor;

        // Si está activa su pestaña cambia el icono del botón
        if (uiFunctions.uiState == UIFunctions.UIState.agenda && uiTab == UITab.agenda)
        {
            background.SetActive(true);
            paintedIcon.SetActive(true);
        }
        else if(uiFunctions.uiState == UIFunctions.UIState.eventos && uiTab == UITab.eventos)
        {
            background.SetActive(true);
            paintedIcon.SetActive(true);
        }
        else if (uiFunctions.uiState == UIFunctions.UIState.config && uiTab == UITab.config)
        {
            background.SetActive(true);
            paintedIcon.SetActive(true);
        }
        else if (uiFunctions.uiState == UIFunctions.UIState.recompensas && uiTab == UITab.recompensas)
        {
            background.SetActive(true);
            paintedIcon.SetActive(true);
        }
        else if (uiFunctions.uiState == UIFunctions.UIState.tareas && uiTab == UITab.tareas)
        {
            background.SetActive(true);
            paintedIcon.SetActive(true);
        }
        else
        {
            background.SetActive(false);
            paintedIcon.SetActive(false);
        }
    }

    // Al hacer click en el botón
    public void ClickEvent()
    {
        // Cambiar la pestaña activa de la interfaz
        switch (uiTab)
        {
            case UITab.tareas:
                uiFunctions.uiState = UIFunctions.UIState.tareas;
                break;
            case UITab.recompensas:
                uiFunctions.uiState = UIFunctions.UIState.recompensas;
                break;
            case UITab.config:
                uiFunctions.uiState = UIFunctions.UIState.config;
                break;
            case UITab.eventos:
                uiFunctions.uiState = UIFunctions.UIState.eventos;
                break;
            case UITab.agenda:
                uiFunctions.uiState = UIFunctions.UIState.agenda;
                break;
        }
    }
}
