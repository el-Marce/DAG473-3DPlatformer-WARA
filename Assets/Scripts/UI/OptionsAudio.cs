using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsAudio : MonoBehaviour
{
    public AudioMixer mixer; // Mezclador asignado en inspector
    public Slider musicSlider;
    public Slider sfxSlider;

    private const string MUSIC_KEY = "musicVolume";
    private const string SFX_KEY = "sfxVolume";

    private void Start()
    {
        // Cargar valores guardados (0..1)
        float musicVal = PlayerPrefs.HasKey(MUSIC_KEY) ? PlayerPrefs.GetFloat(MUSIC_KEY) : 1f;
        float sfxVal = PlayerPrefs.HasKey(SFX_KEY) ? PlayerPrefs.GetFloat(SFX_KEY) : 1f;

        // Asignar sliders
        if (musicSlider != null) musicSlider.value = musicVal;
        if (sfxSlider != null) sfxSlider.value = sfxVal;

        // Aplicar al mixer
        SetMusicVolume(musicVal);
        SetSFXVolume(sfxVal);

        // Añadir listeners a sliders para actualizar en tiempo real
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float linear)
    {
        float dB = LinearToDecibel(linear);
        mixer.SetFloat("MusicVolume", dB);
        PlayerPrefs.SetFloat(MUSIC_KEY, linear);
    }

    public void SetSFXVolume(float linear)
    {
        float dB = LinearToDecibel(linear);
        mixer.SetFloat("SFXVolume", dB);
        PlayerPrefs.SetFloat(SFX_KEY, linear);
    }

    private float LinearToDecibel(float linear)
    {
        if (linear <= 0.0001f) return -80f; // evitar log(0)
        return Mathf.Log10(linear) * 20f;
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
