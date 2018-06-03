using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderController : MonoBehaviour {

    private BoxCollider[] colliders;
    private Rigidbody rb = null;

    private void Start()
    {
        colliders = GetComponents<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update () {
        foreach(BoxCollider col in colliders)
        {
            col.enabled = !GameController.InventoryIsOpen;
        }

        if(rb != null)
        {
            rb.isKinematic = GameController.InventoryIsOpen;
        }
	}
}
