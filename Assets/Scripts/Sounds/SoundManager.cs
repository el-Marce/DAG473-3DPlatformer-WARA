using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Eventos principales")]
    [SerializeField] private EventReference recogerMoneda;
    [SerializeField] private EventReference recogerOveja;
    [SerializeField] private EventReference todasLasOvejas;
    [SerializeField] private EventReference pasos;
    [SerializeField] private EventReference salto;
    [SerializeField] private EventReference dano;
    [SerializeField] private EventReference muerte;

    private EventInstance reproducirPasos;

    private void Awake()
    {
        // Patrón Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }
    private void Start()
    {
        reproducirPasos = RuntimeManager.CreateInstance(pasos);
    }
    public void reproducirMoneda()
    {
        if (recogerMoneda.IsNull) return;
        RuntimeManager.PlayOneShot(recogerMoneda);
    }
    public void reproducirRecogerOveja()
    {
        if (recogerOveja.IsNull) return;
        if (GameManager.instance.ovejasFaltantes)
        {
            RuntimeManager.PlayOneShot(recogerOveja);
            return;
        }
        else
        {
            RuntimeManager.PlayOneShot(todasLasOvejas);
        }
    }
    public void reproducirPasosJugador()
    {
        //Debug.Log("Reproduciendo pasos del jugador otra vez.");
        reproducirPasos.start();
    }
    public void detenerPasosJugador()
    {
        reproducirPasos.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void ReproducirSalto()
    {
        if (salto.IsNull) return;
        RuntimeManager.PlayOneShot(salto);
    }
    public void ReproducirDano()
    {
        if (dano.IsNull) return;
        RuntimeManager.PlayOneShot(dano);
    }
    public void ReproducirMuerte()
    {
        if (muerte.IsNull) return;
        RuntimeManager.PlayOneShot(muerte);
    }
}