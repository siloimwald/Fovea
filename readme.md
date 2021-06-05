### Ray Tracing in One Weekend (and more)

C# implementation of Peter Shirley's excellent free books on ray and path tracing
with some own additions. [Take me to the books!](https://raytracing.github.io/)

Fovea is the name i usually pick for all of my hobbyist rendering adventures, from
[Fovea centralis](https://en.wikipedia.org/wiki/Fovea_centralis).

In addition to the books

- basic CSG support based on
["Ray Tracing CSG Objects Using Single Hit Intersections"](http://xrt.wdfiles.com/local--files/doc%3Acsg/CSG.pdf)
  (though slightly broken when using instancing, can't be bothered to fix that ;)
- more general instancing support through transformation matrices
- cylinders and disks, because why not
- slightly more fancy BVH (iirc based on pbrt), using a SAH approach with binning
- triangle (mesh) support, either as standalone triangles or as a mesh
- instead of the whole xy/xz/.. rect stuff, i've added sort of factory methods to produce
quads/boxes from triangles. i.e. the cornell box is actually all triangles except for the sphere.

Fancy picture time

Teapot mesh with interpolated normals. The whole scene is enclosed with a sphere with a forest
scene texture used for lighting. 

![mesh_forest](https://github.com/siloimwald/Fovea/blob/main/Results/mesh_forest.png)

Cylinder Demo with some textures

![cylinders](https://github.com/siloimwald/Fovea/blob/main/Results/cylinders.png)

the final image of book 3

![book3 result](https://github.com/siloimwald/Fovea/blob/main/Results/book3_final_1500spp.png)

the final image of book 2

![book2 result](https://github.com/siloimwald/Fovea/blob/main/Results/book2_final_10k.png)



