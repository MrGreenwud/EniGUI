using EniGUI.LowLevel;
using EniLife;
using UnityEngine;
using UnityEngine.UI;

namespace EniGUI
{
    public class GameWindowDrawTest : MonoBehaviour
    {
        [SerializeField] private RawImage m_RawImage;
        [SerializeField] private RawImage m_RawImage1;

        private GUIContext m_Context;
        private Material m_Material;

        private void Awake()
        {
            m_Context = new GUIContext((uint)Screen.width, (uint)Screen.height, 10000);
            m_Context.AutoDispose(RuntimeLifetime.Global);

            m_Material = new Material(Shader.Find("EniGUI/Default"));
            m_Material.hideFlags = HideFlags.DontSave;
            m_Material.AutoDispose(RuntimeLifetime.Global);

            m_RawImage.texture = m_Context.Color;
            m_RawImage1.texture = m_Context.ID;
        }

        private bool b = false;

        private void Update()
        {
            //if (b)
            //    return;

            float y = 0;
            float x = 0;

            GUIDrawer.BeginFrame(m_Context);

            //uint r = 0;

            for (int i = 0; i < (uint)Screen.width / 8; i++)
            {
                //GUIDrawer.PushID((uint)i * 3);
                for (int j = 0; j < (uint)Screen.height / 8; j++)
                {
                    //Debug.Log(GUIDrawer.PushID(1 + (uint)i + (uint)x 7 ((uint)j + 1 + (uint)y)));
                    //GUIDrawer.PushID(r);
                    GUIDrawer.DrawQuad(new Rect(x, y, 8, 8), m_Material);
                    y += 8;
                    //GUIDrawer.PopID();

                    //r++;
                }
                //GUIDrawer.PopID();

                y = 0;
                x += 8;
            }

            //Debug.Log(r);

            GUIDrawer.EndFrame();

            //b = false;
        }
    }
}