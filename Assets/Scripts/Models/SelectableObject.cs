using UnityEngine;

namespace SpaceRTS.Models
{
    public class SelectableObject : MonoBehaviour
    {
        protected Outline outline;
        protected Color maxColor = new Color32(170, 255, 255, 100);
        protected Color minColor = new Color32(150, 255, 255, 0);

        public bool IsSelected { get; set; }

        private void Start()
        {
            this.outline = this.GetComponent<Outline>();
            this.ConfigureSelectionOutline();
        }

        private void Update()
        {
            this.HandleObjectSelection();
        }

        private void HandleObjectSelection()
        {
            if (this.IsSelected)
            {
                if (!this.outline.enabled)
                {
                    this.outline.enabled = true;
                }

                this.OscillateOutline();
            }
            else
            {
                if (this.outline.enabled)
                {
                    this.outline.enabled = false;
                }
            }
        }

        protected virtual void ConfigureSelectionOutline()
        {
            this.outline.OutlineWidth = 2;
        }

        private void OscillateOutline()
        {
            this.outline.OutlineColor = Color.Lerp(
                this.minColor, 
                this.maxColor, 
                Mathf.PingPong(Time.realtimeSinceStartup, 1f));
        }
    }
}
