using EniGUI.LowLevel;
using EniLife;
using UnityEngine;
using UnityEngine.UI;

namespace EniGUI
{
    public class GameWindowDrawTest : MonoBehaviour
    {
        [SerializeField] private RawImage m_RawImage;

        private GUIContext m_Context;
        private Material m_Material;

        private void Awake()
        {
            m_Context = new GUIContext(1024, 1024, 10000);
            m_Context.AutoDispose(RuntimeLifetime.Global);

            m_Material = new Material(Shader.Find("EniGUI/Default"));
            m_Material.hideFlags = HideFlags.DontSave;
            m_Material.AutoDispose(RuntimeLifetime.Global);

            m_RawImage.texture = m_Context.Color;
        }

        private void Update()
        {
            GUIDrawer.BeginFrame(m_Context);
            GUIDrawer.DrawQuad(new Rect(0, 0, 100, 100), m_Material);
            GUIDrawer.EndFrame();
        }
    }
}