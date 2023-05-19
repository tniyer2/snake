using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

/*
 * MenuManager manages the level menu.
 * Implements the Singleton pattern, but not thread safe.
 */
public class MenuManager : MonoBehaviour
{
    public static MenuManager self;

    public bool IsOpen { get; private set; }
    public bool Sliding { get; private set; }

    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject emptyButton;
    [SerializeField]
    Vector3 onScreenPosition;
    [SerializeField]
    Vector3 offScreenPosition;

    [SerializeField]
    private float slideTime = 3;
    // Number of times the coroutine slide() is updated per second.
    [SerializeField]
    private int slideFrequency = 25;

    private RectTransform menuTransform;
    private AudioSource audioSource;

    private void Awake()
    {
        // Initializing Singleton
        if (self == null)
        {
            self = this;
        }
        else if (self != this)
        {
            Destroy(this);
            return;
        }

        menuTransform = menu.GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
    }

    // Slides menu into the screen.
    public void open()
    {
        menu.SetActive(true);

        audioSource.Play();
        StartCoroutine(slide(offScreenPosition, onScreenPosition, true));
    }

    // Slides menu off the screen.
    // Returns the time it will take to close the menu
    public float close()
    {
        EventSystem.current.SetSelectedGameObject(null);
        StartCoroutine(slide(onScreenPosition, offScreenPosition, false));
        return slideTime;
    }

    // Reloads Scene
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Quits Game
    public void quit()
    {
        Application.Quit();
    }

    // Slides menu from start to end.
    private IEnumerator slide(Vector3 start, Vector3 end, bool IsOpen)
    {
        Sliding = true;
        menuTransform.anchoredPosition = start;

        int b = (int)(slideTime * slideFrequency);
        for (int a = 1; a < b; a++)
        {
            yield return new WaitForSeconds(1f / slideFrequency);
            menuTransform.anchoredPosition = Vector3.Lerp(start, end, (float)a / b);
        }

        menuTransform.anchoredPosition = end;
        Sliding = false;
        this.IsOpen = IsOpen;
        if(IsOpen)
        {
            EventSystem.current.SetSelectedGameObject(emptyButton);
        }
        else
        {
            menu.SetActive(false);
        }

        yield return null;
    }
}
