Shader "Museum/PageList"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Angle ("Angle", Range(0, 180)) = 0
        _NextTex ("Next Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        

        Pass
        {
            // Cull Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _NextTex;
            float4 _NextTex_ST;

            struct a2v
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 svpos : SV_POSITION;
                float2 svuv : TEXCOORD0;
            };

            v2f vert(a2v i)
            { 
                v2f o;
                o.svpos = UnityObjectToClipPos(i.pos);
                o.svuv = i.uv;
                return o;
            }

            fixed4 frag(v2f u) : SV_Target
            {
                fixed4 tex = tex2D(_NextTex, u.svuv);
                return tex;
            }

            ENDCG
        }
        
    }
}