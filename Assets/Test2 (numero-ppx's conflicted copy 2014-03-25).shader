Shader "Unlit/AlphaSelfIllum TexBlend" {
    Properties {
        _Color ("Color Tint", Color) = (1,1,1,1)
        _MainTex ("SelfIllum Color (RGB) ", 2D) = "white"
       _BlendTex ("Alpha Blended (RGBA) ", 2D) = "white" {}
    }
    SubShader {
    Tags {"Queue"="Transparent"}
    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Pass {
       SetTexture[_MainTex] {
         ConstantColor[_Color]
         Combine texture * constant, constant
       }
       SetTexture[_BlendTex] {Combine texture Lerp(texture) previous, previous}
    }
}
}