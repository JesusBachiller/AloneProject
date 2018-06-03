using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    private RaycastHit _hit;

    private GameObject _slideExtraction;

    private float _timeHoldMouse;
    private bool _sliderIsActive;

    // Use this for initialization
    void Start()
    {
        _slideExtraction = GameObject.FindGameObjectWithTag("SlideExtraction");
        
        ResetSlideExtraction();
        
    }
    /// <summary>
    /// Coloca el player en una pasicionrandom
    /// </summary>
    public bool InitPosition()
    {
        
        RaycastHit raycastHit;

        int ratio = 100;
        float xRandom = UnityEngine.Random.Range(-400f, 400f);
        float yRandom = UnityEngine.Random.Range(-400f, 400f);
        while (xRandom * xRandom + yRandom * yRandom < ratio * ratio)
        {
            xRandom = UnityEngine.Random.Range(-400f, 400f);
            yRandom = UnityEngine.Random.Range(-400f, 400f);
        }
        Vector3 arroundPosition = new Vector3(5325f + xRandom, 170, 6467f + yRandom);
        //situa en un plano por encima del mapa objetos py hace ray cast para conocer la altura de ese punto.
        if (Physics.Raycast(arroundPosition + Vector3.up * 300, -Vector3.up, out raycastHit))
        {
            if (raycastHit.transform.name == "Chunk")
            {
                transform.position = raycastHit.point + Vector3.up * 5;
                transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));

                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
                return false;
        }

    }


    /// <summary>
    /// Resetea el slider  de extracción a 0 y lo oculta.
    /// </summary>
    public void ResetSlideExtraction()
    {
        _timeHoldMouse = 0f;
        _slideExtraction.GetComponent<Slider>().value = 0f;
        _slideExtraction.SetActive(false);

        _sliderIsActive = false;

    }

    /// <summary>
    /// Devuelve TRUE si ha terminado un "ciclo" y ha recolectado. Si, está en proceso devuelve FALSE
    /// </summary>
    /// <param name="collectionTime"></param>
    /// <returns></returns>
    public bool Collect(float collectionTime)
    {
        if (_timeHoldMouse > collectionTime)
        {
            _timeHoldMouse = 0f;

            return true;

        }
        else
        {
            _timeHoldMouse += Time.deltaTime;

            _slideExtraction.GetComponent<Slider>().value = _timeHoldMouse * 100 / collectionTime;

            return false;
        }
    }

    public void ShowSlideExtraction()
    {
        _slideExtraction.SetActive(true);

        _sliderIsActive = true;
    }

    public bool SliderIsActive
    {
        get
        {
            return _sliderIsActive;
        }
    }
}
