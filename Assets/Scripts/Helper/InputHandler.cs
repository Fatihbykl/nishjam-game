using UnityEngine;

namespace Helper
{
    public static class InputHandler
    {
        public static InputSystem_Actions Actions { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Actions = new InputSystem_Actions();
            Actions.Enable();
        
            Application.quitting += () => Actions.Disable();
        }
    }
}
