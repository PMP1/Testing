Shader "Custom/screening Tinting" {

 

    Properties {
        _Color ("Main Tint", COLOR) = (1, 1, 1, 1)
        _Texture ("Texture", 2D) = "black" {}
        _Texture2 ("Texture 2", 2D) = "black" {}
        //_TintColor("Tint", Color) = (1,1,1,1)
        //_TintColor2("Tint 2", Color) = (1,1,1,1)
        _Blend ("Blend", Range (0, 1) ) = 0.0 
    }


    SubShader {

		Tags { "Queue" = "Transparent" }

        Pass
        {
			//Blend One One
			//ZWrite Off
			Fog { Color(0, 0, 0, 0) }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            sampler2D _Texture;
            float4 _Texture_ST;
            sampler2D _Texture2;
            float4 _Texture2_ST;

            //fixed4 _TintColor;
            //fixed4 _TintColor2;
            fixed4 _Color;
            fixed _Blend;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv[2] : TEXCOORD0;                
            };

            v2f vert (appdata_full v)
            {
                v2f o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.uv[0] = v.texcoord.xy * _Texture_ST.xy + _Texture_ST.zw;
                o.uv[1] = v.texcoord.xy * _Texture2_ST.xy + _Texture2_ST.zw;
                return o;
            }
            
//            v2f vert (appdata_t v)
//			{
//				v2f o;
//				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
//				return o;
//			}

            fixed4 frag (v2f i) : COLOR
            {
            
            	//fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			    //fixed4 c = tex * _Color;
			    //o.Albedo = c.rgb;
			    //o.Emission = c.rgb * UNITY_SAMPLE_1CHANNEL(_Illum, IN.uv2_Illum);
			    //o.Alpha = c.a;
            
            
                fixed4 color1 = tex2D(_Texture, i.uv[0]);// * _TintColor;
                fixed4 color2 = tex2D(_Texture2, i.uv[1]);// * _TintColor2;
                
                return color1;
                //return lerp(color1, color2, _Blend) * _Color;
            }
            ENDCG

        }

        

 

 

//Uncomment for vertex lighting

 

 

//       Blend DstColor SrcColor

//    

//       Pass {

//            Material {

//                Diffuse [_Color]

//                Ambient [_Color]

//            }

//            Lighting On

//        }          

 

 

    } 

 

 

    FallBack OFf

 

 

}