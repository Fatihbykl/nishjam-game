Shader "CustomShaders/URP_Dissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise", 2D) = "white" {}

        _Cutoff ("Cut off", Range(0, 1)) = 0.25
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.05
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="TransparentCutout" 
            "Queue"="AlphaTest" 
            "RenderPipeline" = "UniversalPipeline" 
        }
        LOD 200
        Cull [_Cull]

        // ------------------------------------------------------------------
        // PASS 1: Ana Render Pass (UniversalForward)
        // ------------------------------------------------------------------
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _NoiseTex_ST;
                float4 _Color;
                float4 _EdgeColor;
                float _Cutoff;
                float _EdgeWidth;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * _Color;
                half noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv).r;

                clip(noiseValue - _Cutoff);

                half threshold = _Cutoff * (_EdgeWidth + 1.0);
                half3 emission = (noiseValue >= threshold) ? half3(0,0,0) : _EdgeColor.rgb;

                return half4(texColor.rgb + emission, texColor.a);
            }
            ENDHLSL
        }

        // ------------------------------------------------------------------
        // PASS 2: ShadowCaster (Gölge) - ESKİ/YENİ VERSİYON UYUMLU
        // ------------------------------------------------------------------
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // Gerekli global değişken tanımları (Eski URP sürümleri için)
            float3 _LightDirection;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Cutoff;
                float _EdgeWidth;
                float4 _Color;
                float4 _EdgeColor;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                // 1. Pozisyonu Dünya Koordinatına (World Space) çevir
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                // 2. Gölge kaymasını (Bias) uygula
                // Not: ApplyShadowBias fonksiyonu eski/yeni tüm URP sürümlerinde Shadows.hlsl içinde bulunur.
                // _LightDirection Unity tarafından otomatik doldurulur ancak yukarıda float3 olarak tanımlanması gerekebilir.
                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

                output.positionCS = positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half noiseValue = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, input.uv).r;
                clip(noiseValue - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }
}