using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickPistol : MonoBehaviour
{

    public GameObject pistolInDrawer;
    public GameObject pistolInHand;
    public GameObject crossHair;

    private void OnMouseDown()
    {
        pistolInDrawer.SetActive(false);
        pistolInHand.SetActive(true);
        crossHair.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
