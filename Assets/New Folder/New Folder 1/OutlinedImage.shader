Shader "Unlit/OutlinedImage"
{
Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1) // 白色描边
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.05
        _OutlineIntensity ("Outline Intensity", Range(1, 5)) = 2 // 描边强度
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
LOD 100

        ZWrite
Off
        Blend
SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata_t
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _OutlineColor;
float _OutlineWidth;
float _OutlineIntensity;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // 获取主纹理的颜色
    fixed4 mainColor = tex2D(_MainTex, i.uv);

                // 计算描边效果
    float2 offset = _OutlineWidth;
    fixed4 outlineColor = _OutlineColor;

                // 检查周围像素是否有透明部分
    if (tex2D(_MainTex, i.uv + float2(_OutlineWidth, 0)).a < 0.1 ||
                    tex2D(_MainTex, i.uv - float2(_OutlineWidth, 0)).a < 0.1 ||
                    tex2D(_MainTex, i.uv + float2(0, _OutlineWidth)).a < 0.1 ||
                    tex2D(_MainTex, i.uv - float2(0, _OutlineWidth)).a < 0.1)
    {
                    // 增加描边的对比度和亮度
        return fixed4(outlineColor.rgb * _OutlineIntensity, outlineColor.a);
    }

    return mainColor;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
