// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace ProjectNadir
{
    public class @PlayerInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @PlayerInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""PlayerMovement"",
            ""id"": ""0eada310-40f9-4c68-b03a-5ebe01a5e7ad"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""712aa937-fcb5-43e1-ae29-23f5122b14a9"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7572009e-453c-4028-b824-ca23d720b604"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""799bf3f8-c353-468e-b52c-40da88bcbd70"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Melee"",
                    ""type"": ""Button"",
                    ""id"": ""2ccea5b6-0fcd-42a7-b5e9-3f438c7d7704"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e7c9c38b-7f0f-4545-9024-8b7f7e76bb84"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f6432015-8753-48ef-882e-112b5e434aac"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be4ac391-ce74-475a-b71f-eb45ff17f1f3"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d237f7e-d1cf-411b-a114-8f43094ec35c"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Melee"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // PlayerMovement
            m_PlayerMovement = asset.FindActionMap("PlayerMovement", throwIfNotFound: true);
            m_PlayerMovement_Move = m_PlayerMovement.FindAction("Move", throwIfNotFound: true);
            m_PlayerMovement_Jump = m_PlayerMovement.FindAction("Jump", throwIfNotFound: true);
            m_PlayerMovement_Dash = m_PlayerMovement.FindAction("Dash", throwIfNotFound: true);
            m_PlayerMovement_Melee = m_PlayerMovement.FindAction("Melee", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // PlayerMovement
        private readonly InputActionMap m_PlayerMovement;
        private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
        private readonly InputAction m_PlayerMovement_Move;
        private readonly InputAction m_PlayerMovement_Jump;
        private readonly InputAction m_PlayerMovement_Dash;
        private readonly InputAction m_PlayerMovement_Melee;
        public struct PlayerMovementActions
        {
            private @PlayerInput m_Wrapper;
            public PlayerMovementActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_PlayerMovement_Move;
            public InputAction @Jump => m_Wrapper.m_PlayerMovement_Jump;
            public InputAction @Dash => m_Wrapper.m_PlayerMovement_Dash;
            public InputAction @Melee => m_Wrapper.m_PlayerMovement_Melee;
            public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
            public void SetCallbacks(IPlayerMovementActions instance)
            {
                if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                    @Jump.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                    @Dash.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDash;
                    @Dash.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDash;
                    @Dash.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnDash;
                    @Melee.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMelee;
                    @Melee.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMelee;
                    @Melee.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMelee;
                }
                m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Dash.started += instance.OnDash;
                    @Dash.performed += instance.OnDash;
                    @Dash.canceled += instance.OnDash;
                    @Melee.started += instance.OnMelee;
                    @Melee.performed += instance.OnMelee;
                    @Melee.canceled += instance.OnMelee;
                }
            }
        }
        public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);
        public interface IPlayerMovementActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnJump(InputAction.CallbackContext context);
            void OnDash(InputAction.CallbackContext context);
            void OnMelee(InputAction.CallbackContext context);
        }
    }
}
