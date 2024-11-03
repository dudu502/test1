Shader "Unlit/OutlinedImage"
{
Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1) // ��ɫ���
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.05
        _OutlineIntensity ("Outline Intensity", Range(1, 5)) = 2 // ���ǿ��
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
                // ��ȡ���������ɫ
    fixed4 mainColor = tex2D(_MainTex, i.uv);

                // �������Ч��
    float2 offset = _OutlineWidth;
    fixed4 outlineColor = _OutlineColor;

                // �����Χ�����Ƿ���͸������
    if (tex2D(_MainTex, i.uv + float2(_OutlineWidth, 0)).a < 0.1 ||
                    tex2D(_MainTex, i.uv - float2(_OutlineWidth, 0)).a < 0.1 ||
                    tex2D(_MainTex, i.uv + float2(0, _OutlineWidth)).a < 0.1 ||
                    tex2D(_MainTex, i.uv - float2(0, _OutlineWidth)).a < 0.1)
    {
                    // ������ߵĶԱȶȺ�����
        return fixed4(outlineColor.rgb * _OutlineIntensity, outlineColor.a);
    }

    return mainColor;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
