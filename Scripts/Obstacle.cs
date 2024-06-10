using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float rotateSpeed;
    private float currentSpeed;

    private float time;
    private float timer;

    private void Awake()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = new Material(Shader.Find("Standard"));
            meshRenderers[i].material.color = new Color(Random.value, Random.value, Random.value);
        }
        AdjestSpeed();
    }


    private void Update()
    {
        if (!GameController.Instance.IsPlaying)
            return;

        timer += Time.deltaTime;
        if (timer > time)
        {
            if (rotateSpeed != 0)
            {
                rotateSpeed = 0;
                time = 1f + Random.value * 2f;
            }
            else
                AdjestSpeed();
            timer = 0f;
        }
        currentSpeed = Mathf.Lerp(currentSpeed, rotateSpeed, Time.deltaTime * 2f);
        transform.Rotate(0, Time.deltaTime * currentSpeed, 0);
    }

    private void AdjestSpeed()
    {
        rotateSpeed = Random.Range(80, 120f);
        rotateSpeed = Random.Range(0, 2) == 0 ? -rotateSpeed : rotateSpeed;
        time = 3f + Random.value * 2f;
    }

  
}
