using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCollision : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.impulse);
        Debug.Log(collision.relativeVelocity.magnitude);
        Debug.Log(collision.contacts.Length);
    }
}
