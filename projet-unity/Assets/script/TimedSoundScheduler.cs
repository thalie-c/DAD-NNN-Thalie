using UnityEngine;

[System.Serializable]
public class TimedSound
{
    public string name;
    public AudioSource source;          // porte le clip et la position dans l'espace
    [Range(0, 23)] public int hour;
    [Range(0, 59)] public int minute;

    [Header("Animation (optionnel)")]
    public Animator animator;           // l'Animator de la source sonore
    public string stateName;            // nom exact de l'état d'animation dans l'Animator
}

// Joue chaque son pendant sa "fenêtre" : de l'heure de début jusqu'à la fin du clip.
// Si le spectateur arrive en cours d'événement, le son démarre au bon endroit.
// L'animation est pilotée par l'horloge : sa position dépend de l'heure, pas du lancement du jeu.
public class TimedSoundScheduler : MonoBehaviour
{
    public TimedSound[] sounds;

    void Start()
    {
        foreach (var s in sounds)
        {
            if (s.animator == null) continue;

            // L'animation n'avance plus toute seule : c'est l'horloge qui la place
            s.animator.speed = 0f;

            if (!string.IsNullOrEmpty(s.stateName) &&
                !s.animator.HasState(0, Animator.StringToHash(s.stateName)))
            {
                Debug.LogWarning("État d'animation introuvable pour " + s.name + " : " + s.stateName);
            }
        }
    }

    void Update()
    {
        if (RealTimeClock.Instance == null) return;
        float now = RealTimeClock.Instance.SecondsOfDay;

        foreach (var s in sounds)
        {
            if (s.source == null || s.source.clip == null) continue;

            float start = s.hour * 3600f + s.minute * 60f;
            float length = s.source.clip.length;
            bool inWindow = now >= start && now < start + length;

            if (Time.frameCount % 60 == 0)
                Debug.Log(s.name + " | heure : " + now + " | début : " + start + " | durée : " + length);

            // Animation : avant l'événement = position de départ, après = position d'arrivée
            if (s.animator != null && !string.IsNullOrEmpty(s.stateName))
            {
                float t = Mathf.Clamp01((now - start) / length);
                s.animator.Play(s.stateName, 0, t);
            }

            if (inWindow && !s.source.isPlaying)
            {
                Debug.Log("Démarrage de " + s.name);
                s.source.time = Mathf.Min(now - start, length - 0.05f);
                s.source.Play();
            }
            else if (!inWindow && s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
    }
}

