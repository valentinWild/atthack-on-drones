Shader "Gleechi/Hands"
{
    Properties
    {
         _c1("ColorA", Color) = (1,1,1,1)
         _c2("ColorB", Color) = (1,1,1,1)
         _p("Power", Float) = 300
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent+500" }
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _c1;
            fixed4 _c2;
            float _p;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                float f = 1 - saturate(dot(normalize(v.normal), viewDir) / 100);
                //o.color= f;
                o.color = lerp(_c1,_c2,pow(f,_p));
                return o;

            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
