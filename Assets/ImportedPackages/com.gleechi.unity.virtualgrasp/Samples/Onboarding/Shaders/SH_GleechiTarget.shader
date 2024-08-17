Shader "Gleechi/Glass"
{
    Properties
    {
        [NOSCALEOFFSET] _diffuse_HDR("diffuse map", Cube) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"= "Transparent"}
        LOD 100

        Pass
        {
            //Cull Off
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
                float3 color : COLOR;
                

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {                
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 color : COLOR;
                float alpha : TEXCOORD;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            samplerCUBE _diffuse_HDR;
            

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);

                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float f = 1 - saturate(dot(normalize(v.normal), viewDir) / 100);
                o.alpha = lerp(0, 1.34, pow(f, 177));
                
                
               
                o.normal = UnityObjectToWorldNormal(v.normal);
                float3 wsViewDir = WorldSpaceViewDir(float4 (v.vertex.xyz,1));

                

                o.normal = reflect(-wsViewDir, o.normal);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                
                fixed4 col = fixed4(i.color,1);
                fixed4 diffuseLight = texCUBElod(_diffuse_HDR, float4(i.normal, 1));
                diffuseLight /= diffuseLight + 1;
                col *= diffuseLight;
                col.a = saturate(i.alpha);
                //col = (i.alpha);
                return col;

            }
            ENDCG
        }
    }
}
