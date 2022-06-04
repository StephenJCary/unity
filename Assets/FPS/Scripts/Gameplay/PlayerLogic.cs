using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.FPS.Gameplay
{
    [RequireComponent(typeof(PlayerInputHandler))]
    public class PlayerLogic : MonoBehaviour
    {


        PlayerInputHandler m_InputHandler;
        bool m_WantsToShoot = false;



        // Start is called before the first frame update
        void Start()
        {
            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerWeaponsManager>(m_InputHandler, this,
                gameObject);
        }



        // Update is called once per frame
        void Update()
        {
            bool hasFired = HandleShootInputs(
                   m_InputHandler.GetFireInputDown(),
                   m_InputHandler.GetFireInputHeld(),
                   m_InputHandler.GetFireInputReleased());
        }

        public bool HandleShootInputs(bool inputDown, bool inputHeld, bool inputUp)
        {
            m_WantsToShoot = inputDown || inputHeld;
          

            return false;
        }
    }
}
