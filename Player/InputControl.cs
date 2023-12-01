using JhutenFPP.Inventory;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace JhutenFPP.Manager
{
    [System.Serializable]
    public class InputControl : MonoBehaviour
    {
        [SerializeField] private PlayerInput PlayerInput;
        [SerializeField] private GameObject InventoryPanel;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Run { get; private set; }
        public bool Jump { get; private set; }
        public bool Attack { get; private set; }
        public bool Unsheathe { get; private set; }
        public bool Block { get; private set; }
        public bool Inventory { get; private set; }
        public bool Interact { get; private set; }

        //public bool Crouch { get; private set; }

        private InputActionMap _currentMap;
        private InputAction _moveAction;
        private InputAction _lookAction;
        private InputAction _runAction;
        private InputAction _jumpAction;
        private InputAction _rightHandAction;
        private InputAction _leftHandAction;
        private InputAction _sheateAction;
        private InputAction _inventoryAction;
        private InputAction _interactAction;
        //private InputAction _crouchAction;
        private void Awake()
        {
            HideCursor();
            _currentMap = PlayerInput.currentActionMap;
            _moveAction = _currentMap.FindAction("Move");
            _lookAction = _currentMap.FindAction("Look");
            _runAction = _currentMap.FindAction("Run");
            _jumpAction = _currentMap.FindAction("Jump");
            _rightHandAction = _currentMap.FindAction("RightHandAction");
            _leftHandAction = _currentMap.FindAction("LeftHandAction");
            _sheateAction = _currentMap.FindAction("Sheathe");
            _inventoryAction = _currentMap.FindAction("Inventory");
            _interactAction = _currentMap.FindAction("Interact");

            //_crouchAction = _currentMap.FindAction("Crouch");

            _moveAction.performed += onMove;
            _lookAction.performed += onLook;
            _runAction.performed += onRun;
            _jumpAction.performed += onJump;
            _rightHandAction.performed += onRH;
            _leftHandAction.performed += onLH;
            _sheateAction.performed += onSheathe;
            _inventoryAction.performed += onInventory;
            _interactAction.performed += onInteract;
            //_crouchAction.started += onCrouch;

            _moveAction.canceled += onMove;
            _lookAction.canceled += onLook;
            _runAction.canceled += onRun;
            _jumpAction.canceled += onJump;
            _rightHandAction.canceled += onRH;
            _leftHandAction.canceled += onLH;
            _sheateAction.canceled += onSheathe;
            _inventoryAction.canceled += onInventory;
            _interactAction.canceled += onInteract;
            //_crouchAction.canceled += onCrouch;
        }

        public void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (Inventory) Inventory = false;
        }
        public void EnableCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        private void onMove(InputAction.CallbackContext context)
        {
            Move = context.ReadValue<Vector2>();
        }
        private void onLook(InputAction.CallbackContext context)
        {
            Look = context.ReadValue<Vector2>();
        }
        private void onRun(InputAction.CallbackContext context)
        {
            Run = context.ReadValueAsButton();
        }
        private void onJump(InputAction.CallbackContext context)
        {
            Jump = context.ReadValueAsButton();
        }
        private void onRH(InputAction.CallbackContext context)
        {
            if(Unsheathe)
            Attack = context.ReadValueAsButton();
        }
        private void onLH(InputAction.CallbackContext context)
        {
            if(Unsheathe)
            Block = context.ReadValueAsButton();
        }
        private void onSheathe(InputAction.CallbackContext context)
        {
            Unsheathe = context.ReadValueAsButton() ? !Unsheathe: Unsheathe;
        }
        private void onInventory(InputAction.CallbackContext context)
        {
            Inventory = context.ReadValueAsButton() ? !Inventory: Inventory;
            InventoryManager.Instance.OpenInventory();
        }
        private void onInteract(InputAction.CallbackContext context)
        {
            Interact = context.ReadValueAsButton();
        }
        //private void onCrouch(InputAction.CallbackContext context)
        //{
        //    Crouch = context.ReadValueAsButton();
        //}

        private void OnEnable()
        {
            _currentMap.Enable();
        }

        private void OnDisable()
        {
            _currentMap.Disable();
        }

    }
}
