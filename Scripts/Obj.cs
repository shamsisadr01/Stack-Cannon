using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj : MonoBehaviour
{
    private Vector3 targetScale;
    private bool isDestroy;
    private AudioSource source;

    private void Awake()
    {
        targetScale = transform.localScale;
        source = GetComponent<AudioSource>();
        GetComponent<MeshRenderer>().material.color = new Color(Random.value, Random.value, Random.value);
    }

    private void Update()
    {
        if (isDestroy)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, Time.deltaTime * 20f);
            if (transform.localScale == targetScale)
            {
                GameController.Instance.MoveTower(name == "Cube" ? targetScale.y : targetScale.y * 2f);
                Destroy(gameObject);
            }
        }
    }

    public void Destroy()
    {
        targetScale = transform.localScale * 1.35f;
        targetScale.y = transform.localScale.y;
        source.Play();
        isDestroy = true;
    }
}
