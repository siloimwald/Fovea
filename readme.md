
#### work in progress

Currently in the process of adapting this to version 4 of the books,
some things are broken. Current state: Book 2 done, some book 3 leftovers from the previous version are still in.

changes to previous iteration:

- Replaced custom Vec3/Point3 implementations with the `System.Numerics` Vector3 variants
- json scene file format instead of hard coding scenes
- 3rd party image library instead of self made, easier to support texture files in different formats
- requires .NET 8 and uses C# 12 features. Although the latter very sparsely, for now.

long term goals

- add (back in again) CSG through signed distance trickery as the next feature
- experimental tree structures (i.e. swap bvh to kd tree from scene file level)
- some ui to see render progress

### Ray Tracing in One (Many) Weekend(s) (and more)

C# implementation of Peter Shirley's excellent free books on ray and path tracing
with some own additions. [Take me to the books!](https://raytracing.github.io/)

Fovea is the name i usually pick for all of my hobbyist rendering adventures, from
[Fovea centralis](https://en.wikipedia.org/wiki/Fovea_centralis).

In addition to the books

- more general instancing support through transformation matrices
- cylinders and disks, because why not
- slightly more fancy BVH, IIRC based on the (also excellent) [PBR Book](https://pbr-book.org/), using a SAH approach with binning
- Bounding box intersection with SSE Instructions, might not be the best, but it is faster than the regular method
- triangle (mesh) support, either as standalone triangles or as a mesh
- a json scene file parser

With dotnet 8 sdk installed, run either one of the contained scene files or look at the parameters
(which basically override scene settings related to render speed and image sized)
- `dotnet run --project .\Fovea.CmdLine\ -c Release -- --help`
- `dotnet run --project .\Fovea.CmdLine\ -c Release -- -s .\scenes\finalSceneBookOne.json` (or any other scene file)

There is also a scene writer, which allows you to write out programmatically 
generated scenes into json. See `Fovea.SceneWriter` directory.

Fancy picture time (still from book 3 iteration)

Teapot mesh with interpolated normals. The whole scene is enclosed with a sphere with a forest
scene texture used for lighting. 

![mesh_forest](https://github.com/siloimwald/Fovea/blob/main/Results/mesh_forest.png)

Cylinder Demo with some textures

![cylinders](https://github.com/siloimwald/Fovea/blob/main/Results/cylinders.png)

the final image of book 3

![book3 result](https://github.com/siloimwald/Fovea/blob/main/Results/book3_final_1500spp.png)

the final image of book 2

![book2 result](https://github.com/siloimwald/Fovea/blob/main/Results/book2_final_10k.png)



