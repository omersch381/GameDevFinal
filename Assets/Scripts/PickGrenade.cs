using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickGrenade : MonoBehaviour
{
    public GameObject grenadeInDrawer;
    public GameObject grenadeInHand;

     private void OnMouseDown()
    {
        grenadeInDrawer.SetActive(false);
        grenadeInHand.SetActive(true);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
