import bpy, bmesh, random, math
from mathutils.bvhtree import BVHTree

def bounds_overlap(A, B):
    va = [A.matrix_world @ i.co for i in A.data.vertices]
    pa = [i.vertices for i in A.data.polygons]
    ba = BVHTree.FromPolygons(va, pa)
    
    vb = [B.matrix_world @ i.co for i in B.data.vertices]
    pb = [i.vertices for i in B.data.polygons]
    bb = BVHTree.FromPolygons(vb, pb)
    
    overlaps = ba.overlap(bb)
    return len(overlaps) > 0

def kitbash_selected():
    L = len(bpy.context.selected_objects)
    if (L <= 0):
        print("[WARN]: No Objects Selected")
        return
    parent = bpy.data.objects.new("Kitbash Group", None)
    bpy.context.collection.objects.link(parent)
    collisions = 0
    for i in range(0, 10):
        select = random.randrange(0, L)
        obj = bpy.context.selected_objects[select].copy()
        collision = False
        for j in range(0, 10): # 100
            obj.location = (random.uniform(0.0, 10.0),
                            random.uniform(0.0, 10.0),
                            random.uniform(0.0, 10.0))
            obj.rotation_euler = (random.uniform(0.0, 2.0 * math.pi),
                                  random.uniform(0.0, 2.0 * math.pi),
                                  random.uniform(0.0, 2.0 * math.pi))
            bpy.context.collection.objects.link(obj)
            for k in range(0, len(parent.children)):
                if (bounds_overlap(obj, parent.children[k])):
                    collision = True
                    break
            if not collision:
                break
        if (collision):
            collisions += 1
            bpy.context.collection.objects.unlink(obj)
        else:
            obj.parent = parent
    if (collisions > 0):
        print("[WARN]: Collisions Detected: " + str(collisions))

#print(bounds_overlap(bpy.context.selected_objects[0],
#                     bpy.context.selected_objects[1]))
kitbash_selected()

"""
import bpy, random

depsgraph = bpy.context.evaluated_depsgraph_get()
origin = (0, 0, 0)
direction = (1, 1, 1)
distance = 0

options = [depsgraph, origin, direction]
labeled_options = {"distance": distance}
#raycast = bpy.context.scene.ray_cast(*options, *labeled_options)
raycast = bpy.context.scene.ray_cast(*options, distance = distance)
result, location, normal, index, object, matrix = raycast

print(index)
"""

""" # Kitbash Ray Tracing and Bounds Intersect should be done on GPU
import gpu
from gpu_extras.batch import batch_for_shader

vertex_shader = \
'''
uniform mat4 viewProjectionMatrix;

in vec3 position;
out vec3 pos;

void main()
{
    pos = position;
    gl_Position = viewProjectionMatrix * vec4(position, 1.0f);
}
'''

fragment_shader = \
'''
uniform float brightness;

in vec3 pos;
out vec4 FragColor;

void main()
{
    FragColor = vec4(pos * brightness, 1.0);
}
'''

coords = [(1, 1, 1), (2, 0, 0), (-2, -1, 3)]
shader = gpu.types.GPUShader(vertex_shader, fragment_shader)
batch = batch_for_shader(shader, "TRIS", {"position": coords})


def draw():
    shader.bind()
    if (True):
        fb = gpu.state.active_framebuffer_get()
        fb.clear(color = (0.0, 0.0, 0.0, 0.0))
    matrix = bpy.context.region_data.perspective_matrix
    shader.uniform_float("viewProjectionMatrix", matrix)
    shader.uniform_float("brightness", 0.5)
    batch.draw(shader)

bpy.types.SpaceView3D.draw_handler_add(draw, (), 'WINDOW', 'POST_VIEW')
"""