Shader "EniGUI/Default"
{
    Properties
    {
        _MainTex ("Albedo (RGBA)", 2D) = "white" {}
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

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            float4x4 GUI_MATRIX_VP;

            struct appdata
            {
                float3 position : POSITION;
                float2 uv       : TEXCOORD0;
                uint id         : TEXCOORD1;
                uint clip       : TEXCOORD2;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                uint id         : TEXCOORD1;
                uint clip       : TEXCOORD2;
                float depth     : TEXCOORD3;
            };

            v2f vert (appdata v)
            {
                 v2f o;

                 o.pos = mul(GUI_MATRIX_VP, float4(v.position.xy, 1.0, 1.0));
                 o.uv = v.uv;
                 o.id = v.id;
                 o.clip = v.clip;
                 o.depth = v.position.z;

                return o;
            }

            float4 frag (v2f i) : SV_Target0
            {
                return float4(1, 0, 0, 1);
            }
            ENDCG
        }
    }
}
