using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class EventSystemSetup : MonoBehaviour
{
    void Awake()
    {
        SetupEventSystem();
    }

    public static void SetupEventSystem()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            GameObject obj = new GameObject("EventSystem");
            eventSystem = obj.AddComponent<EventSystem>();
            obj.AddComponent<InputSystemUIInputModule>();
        }
        else
        {
            var standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (standaloneInputModule != null)
            {
                DestroyImmediate(standaloneInputModule);
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
            else if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }
            
        }
    }
}
