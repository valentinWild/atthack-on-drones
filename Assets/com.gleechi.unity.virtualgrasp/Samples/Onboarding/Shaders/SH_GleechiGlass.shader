Shader "Gleechi/Target"
{
    Properties
    {
        _c("Color", Color) = (.25, .5, .5, 1)
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"= "Transparent"}
        LOD 100

        Pass
        {
            ZWrite Off
            Blend One One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"           

            struct appdata
            {
                float4 vertex : POSITION;             
                float3 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {                
                float4 vertex : SV_POSITION;                
                float alpha : TEXCOORD;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _c;
            

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float f = 1 - saturate(dot(normalize(v.normal), viewDir) / 100);
                o.alpha = lerp(0, 1.34, pow(f, 250));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                
                
                return _c*i.alpha;

            }
            ENDCG
        }
    }
}
