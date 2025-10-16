Shader "Museum/PageFlip"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Angle ("Angle", Range(0, 180)) = 0
        _Color ("Tint Color", Color) = (1,1,1,0.5)   // ✅ 新增颜色属性
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        
        Pass
        {
            Cull Off
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha   // ✅ 允许颜色透明混合

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Angle;
            float4 _Color;   // ✅ 新增：从脚本接收颜色

            struct a2v
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 svpos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(a2v i)
            {
                float sins;
                float coss;
                sincos(radians(_Angle), sins, coss);

                // 沿Z轴旋转
                float4x4 rotationMatrix = float4x4(
                    coss, sins, 0, 0,
                    -sins, coss, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1
                );

                v2f o;
                i.pos -= float4(5, 0, 0, 0);
                i.pos.y = sin(i.pos.x * 0.5) * sins;
                i.pos = mul(rotationMatrix, i.pos);
                i.pos += float4(5, 0, 0, 0);

                o.svpos = UnityObjectToClipPos(i.pos);
                o.uv = TRANSFORM_TEX(i.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                tex.rgb *= _Color.rgb;   // ✅ 混合颜色叠加
                tex.a *= _Color.a;       // ✅ 保持透明度
                return tex;
            }
            ENDCG
        }
    }
}