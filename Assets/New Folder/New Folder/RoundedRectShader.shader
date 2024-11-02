Shader"Custom/RoundedRectShader"  
{  
    Properties  
    {  
        _BackgroundA ("Background A", 2D) = "white" {}  
        _BackgroundB ("Background B", 2D) = "black" {}  
        _Radius("Corner Radius", Float) = 0.1  
        _RectPosition("Rectangle Position", Vector) = (0, 0, 0, 0)  
        _RectSize("Rectangle Size", Vector) = (1, 1, 0, 0)  
    }  
    SubShader  
    {  
        Tags { "RenderType"="Opaque" }  
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
float _Radius;
float4 _RectPosition;
float4 _RectSize;

v2f vert(appdata_t v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

bool IsInRoundedRect(float2 uv, float4 position, float4 size, float radius)
{
                // ����Բ�Ǿ��εı߽�  
    float2 min = position.xy - size.xy * 0.5;
    float2 max = position.xy + size.xy * 0.5;

                // ����Ƿ���Բ�Ǿ�����  
    bool inside = true;

    if (uv.x < min.x + radius) // ��߽�  
        inside = inside && (uv.y > min.y + radius || uv.y < max.y - radius) || pow(uv.x - min.x, 2) + pow(uv.y - min.y, 2) < pow(radius, 2);
    else if (uv.x > max.x - radius) // �ұ߽�  
        inside = inside && (uv.y > min.y + radius || uv.y < max.y - radius) || pow(uv.x - max.x, 2) + pow(uv.y - min.y, 2) < pow(radius, 2);
                
    if (uv.y < min.y + radius) // �±߽�  
        inside = inside && (uv.x > min.x + radius || uv.x < max.x - radius) || pow(uv.x - min.x, 2) + pow(uv.y - min.y, 2) < pow(radius, 2);
    else if (uv.y > max.y - radius) // �ϱ߽�  
        inside = inside && (uv.x > min.x + radius || uv.x < max.x - radius) || pow(uv.x - min.x, 2) + pow(uv.y - max.y, 2) < pow(radius, 2);

    return inside;
}

fixed4 frag(v2f i) : SV_Target
{
    fixed4 colorA = tex2D(_BackgroundA, i.uv);
    fixed4 colorB = tex2D(_BackgroundB, i.uv);

                // �ж��Ƿ���Բ�Ǿ���������  
    bool inRoundedRect = IsInRoundedRect(i.uv, _RectPosition, _RectSize, _Radius);

                // ��������ѡ����ʾ����ɫ  
    return inRoundedRect ? colorB : colorA;
}
            ENDCG  
        }  
    }  
}  