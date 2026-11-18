
Shader "Custom/URP/XRayOutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (0, 0.8, 1, 1)
        _OutlineWidth ("Outline Width", Float) = 0.03
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "XRayOutline"

            Tags { "LightMode" = "UniversalForward" }

            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }

            ZWrite Off
            ZTest Always
            Cull Front

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float eyeDepth : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                 Varyings output;

                 float3 expandedPosition = input.positionOS.xyz
                     + normalize(input.normalOS) * _OutlineWidth;

                 output.positionHCS = TransformObjectToHClip(expandedPosition);

                 float3 positionWS = TransformObjectToWorld(expandedPosition);
                 float3 positionVS = TransformWorldToView(positionWS);

                 output.eyeDepth = -positionVS.z;

                 return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 screenUV = GetNormalizedScreenSpaceUV(input.positionHCS);

                float rawDepth = SampleSceneDepth(screenUV);

                float sceneEyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);

                if (input.eyeDepth <= sceneEyeDepth + 0.001)
                {
                    discard;
                }

                return _OutlineColor;
            }

            ENDHLSL
        }
    }
}
