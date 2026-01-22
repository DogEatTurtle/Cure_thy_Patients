Shader "Custom/OutlineUnlit"
{
    Properties
    {
        _Color ("Outline Color", Color) = (1,0.85,0,1)
        _OutlineWidth ("Outline Width (world units)", Float) = 0.06
    }
    SubShader
    {
        // Render just before normal geometry so the expanded backfaces are occluded by the original mesh,
        // leaving a halo visible around the silhouette.
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }
        Cull Front        // draw backfaces only (so original front faces occlude the extruded backfaces)
        ZWrite On
        ZTest LEqual

        Pass
        {
            Name "OUTLINE"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _OutlineWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // Transform normal to world space and normalize (handles scaled transforms)
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                worldNormal = normalize(worldNormal);

                // Vertex position in world space
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Extrude along the world-space normal by the requested outline width (world units)
                worldPos += worldNormal * _OutlineWidth;

                o.pos = UnityWorldToClipPos(worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}