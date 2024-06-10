using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private bool isLose;

    private void Update()
    {
        if (!GameController.Instance.IsPlaying)
            return;

        if (!isLose)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Camera.main.transform.position, Time.deltaTime * 100f);
            if(transform.position == Camera.main.transform.position)
            {
                GameController.Instance.Lose();
                Destroy(gameObject);
            }
        }


        if (transform.position.z > 5)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Obj obj = other.GetComponent<Obj>();
        if (obj != null)
        {
            obj.Destroy();
            Destroy(gameObject);
        }
        else
        {
            isLose = true;
        }
    }
}
