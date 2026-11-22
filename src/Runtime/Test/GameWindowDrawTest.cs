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

        [SerializeField] private float timer = 2;
        [SerializeField] private float timerDelay = 2;

        private short startPos = 0;

        private void Awake()
        {
            m_Context = new(Matrix4x4.identity, 1, 1, 5000);
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
            timer += Time.deltaTime;

            if (timer >= timerDelay)
            {
                short y = startPos;
                short x = startPos;

                GUILowLevel.SetMaterial(m_Material);
                GUILowLevel.BeginFrame(m_Context);

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

                //uint width = (uint)Screen.width / 8;
                //uint height = (uint)Screen.height / 8;

                //GUI.Button(new(x, y, 100, 100), m_TestTexture);

                for (uint i = 0; i < 500; i++)
                {
                    GUI.PushID(i);
                    for (int j = 0; j < 500; j++)
                    {
                        GUI.Button(new ShortRect(x, y, 8, 8), m_TestTexture, (uint)(i + j));
                        //GUILowLevel.DrawQuad(new ShortRect(x, y, 8, 8));
                        y += 8;
                    }
                    GUI.PopID();

                    y = 0;
                    x += 8;
                }

                GUILowLevel.ResetID();
                GUILowLevel.EndFrame();

                timer = 0;
            }
        }
    }
}