// dropdownlist taken from http://forum.kerbalspaceprogram.com/threads/68161-A-GUI-DropDownList
// all credit goes to TriggerAu

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace KSPAdvancedFlyByWire
{
    public class DropDownList
    {
        //properties to use
        internal List<String> Items { get; set; }
        internal Int32 SelectedIndex { get; private set; }
        internal String SelectedValue { get { return Items[SelectedIndex]; } }

        internal Boolean ListVisible;

        private Rect rectButton;
        private Rect rectListBox;

        internal GUIStyle styleListItem = new GUIStyle();
        internal GUIStyle styleListBox = new GUIStyle();
        internal GUIStyle styleListBlocker = new GUIStyle();
        internal Int32 ListItemHeight = 20;

        //event for changes
        public delegate void SelectionChangedEventHandler(Int32 OldIndex, Int32 NewIndex);
        public event SelectionChangedEventHandler SelectionChanged;


        private Texture2D black;

        //Constructors
        public DropDownList()
        {
            ListVisible = false;
            SelectedIndex = 0;

            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0, 0, 0, 1));
            tex.Apply();
            black = tex;

            Texture2D tex2 = new Texture2D(1, 1);
            tex2.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 1));
            tex2.Apply();

            styleListBox.normal.background = tex;
            styleListBlocker.normal.background = tex;
            styleListBlocker.onHover.background = tex;
            styleListItem.normal.background = tex;
            styleListItem.onHover.background = tex;
            styleListItem.normal.textColor = new Color(1, 1, 1, 1);
        }

        //Draw the button behind everything else to catch the first mouse click
        internal void DrawBlockingSelector()
        {
            //do we need to draw the blocker
            if (ListVisible && rectListBox != null)
            {
                //This will collect the click event before any other controls under the listrect
                if (GUI.Button(rectListBox, "", styleListBlocker))
                {
                    Int32 oldIndex = SelectedIndex;
                    SelectedIndex = (Int32)Math.Floor((Event.current.mousePosition.y - rectListBox.y) / (rectListBox.height / Items.Count));
                    //Throw an event or some such from here
                    SelectionChanged(oldIndex, SelectedIndex);
                    ListVisible = false;
                }

            }
        }

        //Draw the actual button for the list
        internal Boolean DrawButton()
        {
            Boolean blnReturn = false;
            //this is the dropdown button - toggle list visible if clicked
            if (GUILayout.Button(SelectedValue))
            {
                ListVisible = !ListVisible;
                blnReturn = true;
            }
            //get the drawn button rectangle
            if (Event.current.type == EventType.repaint)
                rectButton = GUILayoutUtility.GetLastRect();
            //draw a dropdown symbol on the right edge
            Rect rectDropIcon = new Rect(rectButton) { x = (rectButton.x + rectButton.width - 20), width = 20 };
            GUI.Box(rectDropIcon, "\\/");

            return blnReturn;
        }

        //Draw the hovering dropdown
        internal void DrawDropDown()
        {
            if (ListVisible)
            {
                //work out the list of items box
                rectListBox = new Rect(rectButton)
                {
                    y = rectButton.y + rectButton.height,
                    height = Items.Count * ListItemHeight
                };
                //and draw it
                GUI.Box(rectListBox, "", styleListBox);

                //now draw each listitem
                for (int i = 0; i < Items.Count; i++)
                {
                    Rect ListButtonRect = new Rect(rectListBox) { y = rectListBox.y + (i * ListItemHeight), height = 20 };

                    if (GUI.Button(ListButtonRect, Items[i], styleListItem))
                    {
                        ListVisible = false;
                        SelectedIndex = i;
                    }
                }
            }

        }

        internal Boolean CloseOnOutsideClick()
        {
            if (ListVisible && Event.current.type == EventType.mouseDown && !rectListBox.Contains(Event.current.mousePosition))
            {
                ListVisible = false;
                return true;
            }
            else { return false; }
        }
    
    }

}
