Shader "Adinmo/MaskedShader"
 {
SubShader {
    Tags { "AdinmoMask"="Masked" "RenderQueue"="Opaque" }
    Pass {
        Fog { Mode Off }       
        Color (0.5,1,1,1)
    }
}
}