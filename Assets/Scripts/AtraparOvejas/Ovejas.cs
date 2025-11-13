using UnityEngine;

public class Ovejas : MonoBehaviour
{
    [SerializeField] private GameObject indicadorE;
    [SerializeField] private Transform indicadorPos;

    public bool atrapada = false;
    public bool entregada = false;

    private void Awake()
    {
        if (indicadorE != null)
            indicadorE.SetActive(false);
    }

    public void Recoger()
    {
        if (!atrapada)
        {
            atrapada = true;
            gameObject.SetActive(false);
            GameManager.instance.ovejasRecolectadas.Add(gameObject);

            if (indicadorE != null)
                indicadorE.SetActive(false);
        }
    }

    // Solo actualiza estado y asegura que esté activo
    public void Entregar()
    {
        if (atrapada && !entregada)
        {
            entregada = true;
            gameObject.SetActive(true);

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                //rb.isKinematic = true;

            if (indicadorE != null)
                indicadorE.SetActive(false);
        }
    }

    public void MostrarIndicador(bool mostrar)
    {
        if (indicadorE != null && !atrapada)
            indicadorE.SetActive(mostrar);
    }
}
