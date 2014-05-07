Shader "Custom/Self-Illumin ColourShader" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
    //_Illum ("Illumin (A)", 2D) = "white" {}
    //_EmissionLM ("Emission (Lightmapper)", Float) = 0
}
 
SubShader {
    Tags { "RenderType" = "Opaque" }
    LOD 200
	 
	CGPROGRAM
	#pragma surface surf Lambert noambient
	 
	sampler2D _MainTex;
	//fixed4 _Color;
	 
	struct Input {
	    float2 uv_MainTex;
	    float4 color : COLOR;
	};
	 
	void surf (Input IN, inout SurfaceOutput o) {
	    ///*fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	    //fixed4 c = tex * _Color;
	    ////o.Albedo = c.rgb;
	    //o.Emission = c.rgb * UNITY_SAMPLE_1CHANNEL(_Illum, IN.uv2_Illum);
	    //o.Alpha = c.a;*/
	    o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
	    
	    float3 light = IN.color.rgb;
	    float sun = IN.color.a;
	    float3 ambient = UNITY_LIGHTMODEL_AMBIENT * sun * 2;
	    ambient = max(ambient, 0.002);
	    ambient = max(ambient, light);
	    o.Emission = o.Albedo * ambient;
	}
	ENDCG
	} 
	FallBack "Legacy Shaders/Lightmapped/Diffuse"
}