using UnityEngine;
using UnityEngine.InputSystem;

namespace KadenZombie8.BIMOS.Rig
{
    public class InputReader : MonoBehaviour
    {
        [Header("Outputs")]
        public Vector2 MoveVector;
        public float CrouchInput, TurnInput;

        [SerializeField]
        private SmoothLocomotion _smoothLocomotion;

        public InputActionReference
            MoveAction,
            RunAction,
            CrouchAction,
            JumpAction,
            TurnAction;

        private void Awake()
        {
            //Enable all actions
            MoveAction.action.Enable();
            RunAction.action.Enable();
            CrouchAction.action.Enable();
            JumpAction.action.Enable();
            TurnAction.action.Enable();

            //Bind actions to their respective procedures
            RunAction.action.performed += OnRun;
            JumpAction.action.performed += OnJumpAnticipate;
            JumpAction.action.canceled += OnJump;
        }

        private void OnDestroy()
        {
            //Unsubscribe
            RunAction.action.performed -= OnRun;
            JumpAction.action.performed -= OnJumpAnticipate;
            JumpAction.action.canceled -= OnJump;
        }

        private void Update()
        {
            OnMove();
            OnCrouchChanged();
            OnTurn();
        }

        //Input actions
        private void OnMove()
        {
            MoveVector = Vector2.ClampMagnitude(MoveAction.action.ReadValue<Vector2>(), 1); //Gets the move vector and clamps it to 1
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            _smoothLocomotion.Run();
        }

        private void OnCrouchChanged()
        {
            CrouchInput = CrouchAction.action.ReadValue<Vector2>().y;
        }

        private void OnJumpAnticipate(InputAction.CallbackContext context)
        {
            _smoothLocomotion.AnticipateJump();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _smoothLocomotion.Jump();
        }

        private void OnTurn()
        {
            TurnInput = TurnAction.action.ReadValue<Vector2>().x;
        }
    }
}