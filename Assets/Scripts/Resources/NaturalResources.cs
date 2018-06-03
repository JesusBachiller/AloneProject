using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalResources : MonoBehaviour {

    private float _collectionTime; //Tiempo en recoger una vez
    [SerializeField]
    private int _numberOfExtractions; //Veces que se puede extraer del item.

    private Player _player;

    [SerializeField]
    private GameObject[] _derivedObjects;
    private int[] _idDerivedObject;

    /// <summary>
    /// Inicializador de los recursos naturales
    /// </summary>
    /// <param name="collectionTime"></param>
    /// <param name="numberOfExtraction"></param>
    /// <param name="player"></param>
    /// <param name="derivedObjects"></param>
    /// <param name="idDerivedObject"></param>
    public void InitNaturalResources(float collectionTime, int numberOfExtraction, Player player, GameObject[] derivedObjects, int[] idDerivedObject)
    {
        _collectionTime = collectionTime;
        _numberOfExtractions = numberOfExtraction;

        _player = player;

        _derivedObjects = derivedObjects;
        _idDerivedObject = idDerivedObject;
    }

    private void OnMouseDown()
    {
        //Si esta cerca
        if (Vector3.Distance(transform.position, _player.transform.position) < ConstantAssistant.MAX_DIST_TO_SELECT_OBJECT)
        {
            _player.ShowSlideExtraction();
            //empieza anim de la mano
            _player.GetComponentInChildren<Animator>().SetBool("Recollect", true);
        }
    }

    private void OnMouseDrag()
    {
        if (_player.SliderIsActive)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) < ConstantAssistant.MAX_DIST_TO_SELECT_OBJECT)
            {
                //Si se ha recolectado el recurso
                if (_player.Collect(_collectionTime))
                {
                    NumberOfExtractions--;

                    dropDerivedObjects();
                    _player.GetComponentInChildren<Animator>().SetBool("Recollect", false);
                }
                else
                {
                    _player.GetComponentInChildren<Animator>().SetBool("Recollect", true);
                }
            }
            else
            {
                _player.ResetSlideExtraction();
            }
        }
    }

    private void OnMouseUp()
    {
        _player.ResetSlideExtraction();
        _player.GetComponentInChildren<Animator>().SetBool("Recollect", false);

    }

    private void OnMouseExit()
    {
        _player.GetComponentInChildren<Animator>().SetBool("Recollect", false);
        _player.ResetSlideExtraction();
    }
    
    /// <summary>
    /// Suelta en el suelo el objeto que se deriba de este recurso natural
    /// </summary>
    private void dropDerivedObjects()
    {
        int count = 0;
        foreach(GameObject derived in _derivedObjects)
        {
            Vector3 playerDirect = _player.transform.position + new Vector3(UnityEngine.Random.Range(-5, 5), 0f, UnityEngine.Random.Range(-5, 5)) - transform.position;

            GameObject inst =  Instantiate(derived,
                                           transform.position + new Vector3(playerDirect.x / UnityEngine.Random.Range(1.7f, 2.5f), 2f, playerDirect.z / UnityEngine.Random.Range(1.7f, 2.5f)),
                                           Quaternion.identity, transform.parent);

            inst.GetComponent<NoNaturalResource>().InitNoNaturalResource(_idDerivedObject[count], _player, derived, false, ConstantAssistant.MODE_INVENTORY_PLAYER);

            count++;
            
        }
    }

    //GET AND SET
    /// <summary>
    /// Tiempo que tarda en recoger un único recurso del objeto.
    /// </summary>
    public float CollectionTime
    {
        get
        {
            return _collectionTime;
        }
    }

    /// <summary>
    /// Número de veces que se puede extraer recursos del objeto.
    /// </summary>
    public int NumberOfExtractions
    {
        get
        {
            return _numberOfExtractions;
        }
        set
        {
            _numberOfExtractions = value;

            if (_numberOfExtractions <= 0)
            {
                Destroy(this.gameObject);
                _player.ResetSlideExtraction();
            }
        }
    }
}
