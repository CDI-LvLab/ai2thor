Shader "Custom/HighlightPost"
{
    Properties
    {
        _MaskTex ("MaskTex", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OverlayColor ("Overlay Color", Color) = (0,1,0,0.3)
        _OutlineThickness ("Thickness", Range(1,5)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;     // Scene texture
            sampler2D _MaskTex;     // Highlight mask
            float4 _OutlineColor;   // Outline color (fully solid)
            float4 _OverlayColor;   // Semi-transparent overlay
            float _OutlineThickness;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;

                // --- Sample the original scene color ---
                fixed4 sceneCol = tex2D(_MainTex, uv);

                // --- Sample mask (WebGL-safe: red channel) ---
                float mask = tex2D(_MaskTex, uv).r;

                // --- Invert mask because black = object, white = background ---
                mask = 1.0 - mask;

                if(mask > 0.001)
                {
                    // --- Apply semi-transparent overlay inside object ---
                    sceneCol.rgb = lerp(sceneCol.rgb, _OverlayColor.rgb, mask * _OverlayColor.a);

                    // --- Compute outline using 8-neighbor max difference (edge detection) ---
                    float2 pixelStep = float2(_OutlineThickness / _ScreenParams.x, _OutlineThickness / _ScreenParams.y);

                    float maskC  = mask;
                    float maskL  = 1.0 - tex2D(_MaskTex, uv + float2(-pixelStep.x, 0)).r;
                    float maskR  = 1.0 - tex2D(_MaskTex, uv + float2( pixelStep.x, 0)).r;
                    float maskU  = 1.0 - tex2D(_MaskTex, uv + float2(0,  pixelStep.y)).r;
                    float maskD  = 1.0 - tex2D(_MaskTex, uv + float2(0, -pixelStep.y)).r;
                    float maskLU = 1.0 - tex2D(_MaskTex, uv + float2(-pixelStep.x,  pixelStep.y)).r;
                    float maskLD = 1.0 - tex2D(_MaskTex, uv + float2(-pixelStep.x, -pixelStep.y)).r;
                    float maskRU = 1.0 - tex2D(_MaskTex, uv + float2( pixelStep.x,  pixelStep.y)).r;
                    float maskRD = 1.0 - tex2D(_MaskTex, uv + float2( pixelStep.x, -pixelStep.y)).r;

                    // Max difference to detect edges only
                    float outline = max(abs(maskC - maskL), abs(maskC - maskR));
                    outline = max(outline, abs(maskC - maskU));
                    outline = max(outline, abs(maskC - maskD));
                    outline = max(outline, abs(maskC - maskLU));
                    outline = max(outline, abs(maskC - maskLD));
                    outline = max(outline, abs(maskC - maskRU));
                    outline = max(outline, abs(maskC - maskRD));

                    // --- Blend outline color over scene (always solid) ---
                    sceneCol.rgb = lerp(sceneCol.rgb, _OutlineColor.rgb, outline * _OutlineColor.a);
                }

                // --- Preserve original scene alpha ---
                sceneCol.a = tex2D(_MainTex, uv).a;

                return sceneCol;
            }

            ENDCG
        }
    }
}
