using System;
using UnityEngine;

// Fournit l'heure de la journée, en secondes depuis minuit.
// Mode normal : lit l'horloge de l'ordinateur.
// Mode test : part d'une heure de ton choix et avance à vitesse normale.
public class RealTimeClock : MonoBehaviour
{
    [Header("Mode test")]
    public bool useSimulatedTime = false;
    [Range(0, 23)] public int simulatedStartHour = 17;
    [Range(0, 59)] public int simulatedStartMinute = 59;
    [Range(0, 59)] public int simulatedStartSecond = 30;

    public static RealTimeClock Instance { get; private set; }

    // Secondes écoulées depuis minuit (0 à 86400)
    public float SecondsOfDay { get; private set; }

    // Déclenché quand l'horloge repasse minuit : la boucle se réinitialise
    public event Action OnMidnight;

    float simulatedSeconds;

    void Awake()
    {
        Instance = this;
        simulatedSeconds = simulatedStartHour * 3600f
                         + simulatedStartMinute * 60f
                         + simulatedStartSecond;
        SecondsOfDay = ReadSeconds();
    }

    void Update()
    {
        if (useSimulatedTime) simulatedSeconds += Time.deltaTime;

        float now = ReadSeconds();
        if (now < SecondsOfDay) OnMidnight?.Invoke();
        SecondsOfDay = now;
    }

    float ReadSeconds()
    {
        if (useSimulatedTime) return simulatedSeconds % 86400f;
        return (float)DateTime.Now.TimeOfDay.TotalSeconds;
    }
}
