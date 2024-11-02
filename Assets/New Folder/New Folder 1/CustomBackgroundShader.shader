Shader"Custom/CustomBackgroundShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SecondaryTex ("Secondary Texture", 2D) = "white" {}
        _RectPosition0 ("Rectangle Positions 0", Vector) = (0, 0, 0, 0)
        _RectSize0 ("Rectangle Sizes 0", Vector) = (0, 0, 0, 0)
        _RectRadii0 ("Rectangle Radii 0", Float) = 0

        _RectPosition1 ("Rectangle Positions 1", Vector) = (0, 0, 0, 0)
        _RectSize1 ("Rectangle Sizes 1", Vector) = (0, 0, 0, 0)
        _RectRadii1 ("Rectangle Radii 1", Float) = 0

        _RectPosition2 ("Rectangle Positions 2", Vector) = (0, 0, 0, 0)
        _RectSize2 ("Rectangle Sizes 2", Vector) = (0, 0, 0, 0)
        _RectRadii2 ("Rectangle Radii 2", Float) = 0


        _RectPosition3 ("Rectangle Positions 3", Vector) = (0, 0, 0, 0)
        _RectSize3 ("Rectangle Sizes 3", Vector) = (0, 0, 0, 0)
        _RectRadii3 ("Rectangle Radii 3", Float) = 0

        _RectPosition4 ("Rectangle Positions 4", Vector) = (0, 0, 0, 0)
        _RectSize4 ("Rectangle Sizes 4", Vector) = (0, 0, 0, 0)
        _RectRadii4 ("Rectangle Radii 4", Float) = 0

        _RectPosition5 ("Rectangle Positions 5", Vector) = (0, 0, 0, 0)
        _RectSize5 ("Rectangle Sizes 5", Vector) = (0, 0, 0, 0)
        _RectRadii5 ("Rectangle Radii 5", Float) = 0

        _RectPosition6 ("Rectangle Positions 6", Vector) = (0, 0, 0, 0)
        _RectSize6 ("Rectangle Sizes 6", Vector) = (0, 0, 0, 0)
        _RectRadii6 ("Rectangle Radii 6", Float) = 0


        _RectPosition7 ("Rectangle Positions 7", Vector) = (0, 0, 0, 0)
        _RectSize7 ("Rectangle Sizes 7", Vector) = (0, 0, 0, 0)
        _RectRadii7 ("Rectangle Radii 7", Float) = 0

        _RectPosition8 ("Rectangle Positions 8", Vector) = (0, 0, 0, 0)
        _RectSize8 ("Rectangle Sizes 8", Vector) = (0, 0, 0, 0)
        _RectRadii8 ("Rectangle Radii 8", Float) = 0

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

            float4 _RectPosition0; 
            float4 _RectSize0;
            half _RectRadii0;

            float4 _RectPosition1; 
            float4 _RectSize1;
            half _RectRadii1;

            float4 _RectPosition2;
            float4 _RectSize2;
            half _RectRadii2;

            float4 _RectPosition3;
            float4 _RectSize3;
            half _RectRadii3;

            float4 _RectPosition4;
            float4 _RectSize4;
            half _RectRadii4;

            float4 _RectPosition5;
            float4 _RectSize5;
            half _RectRadii5;

            float4 _RectPosition6;
            float4 _RectSize6;
            half _RectRadii6;

            float4 _RectPosition7;
            float4 _RectSize7;
            half _RectRadii7;

            float4 _RectPosition8;
            float4 _RectSize8;
            half _RectRadii8;
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
                if (roundRect(i.uv, _RectPosition5.xy, _RectSize5.xy, _RectRadii5.x) <= 0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition6.xy, _RectSize6.xy, _RectRadii6.x) <= 0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition7.xy, _RectSize7.xy, _RectRadii7.x) <= 0.0)
                    return secondaryColor;
                if (roundRect(i.uv, _RectPosition8.xy, _RectSize8.xy, _RectRadii8.x) <= 0.0)
                    return secondaryColor;
                return mainColor;
            }
            ENDCG
        }
    }
    FallBack"Diffuse"
}