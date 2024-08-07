using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int value = 30;

    void Update()
    {
        transform.Rotate(Vector3.up * 30 * Time.deltaTime);
    }
}
