using UnityEngine;

namespace KadenZombie8.BIMOS.Rig
{
    public class SmoothTurn : MonoBehaviour
    {
        private ControllerRig _controllerRig;

        private void Start()
        {
            _controllerRig = BIMOSRig.Instance.ControllerRig;
        }

        private void Update()
        {
            if (Mathf.Abs(_controllerRig.InputReader.TurnInput) < 0.75f)
                return;

            float normalisedTurnInput = _controllerRig.InputReader.TurnInput / Mathf.Abs(_controllerRig.InputReader.TurnInput);
            _controllerRig.transform.Rotate(0f, normalisedTurnInput * 20f * _controllerRig.SmoothTurnSpeed * Time.deltaTime, 0f); //Rotates player
        }
    }
}