using UnityEngine;

namespace Netmos
{
    [RequireComponent(typeof(Renderer))]
    public class PlayerColorRenderer : NetworkBehaviour
    {
        private new Renderer renderer;
        public bool hideCollider = true;
        public bool hideRenderer = true;
        public bool changeColor = true;
        [NetVar] public Color color;

        void Start()
        {
            renderer = GetComponent<Renderer>();
            SetColor(Netmos.NetworkManager.LocalPlayer.Color);
        }

        [NetCmd]
        private void SetColor(Color color)
        {
            if (!changeColor) return;
            this.color = color;
            UpdateRender();
        }

        [NetClientRpc]
        private void UpdateRender()
        {
            if (!changeColor) return;
            renderer.material.SetColor(0, color);
        }

        void Update()
        {
            if (!authority) return;
            if (color != Netmos.NetworkManager.LocalPlayer.Color)
            {
                SetColor(Netmos.NetworkManager.LocalPlayer.Color);
            }
            renderer.enabled = !hideRenderer;
            if(TryGetComponent(out Collider collider)) collider.enabled = !hideCollider;
        }
    }
}