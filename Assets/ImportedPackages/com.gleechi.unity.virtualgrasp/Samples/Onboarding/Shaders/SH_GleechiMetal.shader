Shader "Gleechi/Metal"
{
    Properties
    {
        [NOSCALEOFFSET] _diffuse("Reflection", Cube) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
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

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            samplerCUBE _diffuse;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);                
                o.normal = UnityObjectToWorldNormal(v.normal);
                float3 wsViewDir = WorldSpaceViewDir(float4 (v.vertex.xyz,1));
                o.normal = reflect(-wsViewDir, o.normal);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                
                fixed4 col = fixed4(i.color,1);
                fixed4 diffuseLight = texCUBElod(_diffuse, float4(i.normal, 1));
                diffuseLight /= diffuseLight + 1;
                return col * diffuseLight;

            }
            ENDCG
        }
    }
}
