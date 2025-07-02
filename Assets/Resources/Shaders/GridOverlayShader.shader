Shader "Custom/GridOverlay"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        _GridColor ("Grid Color", Color) = (0, 0, 0, 1)
        _GridSpacing ("Grid Spacing", Float) = 1.0
        _GridThickness ("Grid Thickness", Float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            fixed4 _BaseColor;
            fixed4 _GridColor;
            float _GridSpacing;
            float _GridThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // Compute UV in world space, aligned to grid spacing
                float2 gridPos = i.worldPos.xz / _GridSpacing;

                // Get fractional part, but shift by 0.5 so lines lie on tile edges
                float2 f = frac(gridPos + 0.5);

                // Calculate distance to nearest edge (0 or 1)
                float2 distToEdge = min(f, 1.0 - f);

                // Calculate anti-aliasing width, clamped for stability
                float aa = max(fwidth(gridPos.x), 0.01);

                // Use smooth step to create anti-aliased lines at edges
                float lineX = 1.0 - smoothstep(0.0, aa, distToEdge.x);
                float lineZ = 1.0 - smoothstep(0.0, aa, distToEdge.y);

                // Combine lines on X and Z axis (so grid lines form a cross)
                float gridLine = max(lineX, lineZ);

                // Interpolate between grid color and base color
                fixed4 col = lerp(_BaseColor, _GridColor, gridLine);

                return col;
            }
            ENDCG
        }
    }
}
