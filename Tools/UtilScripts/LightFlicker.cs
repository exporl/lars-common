﻿using UnityEngine;
using System.Collections;
using DG.Tweening;

public enum enColorchannels
{
    all = 0,
    red = 1,
    blue = 2,
    green = 3
}
public enum enWaveFunctions
{
    sinus = 0,
    triangle = 1,
    square = 2,
    sawtooth = 3,
    inverted_saw = 4,
    custom = 5
}
public class LightFlicker : MonoBehaviour
{

    public enColorchannels colorChannel = enColorchannels.all;
    public enWaveFunctions waveFunction = enWaveFunctions.sinus;
    public float offset = 0.0f; // constant offset
    public float amplitude = 1.0f; // amplitude of the wave
    public float phase = 0.0f; // start point inside on wave cycle
    public float frequency = 0.5f; // cycle frequency per second
    public bool affectsIntensity = true;

    // Keep a copy of the original values
    private Color originalColor;
    private float originalIntensity;


    // Use this for initialization
    void Start()
    {
        originalColor = GetComponent<Light>().color;
        originalIntensity = GetComponent<Light>().intensity;

        transform.DOMoveX(.02f, .05f).SetRelative().SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        Light light = GetComponent<Light>();
        if (affectsIntensity)
            light.intensity = originalIntensity * EvalWave();

        Color o = originalColor;
        Color c = GetComponent<Light>().color;

        if (colorChannel == enColorchannels.all)
            light.color = originalColor * EvalWave();
        else
        if (colorChannel == enColorchannels.red)
            light.color = new Color(o.r * EvalWave(), c.g, c.b, c.a);
        else
        if (colorChannel == enColorchannels.green)
            light.color = new Color(c.r, o.g * EvalWave(), c.b, c.a);
        else // blue       
            light.color = new Color(c.r, c.g, o.b * EvalWave(), c.a);
    }

    private float EvalWave()
    {
        float x = (Time.time + phase) * frequency;
        float y;
        x = x - Mathf.Floor(x); // normalized value (0..1)
        if (waveFunction == enWaveFunctions.sinus)
        {
            y = Mathf.Sin(x * 2f * Mathf.PI);
        }
        else if (waveFunction == enWaveFunctions.triangle)
        {
            if (x < 0.5f)
                y = 4.0f * x - 1.0f;
            else
                y = -4.0f * x + 3.0f;
        }
        else if (waveFunction == enWaveFunctions.square)
        {
            if (x < 0.5f)
                y = 1.0f;
            else
                y = -1.0f;
        }
        else if (waveFunction == enWaveFunctions.sawtooth)
        {
            y = x;
        }
        else if (waveFunction == enWaveFunctions.inverted_saw)
        {
            y = 1.0f - x;
        }
        else if (waveFunction == enWaveFunctions.custom)
        {
            float y1 = Mathf.Sin(x * 2f * Mathf.PI);
            float y2 = 1f - (Random.value * 2f);
            y = (y1 + y2) / 2;
        }
        else
        {
            y = 1.0f;

        }
        return (y * amplitude) + offset;

    }

}
