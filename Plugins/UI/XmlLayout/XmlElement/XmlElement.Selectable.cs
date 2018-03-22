using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Xml
{
    public partial class XmlElement
    {
        public enum SelectionState
        {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Disabled = 3,
        }

        public SelectionState selectionState { get; protected set; }

        public void NotifySelectionStateChanged(SelectionState newSelectionState)
        {
            if (newSelectionState == SelectionState.Highlighted)
            {
                Select();
            }
            else
            {
                // We were selected, but aren't any longer
                if (this.selectionState == SelectionState.Highlighted)
                {
                    Deselect();                    
                }
            }

            this.selectionState = newSelectionState;
        }

        private void Select()
        {
            if (!String.IsNullOrEmpty(Tooltip))
            {
                xmlLayout.ShowTooltip(this, Tooltip);
            }

            if (selectable != null && !selectable.interactable) return;

            PlaySound(OnMouseEnterSound);

            if (hoverClasses != null && hoverClasses.Any())
            {
                hoverClasses.ForEach((c) => AddClass(c));
            }
        }

        private void Deselect()
        {
            if (!String.IsNullOrEmpty(Tooltip))
            {
                xmlLayout.HideTooltip(this);
            }

            if (selectable != null && !selectable.interactable) return;

            PlaySound(OnMouseExitSound);

            if (hoverClasses != null && hoverClasses.Any())
            {
                hoverClasses.ForEach((c) => RemoveClass(c));
            }            
        }
    }
}
