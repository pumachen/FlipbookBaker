Shader "Hidden/FlipbookBaker"
{
    Properties {}
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Blend One Zero, One Zero

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"

            Texture2D _FlipbookBlack;
            Texture2D _FlipbookWhite;
            SamplerState sampler_linear_clamp;

            float4 frag (v2f_img i) : SV_Target
            {
                float3 black = _FlipbookBlack.SampleLevel(sampler_linear_clamp, i.uv, 0).rgb;
                float3 white = _FlipbookWhite.SampleLevel(sampler_linear_clamp, i.uv, 0).rgb;
                float a = dot(black - white + 1, 1) / 3;
                float3 rgb = black / a;
                return float4(rgb, a);
            }
            ENDHLSL
        }
    }
}
