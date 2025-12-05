 using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        public DogFollower dogFollower;

        public bool caminando = false;
        public bool caminandoAnterior = false;

        // NEW: Variables para el movimiento de la plataforma
        private Vector3 _platformVelocity = Vector3.zero;
        private Transform _currentPlatformTransform = null;
        private Vector3 _lastPlatformPosition;
        // END NEW


        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // NEW: Inicializar la posición anterior de la plataforma
            _lastPlatformPosition = Vector3.zero;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();

            // NEW: Detección y cálculo de la velocidad de la plataforma ANTES de Move()
            CalculatePlatformMovement();

            Move();
        }


        private void CalculatePlatformMovement()
        {
            // La detección más segura es usar la propiedad 'isGrounded' del CharacterController
            // y buscar un componente de movimiento.

            if (_controller.isGrounded)
            {
                // Disparamos un Raycast (o Physics.SphereCast) hacia abajo para detectar el objeto bajo los pies
                // y obtener su Transform.
                RaycastHit hit;
                Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
                float rayDistance = _controller.height / 2f + 0.2f;

                if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayDistance, GroundLayers, QueryTriggerInteraction.Ignore))
                {
                    Transform platform = hit.transform;

                    // Si el objeto bajo nosotros no es la misma plataforma que antes
                    if (_currentPlatformTransform != platform)
                    {
                        // 1. Nueva Plataforma detectada
                        _currentPlatformTransform = platform;
                        _lastPlatformPosition = _currentPlatformTransform.position;
                    }

                    // 2. Si tenemos una plataforma y su posición ha cambiado
                    if (_currentPlatformTransform != null)
                    {
                        // Calcular el cambio de posición (delta)
                        Vector3 currentPlatformPosition = _currentPlatformTransform.position;
                        _platformVelocity = (currentPlatformPosition - _lastPlatformPosition) / Time.deltaTime;

                        // Actualizar la última posición de la plataforma
                        _lastPlatformPosition = currentPlatformPosition;
                    }
                    else
                    {
                        // No estamos sobre nada que se mueva activamente
                        _platformVelocity = Vector3.zero;
                    }
                }
                else
                {
                    // Estamos en el suelo (según GroundedCheck), pero no detectamos una plataforma debajo.
                    _currentPlatformTransform = null;
                    _platformVelocity = Vector3.zero;
                }
            }
            else
            {
                // No estamos en el suelo, por lo tanto, no hay movimiento de plataforma.
                _currentPlatformTransform = null;
                _platformVelocity = Vector3.zero;
            }
        }


        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // 1. Calcular el vector de movimiento horizontal del jugador basado en la entrada.
            Vector3 playerMovement = targetDirection.normalized * (_speed * Time.deltaTime);

            // 2. Calcular el vector de movimiento vertical (gravedad/salto).
            Vector3 verticalMovement = new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;

            // 3. Calcular el movimiento de la plataforma (velocidad inyectada).
            Vector3 platformMovement = _platformVelocity * Time.deltaTime;

            // 4. Inicializar el movimiento final solo con la gravedad/salto.
            Vector3 finalMovement = verticalMovement;

            // Verificar si estamos siendo transportados por una plataforma móvil.
            bool isCarriedByPlatform = _controller.isGrounded && _platformVelocity != Vector3.zero;

            if (isCarriedByPlatform)
            {
                // A. FIX DE ESTABILIDAD: Añadir un pequeño empuje hacia abajo constante (fricción simulada)
                finalMovement += Vector3.down * 0.1f;

                // B. Añadir el movimiento de la plataforma, que es el control principal ahora.
                finalMovement += platformMovement;

                // C. Si el jugador está activo (presionando teclas), le permitimos moverse sobre la plataforma.
                // Si input.move es cero, el movimiento horizontal del jugador se ANULA.
                if (_input.move != Vector2.zero)
                {
                    finalMovement += playerMovement;
                }
            }
            else
            {
                // Si no estamos en una plataforma móvil, usamos el movimiento normal del jugador.
                finalMovement += playerMovement;
            }

            // move the player: Combinar los vectores de movimiento en una única llamada a Move().
            _controller.Move(finalMovement);


            // --- Lógica de animación y audio de pasos ---

            // FIX para el sonido: El sonido de pasos ahora solo se activa si el jugador
            // está presionando activamente una tecla, ignorando el movimiento de la plataforma.
            bool isPlayerInputtingMovement = _input.move.sqrMagnitude > 0.01f;
            bool nuevoEstado = Grounded && isPlayerInputtingMovement;

            if (nuevoEstado != caminandoAnterior)
            {
                caminando = nuevoEstado;

                if (caminando)
                {
                    SoundManager.instance.reproducirPasosJugador();
                    //Debug.Log("Reproduciendo pasos del jugador.");
                }
                else
                {
                    SoundManager.instance.detenerPasosJugador();
                    //Debug.Log("Deteniendo pasos del jugador.");
                }
            }

            caminandoAnterior = nuevoEstado;


            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
                if (Grounded)
                {
                    _fallTimeoutDelta = FallTimeout;

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, false);
                        _animator.SetBool(_animIDFreeFall, false);
                    }

                    if (_verticalVelocity < 0.0f)
                    {
                        _verticalVelocity = -2f;
                    }

                    // --- 👇 Aquí es donde realmente inicia el salto ---
                    if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                    {

                        _input.jump = false;
                        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    
                        SoundManager.instance.ReproducirSalto();
                        if (_hasAnimator)
                        {
                            _animator.SetBool(_animIDJump, true);
                        }

                        // 🟩 AVISAR AL PERRO QUE SALTE TAMBIÉN
                        if (dogFollower != null)
                        {
                            dogFollower.SaltarConJugador(transform.position);
                        }

                }
                    // --- ↑↑↑ Fin del cambio ---

                    if (_jumpTimeoutDelta >= 0.0f)
                    {
                        _jumpTimeoutDelta -= Time.deltaTime;
                    }
                }
                else
                {
                    _jumpTimeoutDelta = JumpTimeout;

                    if (_fallTimeoutDelta >= 0.0f)
                    {
                        _fallTimeoutDelta -= Time.deltaTime;
                    }
                    else
                    {
                        if (_hasAnimator)
                        {
                            _animator.SetBool(_animIDFreeFall, true);
                        }
                    }

                    _input.jump = false;
                }

                if (_verticalVelocity < _terminalVelocity)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }
            }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}