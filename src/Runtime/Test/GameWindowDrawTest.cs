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
        [SerializeField] private Texture m_TestTexture;

        private GUIContext m_Context;
        private Material m_Material;

        private void Awake()
        {
            m_Context = new(Matrix4x4.identity, 1, 1, 2500);
            m_Context.Resize((uint)Screen.width, (uint)Screen.height);
            m_Context.AutoDispose(RuntimeLifetime.Global);

            m_Material = new(Shader.Find("EniGUI/Default"));
            m_Material.SetTexture("_MainTex", m_TestTexture);
            m_Material.hideFlags = HideFlags.DontSave;
            m_Material.AutoDispose(RuntimeLifetime.Global);

            m_RawImage.texture = m_Context.Color;
            m_RawImage1.texture = m_Context.ID;
        }

        private void Update()
        {
            short y = 0;
            short x = 0;

            GUILowLevel.BeginFrame(m_Context);

            var texture = new LowLevel.GUITexture(1, m_TestTexture);

            //GUI.Button(new ShortRect(x, y, 640, 320), texture, 1);
            //GUI.Button(new ShortRect(x, 50, 640, 320), texture, 2);

            //GUILowLevel.BindTexture(new LowLevel.GUITexture(1, m_TestTexture));
            //GUILowLevel.UnbindTexture();

            //GUILowLevel.PushID(100);
            //GUILowLevel.BindTexture(texture);
            //GUILowLevel.DrawQuad(new ShortRect(0, 0, 100, 200));
            //GUILowLevel.UnbindTexture();
            //GUILowLevel.PopID();
            //GUILowLevel.PushID(300);
            //GUILowLevel.DrawQuad();
            //GUILowLevel.PopID();


            uint width = (uint)Screen.width / 8;
            uint height = (uint)Screen.height / 8;

            for (uint i = 0; i < 1000; i++)
            {
                GUI.PushID(i);
                for (int j = 0; j < 1000; j++)
                {
                    GUI.Image(new ShortRect(x, y, 8, 8), texture);
                    y += 8;
                }
                GUI.PopID();

                y = 0;
                x += 8;
            }

            GUILowLevel.EndFrame();
        }
    }
}