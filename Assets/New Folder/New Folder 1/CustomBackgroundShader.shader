Shader"Custom/CustomBackgroundShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SecondaryTex ("Secondary Texture", 2D) = "white" {}
        _RectCount ("Rectangle Count", Int) = 1
        _RectPosition0 ("Rectangle Positions 0", Vector) = (0, 0, 0, 0)
        _RectSize0 ("Rectangle Sizes 0", Vector) = (0, 0, 0, 0)
        _RectRadii0 ("Rectangle Radii 0", Vector) = (0, 0, 0, 0)

        _RectPosition1 ("Rectangle Positions 1", Vector) = (0, 0, 0, 0)
        _RectSize1 ("Rectangle Sizes 1", Vector) = (0, 0, 0, 0)
        _RectRadii1 ("Rectangle Radii 1", Vector) = (0, 0, 0, 0)

        _RectPosition2 ("Rectangle Positions 2", Vector) = (0, 0, 0, 0)
        _RectSize2 ("Rectangle Sizes 2", Vector) = (0, 0, 0, 0)
        _RectRadii2 ("Rectangle Radii 2", Vector) = (0, 0, 0, 0)


        _RectPosition3 ("Rectangle Positions 3", Vector) = (0, 0, 0, 0)
        _RectSize3 ("Rectangle Sizes 3", Vector) = (0, 0, 0, 0)
        _RectRadii3 ("Rectangle Radii 3", Vector) = (0, 0, 0, 0)

        _RectPosition4 ("Rectangle Positions 4", Vector) = (0, 0, 0, 0)
        _RectSize4 ("Rectangle Sizes 4", Vector) = (0, 0, 0, 0)
        _RectRadii4 ("Rectangle Radii 4", Vector) = (0, 0, 0, 0)

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

            sampler2D _MainTex;
            sampler2D _SecondaryTex;
            int _RectCount;
            float4 _RectPosition0; 
            float4 _RectSize0;
            float4 _RectRadii0;

            float4 _RectPosition1; 
            float4 _RectSize1;
            float4 _RectRadii1;

            float4 _RectPosition2;
            float4 _RectSize2;
            float4 _RectRadii2;

            float4 _RectPosition3;
            float4 _RectSize3;
            float4 _RectRadii3;

            float4 _RectPosition4;
            float4 _RectSize4;
            float4 _RectRadii4;
            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float roundRect(float2 uv, float2 center, float2 size, float radius)
            {
                float2 dist = abs(uv - center) - size + radius;
                return length(max(dist, 0.0)) + min(max(dist.x, dist.y), 0.0) - radius;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, i.uv);
                fixed4 secondaryColor = tex2D(_SecondaryTex, i.uv);

                if (roundRect(i.uv,_RectPosition0.xy,_RectSize0.xy,_RectRadii0.x)<=0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition1.xy, _RectSize1.xy, _RectRadii1.x) <= 0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition2.xy, _RectSize2.xy, _RectRadii2.x) <= 0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition3.xy, _RectSize3.xy, _RectRadii3.x) <= 0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition4.xy, _RectSize4.xy, _RectRadii4.x) <= 0.0)
                    return secondaryColor;

                return mainColor;
            }
            ENDCG
        }
    }
    FallBack"Diffuse"
}