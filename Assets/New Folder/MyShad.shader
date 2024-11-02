Shader"Custom/MyShad"
{
   Properties  
    {  
        _BackgroundA ("Background A", 2D) = "white" {}  
        _BackgroundB ("Background B", 2D) = "black" {}  
        _MaskTex ("Mask Texture", 2D) = "white" {}  
        _MaskColor ("Mask Color", Color) = (1, 1, 1, 1)  
    }  
    SubShader  
    {  
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

sampler2D _BackgroundA;
sampler2D _BackgroundB;
sampler2D _MaskTex;
fixed4 _MaskColor;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
                // ��ȡ����ͼA��B����ɫ  
    fixed4 colorA = tex2D(_BackgroundA, i.uv);
    fixed4 colorB = tex2D(_BackgroundB, i.uv);
                // ��ȡ��Ĥ�������ɫ  
    fixed4 mask = tex2D(_MaskTex, i.uv);
                
                // ʹ����Ĥ��ɫ������λ�ϱ���  
    return lerp(colorA, colorB, mask.r * _MaskColor.r);
}
            ENDCG  
        }  
    }
    FallBack "Diffuse"
}
