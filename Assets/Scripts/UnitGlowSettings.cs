using UnityEngine;

[System.Serializable]
public class UnitGlowSettings : MonoBehaviour
{
    [Header("Glow Effect Settings")]
    [SerializeField] private bool enableGlow = true;
    [SerializeField] private float glowIntensity = 2.0f;
    [SerializeField] private float glowScale = 1.5f;
    [SerializeField] private float glowTransparency = 0.6f;
    [SerializeField] private bool enableTextGlow = true;
    [SerializeField] private float textGlowIntensity = 1.5f;

    [Header("Animation Settings")]
    [SerializeField] private bool enablePulseAnimation = false;
    [SerializeField] private float pulseSpeed = 2.0f;
    [SerializeField] private float pulseMinScale = 1.3f;
    [SerializeField] private float pulseMaxScale = 1.7f;

    // Propriétés publiques pour accéder aux paramètres
    public bool EnableGlow => enableGlow;
    public float GlowIntensity => glowIntensity;
    public float GlowScale => glowScale;
    public float GlowTransparency => glowTransparency;
    public bool EnableTextGlow => enableTextGlow;
    public float TextGlowIntensity => textGlowIntensity;
    public bool EnablePulseAnimation => enablePulseAnimation;
    public float PulseSpeed => pulseSpeed;
    public float PulseMinScale => pulseMinScale;
    public float PulseMaxScale => pulseMaxScale;

    // Instance singleton pour accéder aux paramètres depuis UnitManager
    public static UnitGlowSettings Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}