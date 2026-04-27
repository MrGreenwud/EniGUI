Shader "EniGUI/Default"
{
    Properties
    {
    }
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
        }
        
        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            #pragma require 2darray

            #include "UnityCG.cginc"

            sampler2D TEX0;
            sampler2D TEX1;
            sampler2D TEX2;
            sampler2D TEX3;
            sampler2D TEX4;
            sampler2D TEX5;
            sampler2D TEX6;
            sampler2D TEX7;

            float4x4 GUI_MATRIX_VP;
            
            //struct element
            //{
            //    uint posMax;
            //    uint posMin;

            //    uint uvMin;
            //    uint uvMax;

            //    uint id;
            //    uint meta;
            //    uint metaParams;
            //};

            struct element
            {
                uint posMax;
                uint posMin;

                uint uvMin;
                uint uvMax;

                uint meta0;
                uint meta1;
                uint meta2;
                uint meta3;
            };

            StructuredBuffer<element> ELEMENTS;
            
            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                uint meta0      : TEXCOORD1;
                uint meta1      : TEXCOORD2;
                uint meta2      : TEXCOORD3;
                uint meta3      : TEXCOORD4;
            };

            static const float2 posNormalized[4] =
            {
                float2(0, 1),
                float2(0, 0),
                float2(1, 0),
                float2(1, 1)
            };

            static const uint indexes[6] = 
            {
                0, 1, 2,
                0, 2, 3
            };

            v2f vert (uint id : SV_VertexID)
            {
                uint elementID = id / 6;
                uint corner = id % 6;

                element e = ELEMENTS[elementID];
                uint idex = indexes[corner];

                float2 posN = posNormalized[idex];

                float uvYMax = float((e.uvMax << 16) >> 16) / 256.0;
                float uvXMax = float(e.uvMax >> 16) / 256.0;

                float uvYMin = float((e.uvMin << 16) >> 16) / 256.0;
                float uvXMin = float(e.uvMin >> 16) / 256.0;

                float posYMax = float((e.posMax << 16) >> 16);
                float posXMax = float(e.posMax >> 16);

                float posYMin = float((e.posMin << 16) >> 16);
                float posXMin = float(e.posMin >> 16);

                v2f o;

                o.pos = mul(GUI_MATRIX_VP, float4(
                    posXMin + posN.x * (posXMax - posXMin),
                    posYMin + posN.y * (posYMax - posYMin), 
                    1.0, 1.0));

                o.uv = float2(
                    uvXMin + posN.x * (uvXMax - uvXMin),
                    uvYMin + posN.y * (uvYMax - uvYMin));

                o.meta0 = e.meta0;
                o.meta1 = e.meta1;
                o.meta2 = e.meta2;
                o.meta3 = e.meta3;

                return o;
            }

            struct outFrag
            {
                float4 color : SV_Target0;
                float4 id    : SV_Target1;
            };

            outFrag frag (v2f i) : SV_Target0
            {
                outFrag o;

                uint tex = (i.meta1 >> 16) & 0x7;

                float4 c = float4(1.0, 1.0, 1.0, 1.0);

                switch(tex)
                {
                    case 0: c = tex2D(TEX0, i.uv); break;
                    case 1: c = tex2D(TEX1, i.uv); break;
                    case 2: c = tex2D(TEX2, i.uv); break;
                    case 3: c = tex2D(TEX3, i.uv); break;
                    case 4: c = tex2D(TEX4, i.uv); break;
                    case 5: c = tex2D(TEX5, i.uv); break;
                    case 6: c = tex2D(TEX6, i.uv); break;
                    case 7: c = tex2D(TEX7, i.uv); break;
                }

                o.color = c;

                float a = step(0.5, c.a);

                uint id = i.meta0 & 0xFFFFF;

                uint r = id & 0xFFFF;
                uint g = id >> 16;
                o.id   = float4(r / 65535.0, g / 65535.0, 0, a);

                return o;
            }
            ENDCG
        }
    }
}
