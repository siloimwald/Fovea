### Ray Tracing in One Weekend (and more)

C# implementation of Peter Shirley's excellent free books on ray and path tracing
with some own additions.
Fovea is the name i usually pick for all of my hobbyist rendering adventures, from
[Fovea centralis](https://en.wikipedia.org/wiki/Fovea_centralis)

In addition to the books

- basic CSG support based on
["Ray Tracing CSG Objects Using Single Hit Intersections"](http://xrt.wdfiles.com/local--files/doc%3Acsg/CSG.pdf)
  (though slightly broken when using instancing ;)
- more general instancing support through transformation matrices
- cylinders and disks, because why not
- slightly more fancy BVH (iirc based on pbrt), using a SAH approach with binning
- triangle (mesh) support, either as standalone triangles or as a mesh

Fancy picture time




