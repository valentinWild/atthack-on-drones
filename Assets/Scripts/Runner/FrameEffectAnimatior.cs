using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // Wichtig für den Zugriff auf Universal Rendering Pipeline-Effekte wie Vignette

public class FrameEffectAnimator : MonoBehaviour
{
    private Volume frameEffectVolume; // Referenz zur Volume-Komponente
    private Vignette vignetteEffect; // Referenz zum Vignette-Effekt

    private void Awake()
    {
        // Versuche, das Volume-Component vom GameObject zu bekommen
        frameEffectVolume = GetComponent<Volume>();

        if (frameEffectVolume == null)
        {
            Debug.LogError("Post-process Volume nicht gefunden auf diesem GameObject.");
            return;
        }

        // Prüfe, ob das Volume ein Profil hat und versuche, den Vignette-Effekt zu bekommen
        if (frameEffectVolume.profile != null && frameEffectVolume.profile.TryGet(out vignetteEffect))
        {
            vignetteEffect.intensity.value = 0f; // Initial den Effekt ausschalten
            Debug.Log("Vignette-Effekt erfolgreich geladen.");
        }
        else
        {
            Debug.LogError("Vignette-Effekt ist nicht im Profil enthalten oder Volume-Profil ist null.");
        }
    }

    // Methode zur Animation des Vignette-Effekts
    public void AnimateVignetteEffect(float intensity, float duration)
    {
        if (vignetteEffect != null)
        {
            StartCoroutine(AnimateVignette(intensity, duration));
        }
        else
        {
            Debug.LogError("Vignette-Effekt konnte nicht animiert werden, da er null ist.");
        }
    }

    // Coroutine zur Steuerung des Vignette-Effekts
    private IEnumerator AnimateVignette(float intensity, float duration)
    {
        vignetteEffect.intensity.value = intensity;
        yield return new WaitForSeconds(duration);
        vignetteEffect.intensity.value = 0f; // Effekt deaktivieren nach Ablauf der Zeit
    }
}