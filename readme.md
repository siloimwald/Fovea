### Ray Tracing in One Weekend (and more)

Update: currently in the process of adapting this to version 4 of the books,
some things are broken (notably, depth of field). Replaced custom Vec3/Point3 implementations with
the `System.Numerics` Vector3 variants, leads to some artifacts.

C# implementation of Peter Shirley's excellent free books on ray and path tracing
with some own additions. [Take me to the books!](https://raytracing.github.io/)

Fovea is the name i usually pick for all of my hobbyist rendering adventures, from
[Fovea centralis](https://en.wikipedia.org/wiki/Fovea_centralis).

In addition to the books

- more general instancing support through transformation matrices
- cylinders and disks, because why not
- slightly more fancy BVH (iirc based on pbrt), using a SAH approach with binning
- triangle (mesh) support, either as standalone triangles or as a mesh
- instead of the whole xy/xz/.. rect stuff, i've added sort of factory methods to produce
quads/boxes from triangles. i.e. the cornell box is actually all triangles except for the sphere.

Compile and run:
Should probably compile and run fine with any recent dotnet core. (currently NET7)
The command line executable accepts two parameters:
- -s samples
- -w image width (the height is derived from the aspect ratio)
- the output file is fixed as "output.ppm"
- i.e. run something like `dotnet run -p Fovea.CmdLine -c Release -- -s 10 -w 800`

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



