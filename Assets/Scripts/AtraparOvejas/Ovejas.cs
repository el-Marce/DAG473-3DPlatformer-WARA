using UnityEngine;

public class Ovejas : MonoBehaviour
{
    [SerializeField] public bool atrapada = false;
    [SerializeField] private GameObject indicadorE;
    [SerializeField] private Transform indicadorPos;
    private void Awake()
    {
        if (indicadorE != null && indicadorPos != null)
        {
            indicadorE.SetActive(false);
        }
    }
    public void Recoger()
    {
        if (!atrapada)
        {
            atrapada = true;
            gameObject.SetActive(false);
            if (indicadorE != null)
                indicadorE.SetActive(false);
        }
    }

    public void MostrarIndicador(bool mostrar)
    {
        if (indicadorE != null)
            indicadorE.SetActive(mostrar);
    }
}
