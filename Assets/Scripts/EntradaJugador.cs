using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntradaJugador : MonoBehaviour
{
  public InputActionAsset acciones;

  public InputAction a_atacar;
  public InputAction a_seguir;
  public InputAction a_quieto;

    public DogFollower perro;

    // Start is called before the first frame update
    void Start()
    {
      a_atacar = acciones.FindAction("Attack");
      a_seguir = acciones.FindAction("Follow");
      a_quieto = acciones.FindAction("Stay");

        a_atacar.Enable();
        a_quieto.Enable();
        a_seguir.Enable();

        a_seguir.performed += ctx => perro.EstadoSeguir = true;
        a_quieto.performed += ctx => perro.EstadoSeguir = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        a_atacar = acciones.FindAction("Attack");
        a_seguir = acciones.FindAction("Follow");
        a_quieto = acciones.FindAction("Stay");

        a_atacar.Enable();
        a_seguir.Enable();
        a_quieto.Enable();

        a_seguir.performed += ctx => perro.EstadoSeguir = true;
        a_quieto.performed += ctx => perro.EstadoSeguir = false;
    }
    private void OnDisable()
    {
        a_atacar.Disable();
        a_seguir.Disable();
        a_quieto.Disable();
    }
}