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
                flashLightLight.SetActive(true);
                //Play switch sound on 
                //start humming noise
                _flashLightActive = true;
            }else
            {
                flashLightLight.SetActive(false);
                //play switch sound off
                //stop humming noise
                _flashLightActive = false;
            }
        }
    }
}
