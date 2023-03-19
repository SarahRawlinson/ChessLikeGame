using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float movementSpeed = 2f;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.right);

        if (Input.GetKey(KeyCode.E))
            transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.left);
        
        if (Input.GetKey(KeyCode.Z))
            transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.forward);

        if (Input.GetKey(KeyCode.C))
            transform.Rotate(Time.deltaTime * rotationSpeed * Vector3.back);

        if (Input.GetKey(KeyCode.D) && transform.position.x < 5)
            transform.position += Time.deltaTime * movementSpeed * transform.right;

        if (Input.GetKey(KeyCode.A) && transform.position.x > -5)
            transform.position += Time.deltaTime * movementSpeed * -transform.right;

        if (Input.GetKey(KeyCode.W) && transform.position.z < 5)
            transform.position += Time.deltaTime * movementSpeed * transform.forward;

        if (Input.GetKey(KeyCode.S) && transform.position.z > -5)
            transform.position += Time.deltaTime * movementSpeed * -transform.forward;

        if (Input.GetKey(KeyCode.Space) && transform.position.y < 5)
            transform.position += Time.deltaTime * movementSpeed * transform.up;

        if (Input.GetKey(KeyCode.LeftControl) && transform.position.y > -5)
            transform.position += Time.deltaTime * movementSpeed * -transform.up;

        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < 5)
            transform.position += Time.deltaTime * movementSpeed * Vector3.right;

        if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > -5)
            transform.position += Time.deltaTime * movementSpeed * Vector3.left;

        if (Input.GetKey(KeyCode.UpArrow) && transform.position.z < 5)
            transform.position += Time.deltaTime * movementSpeed * Vector3.forward;

        if (Input.GetKey(KeyCode.DownArrow) && transform.position.z > -5)
            transform.position += Time.deltaTime * movementSpeed * Vector3.back;

        if (Input.GetKey(KeyCode.Home) && transform.position.y < 5)
            transform.position += Time.deltaTime * movementSpeed * new Vector3(0, 0.5f, 0);

        if (Input.GetKey(KeyCode.End) && transform.position.y > -5)
            transform.position += Time.deltaTime * movementSpeed * new Vector3(0, -0.5f, 0);

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            transform.position = new Vector3(0, 0, 0);
            //transform.rotation = ;
        }
            
    }
    
}
