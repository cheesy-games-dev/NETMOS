using UnityEngine;

namespace BIMOS
{
    public class SnapTurn : MonoBehaviour
    {
        private ControllerRig _controllerRig;

        private bool _isTurning; // Prevents multiple turns when only one is wanted

        private void Start()
        {
            _controllerRig = BIMOSRig.Instance.ControllerRig;
        }

        private void Update()
        {
            bool wasTurning = _isTurning;
            _isTurning = Mathf.Abs(_controllerRig.InputReader.TurnInput) >= 0.75f;

            if (wasTurning || !_isTurning)
                return;

            float normalisedTurnInput = _controllerRig.InputReader.TurnInput / Mathf.Abs(_controllerRig.InputReader.TurnInput);
            _controllerRig.transform.Rotate(0f, normalisedTurnInput * _controllerRig.SnapTurnIncrement, 0f); //Rotates player
        }
    }
}