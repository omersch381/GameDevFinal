using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D3Motion : MonoBehaviour
{
    private Animator anim;
    private bool isDrawerClosed;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.transform.parent.GetComponent<Animator>();
        isDrawerClosed = false;
        anim.SetBool("IsD3Open", isDrawerClosed);
    }

    private void OnMouseDown()
    {
        anim.SetBool("IsD3Open", isDrawerClosed);
 
        isDrawerClosed = !isDrawerClosed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
