// Erdroy's Game of Life © 2016-2017 Damian 'Erdroy' Korczowski

using UnityEngine;

namespace Life
{
    public class CellPlacing : MonoBehaviour
    {
        public Transform Cursor;
        public CellRenderer Renderer;

        private float _spaceDown;
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Renderer.Frame();
                _spaceDown = Time.time;
            }

            if (Input.GetKey(KeyCode.Space) && _spaceDown + 1.0f < Time.time)
            {
                Renderer.Frame();
            }

            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var pos = hit.point;
                pos.x *= 0.1f;
                pos.y *= 0.1f;
                pos.x *= 8192;
                pos.y *= 8192;

                var x = (ushort)pos.x;
                var y = (ushort)pos.y;

                Cursor.position = new Vector3((x + 0.5f) / 8192.0f, (y + 0.5f) / 8192.0f) * 10.0f + Vector3.back * 0.0001f;
                
                // TODO: maybe we want some additional shapes? (or copy/paste?)

                if (Input.GetKey(KeyCode.Mouse0))
                    Renderer.PlaceCell(x, y);

                if (Input.GetKey(KeyCode.Mouse1))
                    Renderer.RemoveCell(x, y);
            }
        }
    }
}