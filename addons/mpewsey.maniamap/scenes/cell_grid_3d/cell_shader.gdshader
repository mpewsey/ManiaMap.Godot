shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx, unshaded;

instance uniform vec4 albedo : source_color;

void fragment() {
	ALBEDO = albedo.rgb;
	ALPHA = albedo.a;
}
