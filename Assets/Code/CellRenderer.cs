// Erdroy's Game of Life © 2016-2017 Damian 'Erdroy' Korczowski

using System.Runtime.InteropServices;
using UnityEngine;

namespace Life
{
    public class CellRenderer : MonoBehaviour
    {
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct Int3
        {
            public uint X;
            public uint Y;
            public uint Z;
        }

        public bool Simulate;
        public int FrameRate = 15;

        public Material CellMaterial;
        public ComputeShader SimulationShader;

        private Int3[] _updateBufferArray;
        private ComputeBuffer _updateBuffer;

        private int _updateBufferOffset;

        private float _lastSimulationTime;
        
        private RenderTexture _simulationTexture;
        private RenderTexture _renderTexture;

        private int _kernelUpdate;
        private int _kernelSimulate;
        
        private void Start()
        {
            _updateBufferArray = new Int3[512];
            _updateBuffer = new ComputeBuffer(512, sizeof(int) * 3);


            _simulationTexture = CreateTexture();
            _renderTexture = CreateTexture();

            _kernelUpdate = SimulationShader.FindKernel("Update");
            _kernelSimulate = SimulationShader.FindKernel("Simulate");

            Frame();
        }

        private void Update()
        {
            // update cells
            UpdateCellUpdate();

            // fire new frame when auto simulation is enabled
            if (Simulate)
                Frame();
        }
        
        private void UpdateCellUpdate()
        {
            if (_updateBufferOffset > 0)
            {
                // set update buffer data
                _updateBuffer.SetData(_updateBufferArray);

                // bind buffer
                SimulationShader.SetBuffer(_kernelUpdate, "UpdateBuffer", _updateBuffer);

                // bind output texture which is used by `void Update` in compute shader
                SimulationShader.SetTexture(_kernelUpdate, "Output", _simulationTexture);

                // dispatch!
                SimulationShader.Dispatch(_kernelUpdate, 1, 1, 1);
                
                // set the texture
                CellMaterial.mainTexture = _simulationTexture;

                // clean update buffer data and offset
                for (var i = 0; i < 128; i++)
                    _updateBufferArray[i] = new Int3();

                _updateBufferOffset = 0;
            }
        }

        private RenderTexture CreateTexture()
        {
            // create new RT
            var texture = new RenderTexture(8192, 8192, 0, RenderTextureFormat.R8)
            {
                enableRandomWrite = true,
                filterMode = FilterMode.Point
            };

            texture.Create();
            
            return texture;
        }

        private void SwapTextures()
        {
            // set the current render texture to the cell material main texture
            CellMaterial.mainTexture = _renderTexture;

            // swap simulation and render texture
            var tmp = _simulationTexture;
            _simulationTexture = _renderTexture;
            _renderTexture = tmp;
        }

        private void SetCell(byte value, ushort x, ushort y)
        {
            // push new cell update
            _updateBufferArray[_updateBufferOffset] = new Int3
            {
                X = x,
                Y = y,
                Z = value
            };
            _updateBufferOffset++;
        }

        /// <summary>
        /// Simulates one frame.
        /// </summary>
        public void Frame()
        {
            if (_lastSimulationTime > Time.time)
                return;

            // simulate cells

            // bind input texture
            SimulationShader.SetTexture(_kernelSimulate, "Input", _simulationTexture);

            // bind output texture
            SimulationShader.SetTexture(_kernelSimulate, "Output", _renderTexture);

            // dispatch!
            SimulationShader.Dispatch(_kernelSimulate, 8192 / 16, 8192 / 16, 1);

            // we are done, swap texture and set next simulation time
            SwapTextures();

            _lastSimulationTime = Time.time + 1.0f / FrameRate;
        }

        /// <summary>
        /// Removes cell at [x,y]
        /// </summary>
        public void RemoveCell(ushort x, ushort y)
        {
            SetCell(0, x, y);
        }

        /// <summary>
        /// Places cell at [x,y]
        /// </summary>
        public void PlaceCell(ushort x, ushort y)
        {
            SetCell(1, x, y);
        }

        /// <summary>
        /// Sets auto simulation.
        /// </summary>
        public void SetSimulate(bool value)
        {
            Simulate = value;
        }

        /// <summary>
        /// Sets target frame rate
        /// </summary>
        public void SetFrameRate(float value)
        {
            if (value < 1)
                value = 1;

            FrameRate = (int)value;
        }
    }
}
