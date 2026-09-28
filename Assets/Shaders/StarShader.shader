Shader "Custom/StarShader"
{
    Properties
    {
        [MainColor] _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        [MainTexture] _MainTex("Texture", 2D) = "white" {} 

        _Size ("Size", float) = 1.0
    }

    SubShader
    {
        Tags {"Queue" = "Transparent"}  

        Blend SrcAlpha One
        ZTest Off
        
        LOD 100

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {

                float4 positionOS : POSITION;
                //UNITY_FOG_COORDS(1)
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float _Size;
            CBUFFER_END

            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz * _Size);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                //UNITY_TRANSFORM_FOG(OUT, OUT.POSITION)
                return OUT;
            }

            half4 _Color;

            half4 frag(Varyings IN) : SV_Target
            {
                //Center UV coords -1 -> 1
                float distance_from_center = length((2 * IN.uv) - 1);
                //Function for punchy star drop off
                float inverse_dist = saturate((0.2 / distance_from_center) - 0.2); 
                float4 color =  float4(_Color.rgb, inverse_dist);
                //half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                //UNITY_APPLY_FOG(IN.fogCoord, color);
                

                return color;
            }
            ENDHLSL
        }
    }
}
