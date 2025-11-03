using UnityEditor.Rendering;
using UnityEngine;

public class FlashLight : MonoBehaviour
{

    [SerializeField] GameObject flashLightLight;

    private bool _flashLightActive = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flashLightLight.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(_flashLightActive == false)
            {
                _flashLightActive = true;
            }else
            {
                _flashLightActive = false;
            }
        }
    }
}
