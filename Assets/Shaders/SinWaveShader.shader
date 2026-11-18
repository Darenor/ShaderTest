Shader "Custom/SinWaveShader"
{
    Properties
    {
        _Color1 ("First Color", Color) = (0,1,0,1)
        _Color2 ("Second Color", Color) = (1,0,0,1)
        _Speed ("Speed", Float) = 2
        _Scale ("Scale", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Tags { "LightMode" = "SRPDefaultUnlit" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal//ShaderLibrary/Core.hlsl"

            float4 _Color1;
            float4 _Color2;
            float _Speed;
            float _Scale;

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;

                float wave = sin(_Time.y * _Speed);

                float scale = 1 + wave * _Scale * 0.5;

                input.positionOS.xyz *= scale;

                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float wave = sin(_Time.y * _Speed);
                float t = (wave + 1) * 0.5;

                return lerp(_Color1, _Color2, t);
            }

            ENDHLSL
        }
    }
}