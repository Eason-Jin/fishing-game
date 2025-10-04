Shader "AQUAS-Lite/URP-Frontface"
{
    Properties
    {
        _NormalTexture("Normal Texture", 2D) = "bump" {}
        _NormalTiling("Normal Tiling", Range(0.01, 2)) = 1
        _NormalStrength("Normal Strength", Range(0,2)) = 0
        _WaveSpeed("Wave Speed", Float) = 0

        _MainColor("Main Color", Color) = (0,0.4867925,0.6792453,0)
        _DeepWaterColor("Deep Water Color", Color) = (0.5,0.2712264,0.2712264,0)
        _Density("Density", Range(0,1)) = 1
        _Fade("Fade", Float) = 0

        _FoamTexture("Foam Texture", 2D) = "white" {}
        _FoamTiling("Foam Tiling", Range(0,2)) = 0
        _FoamVisibility("Foam Visibility", Range(0,1)) = 0
        _FoamBlend("Foam Blend", Float) = 0
        _FoamColor("Foam Color", Color) = (0.8773585,0,0,0)
        _FoamIntensity("Foam Intensity", Float) = 0.21
        _FoamSpeed("Foam Speed", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 positionWS : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            sampler2D _NormalTexture;
            float _NormalTiling;
            float _NormalStrength;
            float _WaveSpeed;

            float4 _MainColor;
            float4 _DeepWaterColor;
            float _Density;
            float _Fade;

            sampler2D _FoamTexture;
            float _FoamTiling;
            float _FoamVisibility;
            float _FoamBlend;
            float4 _FoamColor;
            float _FoamIntensity;
            float _FoamSpeed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS);
                OUT.positionWS = worldPos;
                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.uv = IN.uv * _NormalTiling + _Time.y * _WaveSpeed;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float3 normalSample = UnpackNormal(tex2D(_NormalTexture, IN.uv)).xyz;
                float3 normal = lerp(float3(0,0,1), normalSample, _NormalStrength);
                float3 col = lerp(_DeepWaterColor.rgb, _MainColor.rgb, _Density);
                float4 foam = tex2D(_FoamTexture, IN.uv * _FoamTiling + _Time.y * _FoamSpeed) * _FoamIntensity * _FoamVisibility;
                col = col + foam.rgb * _FoamColor.rgb;
                return float4(col, _MainColor.a);
            }
            ENDHLSL
        }
    }
}
