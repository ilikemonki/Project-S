using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject followObject;

    private void FixedUpdate()
    {
        transform.position = followObject.transform.position;
    }
    private void LateUpdate()
    {
        transform.position = followObject.transform.position;
        //transform.position = new Vector3(followObject.transform.position.x, followObject.transform.position.y, transform.position.z);
    }
}
